namespace JSLintNet.VisualStudio.Specifications.Fakes
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using EnvDTE;

    public class EnumerableFake<T> : IEnumerable
    {
        public EnumerableFake()
        {
            this.Items = new Dictionary<object, T>();
        }

        public DTE DTE { get; set; }

        public Dictionary<object, T> Items { get; set; }

        public bool OneBased { get; set; }

        public int Count
        {
            get { return this.Items.Count; }
        }

        public IEnumerator GetEnumerator()
        {
            return this.Items.Values.GetEnumerator();
        }

        public T Item(object index)
        {
            return this.Items[index];
        }

        public void AddKeyedItem(T value)
        {
            this.AddItem(Guid.NewGuid(), value);
        }

        public void AddIndexedItem(T value)
        {
            var next = this.OneBased ? this.Items.Count + 1 : this.Items.Count;

            this.Items.Add(next, value);
        }

        public void AddItem(object key, T value)
        {
            this.Items.Add(key, value);
        }
    }
}
