using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace UndoRedoSystem.View
{
    internal class UndoRedoCommandBoxListBoxItem : ListBoxItem
    {
        public bool UndoRedoPreviousCommandSelected
        {
            get { return (bool)GetValue(UndoRedoPreviousCommandSelectedProperty); }
            set { SetValue(UndoRedoPreviousCommandSelectedProperty, value); }
        }
        public static readonly DependencyProperty UndoRedoPreviousCommandSelectedProperty =
            DependencyProperty.Register("UndoRedoPreviousCommandSelected", typeof(bool), typeof(UndoRedoCommandBoxListBoxItem), new PropertyMetadata(false));
    }
}
