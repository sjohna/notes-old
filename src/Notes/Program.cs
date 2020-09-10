using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;
using System;
using Markdig;
using Markdig.Syntax;
using Notes.MarkdigRenderers;
using System.Text;
using Veldrid.Utilities;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;

namespace Notes
{
    class Program
    {
        private static Sdl2Window _window;
        private static GraphicsDevice _gd;
        private static CommandList _cl;
        private static ImGuiRenderer _controller;

        // UI state
        private static Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        private static ImFontPtr customFont;

        static MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UsePreciseSourceLocation().Build();

        static void Main(string[] args)
        {
            // Create window, GraphicsDevice, and all resources necessary for the demo.
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(50, 50, 1280, 720, WindowState.Normal, "Notes"),
                new GraphicsDeviceOptions(true, null, true),
                out _window,
                out _gd);
            _window.Resized += () =>
            {
                _gd.MainSwapchain.Resize((uint)_window.Width, (uint)_window.Height);
                _controller.WindowResized(_window.Width, _window.Height);
            };
            _cl = _gd.ResourceFactory.CreateCommandList();
            _controller = new ImGuiRenderer(_gd, _gd.MainSwapchain.Framebuffer.OutputDescription, _window.Width, _window.Height);



            // Main application loop
            SaveScreenToFile();

            // Clean up Veldrid resources
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }


        // screenshot code taken from https://github.com/mellinoe/veldrid/issues/99
        private static void SaveScreenToFile()
        {
            var rf = new DisposeCollectorResourceFactory(_gd.ResourceFactory);

            var colorTargetTexture = _gd.SwapchainFramebuffer.ColorTargets[0].Target;
            var pixelFormat = colorTargetTexture.Format; // <- PixelFormat.B8_G8_R8_A8_UNorm, is it OK?

            var textureDescription = colorTargetTexture.GetDescription();
            textureDescription.Usage = TextureUsage.RenderTarget;
            textureDescription.Type = TextureType.Texture2D;
            textureDescription.Format = pixelFormat;

            var textureForRender = rf.CreateTexture(textureDescription);

            //var depthTexture = _gd.SwapchainFramebuffer.DepthTarget.Value.Target;
            //var depthTextureForRender = rf.CreateTexture(depthTexture.GetDescription());

            var framebufferDescription = new FramebufferDescription(null, textureForRender);
            var framebuffer = rf.CreateFramebuffer(framebufferDescription);

            Texture stage = rf.CreateTexture(TextureDescription.Texture2D(
                textureForRender.Width,
                textureForRender.Height,
                1,
                1,
                pixelFormat,
                TextureUsage.Staging));

            SubmitUI();

            _cl.Begin();
            _cl.SetFramebuffer(framebuffer);

            _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
            _controller.Render(_gd, _cl);

            _cl.CopyTexture(
                textureForRender, 0, 0, 0, 0, 0,
                stage, 0, 0, 0, 0, 0,
                stage.Width, stage.Height, 1, 1);
            _cl.End();

            _gd.SubmitCommands(_cl);

            MappedResourceView<Rgba32> map = _gd.Map<Rgba32>(stage, MapMode.Read);

            var image = new Image<Rgba32>((int)stage.Width, (int)stage.Height);

            Rgba32[] pixelData = new Rgba32[stage.Width * stage.Height];
            for (int y = 0; y < stage.Height; y++)
            {
                for (int x = 0; x < stage.Width; x++)
                {
                    //int index = (int)(y * stage.Width + x);
                    //pixelData[index] = map[x, y]; // <- I have to convert BGRA to RGBA pixels here
                    image[x, y] = new Rgba32(map[x, y].B, map[x, y].G, map[x, y].R, map[x, y].A);
                }
            }

            _gd.Unmap(stage);
            rf.DisposeCollector.DisposeAll();

            image.SaveAsPng("test.png");
        }

        private static void MainLoop()
        {
            while (_window.Exists)
            {
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                _controller.Update(1f / 60f, snapshot); // Feed the input events to our ImGui controller, which passes them through to ImGui.

                SubmitUI();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
            }

        }

        // TODO: will eventually need to handle dynamically resizing the buffer used for the input text area
        static string inputText = "";

        static MarkdownDocument parsedMarkdown = Markdown.Parse("");

        static string currentRenderType = "Plain text";

        static IMarkdigRenderer renderer = new MarkdigPlainTextRenderer();

        static bool lastEventWasCharFilter = false;
        static char lastCharFilterChar;

