using Notes.UserInterfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid.Utilities;

namespace Notes
{
    public class Window : IDisposable
    {
        private Sdl2Window _window;

        public Sdl2Window SDLWindow => _window;

        private GraphicsDevice _gd;
        private CommandList _cl;
        private ImGuiRenderer _controller;

        public  IUserInterface UserInterface { get; set; }

        private Vector3 _clearColor = new Vector3(0.45f, 0.55f, 0.6f);

        public Window(int x, int y, int windowWidth, int windowHeight, string windowTitle, bool visible = true)
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(
                new WindowCreateInfo(x, y, windowWidth, windowHeight, visible ? WindowState.Normal : WindowState.Hidden, windowTitle),
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
        }

        public void MainLoop()
        {
            while (_window.Exists)
            {
                InputSnapshot snapshot = _window.PumpEvents();
                if (!_window.Exists) { break; }
                RenderFrame(snapshot);
            }
        }

        public void UpdateControllerWithInputSnapshot(InputSnapshot snapshot)
        {
            _controller.Update(1f / 60f, snapshot);
        }

        public void RenderFrame(InputSnapshot inputEvents)
        {
            _controller.Update(1f / 60f, inputEvents); // Feed the input events to our ImGui controller, which passes them through to ImGui.

            UserInterface.SubmitUI(_window);

            _cl.Begin();
            _cl.SetFramebuffer(_gd.MainSwapchain.Framebuffer);
            _cl.ClearColorTarget(0, new RgbaFloat(_clearColor.X, _clearColor.Y, _clearColor.Z, 1f));
            _controller.Render(_gd, _cl);
            _cl.End();
            _gd.SubmitCommands(_cl);
            _gd.SwapBuffers(_gd.MainSwapchain);
        }

        // screenshot code taken from https://github.com/mellinoe/veldrid/issues/99
        public void SaveScreenToFile(string fileName)
        {
            var rf = new DisposeCollectorResourceFactory(_gd.ResourceFactory);

            var colorTargetTexture = _gd.SwapchainFramebuffer.ColorTargets[0].Target;
            var pixelFormat = colorTargetTexture.Format; // <- PixelFormat.B8_G8_R8_A8_UNorm, is it OK?

            var textureDescription = colorTargetTexture.GetDescription();
            textureDescription.Usage = TextureUsage.RenderTarget;
            textureDescription.Type = TextureType.Texture2D;
            textureDescription.Format = pixelFormat;

            var textureForRender = rf.CreateTexture(textureDescription);

            // the graphics device I've instantiated doesn't seem to have a depthTexture
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

            UserInterface.SubmitUI(_window);

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

            image.SaveAsPng(fileName);
        }

        public void Dispose()
        {
            _window.Close();
            _gd.WaitForIdle();
            _controller.Dispose();
            _cl.Dispose();
            _gd.Dispose();
        }
    }
}
