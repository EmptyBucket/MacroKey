using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace MacroKey
{
    public class ObservableKeyDataCollection : IList<KeyData>, INotifyCollectionChanged
    {
        private IList<KeyData> mKeyDataCollection = new List<KeyData>();

        public int Count
        {
            get
            {
                return mKeyDataCollection.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return mKeyDataCollection.IsReadOnly;
            }
        }

        public KeyData this[int index]
        {
            get
            {
                return mKeyDataCollection[index];
            }

            set
            {
                mKeyDataCollection[index] = value;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ObservableKeyDataCollection(IEnumerable<KeyData> keyDataEnumerable)
        {
            mKeyDataCollection = new List<KeyData>(keyDataEnumerable);
        }

        public ObservableKeyDataCollection() { }

        public override string ToString()
        {
            Func<KeyData, string> getStr = keyData => keyData.KeyMessage == KeyData.KeyboardMessage.WM_KEYDOWM ? keyData.KeyValue.ToLower() : keyData.KeyValue.ToUpper();
            return string.Join("", mKeyDataCollection.Select(getStr));
        }

        public int IndexOf(KeyData item)
        {
            return mKeyDataCollection.IndexOf(item);
        }

        protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, e);
        }

        public void Insert(int index, KeyData item)
        {
            mKeyDataCollection.Insert(index, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void RemoveAt(int index)
        {
            mKeyDataCollection.RemoveAt(index);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Add(KeyData item)
        {
            mKeyDataCollection.Add(item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public void Clear()
        {
            mKeyDataCollection.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(KeyData item)
        {
            return mKeyDataCollection.Contains(item);
        }

        public void CopyTo(KeyData[] array, int arrayIndex)
        {
            mKeyDataCollection.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyData item)
        {
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            return mKeyDataCollection.Remove(item);
        }

        public IEnumerator<KeyData> GetEnumerator()
        {
            return mKeyDataCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return mKeyDataCollection.GetEnumerator();
        }
    }
}
