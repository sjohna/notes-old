using ImGuiNET;
using Notes.Core;
using Notes.MarkdigRenderers;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Notes.Widgets
{
    public class NoteMarkdownDisplay : IWidget
    {
        public string Name { get; private set; }

        public Note Note { get; private set; }
        
        // TODO: refactor how I handle this
        public IMarkdigRenderer Renderer { get; set; }

        public NoteMarkdownDisplay(string name, Note note)
        {
            this.Name = name;
            this.Note = note;
        }

        public void Render(float width, float height)
        {
            ImGui.BeginChild(Name, new Vector2(width, height), true);

            Renderer.Render(Note.Markdown);

            ImGui.End();
        }
    }
}
