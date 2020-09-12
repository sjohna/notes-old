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
        static void Main(string[] args)
        {
            using (var window = new Notes.Window(50,50,1280,800,"Notes"))
            {
                window.MainLoop();
            }
        }
    }
}
