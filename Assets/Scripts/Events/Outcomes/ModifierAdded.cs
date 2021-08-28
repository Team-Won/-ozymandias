using Managers;
using UnityEngine;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Stat Change Outcome", menuName = "Outcomes/Stat Change")]
    public class ModifierAdded : Outcome
    {

        public Stat statToChange;
        public int amount;
        public int turns;
        public string reason;
    
        public override bool Execute()
        {
            if (!Manager.Stats.Modifiers.ContainsKey(statToChange)) return false;
        
            Manager.Stats.Modifiers[statToChange].Add(new Stats.Modifier
            {
                amount = amount,
                turnsLeft = turns,
                reason = reason
            });
            Manager.Stats.ModifiersTotal[statToChange] += amount;
            return true;
        }
    
        public override string Description
        {
            get
            {
                string color;
                if (statToChange == Stat.Threat && amount > 0 || statToChange != Stat.Threat && amount < 0) color = "#820000ff";
                else color = "#007000ff";
                
                if (customDescription != "") return "<color="+color+">" + customDescription + "</color>";
                string desc = "";
                if (amount > 0) desc += "<color="+color+">" + statToChange + " has increased by " + amount;
                else desc += "<color="+color+">" + statToChange + " has decreased by " + Mathf.Abs(amount);
            
                if (turns != -1) desc += " for " + turns + " turns.";
                return desc + "</color>";
            }
        }
    }
}
