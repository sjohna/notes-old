using ImGuiNET;
using Markdig;
using Markdig.Syntax;
using Notes.Core;
using Notes.MarkdigRenderers;
using Notes.Widgets;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid.Sdl2;

namespace Notes.UserInterfaces
{
    public class SimpleTwoPanelUI : IUserInterface
    {
        public Note Note = new Note();

        // TODO: deduplicate these two properties
        public string CurrentRenderType
        {
            get => renderTypeComboBox.CurrentSelection;
            set
            {
                if (value == "AST")
                {
                    noteMarkdownDisplay.Renderer = new MarkdigASTRenderer();
                }
                else if (value == "Plain text")
                {
                    noteMarkdownDisplay.Renderer = new MarkdigPlainTextRenderer();
                }
                else
                {
                    throw new ArgumentException("Invalid render type.", nameof(CurrentRenderType));
                }

                renderTypeComboBox.CurrentSelection = value;
            }
        }

        private Sdl2Window _window;

        private StringListComboBox renderTypeComboBox;
        private NoteMarkdownDisplay noteMarkdownDisplay;
        private NoteEditor noteEditor;

        // TODO: make the SubmitUI method take in the window, not the constructor
        // TODO: new interface: IWindowRenderer. Change UserInterface terminology to Renderer, differentiate between renderers for a whole window, and sub-renderers
        public SimpleTwoPanelUI(Sdl2Window window)
        {
            _window = window;

            noteMarkdownDisplay = new NoteMarkdownDisplay("Markdown area", Note);

            noteEditor = new NoteEditor("Text area", Note);

            renderTypeComboBox = new StringListComboBox("Render Type", new string[] { "AST", "Plain text" }, "Plain text");
            CurrentRenderType = "Plain text";   // super hacky. I should make the combo box widget support objects instead of strings
            renderTypeComboBox.ItemSelected += (sender, args) => { CurrentRenderType = args.SelectedItem; };
        }

        public unsafe void SubmitUI()
        {
            ImGui.GetStyle().WindowRounding = 0;

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(_window.Width, _window.Height));
            ImGui.Begin("", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration);

            renderTypeComboBox.Render();

            var panelCursorY = ImGui.GetCursorPosY();

            float paneHeight = ImGui.GetWindowSize().Y - panelCursorY - 8;  // TODO: parameterize this better...
            float paneWidth = (ImGui.GetWindowSize().X - 24) / 2;

            // TODO: figure out how to handle size parameterization better...
            noteEditor.Width = paneWidth;
            noteEditor.Height = paneHeight;
            noteEditor.Render();

            ImGui.SetCursorPos(new Vector2(ImGui.GetWindowSize().X / 2 + 4, panelCursorY));

            // TODO: figure out how to handle size parameterization better...
            noteMarkdownDisplay.Width = paneWidth;
            noteMarkdownDisplay.Height = paneHeight;
            noteMarkdownDisplay.Render();

            ImGui.End();
        }
    }
}
