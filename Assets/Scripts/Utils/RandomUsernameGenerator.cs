using UnityEngine;

public static class RandomUsernameGenerator
{
    private static string[] adjectives = { "Crimson", "Savage", "Blazing", "Fierce", "Mystic", "Shadow", "Eternal", "Brave", "Epic", "Legendary" };
    private static string[] nouns = { "Warrior", "Champion", "Hunter", "Wizard", "Ninja", "Assassin", "Sorcerer", "Paladin", "Rogue", "Mercenary" };

    public static string GenerateRandomUsername()
    {
        string adjective = adjectives[Random.Range(0, adjectives.Length)];
        string noun = nouns[Random.Range(0, nouns.Length)];

        return adjective + noun;
    }
}