using System;

using UnityEngine;

using HereticalSolutions.Repositories;
using HereticalSolutions.Repositories.Factories;

namespace HereticalSolutions.Quests.Factories
{
    public static partial class QuestsFactory
    {
        public static QuestStage BuildQuestStage(
            IReadOnlyRepository<string, QuestStage> prototypeStages,
            QuestStageDTO stageData)
        {
            return new QuestStage(
                prototypeStages.Get(stageData.ActionID),
                stageData.Properties);
        }

        public static IReadOnlyRepository<string, QuestStage> BuildPrototypeStages()
        {
            IRepository<string, QuestStage> prototypeStages =
                RepositoriesFactory.BuildDictionaryRepository<string, QuestStage>();
            
            AddActions(prototypeStages);
            AddTrackers(prototypeStages);
            AddAwaiters(prototypeStages);
            //AddCheckpoints(prototypeStages);

            return (IReadOnlyRepository<string, QuestStage>)prototypeStages;
        }

        #region Actions

        private static void AddActions(IRepository<string, QuestStage> prototypeStages)
        {
            prototypeStages.Add("NOP_ACTION", NOPAction());
            //prototypeStages.Add("CLOSE_ACTIVE_WINDOW", CloseActiveWindow());
        }

        private static QuestStage NOPAction()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "NOP_ACTION",
                    StageType = EStageTypes.ACTION
                },
                (activeQuestManager, activeQuest) =>
                {
                    activeQuest.StageComplete();
                });
        }

        /*
        private static QuestStage CloseActiveWindow()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "CLOSE_ACTIVE_WINDOW",
                    StageType = EStageTypes.ACTION
                },
                (activeQuestManager, activeQuest) =>
                {
                    // messageBus
                    //     .PopMessage<PlayerClosedWindowEvent>(out var message)
                    //     .Write(message,false)
                    //     .SendImmediately(message);

                    activeQuest.StageComplete();
                });
        }
        */

        #endregion

        #region Trackers

        private static void AddTrackers(IRepository<string, QuestStage> prototypeStages)
        {
            prototypeStages.Add("ENABLE_OBJECTIVE", EnableObjective());
            prototypeStages.Add("ENABLE_ALL_OBJECTIVES", EnableAllObjectives());
            prototypeStages.Add("DISABLE_OBJECTIVE", DisableObjective());
            prototypeStages.Add("DISABLE_ALL_OBJECTIVES", DisableAllObjectives());
        }
        
        private static QuestStage EnableObjective()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "ENABLE_OBJECTIVE",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    int objectiveIndex = activeQuest.CurrentStage.GetInt("OBJECTIVE_INDEX");

                    activeQuestManager.EnableQuestObjective(activeQuest, objectiveIndex);

                    activeQuest.StageComplete();
                });
        }
        
        private static QuestStage EnableAllObjectives()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "ENABLE_ALL_OBJECTIVES",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    activeQuestManager.EnableAllQuestObjectives(activeQuest);

                    activeQuest.StageComplete();
                });
        }
        
        private static QuestStage DisableObjective()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "DISABLE_OBJECTIVE",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    int objectiveIndex = activeQuest.CurrentStage.GetInt("OBJECTIVE_INDEX");

                    activeQuestManager.DisableQuestObjective(activeQuest, objectiveIndex);

                    activeQuest.StageComplete();
                });
        }
        
        private static QuestStage DisableAllObjectives()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "DISABLE_ALL_OBJECTIVES",
                    StageType = EStageTypes.TRACKER
                },
                (activeQuestManager, activeQuest) =>
                {
                    activeQuestManager.DisableAllActiveObjectives(activeQuest);

                    activeQuest.StageComplete();
                });
        }

        #endregion

        #region Awaiters

        private static void AddAwaiters(IRepository<string, QuestStage> prototypeStages)
        {
            prototypeStages.Add("AWAIT_ALL_ACTIVE_OBJECTIVES_COMPLETE", AwaitAllActiveObjectivesComplete());
            prototypeStages.Add("DEBUG_AWAIT_NEVER", DebugAwaitNever());
        }
        
        private static QuestStage AwaitAllActiveObjectivesComplete()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "AWAIT_ALL_ACTIVE_OBJECTIVES_COMPLETE",
                    StageType = EStageTypes.AWAITER
                },
                (activeQuestManager, activeQuest) =>
                {
                    Action cleanup = default(Action);

                    foreach (var tracker in activeQuest.QuestProgressTracker.AllActiveObjectives)
                    {
                        #if UNITY_EDITOR
                        int desiredValueClosure = tracker.Objective.Descriptor.ExpectedValue;

                        string trackerIDClosure = tracker.Objective.Descriptor.ObjectiveID;
                        #endif
                        
                        Action<int> onValueChanged = (value) =>
                        {
                            #if UNITY_EDITOR
                            Debug.Log($"[QuestStageFactory] TRACKER VALUE MODIFIED: {trackerIDClosure} {value} / {desiredValueClosure}");
                            #endif
                            
                            if (activeQuest.QuestProgressTracker.AllActiveObjectivesValidated)
                            {
                                cleanup?.Invoke();

                                activeQuest.StageComplete();
                            }
                        };

                        tracker.Progress.OnValueChanged += onValueChanged;

                        cleanup += (Action)(() =>
                        {
                            tracker.Progress.OnValueChanged -= onValueChanged;
                        });
                    }
                });
        }

        private static QuestStage DebugAwaitNever()
        {
            return new QuestStage(
                new QuestStageDescriptor
                {
                    ActionID = "DEBUG_AWAIT_NEVER",
                    StageType = EStageTypes.AWAITER
                },
                (activeQuestManager, activeQuest) =>
                {
                });
        }

        #endregion

        #region Checkpoints

        /*
        private void AddCheckpoints(Dictionary<string, QuestStage> stages)
        {
            stages.Add("CHECKPOINT", Checkpoint());
        }

        private QuestStage Checkpoint()
        {
            return new QuestStage(
                "CHECKPOINT",
                EStageTypes.CHECKPOINT,
                (quest) =>
                {
                    quest.Checkpoint();

                    quest.StageComplete();
                });
        }
        */

        #endregion
    }
}
