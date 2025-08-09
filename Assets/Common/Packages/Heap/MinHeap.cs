using System.Collections.Generic;

namespace Common.Packages.Heap
{
    public class MinHeap<T>
    {
        private List<(T item, int priority)> heap = new List<(T, int)>();
        private Dictionary<T, int> indexMap = new Dictionary<T, int>();

        public int Count => heap.Count;

        public void Insert(T item, int priority)
        {
            if (indexMap.ContainsKey(item))
            {
                InsertOrUpdate(item, priority);
                return;
            }

            heap.Add((item, priority));
            indexMap[item] = heap.Count - 1;
            HeapifyUp(heap.Count - 1);
        }

        public void InsertOrUpdate(T item, int priority)
        {
            if (indexMap.TryGetValue(item, out int index))
            {
                if (priority < heap[index].priority)
                {
                    heap[index] = (item, priority);
                    HeapifyUp(index);
                }
            }
            else
            {
                Insert(item, priority);
            }
        }

        public T ExtractMin()
        {
            var root = heap[0].item;
            var last = heap[heap.Count - 1];
            heap[0] = last;
            indexMap[last.item] = 0;

            heap.RemoveAt(heap.Count - 1);
            indexMap.Remove(root);

            HeapifyDown(0);
            return root;
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (heap[index].priority >= heap[parent].priority)
                    break;

                Swap(index, parent);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = index * 2 + 1;
                int right = index * 2 + 2;
                int smallest = index;

                if (left < heap.Count && heap[left].priority < heap[smallest].priority)
                    smallest = left;
                if (right < heap.Count && heap[right].priority < heap[smallest].priority)
                    smallest = right;

                if (smallest == index)
                    break;

                Swap(index, smallest);
                index = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            (heap[a], heap[b]) = (heap[b], heap[a]);
            indexMap[heap[a].item] = a;
            indexMap[heap[b].item] = b;
        }
    }
}
