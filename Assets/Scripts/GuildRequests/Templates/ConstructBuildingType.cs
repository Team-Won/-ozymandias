using System;
using Buildings;
using Events;
using UnityEngine;
using Utilities;

namespace GuildRequests.Templates
{
    [CreateAssetMenu(fileName = "Construct Building Type", menuName = "Requests/Construct Building Type")]
    public sealed class ConstructBuildingType : Request
    {
        [SerializeField] private BuildingType buildingType;
        public override string Description => $"Build {required} {buildingType}s";
        protected override int RequiredScaled => 3;

        public override void Start()
        {
            Buildings.Buildings.OnBuild += CheckBuilt;
        }
        
        public override void Complete()
        {
            Buildings.Buildings.OnBuild -= CheckBuilt;
        }

        private void CheckBuilt(Building building)
        {
            if (building.type == buildingType) completed++;
        }
        
        public override void Configure(EventCreator.RequestConfig config)
        {
            buildingType = config.buildingType;
        }
    }
}
