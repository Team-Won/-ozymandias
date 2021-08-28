using GuildRequests;
using GuildRequests.Templates;
using UnityEngine;
using static Managers.GameManager;

namespace Events.Outcomes
{
    [CreateAssetMenu(fileName = "Request Added Outcome", menuName = "Outcomes/Request Added")]

    public class RequestAdded : Outcome
    {
        public Request request;
    
        public override bool Execute()
        {
            Manager.Requests.Add(request);
            return true;
        }
    
        public override string Description
        {
            get
            {
                if (customDescription != "") return "<color=#007000ff>" + customDescription + "</color>";
                return "<color=#007000ff>" + request.guild +" Request Added: " + request.Description + "</color>";
            }
        }
    }
}
