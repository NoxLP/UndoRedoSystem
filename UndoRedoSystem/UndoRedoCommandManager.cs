using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using UndoRedoSystem.UndoRedoCommands;

namespace UndoRedoSystem
{
    public class UndoRedoCommandManager : INotifyPropertyChanged
    {
        #region fields
        private SemaphoreSlim _Semaphore = new SemaphoreSlim(1);
        internal UndoRedoDropoutStack _UndoCommands = new UndoRedoDropoutStack(Properties.Settings.Default.MaxUndoRedoStackCapacity);
        internal UndoRedoDropoutStack _RedoCommands = new UndoRedoDropoutStack(Properties.Settings.Default.MaxUndoRedoStackCapacity);
        private CancellationTokenSource _CTS = new CancellationTokenSource();
        #endregion

        #region properties
        public static UndoRedoCommandManager Instance { get; } = new UndoRedoCommandManager();
        public int UndoCommandsCount { get { return _UndoCommands.Count; } }
        public bool LastCommandIsNull { get { return UndoRedoDropoutStack.LastCommandAdded == null; } }
        public string LastCommandName { get { return UndoRedoDropoutStack.LastCommandAdded == null ? "" : UndoRedoDropoutStack.LastCommandAdded.Name; } }
        public ICollectionView UndoCommands
        {
            get
            {
                var coll = CollectionViewSource.GetDefaultView(_UndoCommands.Where(x => x != null));
                coll.SortDescriptions.Add(new SortDescription(nameof(aUndoRedoCommandBase.Index), ListSortDirection.Descending));
                return coll;
            }
        }
        public ICollectionView RedoCommands
        {
            get
            {
                var coll = CollectionViewSource.GetDefaultView(_RedoCommands.Where(x => x != null));
                coll.SortDescriptions.Add(new SortDescription(nameof(aUndoRedoCommandBase.Index), ListSortDirection.Descending));
                return coll;
            }
        }
        #endregion

        public void ClearAll()
        {
            _CTS.Cancel();
            _UndoCommands.Clear();
            _RedoCommands.Clear();
        }

