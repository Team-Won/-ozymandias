﻿using System.Collections.Generic;
using Managers;
using Utilities;
using static Managers.GameManager;

namespace Events.Outcomes
{
    public class AdventurersAdded : Outcome
    {
        // Either create randomly or with a list
        public List<AdventurerDetails> adventurers;
        
        public int count;
        public Guild guild;
        public bool anyGuild;

        protected override bool Execute()
        {
            for (int i = 0; i < count; i++)
            {
                Manager.Adventurers.Add(anyGuild ? (Guild?)null : guild);
            }
            
            foreach (AdventurerDetails details in adventurers)
            {
                if (details.name != null) Manager.Adventurers.Add(details);
                else Manager.Adventurers.Add();
            }

            return true;
        }

        private static readonly List<string> Descriptors = new List<string> {
            "taken up residence.", "joined the fight!", "found a new home.", "started questing."
        };

        protected override string Description => (
            customDescription != "" ? customDescription :
            $"{adventurers.Count + count} " +
            $"{(anyGuild ? "Adventurer".Pluralise(adventurers.Count + count) : String.GuildWithIcon(guild, adventurers.Count + count))} " +
            $"{(adventurers.Count + count == 1 ? "has" : "have")} " +
            $"{Descriptors.SelectRandom()}"
        ).StatusColor(1);
    }
}
