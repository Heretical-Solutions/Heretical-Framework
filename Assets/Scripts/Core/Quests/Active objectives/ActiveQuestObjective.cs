using System;

using HereticalSolutions.MVVM;
using HereticalSolutions.MVVM.Observable;

namespace HereticalSolutions.Quests
{
    public class ActiveQuestObjective
    {
        public QuestObjective Objective { get; private set; }

        public IObservableProperty<int> Progress { get; private set; }

        public ActiveQuestObjective(
            QuestObjective objective, 
            ObservableProperty<int> progress)
        {
            Objective = objective;

            Progress = progress;
        }

        public void Modify(ETrackerOperation operation, int value)
        {
            switch (operation)
            {
                case ETrackerOperation.SET_TO_VALUE:

                    Progress.Value = value;
                    
                    break;
                
                case ETrackerOperation.INCREMENT:

                    Progress.Value++;
                    
                    break;
                
                case ETrackerOperation.DECREMENT:

                    Progress.Value--;
                    
                    break;
                    
                case ETrackerOperation.ADD:

                    Progress.Value += value;
                    
                    break;
                
                case ETrackerOperation.SUBSTRACT:

                    Progress.Value -= value;
                    
                    break;
                
                case ETrackerOperation.MULTIPLY:

                    Progress.Value *= value;
                    
                    break;
                
                case ETrackerOperation.DIVIDE:

                    Progress.Value = (int)((float)Progress.Value / (float)value);
                    
                    break;
            }
        }

        public bool Validate()
        {
            switch (Objective.Descriptor.Comparison)
            {
                case ETrackerComparison.EQUALS_TO:
                    return Progress.Value == Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.NOT_EQUALS_TO:
                    return Progress.Value != Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.MORE_THAN:
                    return Progress.Value > Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.LESS_THAN:
                    return Progress.Value < Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.EQUALS_OR_MORE_THAN:
                    return Progress.Value >= Objective.Descriptor.ExpectedValue;
                
                case ETrackerComparison.EQUALS_OR_LESS_THAN:
                    return Progress.Value <= Objective.Descriptor.ExpectedValue;
            }
            
            throw new Exception("[QuestObjectiveTracker] INVALID OPERATION");
        }
    }
}