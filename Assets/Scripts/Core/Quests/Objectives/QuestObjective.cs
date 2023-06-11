namespace HereticalSolutions.Quests
{
    public class QuestObjective
    {
        public int Index { get; private set; }
        
        public QuestObjectiveDescriptor Descriptor { get; private set; }

        public bool Active { get; set; }

        public int CurrentValue { get; set; }

        public QuestObjective(
            int index,
            QuestObjectiveDescriptor descriptor,
            bool active,
            int currentValue)
        {
            Index = index;
            
            Descriptor = descriptor;

            Active = active;

            CurrentValue = currentValue;
        }
    }
}
