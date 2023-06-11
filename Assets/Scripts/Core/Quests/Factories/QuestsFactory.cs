using System.Collections.Generic;

using HereticalSolutions.MVVM.Observable;

using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Quests.Factories
{
    public static partial class QuestsFactory
    {
        public static Quest BuildQuest(
            QuestDescriptor questDescriptor,
            EQuestStatus status)
        {
            var objectives = new QuestObjective[questDescriptor.Objectives.Length];

            for (int i = 0; i < objectives.Length; i++)
                objectives[i] = BuildQuestObjective(
                    i,
                    questDescriptor.Objectives[i],
                    false,
                    0);
            
            return new Quest(
                questDescriptor,
                status,
                objectives);
        }

        private static QuestObjective BuildQuestObjective(
            int index,
            QuestObjectiveDescriptor descriptor,
            bool active,
            int currentValue)
        {
            return new QuestObjective(
                index,
                descriptor,
                active,
                currentValue);
        }

        public static ActiveQuest BuildActiveQuest(Quest quest)
        {
            return new ActiveQuest(
                quest,
                new QuestProgressTracker(
                    new List<ActiveQuestObjective>()));
        }

        public static ActiveQuestObjectivesManager BuildActiveQuestObjectivesManager()
        {
            return new ActiveQuestObjectivesManager(
                RepositoriesFactory.BuildDictionaryRepository<string, List<ActiveQuestObjective>>());
        }

        public static ActiveQuestObjective BuildActiveQuestObjective(
            QuestObjective questObjective)
        {
            ObservableProperty<int> observable = new ObservableProperty<int>(questObjective.CurrentValue);

            return new ActiveQuestObjective(
                questObjective,
                observable);
        }
    }
}
