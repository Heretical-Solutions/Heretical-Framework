using UnityEngine;

namespace HereticalSolutions.Quests.Unity
{
    [CreateAssetMenu(fileName = "QuestsSettings", menuName = "Settings/Quests settings", order = 2)]
    public class QuestsSettings : ScriptableObject
    {
        [Header("Chapters")]
        public QuestDescriptor[] Quests;
    }
}