using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager : MonoBehaviour
{
    // Allows remote access for unique instance
    public static EventManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static EventManager instance;

    void Awake()
    {
        instance = this;
    }

    // Generic delegate w single parameter
    public delegate void SingleParameterDelegate<T>(T para);

    // Generic delegate w two parameters
    public delegate void TwoParameterDelegate<T, U>(T para1, U para2);

    // Generic delegate w three parameters
    public delegate void ThreeParameterDelegate<T, U, X>(T para1, U para2, X para3);


    // ####################
    // # Character Screen #
    // ####################
    public static event TwoParameterDelegate<Vector3, Vector3> MoveCameraSmoothEvent;
    public void MoveCameraSmooth(Vector3 song, Vector3 bpm)
    {
        MoveCameraSmoothEvent(song, bpm);
    }


    // ###########
    // # General #
    // ###########
    public static event Action SetupCombatEvent;
    public void SetupCombat()
    {
        SetupCombatEvent();
    }

    public static event Action StartCombatEvent;
    public void StartCombat()
    {
        StartCombatEvent();
    }

    public static event SingleParameterDelegate<int> CountdownEvent;
    public void Countdown(int val)
    {
        CountdownEvent(val);
    }

    public static event Action EnemyTurnIndicatorEvent;
    public void EnemyTurnIndicator()
    {
        EnemyTurnIndicatorEvent();
    }

    public static event SingleParameterDelegate<int> SetControllerUIEvent;
    public void SetControllerUI(int val)
    {
        SetControllerUIEvent(val);
    }

    public static event Action PauseCombatEvent;
    public void PauseCombat()
    {
        PauseCombatEvent();
    }

    public static event Action ResumeCombatEvent;
    public void ResumeCombat()
    {
        ResumeCombatEvent();
    }

    public static event SingleParameterDelegate<string> DialogueEvent;
    public void Dialogue(string id)
    {
        DialogueEvent(id);
    }

    public static event Action TutorialNextPhaseEvent;
    public void TutorialNextPhase()
    {
        TutorialNextPhaseEvent();
    }


    public static event TwoParameterDelegate <AudioClip, int> LoadSongEvent;
    public void LoadSong(AudioClip song, int bpm)
    {
        LoadSongEvent(song, bpm);
    }

    public static event Action NewSongEvent; //TODO: remove if timer deleted
    public void NewSong()
    {
        NewSongEvent();
    }

    public static event Action DampenMusicEvent;
    public void DampenMusic()
    {
        DampenMusicEvent();
    }

    public static event Action PauseGameEvent;
    public void PauseGame()
    {
        PauseGameEvent();
    }

    public static event Action EndGameStateEvent;
    public void EndGameState()
    {
        EndGameStateEvent();
    }

    // ############
    // # Tutorial #
    // ############
    public static event Action TutorialPlayerTurnEvent;
    public void TutorialPlayerTurn()
    {
        TutorialPlayerTurnEvent();
    }

    // ############
    // # Tutorial #
    // ############
    public static event Action InstrumentSwitchEvent;
    public void InstrumentSwitch()
    {
        InstrumentSwitchEvent();
    }

    // ##################
    // # RHYTHM GAME UI #
    // ##################
    public static event SingleParameterDelegate<int> SetUIEnemyHealthValueEvent;
    public void SetUIEnemyHealthValue(int val)
    {
        SetUIEnemyHealthValueEvent(val);
    }

    public static event SingleParameterDelegate<int> SetUIPowerValueEvent;
    public void SetUIPowerValue(int val)
    {
        SetUIPowerValueEvent(val);
    }

    public static event SingleParameterDelegate<int> SetPowerChargeValueEvent;
    public void SetPowerChargeValue(int val)
    {
        SetPowerChargeValueEvent(val);
    }
    
    public static event TwoParameterDelegate<float, int> UpdateStarMeterEvent;
    public void UpdateStarMeter(float val1, int val2)
    {
        UpdateStarMeterEvent(val1, val2);
    }
    
    public static event SingleParameterDelegate<float> SetSpecialEvent;
    public void SetSpecial(float val)
    {
        SetSpecialEvent(val);
    }

    public static event Action SwitchAbilitiesEvent;
    public void SwitchAbilities()
    {
        SwitchAbilitiesEvent();
    }

    public static event SingleParameterDelegate<float> UpdateTimerEvent;
    public void UpdateTimer(float val)
    {
        UpdateTimerEvent(val);
    }

    public static event Action BlockEvent;
    public void Block()
    {
        BlockEvent();
    }

    // ###############
    // # RHYTHM GAME #
    // ###############
    public static event Action RhythmGameStartEvent;
    public void RhythmGameStart()
    {
        RhythmGameStartEvent();
    }

    public static event Action RhythmGameStopEvent;
    public void RhythmGameStop()
    {
        RhythmGameStopEvent();
    }

    public static event Action BeatEvent;
    public void Beat()
    {
        BeatEvent();
    }

    public static event Action BeaterHitEvent;
    public void BeaterHit()
    {
        BeaterHitEvent();
    }

    public static event Action BeaterMissEvent;
    public void BeaterMiss()
    {
        BeaterMissEvent();
    }

    public static event Action NoteHitEvent;
    public void NoteHit()
    {
        NoteHitEvent();
    }

    public static event Action NoteMissEvent;
    public void NoteMiss()
    {
        NoteMissEvent();
    }

    public static event Action NoteUpdateEvent;
    public void NoteUpdate()
    {
        NoteUpdateEvent();
    }

    // #########
    // # Death #
    // #########
    public static event SingleParameterDelegate<Character> DeathEvent;
    public void Death(Character character)
    {
        DeathEvent(character);
    }

    public static event SingleParameterDelegate<bool> EndGameEvent;
    public void EndGame(bool victory)
    {
        EndGameEvent(victory);
    }

    // #########
    // # Enemy #
    // #########
    public static event SingleParameterDelegate<CharacterEnemy[]> EnemyAppearEvent;
    public void EnemyAppear(CharacterEnemy[] enemies)
    {
        EnemyAppearEvent(enemies);
    }

    // #############
    // # Targeting #
    // #############
    public static event Action TargetEnemyToggleEvent;
    public void TargetEnemyToggle()
    {
        TargetEnemyToggleEvent();
    }

    public static event SingleParameterDelegate<bool> AbilityIndicatorStatusEvent;
    public void AbilityIndicatorStatus(bool status)
    {
        AbilityIndicatorStatusEvent(status);
    }

    // ###############
    // # ABILITY USE #
    // ###############
    public static event Action PlayerTurnEvent;
    public void PlayerTurn()
    {
        PlayerTurnEvent();
    }

    public static event Action EnemyTurnEvent;
    public void EnemyTurn()
    {
        EnemyTurnEvent();
    }

    public static event Action EnemyTurnIncomingEvent;
    public void EnemyTurnIncoming()
    {
        EnemyTurnIncomingEvent();
    }

    public static event ThreeParameterDelegate<Ability, Character, Character> AbilityUseEvent;
    public void AbilityUse(Ability ability, Character user, Character target)
    {
        AbilityUseEvent(ability, user, target);
    }

    public static event SingleParameterDelegate<string> AbilityUsedEvent;
    public void AbilityUsed(string ability)
    {
        AbilityUsedEvent(ability);
    }

    public static event SingleParameterDelegate<string> PlayerAbilityMissEvent;
    public void PlayerAbilityMiss(string ability)
    {
        PlayerAbilityMissEvent(ability);
    }

    public static event Action AbilityUseTextEndEvent;
    public void AbilityUsedTextEnd()
    {
        AbilityUseTextEndEvent();
    }

    public static event Action EnemyAttackEndEvent;
    public void EnemyAttackEnd()
    {
        EnemyAttackEndEvent();
    }

    public static event SingleParameterDelegate<bool> SetFriendlyAsSoloTargetEvent;
    public void SetFriendlyAsSoloTarget(bool status)
    {
        SetFriendlyAsSoloTargetEvent(status);
    }

    public static event SingleParameterDelegate<bool> SetEnemyAsSoloTargetEvent;
    public void SetEnemyAsSoloTarget(bool status)
    {
        if (CombatController.Instance.combat)
            SetEnemyAsSoloTargetEvent(status);
    }

    public static event SingleParameterDelegate<string> SetEnemyNameEvent;
    public void SetEnemyName(string val)
    {
        SetEnemyNameEvent(val);
    }

    public static event SingleParameterDelegate<string> SetEnemyAbilityNameEvent;
    public void SetEnemyAbilityName(string val)
    {
        SetEnemyAbilityNameEvent(val);
    }

    public static event SingleParameterDelegate<string> SetFriendlyNameEvent;
    public void SetFriendlyName(string val)
    {
        SetFriendlyNameEvent(val);
    }

    public static event SingleParameterDelegate<string> SetFriendlyAbilityNameEvent;
    public void SetFriendlyAbilityName(string val)
    {
        SetFriendlyAbilityNameEvent(val);
    }

    public static event Action CloseFriendlyNameEvent;
    public void CloseFriendlyName()
    {
        CloseFriendlyNameEvent();
    }

    public static event SingleParameterDelegate<string> SummonEvent;
    public void Summon(string val)
    {
        SummonEvent(val);
    }

    // #########
    // # Death #
    // #########
    public static event SingleParameterDelegate<int> SpawnNextEnemySetEvent;
    public void SpawnNextEnemySet(int val)
    {
        SpawnNextEnemySetEvent(val);
    }

    // ##############
    // # End Combat #
    // ##############
    public static event SingleParameterDelegate<Sprite[]> GetInputIconEvent;
    public void GetInputIcon(Sprite[] val)
    {
        GetInputIconEvent(val);
    }
    
    // ##############
    // # End Combat #
    // ##############

    public static event SingleParameterDelegate<int> SetEndComboEvent;
    public void SetEndCombo(int val)
    {
        SetEndComboEvent(val);
    }

    public static event SingleParameterDelegate<int> SetEndScoreEvent;
    public void SetEndScore(int val)
    {
        SetEndScoreEvent(val);
    }

    // ##############
    // # End Combat #
    // ##############

    public static event SingleParameterDelegate<int> UpdateChainEvent;
    public void UpdateChain(int val)
    {
        UpdateChainEvent(val);
    }

    public static event SingleParameterDelegate<int> UpdateScoreEvent;
    public void UpdateScore(int val)
    {
        UpdateScoreEvent(val);
    }
}