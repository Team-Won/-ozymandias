namespace Utilities
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

    public enum BuildingType
    {
        Terrain,
        Ruins,
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
        FightingPit,
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
        Cartographer,
        Quest
    }

    public enum EventType
    {
        Other = 0,
        Advert = 1,
        Flavour = 2,
        Story = 3,
        AdventurersJoin = 4,
        AdventurersLeave = 5,
        Chaos = 6,
        Threat = 7,
        BrawlerRequest = 8,
        OutriderRequest = 9,
        PerformerRequest = 10,
        DivinerRequest = 11,
        ArcanistRequest = 12
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
        Defence
    }
    
    public enum HighlightState
    {
        Inactive,
        Valid,
        Invalid
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
        InCredits  // └────────────────────────────────┘
    }

    public enum Achievement
    {
        Test = 0,
        BuildOneBuilding = 1
    }
    
    public enum Upgrade
    {
        Test = 0,
    }
}
