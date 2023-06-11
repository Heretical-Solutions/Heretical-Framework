using System.Collections.Generic;

using HereticalSolutions.Repositories;

namespace HereticalSolutions.Quests
{
    public class ActiveQuestObjectivesManager
    {
        private readonly IRepository<string, List<ActiveQuestObjective>> activeObjectivesRepository;

        public ActiveQuestObjectivesManager(IRepository<string, List<ActiveQuestObjective>> activeObjectivesRepository)
        {
            this.activeObjectivesRepository = activeObjectivesRepository;
        }

        public void AddObjective(ActiveQuestObjective activeObjective)
        {
            string objectiveID = activeObjective.Objective.Descriptor.ObjectiveID;
            
            if (!activeObjectivesRepository.Has(objectiveID))
            {
                activeObjectivesRepository.Add(objectiveID, new List<ActiveQuestObjective>());
            }

            var trackersByID = activeObjectivesRepository.Get(objectiveID);

            trackersByID.Add(activeObjective);
        }

        public bool GetObjectives(string objectiveID, out IEnumerable<ActiveQuestObjective> activeObjectives)
        {
            var result = activeObjectivesRepository.TryGet(objectiveID, out List<ActiveQuestObjective> objectivesList);

            activeObjectives = objectivesList;

            return result;
        }

        public IEnumerable<ActiveQuestObjective> GetAllObjectives()
        {
            List<ActiveQuestObjective> result = new List<ActiveQuestObjective>();

            var keys = activeObjectivesRepository.Keys;
            
            foreach (var key in keys)
                result.AddRange(activeObjectivesRepository.Get(key));

            return result;
        }

        public void RemoveObjective(ActiveQuestObjective activeObjective)
        {
            if (!activeObjectivesRepository.Has(activeObjective.Objective.Descriptor.ObjectiveID))
                return;

            activeObjectivesRepository.Get(activeObjective.Objective.Descriptor.ObjectiveID).Remove(activeObjective);
        }
    }
}