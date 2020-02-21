using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UndoRedoSystem.UndoRedoCommands
{
    public abstract class aUndoRedoCommandBase : IUndoRedoCommand, IEquatable<aUndoRedoCommandBase>
    {
        internal aUndoRedoCommandBase(string id, string name, UndoRedoCommandTypesEnum type)
        {
            Id = id;
            Name = name;
            CommandType = type;
        }

        public string Id { get; private set; }
        public string Name { get; private set; }
        public int Index { get; internal set; }
        public UndoRedoCommandTypesEnum CommandType { get; private set; }

        public bool Equals(aUndoRedoCommandBase obj)
        {
            var command = obj as IUndoRedoCommand;
            return command != null && this.Equals(command);
        }
        public override bool Equals(object obj)
        {
            var command = obj as IUndoRedoCommand;
            return command != null && this.Equals(command);
        }
        public override int GetHashCode()
        {
            return 310941579 + EqualityComparer<string>.Default.GetHashCode(Id);
        }
        public bool Equals(IUndoRedoCommand other)
        {
            if (other == null)
                return false;
            return Id.Equals(other.Id);
        }
    }
}
