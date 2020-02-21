using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UndoRedoSystem.UndoRedoCommands;

namespace UndoRedoSystem
{
    public class UndoRedoDropoutStack : IEnumerable<aUndoRedoCommandBase>, IEnumerable, ICollection, IReadOnlyCollection<aUndoRedoCommandBase>, INotifyCollectionChanged
    {
        public UndoRedoDropoutStack(int maxCapacity)
        {
            _Items = new aUndoRedoCommandBase[maxCapacity];
            var s = new Stack<object>();
            
        }
        public UndoRedoDropoutStack(IEnumerable<aUndoRedoCommandBase> collection, int maxCapacity)
        {
            _Items = new aUndoRedoCommandBase[maxCapacity];
            foreach (var item in collection)
            {
                Push(item);
            }
        }

        private static object _LockObject = new object();
        private aUndoRedoCommandBase[] _Items;
        private int _Top = 0;

        public int Count => _Items.Count(x => x != null); //!EqualityComparer<aUndoRedoCommandBase>.Default.Equals(x, default(aUndoRedoCommandBase)));
        public object SyncRoot => this;
        public bool IsSynchronized => false;
        public static aUndoRedoCommandBase LastCommandAdded { get; private set; }

        public void Push(aUndoRedoCommandBase item)
        {
            _Items[_Top] = item;
            item.Index = _Top;
            _Top = (_Top + 1) % _Items.Length;
            lock (_LockObject)
            {
                LastCommandAdded = item;
            }
            UndoRedoCommandManager.Instance.UndoRedoStacksChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }
        public aUndoRedoCommandBase Pop()
        {
            _Top = (_Items.Length + _Top - 1) % _Items.Length;
            var item = _Items[_Top];

            //if (EqualityComparer<aUndoRedoCommandBase>.Default.Equals(item, default(aUndoRedoCommandBase)))
            if (item == null)
                throw new InvalidOperationException("The Stack is empty");

            _Items[_Top] = null;//default(aUndoRedoCommandBase);
            item.Index = -1;
            lock (_LockObject)
            {
                int i = (_Items.Length + _Top - 1) % _Items.Length;
                LastCommandAdded = _Items[(_Items.Length + _Top - 1) % _Items.Length];
            }
            UndoRedoCommandManager.Instance.UndoRedoStacksChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item));
            return item;
        }
        public aUndoRedoCommandBase Peek()
        {
            _Top = (_Items.Length + _Top - 1) % _Items.Length;
            var item = _Items[_Top];

            //if (EqualityComparer<aUndoRedoCommandBase>.Default.Equals(item, default(aUndoRedoCommandBase)))
            if (item == null)
                throw new InvalidOperationException("The Stack is empty");

            return item;
        }
        public bool ExchangeLastCommand(aUndoRedoCommandBase target)
        {
            var index = LastCommandAdded.Index; //Array.IndexOf(_Items, source);
            //if (index == -1)
            //    return false;
            target.Index = index;
            _Items[index] = target;
            lock (_LockObject)
            {
                LastCommandAdded = target;
            }
            UndoRedoCommandManager.Instance.UndoRedoStacksChanged();
            return true;
        }
        public void Clear()
        {
            for (int i = 0; i < _Items.Length; i++)
            {
                var item = _Items[i];
                if (!EqualityComparer<aUndoRedoCommandBase>.Default.Equals(item, default(aUndoRedoCommandBase)))
                    _Items[i] = default(aUndoRedoCommandBase);
            }
            _Top = 0;
            UndoRedoCommandManager.Instance.UndoRedoStacksChanged();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public void CopyTo(Array array, int index)
        {
            _Items.CopyTo(array, index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
        public IEnumerator<aUndoRedoCommandBase> GetEnumerator()
        {
            return _Items.AsEnumerable().GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _Items.GetEnumerator();
        }

        #region collection changed
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged(this, e);
            }
        }
        #endregion
    }
}
