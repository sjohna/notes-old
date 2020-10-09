using ImGuiNET;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Text;

namespace Notes.MarkdigRenderers
{
    public class MarkdigPlainTextRenderer : IMarkdigRenderer
    {
        private bool atLineStart;

        public void Render(MarkdownDocument document)
        {
            atLineStart = true;

            var spacing = ImGui.GetStyle().ItemSpacing;
            spacing.X = 0;
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, spacing);

            foreach (var block in document)
            {
                RenderTopLevelBlock(block as dynamic);
            }

            ImGui.PopStyleVar(1);
        }

        private void RenderTopLevelBlock(ListBlock block)
        {
            RenderBlock(block as dynamic, " ");

            RenderNewLine();    // TODO: deduplicate between this and the other overloads
        }

        private void RenderTopLevelBlock(Block block)
        {
            RenderBlock(block as dynamic, "");

            RenderNewLine();
        }

        private void RenderBlock(Block block, string lineIndent)
        {
            ImGui.Text("ERROR: unrecognized block!");
        }

        private void RenderBlock(QuoteBlock block, string lineIndent)
        {
            RenderBlock(block as ContainerBlock, lineIndent + "> ");
        }

        private void RenderBlock(CodeBlock block, string lineIndent)
        {
            foreach (var line in block.GetLines())
            {
                RenderNonWrappingText(line.ToString(), lineIndent);
                RenderNewLine();
            }
        }

        private void RenderBlock(ThematicBreakBlock block, string lineIndent)
        {
            var windowWidth = ImGui.GetWindowSize().X;  // TODO: include padding in text area width, or make it a parameter
            var dashWidth = ImGui.CalcTextSize("-").X;

            int numDashes = (int)Math.Floor(windowWidth / dashWidth);

            RenderNonWrappingText(new string('-', numDashes), lineIndent);

            RenderNewLine();
        }

        private void RenderBlock(HeadingBlock block, string lineIndent)
        {
            RenderNonWrappingText($"{new string('#', block.Level)} ", lineIndent);

            RenderBlock(block as LeafBlock, lineIndent);

            RenderNewLine();
        }

        private void RenderBlock(ListItemBlock block, string lineIndent)
        {
            string listItemString;

            if (block.Order == 0) listItemString = "- ";
            else listItemString = $"{block.Order}. ";

            RenderNonWrappingText(listItemString, lineIndent);
            
            RenderBlock(block as ContainerBlock, lineIndent + new string(' ', listItemString.Length));
        }

        private void RenderBlock(ListBlock block, string lineIndent)
        {
            RenderBlock(block as ContainerBlock, lineIndent);
        }

        private void RenderBlock(ParagraphBlock block, string lineIndent)
        {
            RenderBlock(block as LeafBlock, lineIndent);

            RenderNewLine();
        }

        private void RenderBlock(ContainerBlock block, string lineIndent)
        {
            foreach (var childBlock in block)
            {
                RenderBlock(childBlock as dynamic, lineIndent);
            }
        }

        private void RenderBlock(LeafBlock block, string lineIndent)
        {
            if (block.Inline == null) return;
            RenderInline(block.Inline as dynamic, lineIndent);
        }

        private void RenderInline(Inline inline, string lineIndent)
        {
            ImGui.Text("ERROR: unrecognized inline!");
        }

        private void RenderInline(ContainerInline inline, string lineIndent)
        {
            foreach (var childInline in inline)
            {
                RenderInline(childInline as dynamic, lineIndent);
            }
        }

        private void RenderInline(CodeInline inline, string lineIndent)
        {
            RenderWrappingText(inline.Content, lineIndent);
        }

        private void RenderInline(LeafInline inline, string lineIndent)
        {
            RenderWrappingText(inline.ToString(), lineIndent);
        }

        private void RenderNewLine()
        {
            ImGui.NewLine();
            atLineStart = true;
        }

        // Render a linebreak that occurs within a block, either due to a LineBreakInline in the markdown, or due to text wrapping
        private void RenderLineIndentIfNecessary(string lineIndent)
        {
            if (atLineStart)
            {
                atLineStart = false;
                RenderText(lineIndent);
            }
        }

        private void RenderInline(LineBreakInline inline, string lineIndent)
        {
            RenderNewLine();
        }

        private void RenderWrappingText(string text, string lineIndent)
        {
            RenderLineIndentIfNecessary(lineIndent);   // ugly. Need to handle rendering newlines better...

            // this line wrapping algorithm won't work in all cases, I think
            var windowWidth = ImGui.GetWindowSize().X;  // TODO: include padding in text area width, or make it a parameter
                                                        // TODO: make width calculation aware of whether there is a scrollbar
            var currentCursorX = ImGui.GetCursorPosX();
            var textExtent = currentCursorX + ImGui.CalcTextSize(text).X;

            if (textExtent < windowWidth)
            {
                RenderNonWrappingText(text, lineIndent);
                return;
            }

            int searchIndex = text.Length - 1;
            int blankIndex = text.LastIndexOf(' ', searchIndex);
            currentCursorX = ImGui.GetCursorPosX();

            while (blankIndex > 0)
            {
                if (currentCursorX + ImGui.CalcTextSize(text.Substring(0,blankIndex)).X < windowWidth)
                {
                    RenderNonWrappingText(text.Substring(0,blankIndex).Trim(), lineIndent);
                    RenderNewLine();
                    RenderWrappingText(text.Substring(blankIndex).Trim(), lineIndent);
                    return;
                }

                searchIndex = blankIndex - 1;
                blankIndex = text.LastIndexOf(' ', searchIndex);
            }

            // TODO: handle this better: maybe we only need to render the first word on a line by itself, and can line break the rest in a better way
            // didn't find a place to break, so just render the whole line
            RenderNonWrappingText(text, lineIndent);
        }


        private void RenderNonWrappingText(string text, string lineIndent)
        {
            RenderLineIndentIfNecessary(lineIndent);

            RenderText(text);
        }

        private void RenderText(string text)
        {
            ImGui.Text(text);
            ImGui.SameLine();
        }
    }
}
