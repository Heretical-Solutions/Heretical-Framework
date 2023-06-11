using System.Collections.Generic;

using System.Linq;

namespace HereticalSolutions.Quests
{
    public class QuestProgressTracker
    {
        private readonly List<ActiveQuestObjective> activeObjectives;

        public QuestProgressTracker(List<ActiveQuestObjective> activeObjectives)
        {
            this.activeObjectives = activeObjectives;
        }

        public int ActiveObjectivesCount { get =>  activeObjectives.Count; }

        public IEnumerable<ActiveQuestObjective> AllActiveObjectives { get =>  activeObjectives; }

        public void AddObjective(ActiveQuestObjective tracker)
        {
            activeObjectives.Add(tracker);
        }

        public ActiveQuestObjective GetObjectiveByID(string objectiveID)
        {
            return activeObjectives.FirstOrDefault(item => item.Objective.Descriptor.ObjectiveID == objectiveID);
        }

        public ActiveQuestObjective GetObjectiveByIndex(int index)
        {
            return activeObjectives.FirstOrDefault(item => item.Objective.Index == index);
        }
        
        public void RemoveObjective(ActiveQuestObjective tracker)
        {
            activeObjectives.Remove(tracker);
        }

        public bool AllActiveObjectivesValidated
        {
            get
            {
                if (ActiveObjectivesCount == 0)
                    return true;

                foreach (var activeObjective in activeObjectives)
                {
                    if (!activeObjective.Validate())
                        return false;
                }

                return true;
            }
        }

        public void Cleanup()
        {
            foreach (var tracker in activeObjectives)
            {
                tracker.Progress.Cleanup();
            }

            activeObjectives.Clear();
        }
    }
}
