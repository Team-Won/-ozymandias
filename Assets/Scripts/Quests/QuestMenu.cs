using System;
using System.Collections.Generic;
using DG.Tweening;
using Structures;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Quests
{
    public class QuestMenu : MonoBehaviour
    {
        [SerializeField] private QuestFlyer[] flyers;
        [SerializeField] private Button closeButton, nextButton, previousButton;
        
        [SerializeField] private GameObject navButtonPrefab;
        [SerializeField] private CanvasGroup navButtonsCanvasGroup;
        [SerializeField] private Transform navButtonsContainer;
        
        private const float AnimateAcrossDuration = 0.75f;
        private const float AnimateInDuration = 0.75f;
        private const float AnimateOutDuration = 0.75f;
        private const float AnimateNavItemsFade = 0.25f;
        private const float AnimateNavItemScale = 0.25f;


        private bool _inAnim;
        private int _openFlyer;
        private int _selectedQuestIndex;
        private CanvasGroup _closeButtonCanvas;
        private Canvas _canvas;

        private readonly Vector3 _offScreenPos = new Vector3(-500, 1500, 0);
        private readonly Vector3 _offScreenRot = new Vector3(0, 0, 40);
        private readonly Vector3 _onScreenPos = new Vector3(0, 50, 0);

        private const int FlyerCount = 2;

        private enum SwapDir
        {
            Right = 1,
            Left = -1
        }

        private Quest SelectedQuest
        {
            get => Current.Count > 0 ? Current[_selectedQuestIndex % Current.Count] : null;
            set => _selectedQuestIndex = Current.IndexOf(value);
        }

        private QuestFlyer OpenFlyer => flyers[_openFlyer];
        private QuestFlyer ClosedFlyer => flyers[_openFlyer == 0 ? 1 : 0];
        private static List<Quest> Current => Manager.Quests.Current;

        private bool _opened;

        private readonly Dictionary<Quest, QuestNavButton> _navButtons = new Dictionary<Quest, QuestNavButton>();

        private void Start()
        {
            _closeButtonCanvas = closeButton.GetComponent<CanvasGroup>();
            _canvas = GetComponent<Canvas>();
            
            closeButton.onClick.AddListener(Close);
            Manager.Inputs.ToggleBook.performed += _ => Close();
            
            nextButton.onClick.AddListener(() => ChangeQuest(SwapDir.Right));
            previousButton.onClick.AddListener(() => ChangeQuest(SwapDir.Left));
            
            Select.OnQuestSelected += quest =>
            {
                SelectedQuest = quest;
                OpenFlyer.UpdateContent(SelectedQuest);
                FocusStructure(SelectedQuest.Structure);
                Open();
            };
            
            QuestButton.OnClicked += () =>
            {
                if (!SelectedQuest)
                {
                    if (Manager.Quests.Count == 0) return;
                    SelectedQuest = Manager.Quests.Current[0];
                }
                OpenFlyer.UpdateContent(SelectedQuest);
                FocusStructure(SelectedQuest.Structure);
                Open();
            };

            Quests.OnQuestAdded += (quest) =>
            {
                QuestNavButton navButton = Instantiate(navButtonPrefab, navButtonsContainer).GetComponent<QuestNavButton>();
                navButton.UpdateDisplay(quest, () => ChangeQuest(quest));
                _navButtons.Add(quest, navButton);
            };
            
            Quests.OnQuestRemoved += (quest) =>
            {
                Destroy(_navButtons[quest].gameObject);
                _navButtons.Remove(quest);
            };
            
            foreach (QuestFlyer flyer in flyers)
            {
                // This is a lambda to the call because we only want
                // SelectedQuest evaluated at call time, not assignment
                flyer.OnStartClicked += (adventurersOffset, durationOffset) =>
                {
                    SelectedQuest.Begin(adventurersOffset, durationOffset);
                    flyer.UpdateContent(SelectedQuest);
                };

                flyer.OnAdventurerValueChanged += value =>
                {
                    flyer.UpdateContent(SelectedQuest, true);
                };
                
                flyer.OnDurationValueChanged += value =>
                {
                    flyer.UpdateContent(SelectedQuest, true);
                };
                
                // Set their positions off-screen
                flyer.transform.localPosition = _offScreenPos;
            }
            
            // Hide buttons on start
            DisplayCloseButton(false);
        }

        private static int CycleIdx(int idx, int collectionLength, SwapDir dir)
        {
            return (idx + (int)dir + collectionLength) % collectionLength; // Loop on positive or negative overflow
        }

        private void ChangeQuest(Quest quest)
        {
            if (_inAnim || quest == SelectedQuest) return;
            _inAnim = true;
            SelectedQuest = quest;
            SwapFlyers(SwapDir.Right, SelectedQuest);
            Manager.Jukebox.PlayScrunch();
        }
        private void ChangeQuest(SwapDir dir)
        {
            if (_inAnim) return;
            _inAnim = true;
            _selectedQuestIndex = CycleIdx(_selectedQuestIndex, Current.Count, dir);
            SwapFlyers(dir, SelectedQuest);
            Manager.Jukebox.PlayScrunch();
        }

        private void SwapFlyers(SwapDir dir, Quest selectedQuest)
        {
            const float offset = 1500;
            float nextStartX = dir == SwapDir.Left ? offset : -offset;

            QuestFlyer currentFlyer = OpenFlyer;
            QuestFlyer nextFlyer = ClosedFlyer;
            
            currentFlyer.OnClose();
            nextFlyer.UpdateContent(selectedQuest);
            nextFlyer.transform.localPosition = new Vector3(nextStartX, 50, 0);
            _openFlyer = CycleIdx(_openFlyer, FlyerCount, dir);

            UpdateNavButtonsScale();
            
            currentFlyer.transform
                .DOLocalMove(new Vector3(-nextStartX,50, 0), AnimateAcrossDuration)
                .OnStart(() =>
                {
                    FocusStructure(selectedQuest.Structure);
                    PunchRotation(currentFlyer.transform, (int)dir);
                    PunchRotation(nextFlyer.transform, (int)dir);
                    nextFlyer.gameObject.SetActive(true);
                    nextFlyer.transform.DOLocalMove(_onScreenPos, AnimateAcrossDuration);
                })
                .OnComplete(() =>
                {
                    nextFlyer.OnOpen();
                    currentFlyer.gameObject.SetActive(false);
                    _inAnim = false;
                });
        }

        private static void PunchRotation(Transform t, int dir)
        {
            t.DOLocalRotate(new Vector3(0, 0, dir * -5), AnimateAcrossDuration/2)
             .OnComplete(() => 
             {
                 t.transform
                     .DOLocalRotate(new Vector3(0, 0, 0), AnimateAcrossDuration/3)
                     .SetDelay(AnimateInDuration/3);
                });
        }

        private static void FocusStructure(Structure structure)
        {
            Vector3 buildingPos = structure.transform.position;
            buildingPos.y = 0f;
            Manager.Camera.MoveTo(buildingPos, AnimateAcrossDuration);
        }

        private void Open()
        {
            if (_opened) return;
            _opened = true;
            DisplayCloseButton(false);
            Manager.State.EnterState(GameState.InMenu);
            Manager.Jukebox.PlayScrunch();
            navButtonsCanvasGroup.gameObject.SetActive(Current.Count > 1);
            UpdateNavButtonsScale();
            _canvas.enabled = true;
            OpenFlyer.transform.eulerAngles = _offScreenRot;
            OpenFlyer.transform
                .DOLocalMove(_onScreenPos, AnimateInDuration)
                .OnStart(() => OpenFlyer.gameObject.SetActive(true))
                .OnComplete(() =>
                {
                    OpenFlyer.OnOpen();
                    DisplayCloseButton(true);
                    Manager.Inputs.OpenQuests.performed += OpenQuests_performed;
                    Manager.Inputs.Close.performed += OpenQuests_performed;
                    if (Current.Count > 1) Manager.Inputs.NavigateBookmark.performed += NavigateFlyers;
                });
            OpenFlyer.transform.DOLocalRotate(Vector3.zero, AnimateInDuration);
        }

        private void Close()
        {
            if (!_opened) return;
            Manager.Inputs.OpenQuests.performed -= OpenQuests_performed;
            Manager.Inputs.Close.performed -= OpenQuests_performed;
            if (Current.Count > 1) Manager.Inputs.NavigateBookmark.performed -= NavigateFlyers;
            DisplayCloseButton(false);
            OpenFlyer.OnClose();
            OpenFlyer.transform.DOLocalMove(_offScreenPos, AnimateOutDuration);
            OpenFlyer.transform
                .DOLocalRotate(_offScreenRot, AnimateOutDuration)
                .OnComplete(() =>
                {
                    _canvas.enabled = false;
                    _opened = false;
                    Manager.State.EnterState(GameState.InGame);
                });
        }

        private void UpdateNavButtonsScale()
        {
            foreach ((Quest quest, QuestNavButton nav) in _navButtons)
                nav.transform.DOScale(SelectedQuest == quest ? 1.2f : 1, AnimateNavItemScale);
        }

        private void DisplayCloseButton(bool display)
        {
            navButtonsCanvasGroup.DOFade(display ? 1.0f : 0.0f, AnimateNavItemsFade);
            _closeButtonCanvas.DOFade(display ? 1.0f : 0.0f, AnimateNavItemsFade);
        }

        private void OpenQuests_performed(InputAction.CallbackContext obj)
        {
            Close();
        }

        private void NavigateFlyers(InputAction.CallbackContext obj)
        {
            SwapDir dir = (SwapDir)obj.ReadValue<float>();
            ChangeQuest(dir);
        }
    }
}
