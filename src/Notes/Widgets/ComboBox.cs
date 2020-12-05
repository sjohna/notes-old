using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Notes.Widgets
{
    public class ComboBox<TSelection> : IWidget
    {
        public string Name { get; private set; }

        private Func<IEnumerable<(string Text, TSelection Value)>> getOptions;

        private Func<(string Text, TSelection Value)> getSelectedItem;

        IEqualityComparer<TSelection> comparer;

        public ComboBox(string name, Func<IEnumerable<(string Text, TSelection Value)>> getOptions, Func<(string Text, TSelection Value)> getCurrentSelection, IEqualityComparer<TSelection> comparer = null)
        {
            this.Name = name;
            this.getOptions = getOptions;
            this.getSelectedItem = getCurrentSelection;
            this.comparer = comparer ?? EqualityComparer<TSelection>.Default;
        }

        public void Render()
        {
            if (ImGui.BeginCombo(Name, getSelectedItem().Text))
            {
                foreach (var option in getOptions())
                {
                    if (ImGui.Selectable(option.Text, comparer.Equals(getSelectedItem().Value, option.Value)))
                    {
                        ItemSelected?.Invoke(this, new StringListAltComboBoxSelectionEventArgs(option, !comparer.Equals(getSelectedItem().Value, option.Value)));
                    }
                }

                ImGui.EndCombo();
            }
        }

        public event EventHandler<StringListAltComboBoxSelectionEventArgs> ItemSelected;

        public class StringListAltComboBoxSelectionEventArgs
        {
            public bool SelectionChanged { get; private set; }

            public (string Text, TSelection Value) SelectedItem { get; private set; }

            public StringListAltComboBoxSelectionEventArgs((string Text, TSelection Value) selectedItem, bool selectionChanged)
            {
                SelectedItem = selectedItem;
                SelectionChanged = selectionChanged;
            }
        }
    }
}
