using System;
using Events;
using GuildFavours;
using GuildRequests;
using Inputs;
using NaughtyAttributes;
using Steamworks;
using Tooltip;
using UnityEngine;
using Utilities;
using Random = UnityEngine.Random;

namespace Managers
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Manager { get; private set; }
        
        // All Managers/ Universal Controllers
        public Inputs.Inputs Inputs { get; private set; }
        public State State { get; private set; }
        public Stats Stats { get; private set; }
        public Adventurers.Adventurers Adventurers { get; private set; }
        public Buildings.Buildings Buildings { get; private set; }
        public Achievements.Achievements Achievements { get; private set; }
        public Cards.Cards Cards { get; private set; }
        public Quests.Quests Quests { get; private set; }
        public Requests Requests { get; private set; }
        public Favours Favours { get; private set; }
        public EventQueue EventQueue { get; private set; }
        public Map.Map Map { get; private set; }
        public Jukebox Jukebox { get; private set; }
        public TooltipDisplay Tooltip { get; private set; }
        public CameraMovement Camera { get; private set; }
        public CursorSelect Cursor { get; private set; }

        private void Awake()
        {
            Manager = this;
            Random.InitState((int)DateTime.Now.Ticks);
            Inputs = new Inputs.Inputs();
            
            State = FindObjectOfType<State>();
            Stats = FindObjectOfType<Stats>();
            Adventurers = FindObjectOfType<Adventurers.Adventurers>();
            Buildings = FindObjectOfType<Buildings.Buildings>();
            Achievements = FindObjectOfType<Achievements.Achievements>();
            Cards = FindObjectOfType<Cards.Cards>();
            Quests = FindObjectOfType<Quests.Quests>();
            Requests = FindObjectOfType<Requests>();
            Favours = FindObjectOfType<Favours>();
            EventQueue = FindObjectOfType<EventQueue>();
            Map = FindObjectOfType<Map.Map>();
            Jukebox = FindObjectOfType<Jukebox>();
            Tooltip = FindObjectOfType<TooltipDisplay>();
            Camera = FindObjectOfType<CameraMovement>();
            Cursor = FindObjectOfType<CursorSelect>();
        }

        public void Start()
        {
            State.EnterState(GameState.Loading);
            Achievements.Unlock(Achievement.Test);
        }

        public static Action OnUpdateUI;
        public static void UpdateUi()
        {
            OnUpdateUI.Invoke();
        }
        
        #region Debug
        
        [Button("Print Save")]
        public void PrintSave()
        {
            Debug.Log(PlayerPrefs.GetString("Save"));
        }
        
        [Button("Take Screenshot")]
        public void Screenshot()
        {
            ScreenCapture.CaptureScreenshot($"FTRM_{DateTime.Now:dd-MM-yyyy-hh-mm-ss}.png");
        }
        
        [Button("Reset Achievements")]
        public void ResetAchievements()
        {
            SteamUserStats.ResetAllStats(true);
        }
        
        [Button("Extra Wealth")]
        public void ExtraWealth()
        {
            Stats.Wealth += 10000;
            UpdateUi();
        }
        
        #endregion
    }
}
