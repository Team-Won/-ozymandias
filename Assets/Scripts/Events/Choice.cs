using System.Collections.Generic;
using Events.Outcomes;
using UnityEngine;

namespace Events
{
    public class Choice : ScriptableObject
    {
        public List<Outcome> outcomes = new List<Outcome>();
        public int cost;
    }
}
