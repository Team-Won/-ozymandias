using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using Utilities;

namespace Achievements
{
    public class SteamAchievementManager
    {
        // Corresponds to the enum values in Enums.cs
        private List<string> achievementNames = new List<string>()
        {
            "TEST_0", 
            "BUILD_1_BUILDING"
        };

        public void SetOneTimeAchievement(Achievement achievement)
        {
            if (IsSteamWorksInitialised()) SteamUserStats.SetAchievement(achievementNames[(int) achievement]);
        }

        bool IsSteamWorksInitialised()
        {
            return SteamManager.Initialized;
        }

        public void SubmitStats()
        {
            if (IsSteamWorksInitialised()) SteamUserStats.StoreStats();
        }
    }
}

