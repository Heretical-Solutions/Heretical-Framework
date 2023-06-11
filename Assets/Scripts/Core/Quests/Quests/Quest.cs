namespace HereticalSolutions.Quests
{
    public class Quest
    {
        public QuestDescriptor Descriptor { get; private set; }

        public EQuestStatus Status { get; set; }

        public QuestObjective[] Objectives { get; private set; }

        public Quest(
            QuestDescriptor descriptor,
            EQuestStatus status,
            QuestObjective[] objectives)
        {
            Descriptor = descriptor;

            Status = status;

            Objectives = objectives;
        }
    }
}