        #region new command
        /// <summary>
        /// Don't use semaphore
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        public UndoRedoNoParametersCommand NewCommand(string name, Action undo, Action redo)
        {
            var command = new UndoRedoNoParametersCommand(undo, redo, Guid.NewGuid().ToString(), name);
            ////UndoRedoDropoutStack.LastCommandAdded = command;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            return command;
        }
        /// <summary>
        /// Don't use semaphore
        /// </summary>
        /// <param name="name"></param>
        /// <param name="undo"></param>
        /// <param name="undoParams"></param>
        /// <param name="redo"></param>
        /// <param name="redoParams"></param>
        /// <returns></returns>
        public UndoRedoWithParametersCommand NewCommand(string name, Action<object[]> undo, object[] undoParams, Action<object[]> redo, object[] redoParams)
        {
            var command = new UndoRedoWithParametersCommand(undo, undoParams, redo, redoParams, Guid.NewGuid().ToString(), name);
            //UndoRedoDropoutStack.LastCommandAdded = command;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            return command;
        }
        /// <summary>
        /// Don't use semaphore
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        public UndoRedoNoParametersCommand NewAsyncCommand(string name, Func<Task> undo, Func<Task> redo)
        {
            var command = new UndoRedoNoParametersCommand(undo, redo, Guid.NewGuid().ToString(), name);
            //UndoRedoDropoutStack.LastCommandAdded = command;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            return command;
        }
        /// <summary>
        /// Don't use semaphore
        /// </summary>
        /// <param name="name"></param>
        /// <param name="undo"></param>
        /// <param name="undoParams"></param>
        /// <param name="redo"></param>
        /// <param name="redoParams"></param>
        /// <returns></returns>
        public UndoRedoWithParametersCommand NewAsyncCommand(string name, Func<object[], Task> undo, object[] undoParams, Func<object[], Task> redo, object[] redoParams)
        {
            var command = new UndoRedoWithParametersCommand(undo, undoParams, redo, redoParams, Guid.NewGuid().ToString(), name);
            //UndoRedoDropoutStack.LastCommandAdded = command;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            return command;
        }
        /// <summary>
        /// Use semaphore.WaitAsync
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        public async Task<UndoRedoNoParametersCommand> NewCommandAsync(string name, Action undo, Action redo)
        {
            var token = _CTS.Token;
            await _Semaphore.WaitAsync();
            if (token.IsCancellationRequested)
                return null;
            var command = new UndoRedoNoParametersCommand(undo, redo, Guid.NewGuid().ToString(), name);
            //UndoRedoDropoutStack.LastCommandAdded = command;
            if (token.IsCancellationRequested)
                return null;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            _Semaphore.Release();
            return command;
        }
        /// <summary>
        /// Use semaphore.WaitAsync
        /// </summary>
        /// <param name="name"></param>
        /// <param name="undo"></param>
        /// <param name="undoParams"></param>
        /// <param name="redo"></param>
        /// <param name="redoParams"></param>
        /// <returns></returns>
        public async Task<UndoRedoWithParametersCommand> NewCommandAsync(string name, Action<object[]> undo, object[] undoParams, Action<object[]> redo, object[] redoParams)
        {
            var token = _CTS.Token;
            await _Semaphore.WaitAsync();
            if (token.IsCancellationRequested)
                return null;
            var command = new UndoRedoWithParametersCommand(undo, undoParams, redo, redoParams, Guid.NewGuid().ToString(), name);
            //UndoRedoDropoutStack.LastCommandAdded = command;
            if (token.IsCancellationRequested)
                return null;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            _Semaphore.Release();
            return command;
        }
        /// <summary>
        /// Use semaphore.WaitAsync
        /// </summary>
        /// <param name="undo"></param>
        /// <param name="redo"></param>
        public async Task<UndoRedoNoParametersCommand> NewAsyncCommandAsync(string name, Func<Task> undo, Func<Task> redo)
        {
            var token = _CTS.Token;
            await _Semaphore.WaitAsync();
            if (token.IsCancellationRequested)
                return null;
            var command = new UndoRedoNoParametersCommand(undo, redo, Guid.NewGuid().ToString(), name);
            //UndoRedoDropoutStack.LastCommandAdded = command;
            if (token.IsCancellationRequested)
                return null;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            _Semaphore.Release();
            return command;
        }
        /// <summary>
        /// Use semaphore.WaitAsync
        /// </summary>
        /// <param name="name"></param>
        /// <param name="undo"></param>
        /// <param name="undoParams"></param>
        /// <param name="redo"></param>
        /// <param name="redoParams"></param>
        /// <returns></returns>
        public async Task<UndoRedoWithParametersCommand> NewAsyncCommandAsync(string name, Func<object[], Task> undo, object[] undoParams, Func<object[], Task> redo, object[] redoParams)
        {
            var token = _CTS.Token;
            await _Semaphore.WaitAsync();
            if (token.IsCancellationRequested)
                return null;
            var command = new UndoRedoWithParametersCommand(undo, undoParams, redo, redoParams, Guid.NewGuid().ToString(), name);
            //UndoRedoDropoutStack.LastCommandAdded = command;
            if (token.IsCancellationRequested)
                return null;
            _UndoCommands.Push(command);
            _RedoCommands.Clear();
            
            _Semaphore.Release();
            return command;
        }

