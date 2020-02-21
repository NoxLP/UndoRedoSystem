using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndoRedoSystem.UndoRedoCommands
{
    public class UndoRedoNoParametersCommand : aUndoRedoCommandBase
    {
        internal UndoRedoNoParametersCommand(Action undo, Action redo, string id, string name) : base(id, name, UndoRedoCommandTypesEnum.NoParameters)
        {
            UndoAction = undo;
            RedoAction = redo;
        }
        internal UndoRedoNoParametersCommand(Func<Task> undo, Func<Task> redo, string id, string name) : base(id, name, UndoRedoCommandTypesEnum.NoParameters)
        {
            AsyncUndoAction = undo;
            AsyncRedoAction = redo;
        }

        public Action UndoAction { get; private set; }
        public Action RedoAction { get; private set; }
        public Func<Task> AsyncUndoAction { get; private set; }
        public Func<Task> AsyncRedoAction { get; private set; }

        internal void AddToActions(Action addToUndo, Action addToRedo)
        {
            if (addToUndo != null)
                UndoAction += addToUndo;
            if (addToRedo != null)
                RedoAction += addToRedo;
        }
        internal void AddToAsyncActions(Func<Task> addToUndo, Func<Task> addToRedo)
        {
            if (addToUndo != null)
                AsyncUndoAction += addToUndo;
            if (addToRedo != null)
                AsyncRedoAction += addToRedo;
        }
    }
}
