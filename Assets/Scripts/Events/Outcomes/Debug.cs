using UnityEngine;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Debug Outcome", menuName = "Outcomes/Debug")]
    public class Debug : Outcome
    {
        public override bool Execute()
        {
            UnityEngine.Debug.Log(Description);
            return true;
        }
    }
}
