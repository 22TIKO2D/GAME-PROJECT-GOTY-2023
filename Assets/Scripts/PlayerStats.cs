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
        /// <summary>Name of the player.</summary>
        public static string Name { get; set; } = "";

        /// <summary>Good experience collected by the player.</summary>
        public static uint Exp { get; set; } = 0;

        /// <summary>Player's position on the map.</summary>
        public static Vector2 Position { get; set; } = Vector2.zero;

        /// <summary>Maximum health of the player character.</summary>
        public static uint MaxHealth => 100 + (uint)(10 * Mathf.Sqrt(Exp));

        /// <summary>The amount of damage player does to enemies.</summary>
        public static uint Damage => 10 + (uint)(10 * Mathf.Log(1 + Exp / 10));

        /// <summary>The speed of the player.</summary>
        public static uint Speed => 4 + (uint)Mathf.Log(1 + Exp / 10);

        /// <summary>Power of the player compared to the difficulty of the enemies.</summary>
        public static uint Power => Damage * Speed * MaxHealth;

        /// <summary>Skills the player possesses.</summary>
        public static List<string> Skills { get; set; } = new List<string>();

        /// <summary>Skill upgrades the player possesses.</summary>
        public static List<string> SkillUpgrades { get; set; } = new List<string>();

        /// <summary>NPC's that player has helped.</summary>
        public static List<string> Beaten { get; set; } = new List<string>();

        /// <summary>Items that the player possesses.</summary>
        public static Dictionary<string, uint> Items { get; private set; } =
            new Dictionary<string, uint>();

        /// <summary>The amount of money that the player has.</summary>
        public static uint Money { get; set; } = 0;

        /// <summary>The game time in minutes.</summary>
        public static uint Time { get; set; } = 0;

        /// <summary>The after dialog NPC's name.</summary>
        public static string AfterDialogName { get; set; } = "";

        /// <summary>The after dialog NPC's text.</summary>
        public static string AfterDialogDesc { get; set; } = "";

        /// <summary>The skill gained from battle.</summary>
        public static string SkillGain { get; set; } = "";

        /// <summary>The current dialog random NPC will show.</summary>
        public static ushort Dialog { get; set; } = 0;

        /// <summary>If the player has seen the tutorial or not.</summary>
        public static bool SeenTutorial { get; set; } = false;

        /// <summary>Add a new item for the player.</summary>
        public static void AddItem(string name, uint count = 1)
        {
            // Add the `count` amount of items.
            Items[name] = GetItemCount(name) + count;
        }

        /// <summary>Get the amount of a specific item.</summary>
        public static uint GetItemCount(string name) =>
            // Get the item or 0 if we have none of that type.
            Items.GetValueOrDefault(name, (uint)0);

        /// <summary>Remove an item from the player.</summary>
        public static void RemoveItem(string name)
        {
            // If we have this type of item.
            if (GetItemCount(name) > 0)
                // Remove it.
                Items[name]--;

            // If we have no items of this type left.
            if (GetItemCount(name) == 0)
                // Remove the item.
                Items.Remove(name);
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

            public string name;
            public uint exp;
            public Vector2 position;
            public string[] skills;
            public string[] skillUpgrades;
            public string[] beaten;
            public Item[] items;
            public uint money;
            public uint time;
            public string afterName;
            public string afterDesc;
            public ushort dialog;
            public bool tutorial;
        }

        /// <summary>Reset the player stats to their default values.</summary>
        public static void Reset()
        {
            Name = "";
            Exp = 0;
            Position = Vector2.zero;
            Skills = new List<string>();
            SkillUpgrades = new List<string>();
            Beaten = new List<string>();
            Items = new Dictionary<string, uint>();
            Money = 10; // Start with some money.
            Time = 0;
            AfterDialogName = "";
            AfterDialogDesc = "";
            Dialog = 0;
            SeenTutorial = false;
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
                    name = Name,
                    exp = Exp,
                    position = Position,
                    skills = Skills.ToArray(),
                    skillUpgrades = SkillUpgrades.ToArray(),
                    beaten = Beaten.ToArray(),
                    items = Items
                        .ToList()
                        // Convert to `SaveData.Item` struct.
                        .Select((item) => new SaveData.Item { name = item.Key, count = item.Value })
                        .ToArray(),
                    money = Money,
                    time = Time,
                    afterName = AfterDialogName,
                    afterDesc = AfterDialogDesc,
                    dialog = Dialog,
                    tutorial = SeenTutorial,
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
                    Name = saveData.name;
                    Exp = saveData.exp;
                    if (saveData.position != null)
                        Position = saveData.position;
                    if (saveData.skills != null)
                        Skills = new List<string>(saveData.skills);
                    if (saveData.skillUpgrades != null)
                        SkillUpgrades = new List<string>(saveData.skillUpgrades);
                    if (saveData.beaten != null)
                        Beaten = new List<string>(saveData.beaten);
                    if (saveData.items != null)
                        Items = new Dictionary<string, uint>(
                            saveData.items.Select(
                                // Convert back to key value pairs.
                                (item) => new KeyValuePair<string, uint>(item.name, item.count)
                            )
                        );
                    Money = saveData.money;
                    Time = saveData.time;
                    AfterDialogName = saveData.afterName;
                    AfterDialogDesc = saveData.afterDesc;
                    Dialog = saveData.dialog;
                    SeenTutorial = saveData.tutorial;
                }
            }
        }

        /// <summary>Remove the save file.</summary>
        public static void RemoveSave() =>
            File.Delete(Path.Join(Application.persistentDataPath, SaveFile));

        /// <summary>Wether the save file exists or not.</summary>
        public static bool SavePresent =>
            File.Exists(Path.Join(Application.persistentDataPath, SaveFile));
    }
}
