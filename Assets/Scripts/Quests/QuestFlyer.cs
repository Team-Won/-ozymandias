﻿using System;
using Inputs;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;
using String = Utilities.String;

namespace Quests
{
    public class QuestFlyer : UIController
    {
        [SerializeField] private TextMeshProUGUI titleText, descriptionText, adventurersText, durationText, rewardText, costText;
        [SerializeField] private Image icon;
        [SerializeField] private Button sendButton;
        [SerializeField] private GameObject stamp;
        [SerializeField] private Slider adventurerSlider, durationSlider;
        
        public Action<int, int> OnStartClicked;
        public Action<int> OnAdventurerValueChanged;
        public Action<int> OnDurationValueChanged;

        private Quest _quest;

        private void Start()
        {
            adventurerSlider.onValueChanged.AddListener(value =>
            {
                OnAdventurerValueChanged?.Invoke((int) value);
            });
            durationSlider.onValueChanged.AddListener(value =>
            {
                OnDurationValueChanged?.Invoke((int) value);
            });
            sendButton.onClick.AddListener(() =>
            {
                if (!Manager.State.InMenu) return;
                InputHelper.OnToggleCursor?.Invoke(false);
                RandomRotateStamps();
                Manager.Jukebox.PlayStamp();
                OnStartClicked?.Invoke((int)adventurerSlider.value, (int)durationSlider.value);
                UpdateContent(_quest, true);
            });
        }

        private void RandomRotateStamps()
        {
            float stampRotation = Random.Range(3f, 6f);
            stampRotation = (Random.value < 0.5) ? stampRotation : -stampRotation;
            sendButton.gameObject.SetActive(false);
            stamp.SetActive(true);
            stamp.transform.localEulerAngles = new Vector3(0, 0, stampRotation);
        }

        public void UpdateContent(Quest quest, bool valueChange = false)
        {
            _quest = quest;
            titleText.text = quest.Title;
            descriptionText.text = quest.Description;
            icon.sprite = quest.image;
            
            sendButton.gameObject.SetActive(!quest.IsActive);
            durationSlider.gameObject.SetActive(!quest.IsActive);
            adventurerSlider.gameObject.SetActive(!quest.IsActive);
            stamp.SetActive(quest.IsActive);

            if (quest.IsActive)
            {
                adventurersText.text = "Adventurers: " + quest.AssignedCount;
                durationText.text = quest.TurnsLeft <= 1 ? "Returning today" : $"Will return in {quest.TurnsLeft} turns";
                rewardText.text = quest.RewardDescription;
                costText.text = "";
            }
            else
            {
                if (!valueChange)
                {
                    adventurerSlider.value = 0;
                    durationSlider.value = 0;
                }

                // Use slider assigned values
                int adventurers = (int)adventurerSlider.value + quest.BaseAdventurers;
                int duration = (int)durationSlider.value + quest.baseDuration;
                int cost = quest.ScaledCost((int)adventurerSlider.value + (int)durationSlider.value);

                bool enoughAdventurers = Manager.Adventurers.Available >= adventurers;
                bool enoughMoney = Manager.Stats.Wealth >= cost;
                adventurersText.text = $"Adventurers: {adventurers}".StatusColor(enoughAdventurers ? 0 : -1);
                durationText.text = $"Duration: {duration} {"turn".Pluralise(duration)}";
                rewardText.text = quest.RewardDescription;
                costText.text = $"Cost: {cost} {String.StatIcon(Stat.Spending)}".StatusColor(enoughMoney ? 0 : -1);

                sendButton.interactable = enoughAdventurers && enoughMoney;
            }
        }

        public override void OnOpen()
        {
            base.OnOpen();
            InputHelper.OnToggleCursor?.Invoke(!_quest.IsActive);
        }
    }
}