        /// <summary>
        /// This creates an empty command waiting to be filled with AddToLastCommand. Take in mind that if you add a new command this one would stop to work, 
        /// since it won't be the last command anymore.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public UndoRedoCollectionWithParametersCommand NewEmptyCollectionCommand(string name)
        {
            var command = new UndoRedoCollectionWithParametersCommand(Guid.NewGuid().ToString(), name, true);

            _UndoCommands.Push(command);
            _RedoCommands.Clear();

            return command;
        }
        /// <summary>
        /// This creates an empty command waiting to be filled with AddToLastCommand. Take in mind that if you add a new command this one would stop to work, 
        /// since it won't be the last command anymore.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task<UndoRedoCollectionWithParametersCommand> NewEmptyCollectionCommandAsync(string name)
        {
            var command = new UndoRedoCollectionWithParametersCommand(Guid.NewGuid().ToString(), name, false);

            _UndoCommands.Push(command);
            _RedoCommands.Clear();

            return command;
        }

        public void AddToLastCommand(Action undo, Action redo)
        {
            var command = UndoRedoDropoutStack.LastCommandAdded as UndoRedoNoParametersCommand;
            if (command == null)
                throw new ArgumentException("Trying to add no parameters action to a command with parameters");
            else if (command.AsyncUndoAction != null)
                throw new ArgumentException("Trying to add sync action to a command with async actions");

            command.AddToActions(undo, redo);
        }
        public void AddToLastCommand(Func<Task> undo, Func<Task> redo)
        {
            var command = UndoRedoDropoutStack.LastCommandAdded as UndoRedoNoParametersCommand;
            if (command == null)
                throw new ArgumentException("Trying to add no parameters action to a command with parameters");
            else if (command.AsyncUndoAction != null)
                throw new ArgumentException("Trying to add async action to a command with sync actions");

            command.AddToAsyncActions(undo, redo);
        }
        public void AddToLastCommand(Action<object[]> undo, object[] undoParams, Action<object[]> redo, object[] redoParams)
        {
            switch (UndoRedoDropoutStack.LastCommandAdded.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    throw new ArgumentException("Trying to add action with parameters to a command without parameters");
                case UndoRedoCommandTypesEnum.WithParameters:
                    var paramsCommand = UndoRedoDropoutStack.LastCommandAdded as UndoRedoWithParametersCommand;
                    if (paramsCommand.AsyncUndoAction != null)
                        throw new ArgumentException("Trying to add sync action to a command with async actions");

                    var undoDict = new Dictionary<Action<object[]>, object[]>();
                    undoDict.Add(paramsCommand.UndoAction, paramsCommand.UndoParameters);
                    if (undo != null)
                    {
                        if (undoParams == null)
                            throw new ArgumentNullException();
                        undoDict.Add(undo, undoParams);
                    }
                    var redoDict = new Dictionary<Action<object[]>, object[]>();
                    redoDict.Add(paramsCommand.RedoAction, paramsCommand.RedoParameters);
                    if (redo != null)
                    {
                        if (redoParams == null)
                            throw new ArgumentNullException();
                        redoDict.Add(redo, redoParams);
                    }
                    var newCommand = new UndoRedoCollectionWithParametersCommand(undoDict, redoDict, Guid.NewGuid().ToString(), paramsCommand.Name);

                    if (!_UndoCommands.ExchangeLastCommand(newCommand))
                    {
                        if (!_RedoCommands.ExchangeLastCommand(newCommand))
                            throw new ArgumentException();
                        //UndoRedoDropoutStack.LastCommandAdded = newCommand;
                    }
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var paramsCollCommand = UndoRedoDropoutStack.LastCommandAdded as UndoRedoCollectionWithParametersCommand;
                    if (paramsCollCommand.AsyncUndoActions != null)
                        throw new ArgumentException("Trying to add sync action to a command with async actions");
                    if (undo != null)
                    {
                        if (undoParams == null)
                            throw new ArgumentNullException();
                        paramsCollCommand.UndoActions.Add(undo, undoParams);
                    }
                    if (redo != null)
                    {
                        if (redoParams == null)
                            throw new ArgumentNullException();
                        paramsCollCommand.RedoActions.Add(redo, redoParams);
                    }
                    break;
            }
        }
        public void AddToLastCommand(Func<object[], Task> undo, object[] undoParams, Func<object[], Task> redo, object[] redoParams)
        {
            switch (UndoRedoDropoutStack.LastCommandAdded.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    throw new ArgumentException("Trying to add action with parameters to a command without parameters");
                case UndoRedoCommandTypesEnum.WithParameters:
                    var paramsCommand = UndoRedoDropoutStack.LastCommandAdded as UndoRedoWithParametersCommand;
                    if (paramsCommand.AsyncUndoAction != null)
                        throw new ArgumentException("Trying to add async action to a command with sync actions");

                    var undoDict = new Dictionary<Func<object[], Task>, object[]>();
                    if (undo != null)
                    {
                        if (undoParams == null)
                            throw new ArgumentNullException();
                        undoDict.Add(paramsCommand.AsyncUndoAction, paramsCommand.UndoParameters);
                        undoDict.Add(undo, undoParams);
                    }
                    var redoDict = new Dictionary<Func<object[], Task>, object[]>();
                    if (redo != null)
                    {
                        if (redoParams == null)
                            throw new ArgumentNullException();
                        redoDict.Add(paramsCommand.AsyncRedoAction, paramsCommand.RedoParameters);
                        redoDict.Add(redo, redoParams);
                    }
                    var newCommand = new UndoRedoCollectionWithParametersCommand(undoDict, redoDict, Guid.NewGuid().ToString(), paramsCommand.Name);

                    if (!_UndoCommands.ExchangeLastCommand(newCommand))
                    {
                        if (!_RedoCommands.ExchangeLastCommand(newCommand))
                            throw new ArgumentException();
                    }
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var paramsCollCommand = UndoRedoDropoutStack.LastCommandAdded as UndoRedoCollectionWithParametersCommand;
                    if (paramsCollCommand.AsyncUndoActions != null)
                        throw new ArgumentException("Trying to add async action to a command with sync actions");
                    if (undo != null)
                    {
                        if (undoParams == null)
                            throw new ArgumentNullException();
                        paramsCollCommand.AsyncUndoActions.Add(undo, undoParams);
                    }
                    if (redo != null)
                    {
                        if (redoParams == null)
                            throw new ArgumentNullException();
                        paramsCollCommand.AsyncRedoActions.Add(redo, redoParams);
                    }
                    break;
            }
        }
        #endregion

