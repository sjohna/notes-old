﻿using ImGuiNET;
using Notes.Core;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Notes.Widgets
{
    public class NoteEditor : IWidget
    {
        public Note Note { get; private set; }

        public string Name { get; private set; }

        private bool lastEventWasCharFilter = false;
        private char lastCharFilterChar;

        public NoteEditor(string name, Note note)
        {
            this.Name = name;
            this.Note = note;
        }

        public unsafe void Render(float width, float height)
        {
            ImGui.BeginChild(Name, new Vector2(width, height), true);

            if (ImGui.InputTextMultiline($"##{Name} text input", ref Note._Text, 1000000, new Vector2(width - 16, height - 16), ImGuiInputTextFlags.CallbackCharFilter | ImGuiInputTextFlags.CallbackAlways | ImGuiInputTextFlags.AllowTabInput, textBoxCallback))
            {
                Note.ParseMarkdown();
            }

            ImGui.End();
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

                if (lastCharFilterChar == '\t') return 1;

                return 0;
            }

            if (dataPtr.EventFlag == ImGuiInputTextFlags.CallbackAlways && lastEventWasCharFilter)
            {
                PrintCallbackData(data);
                lastEventWasCharFilter = false;

                if (lastCharFilterChar == '\n')
                {
                    if (ImGui.GetIO().KeyShift) // on shift+newline, don't do anything fancy
                    {
                        return 0;
                    }

                    var stringInBuffer = Encoding.UTF8.GetString((byte*)dataPtr.Buf, dataPtr.BufTextLen);

                    string previousLine = ExtractInputTextLine(stringInBuffer, dataPtr.CursorPos - 1);

                    if (previousLine.Trim() == "-")
                    {
                        dataPtr.DeleteChars(dataPtr.CursorPos - previousLine.Length - 1, previousLine.Length);

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

                        return 1;
                    }
                }
                else if (lastCharFilterChar == '\t')
                {
                    var stringInBuffer = Encoding.UTF8.GetString((byte*)dataPtr.Buf, dataPtr.BufTextLen);

                    var currentLine = ExtractLine(stringInBuffer, dataPtr.CursorPos-1);

                    if (currentLine.CursorIndexInLine % 2 == 0)
                    {
                        dataPtr.InsertChars(dataPtr.CursorPos, "  ");
                    }
                    else
                    {
                        dataPtr.InsertChars(dataPtr.CursorPos, " ");
                    }
                }
            }

            return 0;
        }

        private (string Line, int CursorIndexInLine) ExtractLine(string stringInBuffer, int location)
        {
            var builder = new StringBuilder();

            int index = location;

            // TODO: I think I can do this better, with substring
            while (index >= 0 && index < stringInBuffer.Length && stringInBuffer[index] != '\n')
            {
                builder.Insert(0, stringInBuffer[index]);
                --index;
            }

            int CursorIndexInLine = builder.Length;

            index = location + 1;

            while (index >= 0 && index < stringInBuffer.Length && stringInBuffer[index] != '\n')
            {
                builder.Append(stringInBuffer[index]);
                ++index;
            }

            return (builder.ToString(), CursorIndexInLine);
        }
    }
}
