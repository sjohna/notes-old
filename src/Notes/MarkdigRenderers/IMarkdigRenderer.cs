using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.MarkdigRenderers
{
    public interface IMarkdigRenderer
    {
        void Render(MarkdownDocument document);
    }
}
