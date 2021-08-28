using UnityEngine;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Stat Change Outcome", menuName = "Outcomes/Stat Change")]
    public class ModifierRemoved : Outcome
    {
        public int idToRemove;
        
        public override bool Execute(bool fromChoice)
        {
            //TODO: Remove for permanent modifiers
            return true;
        }
    
        public override string Description
        {
            get
            {
                return "TODO";
            }
        }
    }
}
