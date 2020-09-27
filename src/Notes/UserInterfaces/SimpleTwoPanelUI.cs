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
                    Renderer = new MarkdigASTRenderer();
                }
                else if (value == "Plain text")
                {
                    Renderer = new MarkdigPlainTextRenderer();
                }
                else
                {
                    throw new ArgumentException("Invalid render type.", nameof(CurrentRenderType));
                }

                renderTypeComboBox.CurrentSelection = value;
            }
        }
        private IMarkdigRenderer Renderer { get; set; }

        private bool lastEventWasCharFilter = false;
        private char lastCharFilterChar;
        private Sdl2Window _window;

        private StringListComboBox renderTypeComboBox;

        // TODO: make the SubmitUI method take in the window, not the constructor
        // TODO: new interface: IWindowRenderer. Change UserInterface terminology to Renderer, differentiate between renderers for a whole window, and sub-renderers
        public SimpleTwoPanelUI(Sdl2Window window)
        {
            _window = window;

            renderTypeComboBox = new StringListComboBox("Render Type", new string[] { "AST", "Plain text" }, "Plain text");
            CurrentRenderType = "Plain text";   // super hacky. I should make the combo box widget support objects instead of strings
            renderTypeComboBox.ItemSelected += (sender, args) => { CurrentRenderType = args.SelectedItem; };
        }

        unsafe void PrintCallbackData(ImGuiInputTextCallbackData* data)
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

        private string ExtractInputTextLine(string stringInBuffer, int lineEndIndex)
        {
            int currIndex = lineEndIndex;

            while (currIndex > 0 && stringInBuffer[currIndex - 1] != '\n')
            {
                currIndex--;
            }

            return stringInBuffer.Substring(currIndex, lineEndIndex - currIndex + 1);
        }

        private unsafe int textBoxCallback(ImGuiInputTextCallbackData* data)
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
                    var stringInBuffer = Encoding.UTF8.GetString((byte*)dataPtr.Buf, dataPtr.BufTextLen);

                    string previousLine = ExtractInputTextLine(stringInBuffer, dataPtr.CursorPos - 1);

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

            ImGui.BeginChild("Text area", new Vector2(paneWidth, paneHeight), true);

            if (ImGui.InputTextMultiline("##Text area input", ref Note._Text, 1000000, new Vector2(paneWidth - 16, paneHeight - 16), ImGuiInputTextFlags.CallbackCharFilter | ImGuiInputTextFlags.CallbackAlways, textBoxCallback))
            {
                Note.ParseMarkdown();
            }

            ImGui.End();

            ImGui.SetCursorPos(new Vector2(ImGui.GetWindowSize().X / 2 + 4, panelCursorY));
            ImGui.BeginChild("Markdown area", new Vector2(paneWidth, paneHeight), true);

            Renderer.Render(Note.Markdown);

            ImGui.End();

            ImGui.End();
        }
    }
}
