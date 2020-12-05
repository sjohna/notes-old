using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Widgets
{
    public interface IFrame
    {
        public string Name { get; }

        public void Render(float width, float height);
    }
}
