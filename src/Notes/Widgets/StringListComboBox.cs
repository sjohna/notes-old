using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;

namespace Notes.Widgets
{
    public class StringListComboBox : IWidget
    {
        public string Name { get; private set; }

        public string CurrentSelection { get; set; }

        private IEnumerable<string> options;



        public StringListComboBox(string name, IEnumerable<string> options, string currentSelection)
        {
            this.Name = name;
            this.CurrentSelection = currentSelection;
            this.options = options;
        }

        public void Render()
        {
            if (ImGui.BeginCombo(Name, CurrentSelection))
            {
                foreach (var option in options)
                {
                    if (ImGui.Selectable(option, CurrentSelection == option))
                    {
                        ItemSelected?.Invoke(this, new StringListComboBoxSelectionEventArgs(option, CurrentSelection == option));
                        CurrentSelection = option;
                    }
                }

                ImGui.EndCombo();
            }
        }

        public event EventHandler<StringListComboBoxSelectionEventArgs> ItemSelected;

        public class StringListComboBoxSelectionEventArgs
        {
            public string SelectedItem { get; private set; }

            public bool SelectionChanged { get; private set; }

            public StringListComboBoxSelectionEventArgs(string selectedItem, bool selectionChanged)
            {
                SelectedItem = selectedItem;
                SelectionChanged = selectionChanged; 
            }
        }
    }
}
