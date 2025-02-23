﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cards;
using DG.Tweening;
using Events;
using Inputs;
using Managers;
using Quests;
using Reports;
using Structures;
using TMPro;
using Tooltip;
using UI;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;
using String = Utilities.String;

namespace Tutorial
{
    public enum GuidePose
    {
        Neutral,
        Dismissive,
        Embarrassed,
        FingerGuns,
        PointingUp
    }

    public class Tutorial : MonoBehaviour
    {
        public static bool Active, DisableSelect, DisableNextTurn, ShowShade;
        
        [SerializeField] private SerializedDictionary<GuidePose, Sprite> poses;
        
        [SerializeField] private Image guide;
        [SerializeField] private TextMeshProUGUI text;
        [SerializeField] private RectTransform dialogue, objectives, objectiveContainer;
        [SerializeField] private GameObject objectivePrefab, blocker;
        [SerializeField] private Button next;
        
        private List<Line> _currentSection;
        private List<Objective> _currentObjectives;
        private int _sectionLine;
        private Action _onObjectivesComplete;

        private GameState _exitState = GameState.InGame;
        private Action _exitAction = null;

        private bool _firstCardUnlock, _firstTownDestroyed;
        
        #region Dialogue
        
        private struct Line
        {
            public Line(string dialogue, GuidePose? pose = null, Action onNext = null)
            {
                Dialogue = dialogue;
                Pose = pose;
                OnNext = onNext;
            }
        
            public readonly string Dialogue;
            public GuidePose? Pose;
            public readonly Action OnNext;
        }
        
        private void Start()
        {
            Manager.Inputs.DialogueNext.performed += _ => NextLine();
            next.onClick.AddListener(NextLine);
            
            State.OnNewGame += StartTutorial;
            UnlockDisplay.OnUnlockDisplayed += StartUnlockDescription;
            Quests.Quests.OnCampAdded += StartCampsDescription;
            State.OnEnterState += StartUpgradesDescription;

            State.OnLoadingEnd += () =>
            {
                _firstCardUnlock = Manager.Cards.UnlockedCards == 0; 
                _firstTownDestroyed = Manager.Achievements.Milestones[Milestone.TownsDestroyed] == 0;
            };
        }

        private void ShowDialogue(List<Line> lines, GameState exitState = GameState.InGame, Action exitAction = null, bool showShade = false)
        {
            _exitState = exitState;
            _exitAction = exitAction;
            ShowShade = showShade;
            Manager.State.EnterState(GameState.InDialogue);
            _currentSection = lines;
            _sectionLine = 0;
            guide.GetComponent<RectTransform>().DOAnchorPosX(0, 0.5f);
            dialogue.DOAnchorPosY(240, 0.5f);
            Manager.GameHud.Hide(GameHud.HudObject.MenuBar);

            text.text = lines[0].Dialogue;
            text.maxVisibleCharacters = lines[0].Dialogue.Length;
            if (lines[0].Pose != null) guide.sprite = poses[lines[0].Pose.Value];
            blocker.SetActive(true);
            
            Manager.Jukebox.PlayScrunch();
        }

        private void HideDialogue()
        {
            ShowShade = false;
            _currentSection = null;
            guide.GetComponent<RectTransform>().DOAnchorPosX(-600, 0.5f);
            dialogue.DOAnchorPosY(0, 0.5f);
            Manager.GameHud.Show(GameHud.HudObject.MenuBar);
            blocker.SetActive(false);
            Manager.State.EnterState(_exitState);
            _exitAction?.Invoke();
        }

        private void NextLine()
        {
            if (_currentSection == null) return;

            _currentSection[_sectionLine].OnNext?.Invoke();

            if (++_sectionLine == _currentSection.Count) HideDialogue();
            else
            {
                Line line = _currentSection[_sectionLine];
                text.text = line.Dialogue;
                StopAllCoroutines();
                StartCoroutine(WriteText(line.Dialogue.Length));
                if (line.Pose != null) guide.sprite = poses[line.Pose.Value];
            }
            Manager.Jukebox.PlayClick();
        }

        private IEnumerator WriteText(int characters)
        {
            float time = 0;
            while (time < 1f)
            {
                time += Time.deltaTime;
                text.maxVisibleCharacters = (int)(characters * time);
                yield return null;
            }
            text.maxVisibleCharacters = characters;
        }
        #endregion
        
        #region Objectives
        private class Objective
        {
            public string Description;
            public TextMeshProUGUI Text;
            
            public Toggle Toggle;
            
            public int Required;
            private int _count;

