using System;
using System.Linq;

using HereticalSolutions.Repositories;

using HereticalSolutions.Quests.Factories;

namespace HereticalSolutions.Quests
{
    /// <summary>
    /// Manages active quests and their objectives
    /// </summary>
    public class ActiveQuestsManager
    {
        private readonly QuestFactory questFactory;

        private readonly QuestStageFactory questStageFactory;

        private readonly IReadOnlyRepository<string, Quest> questsDatabase;

        private readonly IReadOnlyRepository<string, QuestStage> 
            questPrototypeStagesDatabase;

        private readonly IRepository<string, ActiveQuest> activeQuestRepository;

        private readonly ActiveQuestObjectivesManager objectivesManager;
        
        public ActiveQuestsManager(
            QuestFactory questFactory,
            QuestStageFactory questStageFactory,
            IReadOnlyRepository<string, Quest> questsDatabase,
            IReadOnlyRepository<string, QuestStage> questPrototypeStagesDatabase,
            IRepository<string, ActiveQuest> activeQuestRepository,
            ActiveQuestObjectivesManager objectivesManager)
        {
            this.questFactory = questFactory;
            this.questStageFactory = questStageFactory;
            this.questsDatabase = questsDatabase;
            this.questPrototypeStagesDatabase = questPrototypeStagesDatabase;
            this.activeQuestRepository = activeQuestRepository;
            this.objectivesManager = objectivesManager;
        }

        #region Get active quest
        
        public bool IsQuestActive(string questID)
        {
            return activeQuestRepository.Has(questID);
        }
        
        public ActiveQuest GetActiveQuest(string questID)
        {
            if (!IsQuestActive(questID))
                throw new Exception($"[ActiveQuestsManager] QUEST \"{questID}\" IS NOT ACTIVE");
            
            return activeQuestRepository.Get(questID);
        }
        
        #endregion

        #region Start quest
        
        public ActiveQuest StartQuest(string questID)
        {
            if (activeQuestRepository.Has(questID))
                throw new Exception($"[ActiveQuestsManager] QUEST \"{questID}\" IS ALREADY ACTIVE");
            
            if (!questsDatabase.Has(questID))
                throw new Exception($"[ActiveQuestsManager] UNKNOWN QUEST ID \"{questID}\"");

            var quest = questsDatabase.Get(questID);
            
            var activeQuest = questFactory.BuildActiveQuest(quest);
            
            Action<ActiveQuest> onCompletedDelegate = null;

            onCompletedDelegate = (arg) =>
            {
                OnQuestCompleted(arg);

                activeQuest.OnQuestStarted -= OnQuestStarted;
                activeQuest.OnStageStarted -= OnQuestStageStarted;
                activeQuest.OnStageCompleted -= OnQuestStageCompleted;
                activeQuest.OnQuestCompleted -= onCompletedDelegate;
            };

            activeQuest.OnQuestStarted += OnQuestStarted;
            activeQuest.OnStageStarted += OnQuestStageStarted;
            activeQuest.OnStageCompleted += OnQuestStageCompleted;
            activeQuest.OnQuestCompleted += onCompletedDelegate;
            
            activeQuestRepository.Add(activeQuest.Quest.Descriptor.ID, activeQuest);
                
            activeQuest.Start();

            return activeQuest;
        }

        private void OnQuestStarted(ActiveQuest activeQuest)
        {
#if (UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_SERVER || UNITY_WEBGL || UNITY_EDITOR) //TODO: remove
            UnityEngine.Debug.Log($"[ActiveQuestsManager] QUEST \"{activeQuest.Quest.Descriptor.ID}\": STARTED");
#endif
        }

