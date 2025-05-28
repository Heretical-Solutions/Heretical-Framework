using HereticalSolutions.Collections;

namespace HereticalSolutions.ObjectPools.Managed
{
    public class IndexedMetadata
        : IIndexed
    {
        public int Index { get; set; }

        public IndexedMetadata()
        {
            Index = -1;
        }
    }
}