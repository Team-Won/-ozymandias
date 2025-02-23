﻿using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Managers;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;
using String = Utilities.String;

namespace Events
{
    public class Newspaper : UIController
    {
        // Constants
        private static readonly Vector3 ClosePos = new Vector3(2000, 800, 0);
        private static readonly Vector3 CloseRot = new Vector3(0, 0, -20);
        
        // Public fields
        public static Action OnClosed; // For every newspaper close
        public static Action OnNextClosed; // Only on the next newspaper close

        [Serializable]
        public struct ChoiceButton
        {
            public Button button;
            public TextMeshProUGUI description, cost;
        }
        
        // Serialised Fields
        [SerializeField] private TextMeshProUGUI titleText;
        [SerializeField] private NewspaperEvent[] articleList;
        [SerializeField] private Image articleImage;
        [SerializeField] private ChoiceButton[] choiceList;
        [SerializeField] private Button continueButton, openNewspaperButton;
        [SerializeField] private TextMeshProUGUI turnCounter;
        
        [Header("Button States")]
        [SerializeField] private GameObject continueButtonContent;
        [SerializeField] private GameObject disableButtonContent;
        [SerializeField] private GameObject gameOverButtonContent;

        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        private Event _choiceEvent;
        private string _newspaperTitle;
        private Canvas _canvas;

        private bool _nextTurn;

        private enum ButtonState
        {
            Close,
            Choice,
            GameOver
        }
        
        private void Start()
        {
            _canvas = GetComponent<Canvas>();
            _newspaperTitle = GetNewspaperTitle();
            titleText.text = "{ " + _newspaperTitle + " }";
            State.OnNextTurnEnd += NextTurnOpen;
            continueButton.onClick.AddListener(Close);
            openNewspaperButton.onClick.AddListener(Open);
            Manager.Inputs.Close.performed += _ => Close();
            
            // Position it as closed on start
            var xform = transform;
            xform.localPosition = ClosePos;
            xform.localEulerAngles = CloseRot;
        }

        private void NextTurnOpen()
        {
            Open();
            if (Random.Range(0, 5) == 2) Manager.Jukebox.PlayMorning(); // 1/5 chance to play sound
            _nextTurn = true;
        }

        public void UpdateDisplay(List<Event> events, List<string> descriptions)
        {
            turnCounter.text = _newspaperTitle + ", Turn " + Manager.Stats.TurnCounter;
        
            _choiceEvent = events[0];
            if (_choiceEvent.choices.Count > 0) SetContinueButtonState(ButtonState.Choice);
            else SetContinueButtonState(Manager.State.IsGameOver ? ButtonState.GameOver : ButtonState.Close);
        
            // Set the image for the main article and a newspaper title
            if(events[0].image) articleImage.sprite = events[0].image;
            articleImage.gameObject.SetActive(events[0].image);

            // Assign the remaining events to the corresponding spots
            for (int i = 0; i < events.Count; i++)
                articleList[i].SetEvent(events[i], descriptions[i], i != events.Count - 1);

            // Set all event choices on button texts
            for (var i = 0; i < choiceList.Length; i++) SetChoiceActive(i, i < _choiceEvent.choices.Count);
            
            UpdateUi();
        }

        private void SetChoiceActive(int choiceI, bool active)
        {
            ChoiceButton choiceButton = choiceList[choiceI];
            choiceButton.button.gameObject.SetActive(active);
            if (!active) return;

            Choice choice = _choiceEvent.choices[choiceI];
            int cost = (int)(choice.costScale * Manager.Stats.WealthPerTurn);
            bool hasCost = cost != 0;

            bool isLocked = choice.requiresItem && !Manager.EventQueue.Flags[choice.requiredItem];
            bool alreadyPurchased = choice.disableRepurchase && Manager.EventQueue.Flags[choice.requiredItem];
            
            choiceButton.description.text = choice.name +
                $"\n(Spend {cost} / {Manager.Stats.Wealth} {String.StatIcon(Stat.Spending)})".Conditional(hasCost && !alreadyPurchased) +
                "\n(Item Not Owned)".Conditional(isLocked) +
                "\n(Item Already Owned)".Conditional(alreadyPurchased);
            
            choiceButton.button.interactable = !isLocked && !alreadyPurchased && (!hasCost || Manager.Stats.Wealth >= cost);
        }
    
        public void OnChoiceSelected(int choice)
        {
            if (!_canvas.enabled) return; // Ignore input if newspaper is closed
            
            articleList[0].AddChoiceOutcome (_choiceEvent.MakeChoice(choice));
            SetContinueButtonState(ButtonState.Close);
            for (int i = 0; i < choiceList.Length; i++) SetChoiceActive(i,false);
        
            Manager.SelectUi(continueButton.gameObject);
            UpdateUi();
        }

        private static readonly string[] NewspaperTitles = {
            "The Wizarding Post", "The Adventurer's Economist", "The Daily Guild", "Dimensional Press",
            "The Conduit Chronicle", "The Questing Times"
        };

        private static string GetNewspaperTitle()
        {
            return NewspaperTitles[Random.Range(0, NewspaperTitles.Length)];
        }
        
        private void SetContinueButtonState(ButtonState state)
        {
            continueButton.interactable = state != ButtonState.Choice;
            continueButtonContent.SetActive(state == ButtonState.Close);
            disableButtonContent.SetActive(state == ButtonState.Choice);
            gameOverButtonContent.SetActive(state == ButtonState.GameOver);
        }
        
        private void Open()
        {
            Manager.State.EnterState(GameState.InMenu);
            Manager.Jukebox.PlayScrunch();
            
            _canvas.enabled = true;
            transform.DOLocalMove(Vector3.zero, animateInDuration);
            transform.DOLocalRotate(Vector3.zero, animateInDuration).OnComplete(OnOpen);
        }

        private void Close()
        {
            if (!_canvas.enabled || !continueButton.interactable) return;
            OnClose();
            transform.DOLocalMove(ClosePos, animateOutDuration);
            transform.DOLocalRotate(CloseRot, animateOutDuration)
                .OnComplete(() =>
                {
                    if (Manager.State.IsGameOver)
                    {
                        Manager.State.EnterState(GameState.EndGame);
                    }
                    else
                    {
                        Manager.State.EnterState(GameState.InGame);
                        OnClosed?.Invoke();
                        OnNextClosed?.Invoke();
                        if (_nextTurn)
                        {
                            _nextTurn = false;
                            State.OnNewTurn?.Invoke();
                            SaveFile.SaveState(); // Save here so state only locks in after paper is closed
                        }
                        UpdateUi();
                    }
                    OnNextClosed = null;
                    _canvas.enabled = false;
                });
        }
    }
}
