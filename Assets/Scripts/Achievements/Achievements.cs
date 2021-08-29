using System;
using System.Collections.Generic;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;
using Object = System.Object;

namespace Achievements
{
    [Serializable]
    public struct AchievementConfig
    {
        public Achievement achievement;
        public string title;
        public Sprite icon;
        public int points;
        public int targetPoints;
        [TextArea] public string lockedDescription, unlockedDescription;
    }

    public class Achievements : MonoBehaviour
    {
        [SerializeField] private GameObject achievementDisplayPrefab;
        [SerializeField] private Transform achievementDisplayContainer;
        [SerializeField] private List<AchievementConfig> achievementConfigs;
        private List<Achievement> Unlocked { get; set; }
        private SteamAchievementManager steamManager = new SteamAchievementManager();
        
        //[SerializeField] private Color lockedColor, unlockedColor;
        //[SerializeField] private Slider progressBar;
        //[SerializeField] private Image villageBadge, cityBadge, kingdomBadge;
        //[SerializeField] private GameObject notification;

        public void Unlock(Achievement achievement)
        {
            steamManager.SetOneTimeAchievement(achievement);
            steamManager.SubmitStats();
            
            //if (Unlocked.Contains(achievement)) return;
            //TODO: Unlock Sound Effect
            //TODO: Save 
        }
        
        /*public void SetCitySize(int count)
        {
            if (count < 15)
            {
                progressBar.value = (count-5) / 30f; // Fill the bar up to 33%
                villageBadge.color = lockedColor;
                cityBadge.color = lockedColor;
                kingdomBadge.color = lockedColor;
            }
            else if (count < 30)
            {
                progressBar.value = 0.33f + (count - 15) / 45f;
                villageBadge.color = unlockedColor;
                cityBadge.color = lockedColor;
                kingdomBadge.color = lockedColor;
                Unlock("Village People");
            }
            else if (count < 60)
            {
                progressBar.value = 0.67f + (count - 30) / 90f;
                villageBadge.color = unlockedColor;
                cityBadge.color = unlockedColor;
                kingdomBadge.color = lockedColor;
                Unlock("Adventurers at Large");
            }
            else
            {
                progressBar.value = 1;
                villageBadge.color = unlockedColor;
                cityBadge.color = unlockedColor;
                kingdomBadge.color = unlockedColor;
                Unlock("You Dropped This");
            }*/
            /*
        //update achievement progress and change tier gem colours accordingly
        float progressTier = progress.transform.Find("Slider").GetComponent<Slider>().value += 1 / 8f;
        if (progressTier >= 2 / 8f)
        {
            progress.transform.Find("RoadStop").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }
        if (progressTier >= 4 / 8f)
        {
            progress.transform.Find("Village").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }
        if (progressTier >= 6 / 8f)
        {
            progress.transform.Find("City").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }
        if (progressTier >= 1f)
        {
            progress.transform.Find("Kingdom").GetComponent<Image>().color = new Color(255, 160, 0, 255);
        }*/
        

        public AchievementDetails Save()
        {
            return new AchievementDetails
            {
                unlocked = Unlocked,
                
            };
        }

        public void Load(AchievementDetails details)
        {
            // Unlocked = details.unlocked;
            // foreach (var config in achievementConfigs)
            // {
            //     AchievementDisplay display = Instantiate(achievementDisplayPrefab, achievementDisplayContainer).GetComponent<AchievementDisplay>();
            //     display.Display(config, details.unlocked.Contains(config.achievement));
            // }
        }
    }
}
    
