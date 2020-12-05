using Notes.Core;
using Notes.MarkdigRenderers;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Widgets
{
    public class NoteWidget : IWidget
    {
        public string Name { get; private set; }

        public Note Note { get; private set; }

        // TODO: refactor how I handle this
        public IMarkdigRenderer Renderer { get; set; }

        public NoteWidget(string name, Note note)
        {
            this.Name = name;
            this.Note = note;
        }

        public void Render()
        {
            Renderer.Render(Note.Markdown);
        }
    }
}
