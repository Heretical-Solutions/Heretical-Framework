namespace HereticalSolutions.Quests
{
    [System.Serializable]
    public struct QuestObjectiveDescriptor
    {
        public string ObjectiveID;

        public string Description;

        
        public ETrackerComparison Comparison;
        
        public int ExpectedValue;
        
        
        public QuestPropertyDTO[] Properties;
    }
}