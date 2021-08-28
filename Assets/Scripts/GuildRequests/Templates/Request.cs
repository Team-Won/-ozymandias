using Events;
using UnityEngine;
using Utilities;
using Event = Events.Event;

namespace GuildRequests.Templates
{
    public abstract class Request : ScriptableObject
    {
        public Guild guild;
        [HideInInspector] public int completed, required;
        public Event completedEvent;
        public bool IsCompleted => completed >= required;
        public abstract string Description { get; }
        protected abstract int RequiredScaled { get; }

        public void Init()
        {
            completed = 0;
            required = RequiredScaled;
        }
        
        public abstract void Start();
        public abstract void Complete();

        public virtual void Configure(EventCreator.RequestConfig config) {}
    }
}
