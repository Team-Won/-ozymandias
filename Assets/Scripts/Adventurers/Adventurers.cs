using System;
using System.Collections.Generic;
using System.Linq;
using Managers;
using Quests;
using UnityEngine;
using Utilities;
using static Managers.GameManager;
using Random = UnityEngine.Random;

namespace Adventurers
{
    public class Adventurers : MonoBehaviour
    {
        public static Action<Adventurer> OnAdventurerJoin;
        public static Action<Adventurer, bool> OnAdventurerRemoved;

        [SerializeField] private GameObject prefab;
        [SerializeField] private Transform graveyard;

        private readonly List<Adventurer> _adventurers = new List<Adventurer>();

        public int Count => _adventurers.Count;
        public int Available => _adventurers.Count(x => !x.assignedQuest);
        public int Unavailable => _adventurers.Count(x => x.assignedQuest);
        public IEnumerable<Adventurer> List => _adventurers
            .OrderByDescending(x => x.assignedQuest.Title.Conditional(x.assignedQuest))
            .ThenByDescending(x => x.turnJoined);
        
        private void Awake()
        {
            State.OnGameEnd += OnGameEnd;
        }

        public int GetCount(Guild type, bool onlyAvailable = false)
        {
            return _adventurers.Count(a => (!onlyAvailable || !a.assignedQuest) && a.guild == type);
        }
        
        public Adventurer Assign(Quest quest)
        {
            List<Adventurer> available = _adventurers.Where(x => !x.assignedQuest).ToList();
            if (available.Count == 0) return null;

            int randomIndex = Random.Range(0, available.Count);
            available[randomIndex].assignedQuest = quest;
            return available[randomIndex];
        }
        
        public List<Adventurer> Assign(Quest quest, int adventurerCount)
        {
            // Return a list of assigned adventurers
            return Enumerable.Range(0, adventurerCount)
                .Select(i => Assign(quest)).ToList();
        }

        public Adventurer Assign(Quest q, string adventurerName)
        {
            Adventurer assigned = _adventurers.Find(a => a.name == adventurerName);
            if (assigned == null)
            {
                Debug.LogError("Adventurer with name " + name + " not found.");
                return null;
            }
            assigned.assignedQuest = q;
            return assigned;
        }

        private Adventurer New()
        {
            Adventurer adventurer = Instantiate(prefab, transform).GetComponent<Adventurer>();
            _adventurers.Add(adventurer);
            return adventurer;
        }

        public void Add(Guild? category = null)
        {
            Adventurer created = New().Create(category);
            OnAdventurerJoin?.Invoke(created);
        }
        
        public void Add(AdventurerDetails adventurer)
        {
            Adventurer created = New().Load(adventurer);
            if (!Manager.State.Loading) OnAdventurerJoin?.Invoke(created);
        }

        public void Remove(bool kill) // Removes a random adventurer
        {
            List<Adventurer> removable = _adventurers.Where(x => !x.assignedQuest).ToList();
            if (removable.Count == 0) return;
            int randomIndex = Random.Range(0, removable.Count);
            Adventurer toRemove = removable[randomIndex];

            OnAdventurerRemoved?.Invoke(toRemove, kill);
            _adventurers.Remove(toRemove);
            if (kill) toRemove.transform.parent = graveyard.transform;
            else Destroy(toRemove);
        }
        public void Remove(bool kill, Guild guild) // Removes a random adventurer from a guild
        {
            List<Adventurer> removable = _adventurers.Where(x => !x.assignedQuest && x.guild == guild).ToList();
            if (removable.Count == 0) return;
            int randomIndex = Random.Range(0, removable.Count);
            Adventurer toRemove = removable[randomIndex];

            OnAdventurerRemoved?.Invoke(toRemove, kill);
            _adventurers.Remove(toRemove);
            if (kill) toRemove.transform.parent = graveyard.transform;
            else Destroy(toRemove);
        }

        public bool Remove(string adventurerName, bool kill) // Deletes an adventurer by name
        {
            Adventurer toRemove = GameObject.Find(adventurerName)?.GetComponent<Adventurer>();
            if (toRemove == null) return false;
            _adventurers.Remove(toRemove);
            if (kill) toRemove.transform.parent = graveyard.transform;
            else Destroy(toRemove);
            return true;
        }

        public List<AdventurerDetails> Save()
        {
            return _adventurers.Select(x => x.Save()).ToList();
        }

        public void Load(List<AdventurerDetails> adventurers)
        {
            foreach (AdventurerDetails adventurer in adventurers) Add(adventurer);
        }

        private void OnGameEnd()
        {
            _adventurers.Clear();
            foreach (Transform child in transform) Destroy(child.gameObject);
            foreach (Transform child in graveyard) Destroy(child.gameObject);
        }
    }
}
