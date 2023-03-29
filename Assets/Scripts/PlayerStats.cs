using System;
using System.Linq;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    /// <summary>Stats for the player.</summary>
    public static class PlayerStats
    {
        /// <summary>Good experience collected by the player.</summary>
        public static uint Exp { get; set; } = 0;

        /// <summary>Player's position on the map.</summary>
        public static Vector2 Position { get; set; } = Vector2.zero;

        /// <summary>Maximum health of the player character.</summary>
        public static uint MaxHealth => 100 + Exp / 2;

        /// <summary>The amount of damage player does to enemies.</summary>
        public static uint Damage => 10 + Exp;

        /// <summary>The speed of the player.</summary>
        public static uint Speed => 4 + Exp / 20;

        /// <summary>Power of the player compared to the difficulty of the enemies.</summary>
        public static decimal Power => (decimal)Mathf.Pow(Damage, 1.5f) * Speed * MaxHealth;

        /// <summary>Skills the player possesses.</summary>
        public static List<string> Skills { get; set; } = new List<string>();

        /// <summary>Items that the player possesses.</summary>
        public static Dictionary<string, uint> Items { get; private set; } =
            new Dictionary<string, uint>();

        /// <summary>The amount of money that the player has.</summary>
        public static uint Money { get; set; } = 0;

        /// <summary>The game time in minutes.</summary>
        public static uint Time { get; set; } = 0;

        /// <summary>Add a new item for the player.</summary>
        public static void AddItem(string name, uint count = 1)
        {
            // Add the `count` amount of items.
            Items[name] = Items.GetValueOrDefault(name, (uint)0) + count;
        }

        /// <summary>Get the amount of a specific item.</summary>
        public static uint GetItemCount(string name)
        {
            // Get the item or 0 if we have none of that type.
            return Items.GetValueOrDefault(name, (uint)0);
        }

        /// <summary>Remove an item from the player.</summary>
        public static void RemoveItem(string name)
        {
            // If we have this type of item.
            if (GetItemCount(name) > 0)
            {
                // Remove it.
                Items[name]--;
            }

            // If we have no items of this type left.
            if (GetItemCount(name) == 0)
            {
                // Remove the item.
                Items.Remove(name);
            }
        }

        /// <summary>Data structure used for saving the player stats.</summary>
        [Serializable]
        private struct SaveData
        {
            /// <summary>Data holding an item and its count.</summary>
            [Serializable]
            public struct Item
            {
                public string name;
                public uint count;
            }

            public uint exp;
            public Vector2 position;
            public string[] skills;
            public Item[] items;
            public uint money;
        }

        /// <summary>Reset the player stats to their default values.</summary>
        public static void Reset()
        {
            Exp = 0;
            Position = Vector2.zero;
            Skills = new List<string>();
            Items = new Dictionary<string, uint>();
            Money = 10; // Start with some money.
        }

        /// <summary>Save file's name.</summary>
        private const string SaveFile = "player.sav";

        /// <summary>Save the player stats.</summary>
        public static void Save()
        {
            // Use AES encryption to save the JSON to a file.
            // Might be an overkill in this situation,
            // especially as we put the encryption key to PlayerPrefs,
            // which allows easy circumvention of the encryption.
            // Nonetheless this should make it a bit harder to edit the save file by hand.
            using (Aes aes = Aes.Create())
            using (FileStream fs = File.Create(Path.Join(Application.persistentDataPath, SaveFile)))
            using (
                CryptoStream crypto = new CryptoStream(
                    fs,
                    aes.CreateEncryptor(aes.Key, aes.IV),
                    CryptoStreamMode.Write
                )
            )
            using (StreamWriter writer = new StreamWriter(crypto))
            {
                // Write the initialization vector to the start of the file.
                fs.Write(aes.IV, 0, aes.IV.Length);

                // Get the values.
                SaveData saveData = new SaveData
                {
                    exp = Exp,
                    position = Position,
                    skills = Skills.ToArray(),
                    items = Items
                        .ToList()
                        // Convert to `SaveData.Item` struct.
                        .Select((item) => new SaveData.Item { name = item.Key, count = item.Value })
                        .ToArray(),
                    money = Money,
                };

                // Write to the file.
                writer.Write(JsonUtility.ToJson(saveData));

                // Save the key to PlayerPrefs.
                PlayerPrefs.SetString("SaveKey", System.Convert.ToBase64String(aes.Key));
                PlayerPrefs.Save();
            }
        }

        /// <summary>Load the player stats.</summary>
        public static void Load()
        {
            using (Aes aes = Aes.Create())
            using (
                FileStream fs = File.OpenRead(Path.Join(Application.persistentDataPath, SaveFile))
            )
            {
                byte[] IV = new byte[aes.IV.Length];
                // Read the initialization vector from the beginning of the file.
                fs.Read(IV, 0, IV.Length);

                // Get the encryption key from the PlayerPrefs.
                byte[] key = System.Convert.FromBase64String(PlayerPrefs.GetString("SaveKey"));

                using (
                    CryptoStream crypto = new CryptoStream(
                        fs,
                        aes.CreateDecryptor(key, IV),
                        CryptoStreamMode.Read
                    )
                )
                using (StreamReader reader = new StreamReader(crypto))
                {
                    // Read the file.
                    SaveData saveData = JsonUtility.FromJson<SaveData>(reader.ReadToEnd());

                    // Reset before loading.
                    Reset();

                    // Set the values.
                    Exp = saveData.exp;
                    if (saveData.position != null)
                        Position = saveData.position;
                    if (saveData.skills != null)
                        Skills = new List<string>(saveData.skills);
                    if (saveData.items != null)
                        Items = new Dictionary<string, uint>(
                            saveData.items.Select(
                                // Convert back to key value pairs.
                                (item) => new KeyValuePair<string, uint>(item.name, item.count)
                            )
                        );
                    Money = saveData.money;
                }
            }
        }

        /// <summary>Wether the save file exists or not.</summary>
        public static bool SavePresent =>
            File.Exists(Path.Join(Application.persistentDataPath, SaveFile));
    }
}
