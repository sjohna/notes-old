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

        void RenderBlock(Block block, int indent);

        void RenderBlock(ContainerBlock block, int indent);

        void RenderBlock(LeafBlock block, int indent);

        void RenderInline(Inline inline, int indent);

        void RenderInline(ContainerInline inline, int indent);

        void RenderInline(LeafInline inline, int indent);
    }
}
