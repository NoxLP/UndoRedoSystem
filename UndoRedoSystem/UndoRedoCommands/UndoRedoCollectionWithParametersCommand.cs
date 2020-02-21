using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndoRedoSystem.UndoRedoCommands
{
    public class UndoRedoCollectionWithParametersCommand : aUndoRedoCommandBase
    {
        internal UndoRedoCollectionWithParametersCommand(string id, string name, bool sync) : base(id,name, UndoRedoCommandTypesEnum.CollWithParameters)
        {
            if (sync)
            {
                UndoActions = new Dictionary<Action<object[]>, object[]>();
                RedoActions = new Dictionary<Action<object[]>, object[]>();
            }
            else
            {
                AsyncUndoActions = new Dictionary<Func<object[], Task>, object[]>();
                AsyncRedoActions = new Dictionary<Func<object[], Task>, object[]>();
            }
        }
        internal UndoRedoCollectionWithParametersCommand(Dictionary<Action<object[]>, object[]> undoActions, Dictionary<Action<object[]>, object[]> redoActions, string id, string name)
            : base(id, name, UndoRedoCommandTypesEnum.CollWithParameters)
        {
            UndoActions = undoActions;
            RedoActions = redoActions;
        }
        internal UndoRedoCollectionWithParametersCommand(Dictionary<Func<object[], Task>, object[]> undoActions, Dictionary<Func<object[], Task>, object[]> redoActions, string id, string name)
            : base(id, name, UndoRedoCommandTypesEnum.CollWithParameters)
        {
            AsyncUndoActions = undoActions;
            AsyncRedoActions = redoActions;
        }

        public Dictionary<Action<object[]>, object[]> UndoActions { get; private set; }
        public Dictionary<Action<object[]>, object[]> RedoActions { get; private set; }
        public Dictionary<Func<object[], Task>, object[]> AsyncUndoActions { get; private set; }
        public Dictionary<Func<object[], Task>, object[]> AsyncRedoActions { get; private set; }
    }
}