            public bool Increment()
            {
                Text.text = $"{Description}\n({++_count}/{Required})";
                return _count >= Required;
            }
        }
        private Objective CreateObjective(string description, int required = 0)
        {
            GameObject objective = Instantiate(objectivePrefab, objectiveContainer);

            TextMeshProUGUI t = objective.GetComponent<TextMeshProUGUI>();
            t.text = description;
            if (required > 0) t.text += $"\n(0/{required})"; 
            
            return new Objective
            {
                Description = description,
                Text = t,
                Toggle = objective.GetComponentInChildren<Toggle>(),
                Required = required
            };
        }

        private void ClearObjectives()
        {
            foreach (Transform child in objectiveContainer) {
                Destroy(child.gameObject);
            }
        }
        
        private void ShowObjectives()
        {
            objectives.DOAnchorPosX(-250, 0.5f);
        }

        private void HideObjectives()
        {
            objectives.DOAnchorPosX(0, 0.5f);
        }
        private void CompleteObjective(Objective objective)
        {
            objective.Toggle.isOn = true;
            
            if (!_currentObjectives.All(o => o.Toggle.isOn)) return;
            StartCoroutine(Algorithms.DelayCall(1f, () =>
            {
                HideObjectives();
                _onObjectivesComplete?.Invoke();
            }));
        }
        #endregion

        #region Section Callbacks

        private void StartTutorial()
        {
            if (!Active) return;
            ShowDialogue(new List<Line> {
                new Line("Hey you, you're finally awake.", GuidePose.Neutral),
                new Line("I found you washed up on the shore while I was looti... cleaning up the remains of this old town.", GuidePose.Embarrassed),
                new Line("There was a bustling adventuring town here a short while back, got completely overrun. Now just a few ruins is all that's left.", GuidePose.Neutral),
                new Line("Oh! Are you the replacement regional manager that head office was gonna send down?"),
                new Line("You know what, don't answer that, you are now.", GuidePose.Dismissive),
                new Line("I could never run it myself, too much paperwork. But if you were to run the town and leave me to... clean.. then maybe we could work something out.", GuidePose.Neutral), // Should point a 'thinking' pose
                new Line("Let's start you off by showing you around the place...", GuidePose.FingerGuns)
            }, exitAction: StartCameraObjectives);
        }
        
        private void StartCameraObjectives()
        {
            ClearObjectives();
            _currentObjectives = new List<Objective>
            {
                CreateObjective($"Pan Camera\n({(Manager.Inputs.UsingController ? "Left Stick" : "Drag Left Mouse")})"),
                CreateObjective($"Rotate Camera\n({(Manager.Inputs.UsingController ? "Right Stick Horizontal" : "Drag Right Mouse")})"),
                CreateObjective($"Zoom Camera\n({(Manager.Inputs.UsingController ? "Right Stick Vertical" : "Scroll Wheel")})")
            };
            _onObjectivesComplete = StartBuildingDialogue;
            ShowObjectives();

            CameraMovement.OnPan += Pan;
            CameraMovement.OnRotate += Rotate;
            CameraMovement.OnZoom += Zoom;

            void Pan()
            {
                CameraMovement.OnPan -= Pan;
                CompleteObjective(_currentObjectives[0]);
            }
            
            void Rotate()
            {
                CameraMovement.OnRotate -= Rotate;
                CompleteObjective(_currentObjectives[1]);
            }
            
            void Zoom()
            {
                CameraMovement.OnZoom -= Zoom;
                CompleteObjective(_currentObjectives[2]);
            }
        }

        private void StartBuildingDialogue()
        {
            ShowDialogue(new List<Line> {
                new Line("Ok, now that's out of the way let's get this show on the road!", GuidePose.Neutral),
                new Line("There's a few ruins that you can clear to make some space for some new buildings."),
                new Line("You can do it by selecting the ruin, and then the clear button. Then, you can select a card, and place it in the spot!"),
                new Line("Space can be tight when building in the forest, so make sure to rotate the buildings to fit everything in.")
            }, exitAction: StartBuildingObjectives);
        }
        
