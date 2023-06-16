using UnityEngine;
using System.Collections.Generic;

public static class RandomUsernameGenerator
{
    private static string[] adjectives = { "Crimson", "Savage", "Blazing", "Fierce", "Mystic", "Shadow", "Eternal", "Brave", "Epic", "Legendary" };
    private static string[] nouns = { "Warrior", "Champion", "Hunter", "Wizard", "Ninja", "Assassin", "Sorcerer", "Paladin", "Rogue", "Mercenary" };

    private static List<string> generatedUsernames = new List<string>();

    public static string GenerateRandomUsername()
    {
        string username = "";

        do
        {
            string adjective = adjectives[Random.Range(0, adjectives.Length)];
            string noun = nouns[Random.Range(0, nouns.Length)];

            username = adjective + noun;
        }
        while (generatedUsernames.Contains(username));

        generatedUsernames.Add(username);

        return username;
    }
}
