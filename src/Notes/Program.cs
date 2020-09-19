using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using ImGuiNET;
using System;
using Markdig;
using Notes.UserInterfaces;

namespace Notes
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var window = new Notes.Window(50,50,1280,800,"Notes"))
            {
                var userInterface = new SimpleTwoPanelUI(window.SDLWindow);
                window.UserInterface = userInterface;
                window.MainLoop();
            }
        }
    }
}
