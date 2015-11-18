using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MacroKey
{
    [Serializable]
    public class ObservablePropertyCollection<T> : INotifyCollectionChanged, INotifyPropertyChanged, IList<T>
    {
        private IList<T> mCollection;
        public IList<T> Collection
        {
            get
            {
                return mCollection;
            }
            set
            {
                mCollection = value;
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }

        public int Count
        {
            get
            {
                return Collection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return Collection.IsReadOnly;
            }
        }

        public T this[int index]
        {
            get
            {
                return Collection[index];
            }
            set
            {
                T originalItem = Collection[index];
                Collection[index] = value;

                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, value, originalItem, index));
            }
        }

        public ObservablePropertyCollection()
        {
            Collection = new ObservableCollection<T>();
        }

        public ObservablePropertyCollection(IEnumerable<T> keyDataEnumerable)
        {
            Collection = new ObservableCollection<T>(keyDataEnumerable);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnPropertyChanged()
        {
            OnChangedProperty(new PropertyChangedEventArgs("Collection"));
        }

        protected virtual void OnChangedProperty(PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, e);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged();
            var handler = CollectionChanged;
            if (handler != null)
                CollectionChanged(this, e);
        }

        public int IndexOf(T item)
        {
            return Collection.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            Collection.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        }

        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, index));
        }

        public void Add(T item)
        {
            Collection.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
        }

        public void Clear()
        {
            Collection.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(T item)
        {
            return Collection.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            var remObj = Collection.Remove(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return remObj;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }
    }
}
