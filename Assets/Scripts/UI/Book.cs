using System;
using Cards;
using DG.Tweening;
using Inputs;
using Managers;
using NaughtyAttributes;
using Requests;
using Structures;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace UI
{
    public class Book : MonoBehaviour
    {
        [Serializable]
        private class BookGroup
        {
            public CanvasGroup canvasGroup;
            public Button bookRibbon;
        }

        public enum BookPage
        {
            Settings = 0,
            Guide = 1,
            Reports = 2,
            Upgrades = 3,
        }

        const float DEFAULT_RIBBON_HEIGHT = 128;
        const float EXPANDED_RIBBON_HEIGHT = 180f;
        
        // Public fields
        public static Action OnOpened;

        private static readonly Vector3 ClosePos = new Vector3(0, -1000, 0);
        private static readonly Vector3 PunchScale = Vector3.one * 0.5f;
        
        [SerializeField] private Canvas canvas;

        [SerializeField] private Button closeButton, quitButton, clearSaveButton;
        [SerializeField] private Slider sfxSlider;
        [SerializeField] private GameObject clearSaveText, confirmClearText, finalClearText;
        [SerializeField] private float animateInDuration = .5f;
        [SerializeField] private float animateOutDuration = .75f;
        [SerializeField] private SerializedDictionary<BookPage, BookGroup> pages;
        private GameState _closeState; // The state the book will enter when closed
        private bool _isOpen, _transitioning, _changingPage;
        private bool _fromGame; // TODO this state cache must be removed, please do not use it
        private bool _disableNavigation = false;
        private CanvasGroup _closeButtonCanvas;

        [Header("Adjacency Bonuses")]
        private static readonly Vector2 AdjacencyClosedPos = new Vector2(-70,37);
        private static readonly Vector2 AdjacencyOpenPos = new Vector2(180,37);
        [SerializeField] private RectTransform adjacencyBonuses;

        private int _confirmDeleteStep;
        private int ConfirmingDelete
        {
            get => _confirmDeleteStep;
            set
            {
                _confirmDeleteStep = value;
                clearSaveText.SetActive(value == 0);
                confirmClearText.SetActive(value == 1);
                finalClearText.SetActive(value == 2);
            }
        }

        [SerializeField] private ExtendedDropdown resDropdown;
        
        private BookPage _page = BookPage.Settings;
        private void SetPage(BookPage page)
        {
            ConfirmingDelete = 0;
            // Enable or disable the quit to menu button to avoid the raycaster affecting
            // other pages in the book
            // TODO this needs a second menu state available for in-menu/game vs in-menu/intro
            bool enableQuit = _fromGame && !Tutorial.Tutorial.Active && page == BookPage.Settings;
            quitButton.gameObject.SetActive(enableQuit);
            quitButton.interactable = enableQuit;
            quitButton.enabled = enableQuit;
            
            bool enableClear = !_fromGame && !Tutorial.Tutorial.Active && page == BookPage.Settings;
            clearSaveButton.gameObject.SetActive(enableClear);
            clearSaveButton.interactable = enableClear;
            clearSaveButton.enabled = enableClear;

            if (page == BookPage.Settings)
            {
                sfxSlider.navigation = new Navigation
                {
                    selectOnDown = _fromGame ? quitButton : clearSaveButton,
                    selectOnUp = sfxSlider.navigation.selectOnUp,
                    mode = Navigation.Mode.Explicit
                };
            }
            if (page == BookPage.Reports) CardsBookList.ScrollActive = true;

            if (_page == page)
            {
                pages[_page].canvasGroup.alpha = 1;
                pages[_page].canvasGroup.interactable = true;
                pages[_page].canvasGroup.blocksRaycasts = true;
            }
            else
            {
                _changingPage = true;
            
                Manager.Jukebox.PlayPageTurn();

                pages[_page].canvasGroup.interactable = false;
                pages[_page].canvasGroup.blocksRaycasts = false;
                var rt = pages[_page].bookRibbon.transform as RectTransform;
                rt.DOSizeDelta(new Vector2(rt.sizeDelta.x, DEFAULT_RIBBON_HEIGHT), 0.15f);
                pages[_page].canvasGroup.GetComponent<UIController>()?.OnClose();
                pages[_page].canvasGroup.DOFade(0, 0.2f).OnComplete(() =>
                {
                    pages[_page].canvasGroup.DOFade(1f, 0.2f);
                    pages[_page].canvasGroup.interactable = true;
                    pages[_page].canvasGroup.blocksRaycasts = true;
                    pages[_page].canvasGroup.GetComponent<UIController>()?.OnOpen();
                    var rt = pages[_page].bookRibbon.transform as RectTransform;
                    rt.DOSizeDelta(new Vector2(rt.sizeDelta.x, EXPANDED_RIBBON_HEIGHT), 0.15f);
                    _changingPage = false;
                });
                _page = page;
            }
        }
        
        private void Start()
        {
            _closeButtonCanvas = closeButton.GetComponent<CanvasGroup>();
            closeButton.onClick.AddListener(Close);
            quitButton.onClick.AddListener(() =>
            {
                _closeState = GameState.ToIntro;
                Close();
            });
            
            clearSaveButton.onClick.AddListener(() =>
            {
                if (ConfirmingDelete == 2) Globals.ResetGameSave();
                else ConfirmingDelete++;
            });
            
            foreach ((BookPage page, BookGroup group) in pages)
            {
                group.bookRibbon.onClick.AddListener(() => SetPage(page));
            }

            var rt = pages[_page].bookRibbon.transform as RectTransform;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, EXPANDED_RIBBON_HEIGHT);

            Upgrades.Upgrades.OnUpgradePurchased += _ =>
            {
                if (Manager.Upgrades.AnyAdjacencies) adjacencyBonuses.DOAnchorPos(AdjacencyOpenPos, 0.5f);
            };
            
            BookButton.OnClicked += (toUnlocks) =>
            {
                if (toUnlocks) SetPage(BookPage.Upgrades);
                Open();
            };
            
            RequestDisplay.OnNotificationClicked += () =>
            {
                if (!Manager.State.InGame) return;
                Open(BookPage.Upgrades);
            };

            Manager.Inputs.ToggleBook.performed += _ =>
            {
                if (Manager.Cards.SelectedCard != null)
                {
                    Manager.Cards.SelectCard(-1);
                }
                else if (Select.Instance.SelectedStructure != null) Select.Instance.SelectedStructure = null;
                else if (Manager.State.InGame || Manager.State.InIntro || (Manager.State.InMenu && _isOpen)) Toggle();
            };

            Manager.Inputs.Close.performed += _ =>
            {
                if (!_isOpen || _transitioning || Manager.Upgrades.BoxOpen || resDropdown.isOpen) return;
                Close();
            };

            Manager.Inputs.NavigateBookmark.performed += obj => 
            {
                if (!_isOpen || _changingPage || _disableNavigation) return;
                var val = -(int)obj.ReadValue<float>();
                var newPage = Mathf.Abs(((int)_page + val + 4) % 4);
                SetPage((BookPage)newPage);
            };

            // Position it as closed on start
            transform.localPosition = ClosePos;
        }

        public void Open(BookPage page)
        {
            SetPage(page);
            Open();
        }

        private void Open()
        {
            if (!Manager.State.InGame && !Manager.State.InIntro && !Manager.State.InDialogue) return;
            _transitioning = true;
            _closeState = Manager.State.Current;
            _fromGame = Manager.State.InGame;
            SetPage(_page); // Update the current page settings
            Manager.State.EnterState(GameState.InMenu);
            canvas.enabled = true;
            transform.DOPunchScale(PunchScale, animateInDuration, 0, 0);
            transform.DOLocalMove(Vector3.zero, animateInDuration)
                .OnComplete(() =>
                {
                    closeButton.gameObject.SetActive(true);
                    _closeButtonCanvas.alpha = 0;
                    if (!Manager.Inputs.UsingController) _closeButtonCanvas.DOFade(1, 0.5f);
                    _transitioning = false;
                    _isOpen = true;
                    Manager.Jukebox.PlayBookThump();
                    pages[_page].canvasGroup.GetComponent<UIController>().OnOpen();
                    _changingPage = false;
                    if (_page == BookPage.Reports) CardsBookList.ScrollActive = true;

                    if (Manager.Upgrades.AnyAdjacencies) adjacencyBonuses.DOAnchorPos(AdjacencyOpenPos, 0.5f);

                    OnOpened?.Invoke();
                });
        }

        private void Close()
        {
            if (_disableNavigation) return;
            ConfirmingDelete = 0;
            _transitioning = true;
            closeButton.gameObject.SetActive(false); 
            Manager.SelectUi(null);
            adjacencyBonuses.DOAnchorPos(AdjacencyClosedPos, 0.3f);
            transform.DOPunchScale(PunchScale, animateOutDuration, 0, 0);
            transform.DOLocalMove(ClosePos, animateOutDuration)
                .OnComplete(() =>
                {
                    canvas.enabled = false;
                    _transitioning = false;
                    _isOpen = false;
                    Manager.State.EnterState(_closeState);
                });
            pages[_page].canvasGroup.GetComponent<UIController>()?.OnClose();
        }

        public void EnableNavigation()
        {
            _disableNavigation = false;
            foreach ((BookPage page, BookGroup group) in pages) group.bookRibbon.interactable = true;
            closeButton.interactable = true;
        }
        
        public void DisableNavigation()
        {
            _disableNavigation = true;
            foreach ((BookPage page, BookGroup group) in pages) group.bookRibbon.interactable = page == _page;
            closeButton.interactable = false;
        }

        private void Toggle()
        {
            if (_transitioning) return;
            if (_isOpen) Close();
            else Open();
        }
    }
}
