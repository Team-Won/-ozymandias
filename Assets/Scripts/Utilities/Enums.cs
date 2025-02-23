﻿namespace Utilities
{
    public enum Stat
    {
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist,
        Housing,
        Food,
        Spending,
        Defence,
        Threat,
        Stability
    }

    public enum StructureType
    {
        Building,
        Ruins,
        Terrain,
        Quest
    }
    
    public enum BuildingType
    {
        GuildHall,
        Blacksmith,
        Tavern,
        Herbalist,
        Shrine,
        Library,
        House,
        Inn,
        Farm,
        Watchtower,
        Markets,
        Tannery,
        FightingRing,
        Enchanter,
        Plaza,
        BathHouse,
        Monastery,
        Bakery,
        Brewery,
        HuntingLodge,
        Armoury,
        Barracks,
        Tailor,
        Apothecary,
        Jeweller,
        Cartographer
    }

    public enum EventType
    {
        Other = 0,
        Advert = 1,
        Flavour = 2,
        Story = 3,
        AdventurersJoin = 4,
        AdventurersLeave = 5,
        Radiant = 6,
        Threat = 7,
        BrawlerRequest = 8,
        OutriderRequest = 9,
        PerformerRequest = 10,
        DivinerRequest = 11,
        ArcanistRequest = 12,
        Merchant = 13
    }

    public enum OutcomeType
    {
        Debug,
        FlavourText,
        ChainEvent,
        AdventurersAdded,
        AdventurersRemoved,
        BuildingDamaged,
        CardUnlocked,
        GameOver,
        ModifierAdded,
        ModifierRemoved,
        QuestAdded,
        QuestCompleted,
        RequestAdded,
        RequestCompleted,
        ThreatAdded,
        SetFlag,
        WealthAdded,
        WealthAddedRandom,
        TerrainRemoved,
        SetStability,
    }

    public enum RequestType
    {
        AttractAdventurers,
        LoseAdventurers,
        ConstructBuildings,
        DestroyBuildings,
        DestroyStructures,
        PreserveStructures,
        CompleteQuests,
        KeepHappy,
        KeepUpset,
        FoodSurplus,
        HousingShortage,
        LowStability,
        MaxStability,
        ConstructBuildingsInTurn,
        DiscoverCards
    }

    public enum Guild
    {
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist
    }
    
    public enum TooltipType
    {
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist,
        Housing,
        Food,
        Wealth,
        Stability,
        Newspaper,
        Progress,
        Quests,
        NextTurn,
        Threat,
        Defence,
        GuildTokens
    }
    
    public enum HighlightState
    {
        Inactive,
        Valid,
        Invalid,
        Highlighted
    }
    
    public enum Direction
    {
        Left,
        Forward,
        Right,
        Back
    }

    public enum GameState
    {
        Loading,   // ┌────────────────────────────────┐
        ToIntro,   // │              ┌──►NextTurn───┐  │
        InIntro,   // │ ┌►ToGame─►InGame◄──►InMenu◄─┘  │
        ToGame,    // │ │                    ▲ │ │     │
        InGame,    // │ │   ┌────────────────┘ │ └──┐  │
        NextTurn,  // │ │   ▼      ┌─Loading   │    │  │
        InMenu,    // │ ├─InIntro◄─┴──ToIntro◄─┤─┐  │  │
        EndGame,   // │ └►ToCredits─►InCredits─┘ │  ▼  │
        ToCredits, // │                        GameEnd │
        InCredits, // └────────────────────────────────┘
        InDialogue
    }

    public enum UpgradeType
    {
        Discoveries,
        //Starting Stat Boosts
        Brawler,
        Outrider,
        Performer,
        Diviner,
        Arcanist,
        Housing,
        Food,
        Spending,
        Stability,
        Wealth,
        //Price Reductions
        Ruins,
        Refund,
        Terrain,
        QuestCost,
        //Special Abilities
        FreeCard,
        CampSpread,
        MaxAdventurerSpawn,
        //Adjacency Bonuses
        Blacksmith, // + Enchanter: Arcane Upselling - Spending
        Farm, // + 2 Farms: Cross Pollination - Food
        Herbalist, // + Apothecary: Fresh Ingredients - Outrider
        House, // + Bakery: Freshly Baked - Spending
        Inn, // + Fighting Pit: Weekend Games - Brawler
        Library, // + Ruins: Research Expeditions - Arcanist
        Markets, // + Bakery: Camping Supplies - Housing
        Shrine, // + Jewellers: Holy Offerings - Diviners
        Tavern, // + Brewery: Craft Beers - Performers
        Watchtower, // + No Neighbours: Blissful Silence - Arcanists
        Armoury, // + Blacksmith: Military Industrial Complex - Brawler
        Barracks, // + Tailor: Guard Uniforms - Defence
        BathHouse, // + Water source: Hot Springs - Diviners
        Cartographers, // + Forest: Exploration Parties - Outrider
        HuntingLodge, // + Forest: Fresh Meat - Food
        Monastery, // + No Neighbours: Meditative Aura - Diviners
        Plaza, // + Guild Hall: Town Centre - Spending
        Tailor, // + Tannery: Stylish Armour - Performers
        Tannery, // + Hunting Lodge: Extra Padded Armour - Defence
        // More Abilities
        VisibleAdjacencyBonuses
    }
    
    public enum CursorType
    {
        Pointer,
        Build,
        Grab
    }
    
    public enum Location
    {
        Grid,
        Forest,
        Mountains,
        Dock
    }

    public enum Flag
    {
        StoryActive,
        Scarecrows,
        Cosmetics,
        MineStrike,
        DemonicKey,
        GoldenKey,
        InstructionManual,
        BottomlessFlask
    }

    public enum Secret
    {
        Dog,
        Fishing,
        Waterfall,
        WorldEdge,
        GuildHall
    }
}
