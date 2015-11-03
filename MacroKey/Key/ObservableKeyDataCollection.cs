using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;

namespace MacroKey.Key
{
    public class ObservableKeyDataCollection : INotifyCollectionChanged, INotifyPropertyChanged, IList<KeyData>
    {
        public IList<KeyData> Collection { get; }

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

        public KeyData this[int index]
        {
            get
            {
                return Collection[index];
            }

            set
            {
                Collection[index] = value;
            }
        }

        public ObservableKeyDataCollection()
        {
            Collection = new ObservableCollection<KeyData>();
        }

        public ObservableKeyDataCollection(IEnumerable<KeyData> keyDataEnumerable)
        {
            Collection = new ObservableCollection<KeyData>(keyDataEnumerable);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnPropertyChanged()
        {
            OnChangedProperty(this, new PropertyChangedEventArgs("Collection"));
        }

        protected virtual void OnChangedProperty(object sender, PropertyChangedEventArgs e)
        {
            var handler = PropertyChanged;
            if (handler != null)
                PropertyChanged(this, e);
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged();
            var handler = CollectionChanged;
            if (handler != null)
                CollectionChanged(sender, e);
        }

        public int IndexOf(KeyData item)
        {
            return Collection.IndexOf(item);
        }

        public void Insert(int index, KeyData item)
        {
            Collection.Insert(index, item);
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveAt(int index)
        {
            Collection.RemoveAt(index);
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(KeyData item)
        {
            Collection.Add(item);
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Clear()
        {
            Collection.Clear();
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyData item)
        {
            return Collection.Contains(item);
        }

        public void CopyTo(KeyData[] array, int arrayIndex)
        {
            Collection.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyData item)
        {
            var remObj = Collection.Remove(item);
            OnCollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return remObj;
        }

        public IEnumerator<KeyData> GetEnumerator()
        {
            return Collection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Collection.GetEnumerator();
        }
    }
}