        static unsafe void PrintCallbackData(ImGuiInputTextCallbackData* data)
        {
            Console.WriteLine($"Callback event: {data->EventFlag.ToString()}");
            Console.WriteLine($"  BufDirty: {data->BufDirty}");
            Console.WriteLine($"  BufSize: {data->BufSize}");
            Console.WriteLine($"  BufTextLen: {data->BufTextLen}");
            Console.WriteLine($"  CursorPos: {data->CursorPos}");
            Console.WriteLine($"  Event Char: {Encoding.UTF8.GetString(BitConverter.GetBytes(data->EventChar))}");
            Console.WriteLine($"  EventKey: {data->EventKey.ToString()}");
            Console.WriteLine($"  SelectionStart: {data->SelectionStart}");
            Console.WriteLine($"  SelectionEnd: {data->SelectionEnd}");
        }

        private static string ExtractInputTextLine(int lineEndIndex)
        {
            int currIndex = lineEndIndex;

            while (currIndex > 0 && inputText[currIndex - 1] != '\n')
            {
                currIndex--;
            }

            return inputText.Substring(currIndex, lineEndIndex - currIndex + 1);
        }

        private static unsafe int textBoxCallback(ImGuiInputTextCallbackData* data)
        {
            var dataPtr = new ImGuiInputTextCallbackDataPtr(data);

            if (dataPtr.EventFlag == ImGuiInputTextFlags.CallbackCharFilter)
            {
                PrintCallbackData(data);
                lastEventWasCharFilter = true;
                lastCharFilterChar = Encoding.UTF8.GetString(BitConverter.GetBytes(data->EventChar))[0];
                return 0;
            }

            if (dataPtr.EventFlag == ImGuiInputTextFlags.CallbackAlways && lastEventWasCharFilter)
            {
                PrintCallbackData(data);

                if (lastCharFilterChar == '\n')
                {
                    string previousLine = ExtractInputTextLine(dataPtr.CursorPos - 2);
                    
                    if (previousLine.Trim() == "-")
                    {
                        dataPtr.DeleteChars(dataPtr.CursorPos - previousLine.Length - 1, previousLine.Length);

                        lastEventWasCharFilter = false;
                        return 1;
                    }
                    if (previousLine.Trim() != "")
                    {
                        int spaceIndex = 0;
                        while (previousLine[spaceIndex] == ' ') ++spaceIndex;

                        var newInputTextBuilder = new StringBuilder();
                        newInputTextBuilder.Append(new String(' ', spaceIndex));

                        if (previousLine.Length >= spaceIndex + 1 &&
                            previousLine[spaceIndex] == '-' &&
                            previousLine[spaceIndex + 1] == ' ')
                        {
                            newInputTextBuilder.Append("- ");
                        }

                        dataPtr.InsertChars(dataPtr.CursorPos, newInputTextBuilder.ToString());

                        lastEventWasCharFilter = false;
                        return 1;
                    }
                }

                lastEventWasCharFilter = false;
            }

            return 0;
        }

        private static unsafe void SubmitUI()
        {
            {
                ImGui.GetStyle().WindowRounding = 0;

                ImGui.SetNextWindowPos(new Vector2(0, 0));
                ImGui.SetNextWindowSize(new Vector2(_window.Width, _window.Height));
                ImGui.Begin("", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration);

                if (ImGui.BeginCombo("Render Type", currentRenderType))
                {

                    if (ImGui.Selectable("AST", currentRenderType == "AST"))
                    {
                        renderer = new MarkdigASTRenderer();
                        currentRenderType = "AST";
                    }

                    if (ImGui.Selectable("Plain text", currentRenderType == "Plain text")) 
                    {
                        renderer = new MarkdigPlainTextRenderer();
                        currentRenderType = "Plain text";
                    }

                    ImGui.EndCombo();
                }

                var panelCursorY = ImGui.GetCursorPosY();
                

                float paneHeight = ImGui.GetWindowSize().Y - 16;
                float paneWidth = (ImGui.GetWindowSize().X - 24) / 2;

                ImGui.BeginChild("Text area", new Vector2(paneWidth, paneHeight), true);

                if (ImGui.InputTextMultiline("##Text area input", ref inputText, 1000000, new Vector2(paneWidth - 16, paneHeight - 16), ImGuiInputTextFlags.CallbackCharFilter | ImGuiInputTextFlags.CallbackAlways, textBoxCallback))
                {
                    parsedMarkdown = Markdown.Parse(inputText, pipeline);
                }

                ImGui.End();

                ImGui.SetCursorPos(new Vector2(ImGui.GetWindowSize().X / 2 + 4, panelCursorY));
                ImGui.BeginChild("Markdown area", new Vector2(paneWidth, paneHeight), true);

                renderer.Render(parsedMarkdown);

                ImGui.End();

                ImGui.End();
            }
        }
    }
}
