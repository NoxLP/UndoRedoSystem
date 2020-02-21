using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using UndoRedoSystem.UndoRedoCommands;
using NET471WPFVisualTreeHelperExtensions;

namespace UndoRedoSystem.View
{
    /// <summary>
    /// Lógica de interacción para UndoRedoCommandsBox.xaml
    /// </summary>
    public partial class UndoRedoCommandsBox : UserControl
    {
        public enum UndoRedoCommandsBoxTypeEnum { Undo, Redo }

        public UndoRedoCommandsBox()
        {
            InitializeComponent();
            if (BoxType == UndoRedoCommandsBoxTypeEnum.Undo)
            {
                var b = new Binding("UndoCommands") { Source = UndoRedoCommandManager.Instance, Mode = BindingMode.OneWay };
                BindingOperations.SetBinding(CommandsListBox, ListBox.ItemsSourceProperty, b);
            }
            else
            {
                var b = new Binding("RedoCommands") { Source = UndoRedoCommandManager.Instance, Mode = BindingMode.OneWay };
                BindingOperations.SetBinding(CommandsListBox, ListBox.ItemsSourceProperty, b);
            }
        }

        public static bool GetUndoRedoPreviousCommandSelected(DependencyObject obj)
        {
            return (bool)obj.GetValue(UndoRedoPreviousCommandSelectedProperty);
        }
        public static void SetUndoRedoPreviousCommandSelected(DependencyObject obj, bool value)
        {
            obj.SetValue(UndoRedoPreviousCommandSelectedProperty, value);
        }
        public static readonly DependencyProperty UndoRedoPreviousCommandSelectedProperty =
            DependencyProperty.RegisterAttached("UndoRedoPreviousCommandSelected", typeof(bool), typeof(UndoRedoCommandsBox), new PropertyMetadata(false));

        public aUndoRedoCommandBase SelectedUndoRedoCommand
        {
            get { return (aUndoRedoCommandBase)GetValue(SelectedUndoRedoCommandProperty); }
            set { SetValue(SelectedUndoRedoCommandProperty, value); }
        }
        public static readonly DependencyProperty SelectedUndoRedoCommandProperty =
            DependencyProperty.Register("SelectedUndoRedoCommand", typeof(aUndoRedoCommandBase), typeof(UndoRedoCommandsBox), new PropertyMetadata(null));
        public ICommand ItemsClickCommand
        {
            get { return (ICommand)GetValue(ItemsClickCommandProperty); }
            set { SetValue(ItemsClickCommandProperty, value); }
        }
        public static readonly DependencyProperty ItemsClickCommandProperty =
            DependencyProperty.Register("ItemsClickCommand", typeof(ICommand), typeof(UndoRedoCommandsBox), new PropertyMetadata(null));

        public UndoRedoCommandsBoxTypeEnum BoxType
        {
            get { return (UndoRedoCommandsBoxTypeEnum)GetValue(BoxTypeProperty); }
            set { SetValue(BoxTypeProperty, value); }
        }
        public static readonly DependencyProperty BoxTypeProperty =
            DependencyProperty.Register("BoxType", typeof(UndoRedoCommandsBoxTypeEnum), typeof(UndoRedoCommandsBox), new PropertyMetadata(UndoRedoCommandsBoxTypeEnum.Undo, BoxTypePropertyChanged));
        private static void BoxTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((UndoRedoCommandsBox)d).BoxTypeChanged();
        }
        private void BoxTypeChanged()
        {
            if (BoxType == UndoRedoCommandsBoxTypeEnum.Undo)
            {
                var b = new Binding("UndoCommands") { Source = UndoRedoCommandManager.Instance, Mode = BindingMode.OneWay };
                BindingOperations.SetBinding(CommandsListBox, ListBox.ItemsSourceProperty, b);
            }
            else
            {
                var b = new Binding("RedoCommands") { Source = UndoRedoCommandManager.Instance, Mode = BindingMode.OneWay };
                BindingOperations.SetBinding(CommandsListBox, ListBox.ItemsSourceProperty, b);
            }
        }

        private void LBItemBorder_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;
            var item = border.GetParentOfType<ListBoxItem>();
            CommandsListBox.SelectedItem = item;
            if (ItemsClickCommand.CanExecute(null))
                ItemsClickCommand.Execute(null);
        }
        private void LBItemBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            var item = border.GetParentOfType<ListBoxItem>();

            bool changeTo = true;
            for (int i = 0; i < CommandsListBox.Items.Count; i++)
            {
                var currentItem = CommandsListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                if (GetUndoRedoPreviousCommandSelected(currentItem) != changeTo)
                    SetUndoRedoPreviousCommandSelected(currentItem, changeTo);

                if (item.Equals(currentItem))
                    changeTo = false;
            }
        }
        private bool IsMouseOverItem(Visual item, Point mouseOverPoint)
        {
            Rect currentDescendantBounds = VisualTreeHelper.GetDescendantBounds(item);
            return currentDescendantBounds.Contains(mouseOverPoint);
        }

        private void CommandsListBox_MouseLeave(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < CommandsListBox.Items.Count; i++)
            {
                var currentItem = CommandsListBox.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;

                if (GetUndoRedoPreviousCommandSelected(currentItem))
                    SetUndoRedoPreviousCommandSelected(currentItem, false);
            }
        }
    }
}
