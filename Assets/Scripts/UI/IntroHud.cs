using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class IntroHud : UIController
    {
        public static bool DisableOpen = false;
        
        [SerializeField] private float animateInDuration = 0.5f, animateOutDuration = 0.5f;
        [SerializeField] private RectTransform title, buttons, socials;
        [SerializeField] private Button playButton, creditsButton, quitButton, settingsButton, upgradesButton;

        public enum HudObject
        {
            Title,
            Buttons,
            Socials,
        }

        private readonly struct HudObjectValues
        {
            public readonly RectTransform RectTransform;
            public readonly Vector2 ShowPos;
            public readonly Vector2 HidePos;

            public HudObjectValues(RectTransform rect, Vector2 show, Vector2 hide)
            {
                RectTransform = rect;
                ShowPos = show;
                HidePos = hide;
            }
        }

        private Dictionary<HudObject, HudObjectValues> _hudValuesMap;

        private void Start()
        {
            OnOpen();
            playButton.onClick.AddListener(() => { if (Manager.State.InIntro) Manager.State.EnterState(GameState.ToGame); });
            settingsButton.onClick.AddListener(() => { if (Manager.State.InIntro) Manager.Book.Open(Book.BookPage.Settings); });
            upgradesButton.onClick.AddListener(() => { if (Manager.State.InIntro) Manager.Book.Open(Book.BookPage.Upgrades); });
            creditsButton.onClick.AddListener(() => { if (Manager.State.InIntro) Manager.State.EnterState(GameState.ToCredits); });
            quitButton.onClick.AddListener(() => { if (Manager.State.InIntro) Application.Quit(); });
            settingsButton.onClick.AddListener(OnClose);
            upgradesButton.onClick.AddListener(OnClose);
            _hudValuesMap = new Dictionary<HudObject, HudObjectValues>
            {
                {HudObject.Title, new HudObjectValues(title, new Vector2(600,320), new Vector2(600,700))},
                {HudObject.Buttons, new HudObjectValues(buttons, new Vector2(600,-100), new Vector2(-400,-100))},
                {HudObject.Socials, new HudObjectValues(socials, Vector2.zero, new Vector2(250,0))},
            };
        }
        
        public void Hide(bool animate = true)
        {
            OnClose();
            Hide(new List<HudObject>(_hudValuesMap.Keys), animate);
        }

        public void Hide(List<HudObject> objects, bool animate = true)
        {
            foreach (HudObject obj in objects) Hide(obj, animate);
        }
        
        public void Hide(HudObject obj, bool animate = true)
        {
            HudObjectValues objValues = _hudValuesMap[obj];
            if (animate) objValues.RectTransform.DOAnchorPos(objValues.HidePos, animateOutDuration);
            else objValues.RectTransform.anchoredPosition = objValues.HidePos;
        }
        
        public void Show(bool animate = true)
        {
            if (DisableOpen) return;
            OnOpen();
            Show(new List<HudObject>(_hudValuesMap.Keys), animate);
        }
        
        public void Show(List<HudObject> objects, bool animate = true)
        {
            foreach (HudObject obj in objects) Show(obj, animate);
        }

        public void Show(HudObject obj, bool animate = true)
        {
            HudObjectValues objValues = _hudValuesMap[obj];
            if (animate) objValues.RectTransform.DOAnchorPos(objValues.ShowPos, animateInDuration);
            else objValues.RectTransform.anchoredPosition = objValues.ShowPos;
        }
    }
}
