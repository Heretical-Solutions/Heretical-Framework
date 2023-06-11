namespace HereticalSolutions.Quests
{
    [System.Serializable]
    public struct QuestDescriptor
    {
        public string ID;
        
        public string Name;

        public string Description;

        public QuestObjectiveDescriptor[] Objectives;
        
        public QuestStageDTO[] Stages;
    }
}
