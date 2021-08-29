using System;
using System.Collections.Generic;
using Cards;
using Inputs;
using Managers;
using Map;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Utilities;
using static Managers.GameManager;

namespace Buildings
{
    public class Place : MonoBehaviour
    {
        public static Action OnBuildingPlaced;

        public const int Deselected = -1;
        public static int Selected = Deselected;

        [SerializeField] private Card[] cards;
        [SerializeField] private LayerMask layerMask;

        //TODO: Move this variable and logic into the BuildingCards
        private List<GameObject> _remainingBuildings = new List<GameObject>();

        private Camera _cam;
        private int _rotation;
        private List<Cell> _selectedCells = new List<Cell>();
        private int _previousSelected = Selected;
        private ToggleGroup _toggleGroup;
        
        private Cell ClosestCellToCursor
        {
            get
            {
                Ray ray = _cam.ScreenPointToRay(new Vector3(Manager.Inputs.MousePosition.x, Manager.Inputs.MousePosition.y,
                    _cam.nearClipPlane));
                Physics.Raycast(ray, out RaycastHit hit, 200f, layerMask);
                return Manager.Map.GetClosestCell(hit.point);       
            }
        }

        private void Start()
        {
            _cam = Camera.main;

            Click.OnLeftClick += LeftClick;
            Click.OnRightClick += RightClick;
            State.OnEnterState += () =>
            {
                if( Manager.State.NextTurn) NewCards();
                _toggleGroup.SetAllTogglesOff();
            };
            
            _remainingBuildings = Manager.Cards.All;
            for (var i = 0; i < 3; i++) cards[i].buildingPrefab = _remainingBuildings.PopRandom();
            _toggleGroup = GetComponent<ToggleGroup>();
            Manager.Inputs.IA_RotateBuilding.performed += RotateBuilding;
        }

        private void RotateBuilding(UnityEngine.InputSystem.InputAction.CallbackContext obj)
        {
            if (Selected == Deselected) return;
            int dir = (int)Mathf.Sign(obj.ReadValue<float>());
            _rotation += dir;
            if (_rotation < 0) _rotation = 3;
            _rotation %= 4;
        }

        //TODO: Move this logic from update to on mouse input so its not recalculating every frame
        private void Update()
        {
            // Clear previous highlights
            Manager.Map.Highlight(_selectedCells, HighlightState.Inactive);
            _selectedCells.Clear();

            if (_previousSelected != Selected) // If selected has changed
            {
                Manager.Cursor.Current = Selected != Deselected
                        ? CursorSelect.CursorType.Build
                        : CursorSelect.CursorType.Pointer;
                _previousSelected = Selected;
            }

            if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;

            Cell closest = ClosestCellToCursor;
            if (closest == null || !closest.Active) return;

            Building building = cards[Selected].buildingPrefab.GetComponent<Building>();

            _selectedCells = Manager.Map.GetCells(building, closest.Id, _rotation);

            HighlightState state = Cell.IsValid(_selectedCells)
                ? HighlightState.Valid
                : HighlightState.Invalid;
            Manager.Map.Highlight(_selectedCells, state);
        }

        public bool ChangingCard(int cardNum) => cards[cardNum].isReplacing;

        public void ImitateHover(int cardNum)
        {

            if (cardNum >= 0)
            {
                if (cards[cardNum].isReplacing) return;
                cards[cardNum].SelectCard();
                if (cards[cardNum].toggle.interactable)
                {
                    cards[cardNum].toggle.isOn = true;
                }
            }

            for (int i = 0; i < cards.Length; i++)
            {
                if (i == cardNum) continue;

                cards[i].DeselectCard();
                if (cards[i].toggle.isOn) cards[i].toggle.isOn = false;
            }
        }

        public int NavigateCards(int newCardNum)
        {
            if (newCardNum > cards.Length - 1)
                newCardNum = 0;
            else if (newCardNum < 0)
                newCardNum = cards.Length - 1;

            return newCardNum;
        }

        private void LeftClick()
        {
            if (Selected == Deselected || EventSystem.current.IsPointerOverGameObject()) return;
            Cell closest = ClosestCellToCursor;
            if (closest == null) return;
            Click.PlacingBuilding = true;
            
            int i = Selected;
            Building building = Instantiate(cards[i].buildingPrefab, Manager.Buildings.transform)
                .GetComponent<Building>();
            if (!Manager.Buildings.Add(building, closest.Id, _rotation, animate: true))
            {
                Destroy(building.gameObject);
                return;
            }

            cards[i].SwitchCard(ChangeCard);
            cards[i].toggle.isOn = false;
            Selected = Deselected;

            OnBuildingPlaced?.Invoke();
            
            GameManager.Manager.Achievements.Unlock(Achievement.BuildOneBuilding);
        }

        private void RightClick()
        {
            if (Selected == Deselected) return;
            _rotation++;
            _rotation %= 4;
        }

        private void NewCards()
        {
            _toggleGroup.SetAllTogglesOff();
            for (int i = 0; i < 3; i++) cards[i].SwitchCard(ChangeCard);
        }

        private void ChangeCard(int i)
        {
            if (_remainingBuildings.Count == 0) _remainingBuildings = Manager.Cards.All;
            bool valid = false;

            // Confirm no duplicate buildings
            while (!valid)
            {
                valid = true;
                cards[i].buildingPrefab = _remainingBuildings.PopRandom();
                for (int j = 0; j < 3; j++)
                {
                    if (i == j) continue;
                    if (cards[j].buildingPrefab == cards[i].buildingPrefab) valid = false;
                }
            }

            UpdateUi();
        }
    }
}
