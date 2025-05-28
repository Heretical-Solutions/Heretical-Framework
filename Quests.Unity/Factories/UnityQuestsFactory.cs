using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

using HereticalSolutions.Quests.Factories;

namespace HereticalSolutions.Quests.Unity.Factories
{
    public class UnityQuestsFactory
    {
        private readonly RepositoryFactory repositoryFactory;

        private readonly QuestFactory questFactory;

        private readonly QuestStageFactory questStageFactory;

        public UnityQuestsFactory(
            RepositoryFactory repositoryFactory,
            QuestFactory questFactory,
            QuestStageFactory questStageFactory)
        {
            this.repositoryFactory = repositoryFactory;

            this.questFactory = questFactory;

            this.questStageFactory = questStageFactory;
        }

        public ActiveQuestsManager BuildActiveQuestManager(
            QuestsSettings questsSettings,
            ActiveQuestObjectivesManager activeQuestObjectivesManager)
        {
            return new ActiveQuestsManager(
                questFactory,
                questStageFactory,
                BuildQuestDatabase(questsSettings),
                questStageFactory.BuildPrototypeStages(),
                repositoryFactory.BuildDictionaryRepository<string, ActiveQuest>(),
                activeQuestObjectivesManager);
        }

        public IReadOnlyRepository<string, Quest> BuildQuestDatabase(QuestsSettings questsSettings)
        {
            IRepository<string, Quest> database = repositoryFactory.
                BuildDictionaryRepository<string, Quest>();

            for (int i = 0; i < questsSettings.Quests.Length; i++)
            {
                var questDescriptor = questsSettings.Quests[i];

                var quest = questFactory.BuildQuest(questDescriptor, EQuestStatus.IDLE);

                database.Add(questDescriptor.ID, quest);
            }

            return (IReadOnlyRepository<string, Quest>)database;
        }
    }
}