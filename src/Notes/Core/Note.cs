using Markdig;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Core
{
    public class Note
    {
        // TODO: will eventually need to handle dynamically resizing the buffer used for the input text area, maybe?
        internal string _Text = "";

        public MarkdownDocument Markdown = Markdig.Markdown.Parse("");

        private static MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UsePreciseSourceLocation().Build();

        public string Text
        {
            get => _Text;
            set
            {
                _Text = value;
                ParseMarkdown();
            }
        }

        public void ParseMarkdown()
        {
            Markdown = Markdig.Markdown.Parse(_Text, pipeline);
        }
    }
}
