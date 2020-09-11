using ImGuiNET;
using Markdig;
using Markdig.Syntax;
using Notes.MarkdigRenderers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using Veldrid.Sdl2;

namespace Notes.UserInterfaces
{
    public class SimpleTwoPanelUI : IUserInterface
    {
        // TODO: will eventually need to handle dynamically resizing the buffer used for the input text area, maybe?
        private string inputText = "";
        private MarkdownDocument parsedMarkdown = Markdown.Parse("");
        private string currentRenderType = "Plain text";
        private IMarkdigRenderer renderer = new MarkdigPlainTextRenderer();
        private bool lastEventWasCharFilter = false;
        private char lastCharFilterChar;
        private Sdl2Window _window;
        private MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UsePreciseSourceLocation().Build();

        public SimpleTwoPanelUI(Sdl2Window window)
        {
            _window = window;
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

        private string ExtractInputTextLine(int lineEndIndex)
        {
            int currIndex = lineEndIndex;

            while (currIndex > 0 && inputText[currIndex - 1] != '\n')
            {
                currIndex--;
            }

            return inputText.Substring(currIndex, lineEndIndex - currIndex + 1);
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

        public unsafe void SubmitUI()
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
