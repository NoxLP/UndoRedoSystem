using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndoRedoSystem.UndoRedoCommands
{
    public class UndoRedoWithParametersCommand : aUndoRedoCommandBase
    {
        internal UndoRedoWithParametersCommand(Action<object[]> undo, object[] undoParams, Action<object[]> redo, object[] redoParams, string id, string name) 
            : base(id, name, UndoRedoCommandTypesEnum.WithParameters)
        {
            UndoAction = undo;
            RedoAction = redo;

            UndoParameters = undoParams;
            RedoParameters = redoParams;
        }
        internal UndoRedoWithParametersCommand(Func<object[], Task> undo, object[] undoParams, Func<object[], Task> redo, object[] redoParams, string id, string name) 
            : base(id, name, UndoRedoCommandTypesEnum.WithParameters)
        {
            AsyncUndoAction = undo;
            AsyncRedoAction = redo;

            UndoParameters = undoParams;
            RedoParameters = redoParams;
        }

        public object[] UndoParameters { get; private set; }
        public object[] RedoParameters { get; private set; }
        public Action<object[]> UndoAction { get; private set; }
        public Action<object[]> RedoAction { get; private set; }
        public Func<object[], Task> AsyncUndoAction { get; private set; }
        public Func<object[], Task> AsyncRedoAction { get; private set; }
    }
}
