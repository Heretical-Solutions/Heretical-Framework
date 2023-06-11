using System;
using System.Linq;

namespace HereticalSolutions.Quests
{
    public static class QuestsExtensions
    {
        public static int GetInt(this QuestStage stage, string id)
        {
            var property = stage.Properties.FirstOrDefault(item => item.Key == id);

            if (property.Equals(default(QuestPropertyDTO)))
                throw new Exception(string.Format("[QuestExtensions] QUEST STAGE HAS NO PROPERTY BY THE KEY {0}", id));

            return Convert.ToInt32(property.Value);
        }

        public static float GetFloat(this QuestStage stage, string id)
        {
            var property = stage.Properties.FirstOrDefault(item => item.Key == id);

            if (property.Equals(default(QuestPropertyDTO)))
                throw new Exception(string.Format("[QuestExtensions] QUEST STAGE HAS NO PROPERTY BY THE KEY {0}", id));

            return Convert.ToSingle(property.Value);
        }

        public static string GetString(this QuestStage stage, string id)
        {
            var property = stage.Properties.FirstOrDefault(item => item.Key == id);

            if (property.Equals(default(QuestPropertyDTO)))
                throw new Exception(string.Format("[QuestExtensions] QUEST STAGE HAS NO PROPERTY BY THE KEY {0}", id));

            return property.Value;
        }

        public static QuestPropertyDTO ByKey(this QuestPropertyDTO[] properties, string key)
        {
            return properties.FirstOrDefault(item => item.Key == key);
        }
    }
}