        private void OnQuestStageStarted(ActiveQuest activeQuest)
        {
#if (UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_SERVER || UNITY_WEBGL || UNITY_EDITOR) //TODO: remove
            UnityEngine.Debug.Log($"[ActiveQuestsManager] QUEST \"{activeQuest.Quest.Descriptor.ID}\": STAGE {activeQuest.CurrentStageIndex} STARTED");
#endif

            var stageData = activeQuest.Quest.Descriptor.Stages[activeQuest.CurrentStageIndex];
            
            var questStage = questStageFactory.BuildQuestStage(
                questPrototypeStagesDatabase, stageData);

            questStage.Handler.Invoke(this, activeQuest);
        }

        private void OnQuestStageCompleted(ActiveQuest activeQuest)
        {
#if (UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_SERVER || UNITY_WEBGL || UNITY_EDITOR) //TODO: remove
            UnityEngine.Debug.Log($"[ActiveQuestsManager] QUEST \"{activeQuest.Quest.Descriptor.ID}\": STAGE {activeQuest.CurrentStageIndex} COMPLETED");
#endif
        }

        private void OnQuestCompleted(ActiveQuest activeQuest)
        {
#if (UNITY_STANDALONE || UNITY_IOS || UNITY_IPHONE || UNITY_ANDROID || UNITY_SERVER || UNITY_WEBGL || UNITY_EDITOR) //TODO: remove
            UnityEngine.Debug.Log($"[ActiveQuestsManager] QUEST \"{activeQuest.Quest.Descriptor.ID}\": COMPLETED");
#endif

            activeQuestRepository.Remove(activeQuest.Quest.Descriptor.ID);
        }
        
        #endregion

        #region Toggle objectives

        public ActiveQuestObjective EnableQuestObjective(ActiveQuest activeQuest, int objectiveIndex)
        {
            var objective = activeQuest.Quest.Objectives[objectiveIndex];

            var activeObjective = questFactory.BuildActiveQuestObjective(objective);
            
            activeQuest.QuestProgressTracker.AddObjective(activeObjective);
            
            objectivesManager.AddObjective(activeObjective);

            return activeObjective;
        }
        
        public ActiveQuestObjective EnableQuestObjective(ActiveQuest activeQuest, string objectiveID)
        {
            var objective = activeQuest.Quest.Objectives.FirstOrDefault(item => item.Descriptor.ObjectiveID == objectiveID);

            var activeObjective = questFactory.BuildActiveQuestObjective(objective);
            
            activeQuest.QuestProgressTracker.AddObjective(activeObjective);
            
            objectivesManager.AddObjective(activeObjective);

            return activeObjective;
        }
        
        public void EnableAllQuestObjectives(
            ActiveQuest activeQuest)
        {
            for (int i = 0; i < activeQuest.Quest.Objectives.Length; i++)
            {
                var objective = activeQuest.Quest.Objectives[i];

                var activeObjective = questFactory.BuildActiveQuestObjective(objective);

                activeQuest.QuestProgressTracker.AddObjective(activeObjective);

                objectivesManager.AddObjective(activeObjective);
            }
        }
        
        public void DisableQuestObjective(ActiveQuest activeQuest, int objectiveIndex)
        {
            var activeObjective = activeQuest.QuestProgressTracker.GetObjectiveByIndex(objectiveIndex);
            
            activeQuest.QuestProgressTracker.RemoveObjective(activeObjective);
            
            objectivesManager.RemoveObjective(activeObjective);
        }
        
        public void DisableQuestObjective(ActiveQuest activeQuest, string objectiveID)
        {
            var activeObjective = activeQuest.QuestProgressTracker.GetObjectiveByID(objectiveID);
            
            activeQuest.QuestProgressTracker.RemoveObjective(activeObjective);
            
            objectivesManager.RemoveObjective(activeObjective);
        }

        public void DisableAllActiveObjectives(ActiveQuest activeQuest)
        {
            var allActiveObjectives = activeQuest.QuestProgressTracker.AllActiveObjectives.ToArray();
            
            foreach (var activeObjective in allActiveObjectives)
            {
                activeQuest.QuestProgressTracker.RemoveObjective(activeObjective);
            
                objectivesManager.RemoveObjective(activeObjective);
            }
        }

        #endregion
    }
}