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
using Notes.UserInterfaces;

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

        static MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UsePreciseSourceLocation().Build();

        static IUserInterface userInterface;

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

            userInterface = new SimpleTwoPanelUI(_window);

            // Main application loop
            MainLoop();

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

            userInterface.SubmitUI();

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

                userInterface.SubmitUI();

                _cl.Begin();
                _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
                _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
                _controller.Render(_gd, _cl);
                _cl.End();
                _gd.SubmitCommands(_cl);
                _gd.SwapBuffers(_gd.MainSwapchain);
            }

        }
    }
}
