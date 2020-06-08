using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum Stat
{
    Stamina,
    Technique,
    Confidence
}

public class PartyController : MonoBehaviour
{
    // Allows remote access for unique instance
    public static PartyController Instance
    {
        get
        {
            return instance;
        }
    }
    private static PartyController instance;

    [Header("Abilities")]
    public int abilitySet;

    private string ability1Name, ability2Name, ability3Name;
    private int ability1Cost, ability2Cost, ability3Cost;

    public Ability[] abilities, tutorialAbilities;
    public Ability[] abilitySet1, abilitySet2;

    private int[] secondInstrumentSet;
    public int[] currentInstrumentSet;

    [Header("Stats")]

    public int[] stamina, technique, confidence; // health, attack, defense
    private int score, scorePeak;

    private void OnEnable()
    {
        EventManager.SwitchAbilitiesEvent += SetupAbilities;
        SceneManager.sceneLoaded += LevelWasLoaded;

    }

    private void OnDisable()
    {
        EventManager.SwitchAbilitiesEvent -= SetupAbilities;
        SceneManager.sceneLoaded -= LevelWasLoaded;

    }

    private void Awake()
    {
        if (!instance) // Singleton pattern
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void LevelWasLoaded(Scene scene, LoadSceneMode mode)
    {
        abilities = abilitySet1;
        currentInstrumentSet = PlayerData.Instance.currentInstruments;
    }

    private void Start()
    {
        // TODO: placeholder
        PlayerData.Instance.New();

        PlayerData.Instance.unlockedInstruments = new int[] { 1, 1, 1 };
        PlayerData.Instance.currentInstruments = new int[] { 1, 1, 1 };


        PlayerData.Instance.abilityPointsLeft[0] = 5;
        PlayerData.Instance.abilityPointsLeft[1] = 5;
        PlayerData.Instance.abilityPointsLeft[2] = 5;

        PlayerData.Instance.Load();
        // end placeholder

        currentInstrumentSet = PlayerData.Instance.currentInstruments;
        secondInstrumentSet = PlayerData.Instance.currentInstruments;

        tutorialAbilities = new Ability[] { AbilityManager.Instance.abilityDB["Shocking Solo"] };

        abilitySet1 = new Ability[] { AbilityManager.Instance.abilityDB["Soul Flare"],
                                            AbilityManager.Instance.abilityDB["Thunderous Solo"],
                                            AbilityManager.Instance.abilityDB["Soothing Melody"],
                                            AbilityManager.Instance.abilityDB["Black Hole"]};

        abilitySet2 = new Ability[] { AbilityManager.Instance.abilityDB["Soul Flare"],
                                            AbilityManager.Instance.abilityDB["Thunderous Solo"],
                                            AbilityManager.Instance.abilityDB["Soothing Melody"],
                                            AbilityManager.Instance.abilityDB["Black Hole"]};

        abilitySet = 1;

        SetAbilitySets();
        
        if (CombatController.Instance != null)
            EventManager.Instance.SwitchAbilities();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneController.Instance.LoadScene("MainMenu"); // TODO: Remove this (temp)
        }
    }

    public void SetupAbilities()
    {
        if (CombatController.Instance == null)
            return;

        if (abilitySet == 0)
        {
            abilitySet = 1;
        } else
        {
            abilitySet = 0;
        }


        switch (abilitySet)
        {
            case 0:
                abilities = abilitySet1;
                currentInstrumentSet = new int[] { 0, 0, 0 };
                break;

            case 1:
                abilities = abilitySet2;
                currentInstrumentSet = secondInstrumentSet;
                break;
        }

        EventManager.Instance.InstrumentSwitch();

    }

    public Ability[] GetAbilities()
    {
        return abilities;
    }

    private void SetAbilitySets()
    {
        ChangeAbility(0, 1, AbilityManager.Instance.LookupAbility(0, PlayerData.Instance.currentInstruments[0]));
        ChangeAbility(1, 1, AbilityManager.Instance.LookupAbility(1, PlayerData.Instance.currentInstruments[1]));
        ChangeAbility(2, 1, AbilityManager.Instance.LookupAbility(2, PlayerData.Instance.currentInstruments[2]));
    }


    // #########
    // # Stats #
    // #########
    public void AddFameScore(int scoreVal)
    {
        PlayerData.Instance.playerFame += CalculateFameGain(scoreVal);
        if (LevelGained())
        {
            PlayerData.Instance.abilityPointsLeft[0] += 1;
            PlayerData.Instance.abilityPointsLeft[1] += 1;
            PlayerData.Instance.abilityPointsLeft[2] += 1;
        }

        if (PlayerData.Instance.levelScores.ContainsKey(SceneController.Instance.currentScene))
        {
            PlayerData.Instance.levelScores[SceneController.Instance.currentScene][PlayerData.Instance.difficulty] = scoreVal;
        }
    }

    // Fame gain depends on difficulty and score
    private int CalculateFameGain(int scoreVal)
    {
        score = scoreVal;
        scorePeak = scoreVal * 2; // TODO: set top score

        if (score > scorePeak)
        {
            score = 1;
            scorePeak = 1;
        }

        var fameModifier = 1f;

        fameModifier += (PlayerData.Instance.difficulty * 0.25f);

        fameModifier += (score / (float)scorePeak);

        var fameFinal = Mathf.RoundToInt(50 * fameModifier);

        return fameFinal;
    }

    private bool LevelGained()
    {
        if (PlayerData.Instance.playerFame >= PlayerData.Instance.playerLevel * 100) // example: Level 2 required 100 Fame, etc.
        {
            PlayerData.Instance.playerLevel++; // Level Up

            return true;
        }
        else
        {
            return false;
        }
    }

    // ###############
    // # Instruments #
    // ###############

    public void ChangeAbility(int character, int abilitySet, Ability ability)
    {
        if (abilitySet == 0)
        {
            abilitySet1[character] = ability;

        }
        else
        {
            abilitySet2[character] = ability;
        }


    }

}