        #region undo
        /// <summary>
        /// Don't use semaphore. If command has correct UndoAction, does UndoAction. Else do Task.Run(AsyncUndoAction)
        /// </summary>
        public void UndoNextCommand()
        {
            if (_UndoCommands.Count == 0)
                return;

            var commandBase = _UndoCommands.Pop();
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.UndoAction == null)
                        //Task.Run(command.AsyncUndoAction);
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");
                    else
                        command.UndoAction();
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.UndoAction == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");//Task.Run(() => commandWParams.AsyncUndoAction(commandWParams.UndoParameters));
                    else
                        commandWParams.UndoAction(commandWParams.UndoParameters);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.UndoActions == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");
                    else
                    {
                        foreach (var kvp in collCommand.UndoActions)
                        {
                            kvp.Key(kvp.Value);
                        }
                    }
                    break;
            }
            _RedoCommands.Push(commandBase);
        }
        private aUndoRedoCommandBase UndoNextAndReturnCommand()
        {
            if (_UndoCommands.Count == 0)
                return null;

            var commandBase = _UndoCommands.Pop();
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.UndoAction == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");//Task.Run(command.AsyncUndoAction);
                    else
                        command.UndoAction();
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.UndoAction == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");//Task.Run(() => commandWParams.AsyncUndoAction(commandWParams.UndoParameters));
                    else
                        commandWParams.UndoAction(commandWParams.UndoParameters);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.UndoActions == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");
                    else
                    {
                        foreach (var kvp in collCommand.UndoActions)
                        {
                            kvp.Key(kvp.Value);
                        }
                    }
                    break;
            }
            _RedoCommands.Push(commandBase);

            return commandBase;
        }
        /// <summary>
        /// Don't use semaphore. If commands have correct UndoAction, does UndoAction. Else do Task.Run(AsyncUndoAction)
        /// </summary>
        /// <param name="lastCommand"></param>
        public void UndoCommandsUntil(aUndoRedoCommandBase lastCommand)
        {
            aUndoRedoCommandBase currentCommand;
            while (true)
            {
                currentCommand = UndoNextAndReturnCommand();
                if (currentCommand == null || currentCommand.Equals(lastCommand))
                    break;
            }
            //do
            //{
            //    currentCommand = UndoNextAndReturnCommand();
            //}
            //while (currentCommand != null && !currentCommand.Equals(lastCommand));
            //while ((currentCommand = UndoNextAndReturnCommand()) != null && !currentCommand.Equals(lastCommand)) ;
        }
        /// <summary>
        /// Use semaphore.WaitAsync. If command has correct AsyncUndoAction, does AsyncUndoAction. Else do Task.Run(UndoAction)
        /// </summary>
        /// <returns></returns>
        public async Task UndoNextCommandAsync()
        {
            await _Semaphore.WaitAsync();
            if (_UndoCommands.Count == 0)
                return;

            var token = _CTS.Token;
            var commandBase = _UndoCommands.Pop();
            if (token.IsCancellationRequested)
                return;
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.AsyncUndoAction == null)
                        await command.AsyncUndoAction();
                    else
                        await Task.Run(command.UndoAction);

                    //_RedoCommands.Push(command);
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.AsyncUndoAction == null)
                        await commandWParams.AsyncUndoAction(commandWParams.UndoParameters);
                    else
                        await Task.Run(() => commandWParams.UndoAction(commandWParams.UndoParameters));

                    //_RedoCommands.Push(commandWParams);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.UndoActions == null)
                    {
                        foreach (var kvp in collCommand.AsyncUndoActions)
                        {
                            await kvp.Key(kvp.Value);
                            if (token.IsCancellationRequested)
                                return;
                        }
                    }
                    else
                    {
                        foreach (var kvp in collCommand.UndoActions)
                        {
                            await Task.Run(() => kvp.Key(kvp.Value));
                            if (token.IsCancellationRequested)
                                return;
                        }
                    }

                    //_RedoCommands.Push(collCommand);
                    break;
            }

            if (token.IsCancellationRequested)
                return;
            _RedoCommands.Push(commandBase);
            _Semaphore.Release();
        }
        private async Task<aUndoRedoCommandBase> UndoNextAndReturnCommandAsync()
        {
            if (_UndoCommands.Count == 0)
                return null;

            var token = _CTS.Token;
            var commandBase = _UndoCommands.Pop();
            if (token.IsCancellationRequested)
                return null;
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.UndoAction == null)
                        await command.AsyncUndoAction();
                    else
                        await Task.Run(() => command.UndoAction());

                    //_RedoCommands.Push(command);
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.UndoAction == null)
                        await commandWParams.AsyncUndoAction(commandWParams.UndoParameters);
                    else
                        await Task.Run(() =>commandWParams.UndoAction(commandWParams.UndoParameters));

                    //_RedoCommands.Push(commandWParams);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.UndoActions == null)
                    {
                        foreach (var kvp in collCommand.AsyncUndoActions)
                        {
                            await kvp.Key(kvp.Value);
                            if (token.IsCancellationRequested)
                                return null;
                        }
                    }
                    else
                    {
                        foreach (var kvp in collCommand.UndoActions)
                        {
                            await Task.Run(() => kvp.Key(kvp.Value));
                            if (token.IsCancellationRequested)
                                return null;
                        }
                    }

                    //_RedoCommands.Push(collCommand);
                    break;
            }

            if (token.IsCancellationRequested)
                return null;
            _RedoCommands.Push(commandBase);

            return commandBase;
        }
        /// <summary>
        /// Use semaphore.WaitAsync. If command has correct AsyncUndoAction, does AsyncUndoAction. Else do Task.Run(UndoAction)
        /// </summary>
        /// <param name="lastCommand"></param>
        /// <returns></returns>
        public async Task UndoCommandsUntilAsync(aUndoRedoCommandBase lastCommand)
        {
            aUndoRedoCommandBase currentCommand;
            var token = _CTS.Token;
            while ((currentCommand = await UndoNextAndReturnCommandAsync()) != null && !currentCommand.Equals(lastCommand))
            {
                if (token.IsCancellationRequested)
                    return;
            }
        }
        #endregion

        #region redo
        /// <summary>
        /// Don't use semaphore. If command has correct RedoAction, does RedoAction. Else do Task.Run(AsyncRedoAction)
        /// </summary>
        public void RedoNextCommand()
        {
            if (_RedoCommands.Count == 0)
                return;

            var commandBase = _RedoCommands.Pop();
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.RedoAction == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");//Task.Run(command.AsyncRedoAction);
                    else
                        command.RedoAction();

                    //_UndoCommands.Push(command);
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.RedoAction == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");//Task.Run(() => commandWParams.AsyncRedoAction(commandWParams.RedoParameters));
                    else
                        commandWParams.RedoAction(commandWParams.RedoParameters);

                    //_UndoCommands.Push(commandWParams);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.RedoActions == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");
                    else
                    {
                        foreach (var kvp in collCommand.RedoActions)
                        {
                            kvp.Key(kvp.Value);
                        }
                    }

                    //_UndoCommands.Push(collCommand);
                    break;
            }
            _UndoCommands.Push(commandBase);
        }
        private aUndoRedoCommandBase RedoNextAndReturnCommand()
        {
            if (_RedoCommands.Count == 0)
                return null;

            var commandBase = _RedoCommands.Pop();
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.RedoAction == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");//Task.Run(command.AsyncRedoAction);
                    else
                        command.RedoAction();
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.RedoAction == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");//Task.Run(() => commandWParams.AsyncRedoAction(commandWParams.RedoParameters));
                    else
                        commandWParams.RedoAction(commandWParams.RedoParameters);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.RedoActions == null)
                        throw new InvalidOperationException("Trying to execute an async command from a sync method");
                    else
                    {
                        foreach (var kvp in collCommand.RedoActions)
                        {
                            kvp.Key(kvp.Value);
                        }
                    }
                    break;
            }
            _UndoCommands.Push(commandBase);

            return commandBase;
        }
        /// <summary>
        /// Don't use semaphore. If commands have correct RedoAction, does RedoAction. Else do Task.Run(AsyncRedoAction)
        /// </summary>
        /// <param name="lastCommand"></param>
        public void RedoCommandsUntil(aUndoRedoCommandBase lastCommand)
        {
            aUndoRedoCommandBase currentCommand;
            while ((currentCommand = RedoNextAndReturnCommand()) != null && !currentCommand.Equals(lastCommand)) ;
        }
        /// <summary>
        /// Use semaphore.WaitAsync. If command has correct AsyncRedoAction, does AsyncRedoAction. Else do Task.Run(RedoAction)
        /// </summary>
        /// <returns></returns>
        public async Task RedoNextCommandAsync()
        {
            await _Semaphore.WaitAsync();
            if (_RedoCommands.Count == 0)
                return;

            var token = _CTS.Token;
            var commandBase = _RedoCommands.Pop();
            if (token.IsCancellationRequested)
                return;
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.AsyncRedoAction == null)
                        await command.AsyncRedoAction();
                    else
                        await Task.Run(command.RedoAction);

                    //_UndoCommands.Push(command);
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.AsyncRedoAction == null)
                        await commandWParams.AsyncRedoAction(commandWParams.RedoParameters);
                    else
                        await Task.Run(() => commandWParams.RedoAction(commandWParams.RedoParameters));

                    //_UndoCommands.Push(commandWParams);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.RedoActions == null)
                    {
                        foreach (var kvp in collCommand.AsyncRedoActions)
                        {
                            await kvp.Key(kvp.Value);
                            if (token.IsCancellationRequested)
                                return;
                        }
                    }
                    else
                    {
                        foreach (var kvp in collCommand.AsyncRedoActions)
                        {
                            await Task.Run(() => kvp.Key(kvp.Value));
                            if (token.IsCancellationRequested)
                                return;
                        }
                    }

                    //_UndoCommands.Push(collCommand);
                    break;
            }

            if (token.IsCancellationRequested)
                return;
            _UndoCommands.Push(commandBase);
            _Semaphore.Release();
        }
        public async Task<aUndoRedoCommandBase> RedoNextAndReturnCommandAsync()
        {
            await _Semaphore.WaitAsync();
            if (_RedoCommands.Count == 0)
                return null;

            var token = _CTS.Token;
            var commandBase = _RedoCommands.Pop();
            if (token.IsCancellationRequested)
                return null;
            switch (commandBase.CommandType)
            {
                case UndoRedoCommandTypesEnum.NoParameters:
                    var command = commandBase as UndoRedoNoParametersCommand;
                    if (command.AsyncRedoAction == null)
                        await command.AsyncRedoAction();
                    else
                        await Task.Run(command.RedoAction);

                    //_UndoCommands.Push(command);
                    break;
                case UndoRedoCommandTypesEnum.WithParameters:
                    var commandWParams = commandBase as UndoRedoWithParametersCommand;
                    if (commandWParams.AsyncRedoAction == null)
                        await commandWParams.AsyncRedoAction(commandWParams.RedoParameters);
                    else
                        await Task.Run(() => commandWParams.RedoAction(commandWParams.RedoParameters));

                    //_UndoCommands.Push(commandWParams);
                    break;
                case UndoRedoCommandTypesEnum.CollWithParameters:
                    var collCommand = commandBase as UndoRedoCollectionWithParametersCommand;
                    if (collCommand.RedoActions == null)
                    {
                        foreach (var kvp in collCommand.AsyncRedoActions)
                        {
                            await kvp.Key(kvp.Value);
                            if (token.IsCancellationRequested)
                                return null;
                        }
                    }
                    else
                    {
                        foreach (var kvp in collCommand.AsyncRedoActions)
                        {
                            await Task.Run(() => kvp.Key(kvp.Value));
                            if (token.IsCancellationRequested)
                                return null;
                        }
                    }

                    //_UndoCommands.Push(collCommand);
                    break;
            }

            if (token.IsCancellationRequested)
                return null;
            _UndoCommands.Push(commandBase);
            _Semaphore.Release();

            return commandBase;
        }
        /// <summary>
        /// Use semaphore.WaitAsync. If command has correct AsyncRedoAction, does AsyncRedoAction. Else do Task.Run(RedoAction)
        /// </summary>
        /// <param name="lastCommand"></param>
        /// <returns></returns>
        public async Task RedoCommandsUntilAsync(aUndoRedoCommandBase lastCommand)
        {
            aUndoRedoCommandBase currentCommand;
            var token = _CTS.Token;
            while ((currentCommand = await RedoNextAndReturnCommandAsync()) != null && !currentCommand.Equals(lastCommand))
            {
                if (token.IsCancellationRequested)
                    return;
            }
        }
        #endregion

        #region prop changed
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        internal void UndoRedoStacksChanged()
        {
            UndoCommands.Refresh();
            OnPropertyChanged(nameof(UndoCommands));
            RedoCommands.Refresh();
            OnPropertyChanged(nameof(RedoCommands));
        }
        #endregion
    }
}
