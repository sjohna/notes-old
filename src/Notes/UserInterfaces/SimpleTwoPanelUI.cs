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

        private StringListComboBox renderTypeComboBox;
        private NoteMarkdownDisplay noteMarkdownDisplay;
        private NoteEditor noteEditor;

        private float leftPanelProportion = 0.50f;

        private bool isDragging = false;
        private Vector2 initialDragLocation;

        // TODO: make the SubmitUI method take in the window, not the constructor
        // TODO: new interface: IWindowRenderer. Change UserInterface terminology to Renderer, differentiate between renderers for a whole window, and sub-renderers
        public SimpleTwoPanelUI(float leftPanelProportion = 0.5f)
        {
            this.leftPanelProportion = leftPanelProportion;

            noteMarkdownDisplay = new NoteMarkdownDisplay("Markdown area", Note);

            noteEditor = new NoteEditor("Text area", Note);

            renderTypeComboBox = new StringListComboBox("Render Type", new string[] { "AST", "Plain text" }, "Plain text");
            CurrentRenderType = "Plain text";   // super hacky. I should make the combo box widget support objects instead of strings
            renderTypeComboBox.ItemSelected += (sender, args) => { CurrentRenderType = args.SelectedItem; };
        }

        public unsafe void SubmitUI(Sdl2Window _window)
        {
            ImGui.GetStyle().WindowRounding = 0;

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(_window.Width, _window.Height));
            ImGui.Begin("", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration);

            // TODO: figure out a better way to handle widgets that don't care about size...
            renderTypeComboBox.Render(-1,-1);

            // set cursor if it's close to the middle

            if (Math.Abs(ImGui.GetMousePos().X - (ImGui.GetWindowSize().X * leftPanelProportion)) <= 4)
            {
                if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
                {
                    isDragging = true;
                    initialDragLocation = ImGui.GetMousePos();
                }
            }

            if (isDragging && ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                isDragging = false;
            }

            if (isDragging)
            {
                float dragToX = initialDragLocation.X + ImGui.GetMouseDragDelta().X;

                leftPanelProportion = dragToX / ImGui.GetWindowSize().X;

                if (leftPanelProportion < 0) leftPanelProportion = 0;
                if (leftPanelProportion > 1) leftPanelProportion = 1;
            }

            var panelCursorY = ImGui.GetCursorPosY();

            float paneHeight = ImGui.GetWindowSize().Y - panelCursorY - 8;  // TODO: parameterize this better...
            //float paneWidth = (ImGui.GetWindowSize().X - 24) / 2;
            float paneWidth = paneWidth = Math.Min((ImGui.GetWindowSize().X * leftPanelProportion) - 12, ImGui.GetWindowSize().X - 16);

            if (paneWidth >= 0)
            {
                noteEditor.Render(paneWidth, paneHeight);
                ImGui.SetCursorPos(new Vector2(ImGui.GetWindowSize().X * leftPanelProportion + 4, panelCursorY));
            }

            paneWidth = Math.Min((ImGui.GetWindowSize().X * (1-leftPanelProportion)) - 12, ImGui.GetWindowSize().X - 16);

            if (paneWidth >= 0) noteMarkdownDisplay.Render(paneWidth, paneHeight);

            ImGui.End();
        }

        public void SetNoteEditorKeyboardFocus()
        {
            noteEditor.SetKeyboardFocus();
        }
    }
}
