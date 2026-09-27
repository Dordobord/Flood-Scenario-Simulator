using System.Collections.Generic;
using UnityEngine;

namespace StormWaits
{
    [CreateAssetMenu(fileName = "NewScenarioTaskSet", menuName = "Storm Waits/Scenario Task Set")]
    public class ScenarioTaskSet : ScriptableObject
    {
        public string scenarioName = "Sample";
        public List<TaskData> tasks = new List<TaskData>();
    }
}
