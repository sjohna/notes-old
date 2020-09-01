using ImGuiNET;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;

namespace Notes.MarkdigRenderers
{
    class MarkdigPlainTextRenderer : IMarkdigRenderer
    {
        private bool newLine;
        private bool startParagraph;
        private int listIndent;

        private int TextIndent => listIndent == 0 ? 0 : listIndent + 3;

        public void Render(MarkdownDocument document)
        {
            newLine = false;
            startParagraph = false;
            listIndent = 0;

            var spacing = ImGui.GetStyle().ItemSpacing;
            spacing.X = 0;
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, spacing);

            foreach (var block in document)
            {
                RenderBlock(block as dynamic);
            }

            ImGui.PopStyleVar(1);
        }

        private void RenderBlock(Block block)
        {
            ImGui.Text("ERROR: unrecognized block!");
        }

        private void RenderBlock(ListItemBlock block)
        {
            if (newLine) newLine = false;
            else ImGui.SameLine();

            ImGui.Text($"{new string(' ', listIndent)} - ");

            RenderBlock(block as ContainerBlock);
        }

        private void RenderBlock(ListBlock block)
        {
            listIndent += 2;
            RenderBlock(block as ContainerBlock);
            listIndent -= 2;
        }

        private void RenderBlock(ParagraphBlock block)
        {
            RenderBlock(block as LeafBlock);

            startParagraph = true;
            newLine = true;
        }

        private void RenderBlock(ContainerBlock block)
        {
            foreach (var childBlock in block)
            {
                RenderBlock(childBlock as dynamic);
            }
        }

        private void RenderBlock(LeafBlock block)
        {
            if (block.Inline == null) return;
            RenderInline(block.Inline as dynamic);
        }

        private void RenderInline(Inline inline)
        {
            ImGui.Text("ERROR: unrecognized inline!");
        }

        private void RenderInline(ContainerInline inline)
        {
            foreach (var childInline in inline)
            {
                RenderInline(childInline as dynamic);
            }
        }

        private void RenderInline(LeafInline inline)
        {
            if (startParagraph && listIndent == 0)
            {
                ImGui.NewLine();
            }
            startParagraph = false;

            if (newLine)
            {
                newLine = false;
                ImGui.Text(new string(' ', TextIndent));
                ImGui.SameLine();
            }
            else ImGui.SameLine();

            ImGui.Text($"{inline.ToString()}");
        }

        private void RenderInline(LineBreakInline inline)
        {
            newLine = true;
        }
    }
}