        private void StartBuildingObjectives()
        {
            // Workaround to make sure the selection is not active immediately after clicking next
            // (to stop controllers selecting structures on exiting tutorial)
            Manager.Camera.MoveTo(Manager.Structures.TownCentre).OnComplete(() => DisableSelect = false);
            
            ClearObjectives();
            _currentObjectives = new List<Objective>
            {
                CreateObjective("Clear Ruins", 3),
                CreateObjective("Place Buildings in Cleared Space", 3),
                CreateObjective($"Rotate to Fit\n({(Manager.Inputs.UsingController ? "LB/RB" : "Right Click")})")
            };
            _onObjectivesComplete = StartAdventurerDialogue;
            ShowObjectives();

            Select.OnClear += ClearRuin;
            Structures.Structures.OnBuild += Build;
            Cards.Cards.OnBuildingRotate += RotateBuilding;
            Manager.Cards.SetTutorialCards();
            Manager.GameHud.Show(GameHud.HudObject.Cards);

            void ClearRuin(Structure structure)
            {
                if (!_currentObjectives[0].Increment()) return;
                Select.OnClear -= ClearRuin;
                CompleteObjective(_currentObjectives[0]);
            }
            
            void Build(Structure structure)
            {
                if (!_currentObjectives[1].Increment()) return;
                Structures.Structures.OnBuild -= Build;
                CompleteObjective(_currentObjectives[1]);
            }
            
            void RotateBuilding()
            {
                Cards.Cards.OnBuildingRotate -= RotateBuilding;
                CompleteObjective(_currentObjectives[2]);
            }
        }

        private void StartAdventurerDialogue()
        {
            void SpawnGuildHall()
            {
                Manager.Camera.MoveTo(Manager.Structures.TownCentre)
                    .OnComplete(() =>
                    {
                        Manager.Structures.SpawnGuildHall();
                    });
            }

            void ShowGameUi()
            {
                Manager.Stats.Wealth = StartingWealth;
                Manager.GameHud.Show(new List<GameHud.HudObject>()
                {
                    GameHud.HudObject.TopBar,
                    GameHud.HudObject.RightButtons
                });
            }

            ShowDialogue(new List<Line> {
                new Line("Nice! That'll hopefully set us up for some success.", GuidePose.Neutral),
                new Line("Now every adventuring town's lifeblood is the Guild Hall. I'll pop one in now, will even clear some space for you, you can thank me later.", onNext: SpawnGuildHall),
                new Line("Pop! I always love doing that.", GuidePose.FingerGuns, ShowGameUi),
                new Line("This next part's gonna be a bit wordy, so buckle up:\nSee all those badges up there? That's your towns stats...", GuidePose.PointingUp),
                new Line($"There are 5 adventuring guilds ({String.StatIcon(Stat.Brawler)}, {String.StatIcon(Stat.Outrider)}, {String.StatIcon(Stat.Performer)}, {String.StatIcon(Stat.Diviner)}, {String.StatIcon(Stat.Arcanist)}), each with their own needs. The happier they are, the more likely an adventurer from that guild will join each turn."),
                new Line($"Your town also needs {String.StatWithIcon(Stat.Housing)}, which gives a chance to spawn random adventurers, and {String.StatWithIcon(Stat.Food)}, which gives a modifier to all guilds satisfaction."),
                new Line("As your population grows, you'll need to keep building to keep everyone happy."),
            }, exitAction: StartAdventurerObjectives);
        }
        
        private void StartAdventurerObjectives()
        {
            DisableNextTurn = false;
            
            // Workaround to make sure the selection is not active immediately after clicking next
            // (to stop controllers selecting structures on exiting tutorial)
            DisableSelect = true;
            Manager.Camera.MoveTo(Manager.Structures.TownCentre).OnComplete(() => DisableSelect = false);
            
            ClearObjectives();
            _currentObjectives = new List<Objective>
            {
                CreateObjective("Go To Next Turn"),
                CreateObjective("Read The News"),
                CreateObjective($"Check Tooltips\n({(Manager.Inputs.UsingController ? "L3" : "Hover over stats")})"),
            };
            _onObjectivesComplete = EndTutorialDialogue;
            ShowObjectives();

            State.OnNextTurnBegin += NextTurn;
            Newspaper.OnNextClosed += ReadNews;
            TooltipPlacement.OnTooltipClosed += CheckTooltips;

            void NextTurn()
            {
                State.OnNextTurnBegin -= NextTurn;
                CompleteObjective(_currentObjectives[0]);
            }
            
            void ReadNews()
            {
                CompleteObjective(_currentObjectives[1]);
            }
            
            void CheckTooltips()
            {
                TooltipPlacement.OnTooltipClosed -= CheckTooltips;
                CompleteObjective(_currentObjectives[2]);
            }
        }

        private void EndTutorialDialogue()
        {
            ShowDialogue(new List<Line> {
                new Line("Well, that's all for now!", GuidePose.Neutral),
                new Line("Wait, I totally forgot to mention how the last town got overrun, huh?", GuidePose.Embarrassed),
                new Line("That bar up the top there is your towns stability, it hits 0, well you can probably guess...", GuidePose.PointingUp),
                new Line($"You want your {String.StatWithIcon(Stat.Defence)} to be larger than {String.StatWithIcon(Stat.Threat)}, which grows over time."),
                new Line("You'll probably manage to make it at least a little while before the hoards of monsters and bandits take over.", GuidePose.Neutral),
                new Line("But no loss, even when this place does inevitably fall apart, you can always try again, and again...", GuidePose.Dismissive),
                new Line("Good Luck!", GuidePose.FingerGuns, EndTutorial)
            });
        }

