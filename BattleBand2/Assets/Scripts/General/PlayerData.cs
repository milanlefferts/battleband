using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

// # Player Data storage class #
[Serializable]
class PlayerSaveData
{
    public Dictionary<string, int[]> levelScores = new Dictionary<string, int[]>();
    public int[] abilityPointsLeft, statStamina, statConfidence, statTechnique, unlockedInstruments, currentInstruments;
    public int playerLevel, playerFame;
    public bool firstTime;
}

public class PlayerData : MonoBehaviour {
    // Allows remote access of unique instance
    public static PlayerData Instance
    {
        get
        {
            return instance;
        }
    }
    private static PlayerData instance;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }

        New();
    }

    // # Data to be stored
    public Dictionary<string, int[]> levelScores = new Dictionary<string, int[]>();

    public int[] abilityPointsLeft, statStamina, statConfidence, statTechnique, unlockedInstruments, currentInstruments;
    public int storyProgress, playerLevel, playerFame, difficulty;
    public bool firstTime;

    public void New ()
    {
        storyProgress = 0;
        playerLevel = 1;
        playerFame = 50;

        levelScores.Clear();

        abilityPointsLeft = new int[] { 0, 0, 0 };
        statStamina = new int[] { 0, 0, 0 };
        statConfidence = new int[] { 0, 0, 0 };
        statTechnique = new int[] { 0, 0, 0 };
        unlockedInstruments = new int[] { 0, 0, 0 };
        currentInstruments = new int[] { 0, 0, 0 };

        levelScores.Add("Rooftop", new int[] { 0, 0, 0 });
        firstTime = true;
    }

    public void Save ()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/playerInfo.dat");
        PlayerSaveData data = new PlayerSaveData();

        // Save stuff here
        data.levelScores = levelScores;

        data.abilityPointsLeft = abilityPointsLeft;
        data.statStamina = statStamina;
        data.statConfidence = statConfidence;
        data.statTechnique = statTechnique;
        data.unlockedInstruments = unlockedInstruments;
        data.currentInstruments = currentInstruments;

        data.playerLevel = playerLevel;
        data.playerFame = playerFame;

        data.firstTime = firstTime;

        bf.Serialize(file, data);
        file.Close();
        Debug.Log("Data Saved");
    }

    public void Load ()
    {
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerSaveData data = (PlayerSaveData)bf.Deserialize(file);
            file.Close();

            //Load stuff here
            levelScores = data.levelScores;

            abilityPointsLeft = data.abilityPointsLeft;
            statStamina = data.statStamina;
            statConfidence = data.statConfidence;
            statTechnique = data.statTechnique;
            unlockedInstruments = data.unlockedInstruments;
            currentInstruments = data.currentInstruments;

            playerLevel = data.playerLevel;
            playerFame = data.playerFame;

            firstTime = data.firstTime;

            Debug.Log("Data Loaded");
        }
    }

    public void AddStat(Stat stat, int pos)
    {
        if (abilityPointsLeft[pos] <= 0)
        {
            return; // play error sound
        }

        switch (stat)
        {
            case Stat.Stamina:
                if (statStamina[pos] + 1 <= 5)
                {
                    statStamina[pos]++;
                    // play positive sound
                }
                else
                {
                    return; // play negative sound
                }
                break;
            case Stat.Technique:
                if (statTechnique[pos] + 1 <= 5)
                {
                    statTechnique[pos]++;
                    // play positive sound
                }
                else
                {
                    return; // play negative sound
                }
                break;
            case Stat.Confidence:
                if (statConfidence[pos] + 1 <= 5)
                {
                    statConfidence[pos]++;
                    // play positive sound
                }
                else
                {
                    return; // play negative sound
                }
                break;
            default:
                break;
        }
        abilityPointsLeft[pos]--;
    }

    public void RemoveStat(Stat stat, int pos)
    {
        switch (stat)
        {
            case Stat.Stamina:
                if (statStamina[pos] - 1 >= 0)
                {
                    statStamina[pos]--;
                    abilityPointsLeft[pos]++;

                    // play positive sound
                }
                else
                {
                    return; // play negative sound
                }
                break;
            case Stat.Technique:
                if (statTechnique[pos] - 1 >= 0)
                {
                    statTechnique[pos]--;
                    abilityPointsLeft[pos]++;

                    // play positive sound
                }
                else
                {
                    return; // play negative sound
                }
                break;
            case Stat.Confidence:
                if (statConfidence[pos] - 1 >= 0)
                {
                    statConfidence[pos]--;
                    abilityPointsLeft[pos]++;

                    // play positive sound
                }
                else
                {
                    return; // play negative sound
                }
                break;
            default:
                break;
        }
    }
	
}