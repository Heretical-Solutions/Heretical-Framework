namespace HereticalSolutions.Quests
{
    [System.Serializable]
    public struct QuestDTO
    {
        public string QuestID;

        public EQuestStatus Status;

        public QuestObjectiveDTO[] ObjectiveTrackersData;
    }
}