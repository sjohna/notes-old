﻿using ImGuiNET;
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
            get => renderTypeComboBox.CurrentSelection.OptionText;
            set
            {
                if (value == "AST")
                {
                    noteWidget.Renderer = new MarkdigASTRenderer();
                }
                else if (value == "Plain text")
                {
                    noteWidget.Renderer = new MarkdigPlainTextRenderer();
                }
                else
                {
                    throw new ArgumentException("Invalid render type.", nameof(CurrentRenderType));
                }

                renderTypeComboBox.SetCurrentSelection(value);
            }
        }

        private ComboBox<String> renderTypeComboBox;
        private NoteWidget noteWidget;
        private NoteEditor noteEditor;

        private GenericFrame noteEditorFrame;
        private GenericFrame noteMarkdownDisplayFrame;

        private float leftPanelProportion = 0.50f;

        private bool isDragging = false;
        private Vector2 initialDragLocation;

        // TODO: make the SubmitUI method take in the window, not the constructor
        // TODO: new interface: IWindowRenderer. Change UserInterface terminology to Renderer, differentiate between renderers for a whole window, and sub-renderers
        public SimpleTwoPanelUI(float leftPanelProportion = 0.5f)
        {
            this.leftPanelProportion = leftPanelProportion;

            noteWidget = new NoteWidget("Markdown area", Note);
            noteMarkdownDisplayFrame = new GenericFrame("Markdown frame", noteWidget);

            noteEditor = new NoteEditor("Text area", Note);
            noteEditorFrame = new GenericFrame("Text area frame", noteEditor);

            renderTypeComboBox = new ComboBox<String>("Render Type", new string[] { "AST", "Plain text" }, "Plain text");
            CurrentRenderType = "Plain text";   // super hacky. I should make the combo box widget support objects instead of strings
            renderTypeComboBox.ItemSelected += (sender, args) => { CurrentRenderType = args.SelectedItemText; };
        }

        public unsafe void SubmitUI(Sdl2Window _window)
        {
            ImGui.GetStyle().WindowRounding = 0;

            ImGui.SetNextWindowPos(new Vector2(0, 0));
            ImGui.SetNextWindowSize(new Vector2(_window.Width, _window.Height));
            ImGui.Begin("", ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration);

            renderTypeComboBox.Render();

            // TODO: set mouse cursor if it's close to the middle

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
                noteEditorFrame.Render(paneWidth, paneHeight);
                ImGui.SetCursorPos(new Vector2(ImGui.GetWindowSize().X * leftPanelProportion + 4, panelCursorY));
            }

            paneWidth = Math.Min((ImGui.GetWindowSize().X * (1-leftPanelProportion)) - 12, ImGui.GetWindowSize().X - 16);

            if (paneWidth >= 0) noteMarkdownDisplayFrame.Render(paneWidth, paneHeight);

            ImGui.End();
        }

        public void SetNoteEditorKeyboardFocus()
        {
            noteEditor.SetKeyboardFocus();
        }
    }
}
