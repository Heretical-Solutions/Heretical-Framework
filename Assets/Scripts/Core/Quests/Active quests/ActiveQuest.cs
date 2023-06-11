using System;

namespace HereticalSolutions.Quests
{
    public class ActiveQuest
    {
        public Quest Quest { get; private set; }

        #region Current stage

        public QuestStage CurrentStage { get; set; }

        private int currentStageIndex;

        public int CurrentStageIndex { get =>  currentStageIndex; }

        //public QuestStageDTO CurrentStageData { get => Quest.Descriptor.Stages[currentStageIndex]; }

        #endregion

        #region Progress
        
        public QuestProgressTracker QuestProgressTracker { get; private set; }

        #endregion
        
        #region Callbacks
        
        public Action<ActiveQuest> OnQuestStarted { get; set; }
        
        public Action<ActiveQuest> OnStageStarted { get; set; }
        
        public Action<ActiveQuest> OnStageCompleted { get; set; }
        
        public Action<ActiveQuest> OnQuestCompleted { get; set; }

        #endregion
        
        public ActiveQuest(
            Quest quest,
            QuestProgressTracker questProgressTracker)
        {
            Quest = quest;

            currentStageIndex = 0;

            QuestProgressTracker = questProgressTracker;
        }

        public void Start()
        {
            Quest.Status = EQuestStatus.ACTIVE;
            
            OnQuestStarted?.Invoke(this);
            
            if (currentStageIndex >= Quest.Descriptor.Stages.Length)
                QuestComplete();
            else
            {
                StartNextStage();
            }
        }

        public void StageComplete()
        {
            OnStageCompleted?.Invoke(this);

            currentStageIndex++;

            if (currentStageIndex >= Quest.Descriptor.Stages.Length)
                QuestComplete();
            else
            {
                StartNextStage();
            }
        }

        private void StartNextStage()
        {
            OnStageStarted?.Invoke(this);
        }

        private void QuestComplete()
        {
            Quest.Status = EQuestStatus.COMPLETED;

            OnQuestCompleted?.Invoke(this);

            QuestProgressTracker.Cleanup();
        }
    }
}
