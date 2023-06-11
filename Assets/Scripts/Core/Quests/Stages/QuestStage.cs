using System;

namespace HereticalSolutions.Quests
{
    public class QuestStage
    {
        public QuestStageDescriptor Descriptor { get; private set; }

        public QuestPropertyDTO[] Properties { get; private set; }
        
        public Action<ActiveQuestsManager, ActiveQuest> Handler { get; private set; }

        public QuestStage(
            QuestStageDescriptor descriptor,
            Action<ActiveQuestsManager, ActiveQuest> handler)
        {
            Descriptor = descriptor;

            Properties = Array.Empty<QuestPropertyDTO>();

            Handler = handler;
        }
        
        public QuestStage(
            QuestStage prototype,
            QuestPropertyDTO[] properties)
        {
            Descriptor = prototype.Descriptor;

            Properties = properties;

            Handler = prototype.Handler;
        }
    }
}
