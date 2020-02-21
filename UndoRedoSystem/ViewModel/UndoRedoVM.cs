using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using UndoRedoSystem.UndoRedoCommands;
using WPFHelpers.Commands;

namespace UndoRedoSystem.ViewModel
{
    public class UndoRedoVM : WPFHelpers.ViewModelBase.aViewModelBase
    {
        public UndoRedoVM()
        {
            UndoUntilCommand = new DelegateCommand(x => UndoUntil(), null);
            RedoUntilCommand = new DelegateCommand(x => RedoUntil(), null);
            UndoSingleCommand = new DelegateCommand(x => UndoSingle(), null);
            RedoSingleCommand = new DelegateCommand(x => RedoSingle(), null);

            UndoUntilCommandAsync = new AsyncDelegateCommand(x => UndoUntilAsync(), null);
            RedoUntilCommandAsync = new AsyncDelegateCommand(x => RedoUntilAsync(), null);
            UndoSingleCommandAsync = new AsyncDelegateCommand(x => UndoSingleAsync(), null);
            RedoSingleCommandAsync = new AsyncDelegateCommand(x => RedoSingleAsync(), null);

            UndoRedoCommandManager.Instance._UndoCommands.CollectionChanged += _UndoCommands_CollectionChanged;
            UndoRedoCommandManager.Instance._RedoCommands.CollectionChanged += _RedoCommands_CollectionChanged;
        }
        
        #region fields
        private aUndoRedoCommandBase _UndoCommandSelectedItem;
        private aUndoRedoCommandBase _RedoCommandSelectedItem;
        private bool _SomeUndoCommand;
        private bool _SomeRedoCommand;
        #endregion

        #region properties
        public aUndoRedoCommandBase UndoCommandSelectedItem
        {
            get { return _UndoCommandSelectedItem; }
            set
            {
                if (value == null)
                {
                    if (_UndoCommandSelectedItem != null)
                    {
                        _UndoCommandSelectedItem = null;
                        OnPropertyChanged(nameof(UndoCommandSelectedItem));
                    }
                }
                else if (!value.Equals(_UndoCommandSelectedItem))
                {
                    _UndoCommandSelectedItem = value;
                    OnPropertyChanged(nameof(UndoCommandSelectedItem));
                }
            }
        }
        public aUndoRedoCommandBase RedoCommandSelectedItem
        {
            get { return _RedoCommandSelectedItem; }
            set
            {
                if (value == null)
                {
                    if (_RedoCommandSelectedItem != null)
                    {
                        _RedoCommandSelectedItem = null;
                        OnPropertyChanged(nameof(RedoCommandSelectedItem));
                    }
                }
                else if (!value.Equals(_RedoCommandSelectedItem))
                {
                    _RedoCommandSelectedItem = value;
                    OnPropertyChanged(nameof(RedoCommandSelectedItem));
                }
            }
        }
        public bool SomeUndoCommand
        {
            get { return _SomeUndoCommand; }
            set
            {
                if (value != _SomeUndoCommand)
                {
                    _SomeUndoCommand = value;
                    OnPropertyChanged(nameof(SomeUndoCommand));
                }
            }
        }
        public bool SomeRedoCommand
        {
            get { return _SomeRedoCommand; }
            set
            {
                if (value != _SomeRedoCommand)
                {
                    _SomeRedoCommand = value;
                    OnPropertyChanged(nameof(SomeRedoCommand));
                }
            }
        }
        #endregion

        #region commands
        public ICommand UndoUntilCommand { get; private set; }
        public ICommand RedoUntilCommand { get; private set; }
        public ICommand UndoSingleCommand { get; private set; }
        public ICommand RedoSingleCommand { get; private set; }
        public ICommand UndoUntilCommandAsync { get; private set; }
        public ICommand RedoUntilCommandAsync { get; private set; }
        public ICommand UndoSingleCommandAsync { get; private set; }
        public ICommand RedoSingleCommandAsync { get; private set; }

        private void UndoUntil()
        {
            UndoRedoCommandManager.Instance.UndoCommandsUntil(UndoCommandSelectedItem);
        }
        private void RedoUntil()
        {
            UndoRedoCommandManager.Instance.RedoCommandsUntil(RedoCommandSelectedItem);
        }
        private void UndoSingle()
        {
            UndoRedoCommandManager.Instance.UndoNextCommand();
        }
        private void RedoSingle()
        {
            UndoRedoCommandManager.Instance.RedoNextCommand();
        }
        private async Task UndoUntilAsync()
        {
            await UndoRedoCommandManager.Instance.UndoCommandsUntilAsync(UndoCommandSelectedItem);
        }
        private async Task RedoUntilAsync()
        {
            await UndoRedoCommandManager.Instance.RedoCommandsUntilAsync(RedoCommandSelectedItem);
        }
        private async Task UndoSingleAsync()
        {
            await UndoRedoCommandManager.Instance.UndoNextCommandAsync();
        }
        private async Task RedoSingleAsync()
        {
            await UndoRedoCommandManager.Instance.RedoNextCommandAsync();
        }
        #endregion

        private void _UndoCommands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SomeUndoCommand = UndoRedoCommandManager.Instance._UndoCommands.Count > 0;
        }
        private void _RedoCommands_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SomeRedoCommand = UndoRedoCommandManager.Instance._RedoCommands.Count > 0;
        }
    }
}
