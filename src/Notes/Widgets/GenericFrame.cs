using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace Notes.Widgets
{
    public class GenericFrame : IFrame
    {
        public string Name { get; private set; }

        private List<IWidget> m_widgets;

        public GenericFrame(string name, params IWidget[] widgets)
        {
            this.Name = name;
            m_widgets = new List<IWidget>();
            m_widgets.AddRange(widgets);
        }

        public void Render(float width, float height)
        {
            ImGui.BeginChild(Name, new Vector2(width, height), true);

            foreach (var widget in m_widgets)
            {
                widget.Render();
            }

            ImGui.End();
        }
    }
}
