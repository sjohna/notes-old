using ImGuiNET;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using System;
using System.Text;

namespace Notes.MarkdigRenderers
{
    public class MarkdigPlainTextRenderer : IMarkdigRenderer
    {
        private bool newLine;
        private int textIndent; // text indent within current list level (list level is zero outside of lists)
        private int listLevel;
        private bool firstBlock;
        private int quoteLevel;

        private int ListIndent => Math.Max((listLevel - 1) * 2, 0); // this is a kludge. I don't think I should need to do a Max here...
        private int TotalTextIndent => ListIndent + textIndent;

        public void Render(MarkdownDocument document)
        {
            newLine = false;
            firstBlock = true;
            quoteLevel = 0;
            textIndent = 0;
            listLevel = 0;

            var spacing = ImGui.GetStyle().ItemSpacing;
            spacing.X = 0;
            ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, spacing);

            foreach (var block in document)
            {
                RenderTopLevelBlock(block as dynamic);
            }

            ImGui.PopStyleVar(1);
        }

        private void RenderTopLevelBlock(Block block)
        {
            if (!firstBlock)
            {
                ImGui.NewLine();
                ImGui.NewLine();
            }
            firstBlock = false;
            newLine = false;

            RenderBlock(block as dynamic);
        }

        private void RenderBlock(Block block)
        {
            ImGui.Text("ERROR: unrecognized block!");
        }

        private void RenderBlock(QuoteBlock block)
        {
            RenderNonWrappingText("> ");    // render caret for first line of quote. This should be handled in a different way...

            quoteLevel += 1;
            textIndent += 2;
            RenderBlock(block as ContainerBlock);
            textIndent -= 2;
            quoteLevel -= 1;
        }

        private void RenderBlock(CodeBlock block)
        {
            foreach (var line in block.GetLines())
            {
                RenderNonWrappingText(line.ToString());
                newLine = true;
            }
        }

        private void RenderBlock(ThematicBreakBlock block)
        {
            var windowWidth = ImGui.GetWindowSize().X;  // TODO: include padding in text area width, or make it a parameter
            var dashWidth = ImGui.CalcTextSize("-").X;

            int numDashes = (int)Math.Floor(windowWidth / dashWidth);

            RenderNonWrappingText(new string('-', numDashes));
        }

        private void RenderBlock(HeadingBlock block)
        {
            RenderNonWrappingText($"{new string('#', block.Level)} ");

            RenderBlock(block as LeafBlock);
        }

        private void RenderBlock(ListItemBlock block)
        {
            listLevel += 1;                             // I find this whole thing kludgy. I need a better way of determining how to indent text...
            textIndent = 0;

            string listItemString;

            if (block.Order == 0) listItemString = " - ";
            else listItemString = $" {block.Order}. ";

            RenderNonWrappingText(listItemString);
            
            textIndent = listItemString.Length;
            RenderBlock(block as ContainerBlock);
            listLevel -= 1;

            textIndent = 0;
        }

        private void RenderBlock(ListBlock block)
        {
            RenderBlock(block as ContainerBlock);
            
        }

        private void RenderBlock(ParagraphBlock block)
        {
            RenderBlock(block as LeafBlock);

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

        private void RenderInline(CodeInline inline)
        {
            RenderWrappingText(inline.Content);
        }

        private void RenderInline(LeafInline inline)
        {
            RenderWrappingText(inline.ToString());
        }

        // Render a linebreak that occurs within a block, either due to a LineBreakInline in the markdown, or due to text wrapping
        private void RenderLineBreakIfNecessary()
        {
            if (!newLine) return;

            ImGui.NewLine();
            newLine = false;


            for (int i = 0; i < quoteLevel; ++i) RenderNonWrappingText("> ");
            RenderNonWrappingText(new string(' ', TotalTextIndent - 2*quoteLevel));
        }

        private void RenderInline(LineBreakInline inline)
        {
            newLine = true;
        }

        private void RenderWrappingText(string text)
        {
            // this line wrapping algorithm won't work in all cases, I think
            var lineBuilder = new StringBuilder();

            var windowWidth = ImGui.GetWindowSize().X;  // TODO: include padding in text area width, or make it a parameter
            var currentCursorX = ImGui.GetCursorPosX();
            var textExtent = currentCursorX + ImGui.CalcTextSize(text).X;

            if (textExtent < windowWidth)
            {
                RenderNonWrappingText(text);
                return;
            }

            int searchIndex = text.Length - 1;
            int blankIndex = text.LastIndexOf(' ', searchIndex);

            while (blankIndex > 0)
            {

                if (currentCursorX + ImGui.CalcTextSize(text.Substring(0,blankIndex)).X < windowWidth)
                {
                    RenderNonWrappingText(text.Substring(0,blankIndex).Trim());
                    newLine = true;
                    RenderLineBreakIfNecessary();   // ugly. Need to handle rendering newlines better...
                    RenderWrappingText(text.Substring(blankIndex).Trim());
                    return;
                }

                searchIndex = blankIndex - 1;
                blankIndex = text.LastIndexOf(' ', searchIndex);
            }

            // didn't find a place to break, so just render the whole line
            RenderNonWrappingText(text);
        }


        private void RenderNonWrappingText(string text)
        {
            RenderLineBreakIfNecessary();

            ImGui.Text(text);
            ImGui.SameLine();
        }
    }
}
