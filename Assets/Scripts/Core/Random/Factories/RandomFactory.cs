namespace HereticalSolutions.RandomGeneration.Factories
{
    public static class RandomFactory
    {
        public static SystemRandomGenerator BuildSystemRandomGenerator()
        {
            return new SystemRandomGenerator();
        }
    }
}