using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndoRedoSystem.UndoRedoCommands
{
    public interface IUndoRedoCommand : IEquatable<IUndoRedoCommand>
    {
        UndoRedoCommandTypesEnum CommandType { get; }
        string Id { get; }
        string Name { get; }
        int Index { get; }
    }
}
