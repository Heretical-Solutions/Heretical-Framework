namespace HereticalSolutions.Allocations.Factories
{
    public static class IDAllocationsFactory
    {
        public static long BuildLongFromTwoRandomInts()
        {
            int value1 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            
            int value2 = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
            
            return value1 + ((long)value2 << 32);
        }
    }
}