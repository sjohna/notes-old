using Markdig.Helpers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes
{
    public static class MarkdigExtensions
    {
        public static IEnumerable<StringLine> GetLines(this LeafBlock block)
        {
            if (block != null)
            {
                for (int i = 0; i < block.Lines.Count; ++i)
                {
                    yield return block.Lines.Lines[i];
                }
            }
        }
    }
}
