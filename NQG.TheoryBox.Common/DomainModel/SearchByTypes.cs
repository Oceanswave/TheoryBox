namespace NQG.TheoryBox.DomainModel
{
    using System;

    [Flags]
    public enum SearchByTypes
    {
        Artifact = 1,
        Basic = 2,
        Conspiracy = 4,
        Creature = 8,
        Enchantment = 16,
        Instant = 32,
        Land = 64,
        Legendary = 128,
        Ongoing = 256,
        Phenomenon = 512,
        Plane = 1024,
        Planeswalker = 2048,
        Scheme = 4096,
        Snow = 8192,
        Sorcery = 16384,
        Tribal = 32768,
        Vanguard = 65536,
        World = 131072
    }
}
