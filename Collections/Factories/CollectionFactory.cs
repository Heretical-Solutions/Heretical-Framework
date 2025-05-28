namespace HereticalSolutions.Collections.Factories
{
    public class CollectionFactory
    {
        private const int DEFAULT_B_PLUS_TREE_DEGREE = 32;

        private int bPlusTreeDegree = DEFAULT_B_PLUS_TREE_DEGREE;

        public int BPlusTreeDegree => bPlusTreeDegree;


        private const int DEFAULT_CIRCULAR_BUFFER_CAPACITY = 1024;

        public int circularBufferCapacity = DEFAULT_CIRCULAR_BUFFER_CAPACITY;

        public int CircularBufferCapacity => circularBufferCapacity;

        #region B+ trees

        public BPlusTree<T> BuildBPlusTree<T>()
        {
            return new BPlusTree<T>(
                BPlusTreeDegree);
        }

        public BPlusTree<T> BuildBPlusTree<T>(
            int degree)
        {
            return new BPlusTree<T>(
                degree);
        }

        public BPlusTreeMap<TKey, TValue> BuildBPlusTreeMap<TKey, TValue>()
        {
            return new BPlusTreeMap<TKey, TValue>(
                BPlusTreeDegree);
        }

        public BPlusTreeMap<TKey, TValue> BuildBPlusTreeMap<TKey, TValue>(
            int degree)
        {
            return new BPlusTreeMap<TKey, TValue>(
                degree);
        }

        #endregion
    }
}