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

        public (string OptionText, TSelection Option) CurrentSelection { get; set; }

        private List<(string OptionText, TSelection Option)> options;

        IEqualityComparer<TSelection> comparer;

        public ComboBox(string name, IEnumerable<TSelection> options, TSelection currentSelection, IEqualityComparer<TSelection> comparer = null)
        {
            this.Name = name;
            this.CurrentSelection = (currentSelection.ToString(), currentSelection);
            this.options = new List<(string OptionText, TSelection Option)>();
            this.comparer = comparer ?? EqualityComparer<TSelection>.Default;

            foreach (var option in options)
            {
                this.options.Add((option.ToString(), option));
            }
        }

        public void SetCurrentSelection(TSelection selection)
        {
            foreach (var option in options)
            {
                if (comparer.Equals(option.Option, selection))
                {
                    CurrentSelection = option;
                    return;
                }
            }

            // TODO: better exception?
            throw new Exception("Invalid selection!");
        }

        public void Render()
        {
            if (ImGui.BeginCombo(Name, CurrentSelection.OptionText))
            {
                foreach (var option in options)
                {
                    if (ImGui.Selectable(option.OptionText, comparer.Equals(CurrentSelection.Option, option.Option)))
                    {
                        ItemSelected?.Invoke(this, new StringListComboBoxSelectionEventArgs(option.OptionText, option.Option, comparer.Equals(CurrentSelection.Option, option.Option)));
                        CurrentSelection = option;
                    }
                }

                ImGui.EndCombo();
            }
        }

        public event EventHandler<StringListComboBoxSelectionEventArgs> ItemSelected;

        public class StringListComboBoxSelectionEventArgs
        {
            public string SelectedItemText { get; private set; }

            public TSelection SelectedItem { get; private set; }

            public bool SelectionChanged { get; private set; }

            public StringListComboBoxSelectionEventArgs(string selectedItemText, TSelection selectedItem, bool selectionChanged)
            {
                SelectedItemText = selectedItemText;
                SelectedItem = selectedItem;
                SelectionChanged = selectionChanged; 
            }
        }
    }
}