        private void EndTutorial()
        {
            // Workaround to make sure the selection is not active immediately after clicking next
            // (to stop controllers selecting structures on exiting tutorial)
            DisableSelect = true;
            Manager.Camera.MoveTo(Manager.Structures.TownCentre).OnComplete(() => DisableSelect = false);
            Manager.EventQueue.AddTutorialEndEvents();

            Active = false;
            SaveFile.SaveState(false);
        }

        private void StartCampsDescription(Quest quest)
        {
            if (Manager.Achievements.Milestones[Milestone.CampsCleared] > 0 || !quest.IsRadiant || Manager.Quests.RadiantCount > 1) return;
            ShowDialogue(new List<Line> {
                new Line("Heads up, our scouts have found an enemy camp!", GuidePose.Neutral),
                new Line($"They don't look too threatening right now, but give them a few days to grow and they could become a real problem. Each space they take up adds +1 {String.StatWithIcon(Stat.Threat)} until cleared."),
                new Line("You'll probably wanna get on sending a few adventurers to deal with them. How much you spend to gear them up will influence how long a quest will take."),
                new Line("Just be warned, while they're out on quests, they won't be providing defence to your town."),
            });
        }
        
        private void StartUnlockDescription()
        {
            if (!_firstCardUnlock) return;
            _firstCardUnlock = false;
            
            // Workaround to make sure the selection is not active immediately after clicking next
            // (to stop controllers closing the card on exiting tutorial)
            DisableSelect = true;

            ShowDialogue(new List<Line> {
                new Line("You've just unlocked a new building card! Keep an eye on news in the town, and you might be able to find more.", GuidePose.Neutral),
                new Line("This building will be added to your cards, at least until they all get lost in the ruins of your town..."),
                new Line("But fear not! Check the upgrades page in your book to purchase the ability to rediscover them from the ruins of your previous towns."),
                new Line("Discovering more buildings might just give us the edge to lasting a little longer out here...")
            }, GameState.InMenu, () => StartCoroutine(Algorithms.DelayCall(0.5f, () => DisableSelect = false)), true);
        }
        
        private void StartUpgradesDescription(GameState state)
        {
            if (state != GameState.InIntro || !_firstTownDestroyed || Manager.Achievements.Milestones[Milestone.TownsDestroyed] != 1) return;
            _firstTownDestroyed = false;

            
            IntroHud.DisableOpen = true;
            ShowDialogue(new List<Line> {
                new Line("Well... It all fell to ruins. Don't worry, these things happen, it's really just a matter of time.", GuidePose.Neutral),
                new Line("We can always start again, build in the same deadly forest, hope for the best. I think they call it 'failing forward'."),
                new Line("The good news is, you've attracted some attention, the good kind this time!", GuidePose.FingerGuns),
                new Line("While you were busy getting a town blown up, I was reaching out some to some old friends, letting them know about our little venture.")
            }, GameState.InIntro, OpenBook);
            
            void OpenBook()
            {
                Manager.Book.Open(Book.BookPage.Upgrades);
                Manager.Book.DisableNavigation();
                ShowDialogue(new List<Line> {
                    new Line("Each guild has agreed to help us out, if we do what they want, of course.", GuidePose.PointingUp),
                    new Line("I've cashed in a few favours, should be enough to get you your first upgrade, the rest is on you.", onNext: AddTokens),
                    new Line("If you come across any of your old ruins, that upgrade will let you rediscover previously unlocked cards.", GuidePose.Neutral),
                    new Line("Keep checking the newspaper, some requests will start coming in soon."),
                    new Line("With their support, you might even be able to make it a whole year before it all falls apart!", GuidePose.FingerGuns, EndUpgradesDescription),
                }, GameState.InMenu, showShade: true);
            }
            
            void AddTokens()
            {
                foreach (Guild guild in Enum.GetValues(typeof(Guild))) Manager.Upgrades.GuildTokens[guild]++;
                Manager.Upgrades.Display();
                UpdateUi();
            }
            
            void EndUpgradesDescription()
            {
                IntroHud.DisableOpen = false;
                Upgrades.Upgrades.OnUpgradePurchased += EndUpgradesTutorial;
            }
            
            void EndUpgradesTutorial(UpgradeType upgradeType)
            {
                Manager.Book.EnableNavigation();
                SaveFile.SaveState(false);
                Upgrades.Upgrades.OnUpgradePurchased -= EndUpgradesTutorial;
            }
        }
        
        #endregion
    }
}
