using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using General.ControllerInput;
using UnityEngine.SceneManagement;

public class TutorialRhythmGameController : MonoBehaviour
{

    // Allows remote access for unique instance
    public static TutorialRhythmGameController Instance
    {
        get
        {
            return instance;
        }
    }
    private static TutorialRhythmGameController instance;

    // Stats
    public int power, powerMax;
    public int favor;
    public int comboMultiplier, comboLevel, comboLevelMax, comboLevelThreshold;
    public int enemyHealth;
    public bool timerActive;

    // Turns
    public bool combat;
    private int countdown;

    public float timerDuration;
    public float timerBeats;
    private float turnTime;
    private bool turnEnding;
    public bool playerTurn;
    public int enemyDeathCounter, friendlyDeathCounter;

    // Character Selection
    public Character currentCharacter;
    public CharacterEnemy attacker;

    // Targeting
    public List<CharacterFriendly> friendlyCharacters = new List<CharacterFriendly>();
    public List<CharacterEnemy> enemyCharacters = new List<CharacterEnemy>();
    private int currentTargetNr;

    IEnumerator deselectTimer = null;

    public bool targetingEnemy;
    private int currentEnemyNr;
    private List<int> targetingDirections = new List<int>();

    public Ability currentAbility;

    public Conductor conductor;

    // Test
    public bool started;
    int beatCounter;
    public bool tutorial;

    // Controller
    public ControllerInput controllerInput;

    // Audio
    private AudioSource audioSource;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        Cursor.visible = false;
        controllerInput = new ControllerInput();
        controllerInput.SetupControls();

        audioSource = GetComponent<AudioSource>();

        comboLevelMax = 4;
        comboLevelThreshold = 10;
        playerTurn = true;

    }

    private void OnEnable()
    {
        EventManager.BeatEvent += TimerBeat;
    }

    private void OnDisable()
    {
        EventManager.BeatEvent -= TimerBeat;
    }
    // #########
    // # Input #
    // #########
    private void Update()
    {
        if (controllerInput.hasGamepad)
        {
            controllerInput.DpadInputCheck();
        }

        if (!combat)
        {
            return;
        }

        if (conductor.beatPressWindow && !conductor.rhythmGameActive)
        {

            // Jam
            if (Input.GetKeyDown(controllerInput.buttonDown))
            {
                EventManager.Instance.BeaterHit();
                AdjustFavorBar(25);
                EventManager.Instance.AbilityUsed("Jam");

                    if (Tutorial.Instance.currentPhase == 2)
                    {
                        Tutorial.Instance.NextPhase();
                    }
            }

            // Ability0
            if ((Input.GetKeyDown(controllerInput.buttonLeft)) && CombatController.Instance.power >= 1 && !targetingEnemy && CombatController.Instance.playerTurn && Tutorial.Instance.currentPhase == 6)
            {
                CheckAbilityUse(0);
                controllerInput.left = false;

                Tutorial.Instance.NextPhase();

            }
            // # Ability1 #
            else if ((Input.GetKeyDown(controllerInput.buttonUp)) && CombatController.Instance.power >= 1 && !targetingEnemy && CombatController.Instance.playerTurn && Tutorial.Instance.currentPhase == 18)
            {
                CheckAbilityUse(1);
                controllerInput.up = false;
                Tutorial.Instance.NextPhase();

            }
            // # Ability2 #
            else if ((Input.GetKeyDown(controllerInput.buttonRight)) && CombatController.Instance.power >= 1 && !targetingEnemy && CombatController.Instance.playerTurn && Tutorial.Instance.currentPhase == 12)
            {
                CheckAbilityUse(2);
                controllerInput.right = false;
                Tutorial.Instance.NextPhase();
            }
            // # Switch #
            else if ((Input.GetKeyDown(controllerInput.buttonLeftTrigger)) && !targetingEnemy && playerTurn && !tutorial)
            {
                EventManager.Instance.SwitchAbilities();
                EventManager.Instance.BeaterHit();
                EventManager.Instance.AbilityUsed("Switch");
            }

            else if ((Input.GetKeyDown(controllerInput.buttonLeft)) && CombatController.Instance.power < 1)
            {
                controllerInput.left = false;
                AbilityMiss("Ability1");
            }
            else if ((Input.GetKeyDown(controllerInput.buttonUp)) && CombatController.Instance.power < 1)
            {
                controllerInput.up = false;
                AbilityMiss("Ability2");
            }
            else if ((Input.GetKeyDown(controllerInput.buttonRight)) && CombatController.Instance.power < 1)
            {
                controllerInput.right = false;
                AbilityMiss("Ability3");
            }

            // Selecting Targets
            if (targetingEnemy && (Input.GetKeyDown(controllerInput.dpadLeft) || controllerInput.left)  && Tutorial.Instance.currentPhase == 7)
            {
                controllerInput.up = false;
                ConfirmEnemyTarget(0);
  
                EventManager.Instance.BeaterHit();
                Tutorial.Instance.NextPhase();
            }/*
            else if (false && targetingEnemy && (Input.GetKeyDown(controllerInput.dpadRight) || controllerInput.right) && Tutorial.Instance.currentPhase == 7)
            {
                controllerInput.up = false;
                ConfirmEnemyTarget(1);
                EventManager.Instance.BeaterHit();
                Tutorial.Instance.NextPhase();
            }
            else if (false && targetingEnemy && (Input.GetKeyDown(controllerInput.dpadUp) || controllerInput.up) && Tutorial.Instance.currentPhase == 7)
            {
                controllerInput.up = false;
                ConfirmEnemyTarget(2);
                EventManager.Instance.BeaterHit();
                Tutorial.Instance.NextPhase();

            }
            else if (false && targetingEnemy && (Input.GetKeyDown(controllerInput.dpadDown) || controllerInput.down) && Tutorial.Instance.currentPhase == 7)
            {
                controllerInput.up = false;
                ConfirmEnemyTarget(3);
                EventManager.Instance.BeaterHit();
                Tutorial.Instance.NextPhase();
            }*/
        }

        // Jam (rhythmgame)
        else if (Input.GetKeyDown(controllerInput.buttonDown) && conductor.rhythmGameActive && combat && conductor.beatPressWindow && !targetingEnemy)
        {
            EventManager.Instance.BeaterHit();
            AdjustFavorBar(25);
            EventManager.Instance.AbilityUsed("Jam");
        }

        // Miss beat
        else if ((!conductor.beatPressWindow && !conductor.rhythmGameActive))
        {
            if (Input.GetKeyDown(controllerInput.buttonDown))
            {
                AbilityMiss("Jam");
            }
            else if ((Input.GetKeyDown(controllerInput.buttonLeft)))
            {
                AbilityMiss("Ability1");
            }
            else if ((Input.GetKeyDown(controllerInput.buttonUp)))
            {
                AbilityMiss("Ability2");
            }
            else if ((Input.GetKeyDown(controllerInput.buttonRight)))
            {
                AbilityMiss("Ability3");

            }
            else if (Input.GetKeyDown(controllerInput.buttonLeftTrigger) && !tutorial)
            {
                AbilityMiss("Switch");
            }
        }
    }

    // ##########
    // # Combat #
    // ##########
    private IEnumerator Intro()
    {
        EventManager.Instance.SetupCombat();
        SetupCharacterList();

        yield return new WaitForSeconds(1.5f);

        //SongManager.Instance.PlayRandomSong();
        SongManager.Instance.PlaySong("Necro"); // Sloan, Kalax, Morse, Necro        

        EventManager.Instance.Dialogue("rooftop_intro");
    }

    private IEnumerator Combat()
    {
        yield return null;
        combat = true;
        playerTurn = true;
        StartTimer();
    }

    private void PauseCombat()
    {
        combat = false;
    }

    private void ResumeCombat() // Starts Combat countdown, or resumes it if it was paused
    {
        if (!tutorial && !started)
        {
            started = true;
            EventManager.BeatEvent += Countdown;
        }
        else
        {
            combat = true;
            ContinueTimer();
        }
    }

    private void Countdown()
    {
        beatCounter++;
        EventManager.Instance.Countdown(countdown);
        if (countdown < 0)
        {
            EventManager.BeatEvent -= Countdown;
            StartCoroutine(Combat());
            return;
        }
        countdown--;

        //if (beatCounter % 2 == 0)
    }

    public void StartTimer()
    {
        ResetTimer();
        ContinueTimer();
    }

    private void ResetTimer()
    {
        timerBeats = 0;
        EventManager.Instance.UpdateTimer(timerBeats);
        turnEnding = false;
    }

    private void ContinueTimer()
    {
        if (combat)
            timerActive = true;
    }

    public void PauseTimer()
    {
        timerActive = false;
    }

    private void SwitchTurn()
    {

        if (!combat)
        {
            return;
        }

        foreach (Character character in friendlyCharacters)
        {
            character.SetAsSoloUser(false);
            character.SetAsSoloTarget(false);
            character.CharacterDeselect();
        }

        foreach (Character character in enemyCharacters)
        {
            character.SetAsSoloUser(false);
            character.SetAsSoloTarget(false);
            character.CharacterDeselect();
        }

        StopCoroutine("StartTurn");
        StartCoroutine("StartTurn");
    }

    private IEnumerator StartTurn()
    {

        // Enemy Turn
        if (playerTurn)
        {
            EventManager.Instance.EnemyTurn();

            playerTurn = false;

            CameraMove.Instance.EnemyTurn();

            yield return new WaitForSeconds(1f);

            EnemyTurn();
        }

        // Player Turn
        else
        {
            CameraMove.Instance.ReturnCamera();

            EventManager.Instance.PlayerTurn();
            StartTimer(); // starts enemy turn countdown

            playerTurn = true;
        }
    }

    private void TimerBeat()
    {
        if (timerActive && playerTurn)
        {
            timerBeats++;
            EventManager.Instance.UpdateTimer(timerBeats);

            if (!turnEnding && (timerBeats > timerDuration * 0.75f))
            {
                EventManager.Instance.EnemyTurnIncoming();
                turnEnding = true;
            }

            if (timerBeats >= timerDuration)
            {
                timerBeats = 0;
                turnEnding = false;
                SwitchTurn();
            }
        }
    }

    // ##############
    // # Enemy Turn #
    // ##############
    private void EnemyTurn()
    {
        if (targetingEnemy) // reset targeting enemy
        {
            targetingEnemy = false;
            if (currentCharacter != null)
            {
                currentCharacter.ResetAttackMode();
                currentCharacter.CharacterDeselect();
                currentCharacter = null;
            }
            EventManager.Instance.TargetEnemyToggle();
        }

        // Select attacker
        attacker = NextEnemy();
        if (attacker == null)
        {
            CheckForGameEnding();
            return;
        }

        attacker.SetAttacking(true);
        attacker.CharacterSelect();

        EventManager.Instance.SetEnemyName(enemyCharacters[currentEnemyNr].characterName); // Set UI

        currentAbility = attacker.SelectAttack();

        if (currentAbility.hasAttackMode) // Determine whether an Attack Mode animation must play
        {
            attacker.PlayAbilityAnimation("AttackMode"); // play animation
        }
        else
        {
            attacker.PlayAbilityAnimation("Attack"); // play animation
        }

        switch (currentAbility.targets)
        {
            case "EnemyRandom": // Instant
                EnemyAbilityInstant();
                break;
            case "EnemyRhythm": // Rhythm
                EnemyRhythm();
                break;
            default:
                break;
        }
    }

    private CharacterEnemy NextEnemy()
    {
        CharacterEnemy nextEnemy = null;

        IncrementEnemyNr();

        for (int i = 0; i < enemyCharacters.Count - 1; i++)
        {
            if (!combat)
                break;

            if (enemyCharacters[currentEnemyNr].isAlive)
            {
                nextEnemy = enemyCharacters[currentEnemyNr];
                break;
            }
            else
            {
                IncrementEnemyNr();
            }
        }
        if (nextEnemy == null)
            nextEnemy = enemyCharacters[0];

        return nextEnemy;
    }

    private void IncrementEnemyNr()
    {
        currentEnemyNr = (currentEnemyNr + 1) > enemyCharacters.Count - 1 ? 0 : currentEnemyNr + 1;
    }

    private void EnemyRhythm()
    {
        EventManager.Instance.SetFriendlyAsSoloTarget(true); // Sets friendlies as target
        enemyCharacters[currentEnemyNr].SetAsSoloUser(true);

        EventManager.Instance.SetEnemyAbilityName(currentAbility.GetName()); // Set UI

        EventManager.Instance.RhythmGameStart();
        EventManager.RhythmGameStopEvent += EnemyRhythmEnd;
    }

    private void EnemyRhythmEnd()
    {
        SwitchTurn();

        enemyCharacters[currentEnemyNr].CharacterDeselect();

        EventManager.RhythmGameStopEvent -= EnemyRhythmEnd;
    }

    private void EnemyAbilityInstant()
    {
        StartCoroutine(EnemyAbilityInstantTime());
    }

    private IEnumerator EnemyAbilityInstantTime()
    {
        EventManager.Instance.SetEnemyAbilityName(currentAbility.GetName()); // Set UI

        yield return new WaitForSeconds(0.5f);

        // Attacker
        attacker.AttackAnimation(currentAbility);

        // Select target
        CharacterFriendly target = SelectRandomTarget();
        if (tutorial) // TODO: cleanup
        {
            target = friendlyCharacters[1];
        }

        audioSource.PlayOneShot(currentAbility.sound);
        currentAbility.effect(target, attacker);

       // EventManager.Instance.AbilityUsed(attacker.abilities[0].GetName());
        SpawnVisualEffect(target);

        yield return new WaitForSeconds(1f);

        enemyCharacters[currentEnemyNr].CharacterDeselect(); // Deselect character

        SwitchTurn(); // Enemy Turn end
    }

    private CharacterFriendly SelectRandomTarget()
    {
        CharacterFriendly target = null;
        int rand;

        while (target == null)
        {
            rand = Random.Range(0, friendlyCharacters.Count);
            if (friendlyCharacters[rand].isAlive)
                target = friendlyCharacters[rand];
        }
        return target;
    }

    public void EnemyAppear(CharacterEnemy[] enemies)
    {
        CharacterEnemy first = null;
        foreach (CharacterEnemy enemy in enemies)
        {
            enemyCharacters.Add(enemy);
            enemy.gameObject.SetActive(true);
            if (first == null) // Play first enemy's voice clip
            {
                first = enemy;
                //enemy.audioSource.Play();
            }
        }

        SetupCharacterList();
    }

    // ###############
    // # Ability Use #
    // ###############
    private void AbilityMiss(string ability)
    {
        EventManager.Instance.PlayerAbilityMiss(ability);
        EventManager.Instance.BeaterMiss();
        AdjustFavorBar(-25);
    }

    // Checks the ability's target and performs its effects if applicable
    private void CheckAbilityUse(int charNr)
    {
        EventManager.Instance.BeaterHit();
        EventManager.Instance.AbilityUsed("Ability" + (charNr + 1)); // UI feedback

        currentAbility = PartyController.Instance.abilities[charNr]; // select ability
        currentCharacter = friendlyCharacters[charNr];
        currentCharacter.CharacterSelect();

        if (currentAbility.hasAttackMode) // Determine whether an Attack Mode animation must play
        {
            friendlyCharacters[charNr].PlayAbilityAnimation("AttackMode"); // play animation
        }
        else
        {
            friendlyCharacters[charNr].PlayAbilityAnimation("Attack"); // play animation
        }

        EventManager.Instance.SetFriendlyAbilityName(currentAbility.GetName());
        EventManager.Instance.SetFriendlyName(currentCharacter.characterName);

        if (tutorial && charNr == 1)
        {
            currentAbility = PartyController.Instance.tutorialAbilities[0];
            CombatController.Instance.currentAbility = PartyController.Instance.tutorialAbilities[0];
        }

        switch (currentAbility.targets)
        {
            case "EnemyTarget":
                EnemyTargetMode();
                break;

            case "EnemyRhythm":

                CameraMove.Instance.PartyBottomView();

                EventManager.Instance.RhythmGameStart();
                EventManager.Instance.SetEnemyAsSoloTarget(true); // Sets enemy as target
                friendlyCharacters[charNr].SetAsSoloUser(true);
                AdjustPower(-1);
                break;

            case "FriendlyRhythm":

                CameraMove.Instance.PartyBottomView();

                EventManager.Instance.RhythmGameStart();
                EventManager.Instance.SetFriendlyAsSoloTarget(true); // Sets friendly as target
                friendlyCharacters[charNr].SetAsSoloUser(true);
                AdjustPower(-1);
                break;

            case "FriendlyAll":
                audioSource.PlayOneShot(currentAbility.sound);

                CameraMove.Instance.PartyFocus();
                //CameraMove.Instance.PartyZoom();

                currentAbility.effect(friendlyCharacters[0], currentCharacter);
                foreach (CharacterFriendly friendly in friendlyCharacters)
                {
                    if (friendly.isAlive)
                        SpawnVisualEffect(friendly);
                }
                AbilityUseReset();
                break;

            case "EnemyAll":
                audioSource.PlayOneShot(currentAbility.sound);

                currentAbility.effect(enemyCharacters[0], currentCharacter);
                foreach (CharacterEnemy enemy in enemyCharacters)
                {
                    if (enemy.isAlive)
                        SpawnVisualEffect(enemy);
                }
                AbilityUseReset();
                break;

            case "Self":
                audioSource.PlayOneShot(currentAbility.sound);

                //CameraMove.Instance.PartyZoom();
                //CameraMove.Instance.PartyFocus();
                currentAbility.effect(currentCharacter, currentCharacter);
                SpawnVisualEffect(currentCharacter);
                AbilityUseReset();
                break;
        }
    }

    private void AbilityUseReset()
    {
        AdjustPower(-1);
        if (deselectTimer != null)
            StopCoroutine(deselectTimer);
        deselectTimer = DeselectTimer((CharacterFriendly)currentCharacter);
        StartCoroutine(deselectTimer);

    }

    private IEnumerator DeselectTimer(CharacterFriendly character)
    {
        yield return new WaitForSeconds(0.2f);
        currentCharacter = null;
        character.CharacterDeselect();
        EventManager.Instance.CloseFriendlyName();
        CameraMove.Instance.ReturnCamera();

    }

    // Start Enemy targeting
    private void EnemyTargetMode()
    {
        CameraMove.Instance.EnemyTargetZoom();
        EventManager.Instance.AbilityIndicatorStatus(false);
        EventManager.Instance.TargetEnemyToggle();
        targetingEnemy = true;
    }

    private void ConfirmEnemyTarget(int target)
    {
        currentCharacter.ResetAttackMode();
        AbilityUseReset();

        // Deactivate enemy targeting objects
        EventManager.Instance.AbilityIndicatorStatus(true);
        EventManager.Instance.TargetEnemyToggle();

        CharacterEnemy tar = LookupEnemyByTargetDirection(target);
        if (tar == null)
        {
            print("No Enemy Target Found");
            return;
        }

        // Spawn damage effect of ability
        SpawnVisualEffect(tar);
        // Confirm and use attack on target
        audioSource.PlayOneShot(currentAbility.sound);
        currentAbility.effect(tar, currentCharacter);

        targetingEnemy = false;
    }

    // #######################
    // # Character Selection #
    // #######################
    private void SetupCharacterList()
    {
        // FRIENDLY Characters
        GameObject[] characterArray = GameObject.FindGameObjectsWithTag("PlayerCharacter");
        friendlyCharacters.Clear();
        foreach (GameObject character in characterArray)
        {
            friendlyCharacters.Add(character.GetComponent<CharacterFriendly>());
        }
        // Sort friendly list by name
        friendlyCharacters = friendlyCharacters.OrderBy(a => a.name).ToList();

        // ENEMY Characters
        characterArray = GameObject.FindGameObjectsWithTag("NPC");
        enemyCharacters.Clear();
        foreach (GameObject character in characterArray)
        {
            if (character.activeInHierarchy)
                enemyCharacters.Add(character.GetComponent<CharacterEnemy>());
        }
        // Sort enemy list by name
        enemyCharacters = enemyCharacters.OrderBy(a => a.name).ToList();

        AssignEnemyTargetDirections();
    }

    private void AssignEnemyTargetDirections()
    {
        int direction = 0;
        targetingDirections.Clear();

        foreach (CharacterEnemy enemy in enemyCharacters)
        {
            enemy.SetTargetingDirection(direction);
            targetingDirections.Add(enemy.targetingDirection);
            //direction++;
        }
    }

    private bool IsTargetAlive(int dir)
    {
        bool tempBool = false;
        foreach (CharacterEnemy enemy in enemyCharacters)
        {
            if (enemy.targetingDirection == dir)
            {
                if (enemy.isAlive)
                {
                    tempBool = true;
                    break;
                }
            }
        }
        return tempBool;
    }


    private CharacterEnemy LookupEnemyByTargetDirection(int dir)
    {
        CharacterEnemy tempEnemy = null;
        foreach (CharacterEnemy enemy in enemyCharacters)
        {
            if (enemy.targetingDirection == dir)
            {

                tempEnemy = enemy;
                break;
            }
        }
        return tempEnemy;
    }



    // ################
    // # Stat Changes #
    // ################
    private void AdjustPower(int change)
    {
        CombatController.Instance.power += change;
        
        if (CombatController.Instance.power >= CombatController.Instance.powerMax)
        {
            CombatController.Instance.power = CombatController.Instance.powerMax;
        }

        if (CombatController.Instance.power < 0)
        {
            CombatController.Instance.power = 0;
        }
        EventManager.Instance.SetUIPowerValue(CombatController.Instance.power);
    }

    private void AdjustFavorBar(int change)
    {
        if (CombatController.Instance.power >= CombatController.Instance.powerMax)
        {
            EventManager.Instance.SetPowerChargeValue(CombatController.Instance.powerCharge);
            return;
        }

        CombatController.Instance.powerCharge += change;

        if (CombatController.Instance.powerCharge >= 100)
        {
            EventManager.Instance.SetPowerChargeValue(CombatController.Instance.powerCharge);
            if (CombatController.Instance.power >= CombatController.Instance.powerMax)
            {
                CombatController.Instance.powerCharge = 100;
                EventManager.Instance.SetPowerChargeValue(CombatController.Instance.powerCharge);
            }
            else
            {
                CombatController.Instance.powerCharge = 0;
                AdjustPower(1);
            }
        }
        else if (CombatController.Instance.powerCharge < 0)
        {
            CombatController.Instance.powerCharge = 0;
            EventManager.Instance.SetPowerChargeValue(CombatController.Instance.powerCharge);
        }
        else
        {
            EventManager.Instance.SetPowerChargeValue(CombatController.Instance.powerCharge);
        }
    }
    /*
    private void ComboMultiplierIncrease()
    {
        comboMultiplier += 1;
        if (comboMultiplier % comboLevelThreshold == 0)
        {
            comboLevel++;
            comboLevel = comboLevel > comboLevelMax ? comboLevelMax : comboLevel;
        }
        EventManager.Instance.SetScore(comboMultiplier, comboLevel);
    }

    private void ComboMultiplierReset()
    {
        comboMultiplier = 0;
        comboLevel--;
        comboLevel = comboLevel < 0 ? 0 : comboLevel;
        EventManager.Instance.SetScore(comboMultiplier, comboLevel);
    }*/

    // #########
    // # Death #
    // #########
    private void CharacterDeath(Character character)
    {
        if (character.isFriendlyCharacter) // if friendly character
        {
            friendlyDeathCounter++;
            //friendlyCharacters.Remove((CharacterFriendly)character);
        }
        else // if enemy character
        {
            enemyDeathCounter++;
            //enemyCharacters.Remove((CharacterEnemy)character);
        }
        CheckForGameEnding();
    }

    private void CheckForGameEnding()
    {
        if (friendlyCharacters.Count <= friendlyDeathCounter) // if all friendlies are defeated, lose game
        {
            combat = false;
            EventManager.Instance.RhythmGameStop();
            PauseTimer();

            EventManager.Instance.EndGame(false);
        }
        else if (enemyCharacters.Count <= enemyDeathCounter) // if all enemies are defeated, win game
        {
            combat = false;
            EventManager.Instance.RhythmGameStop();
            PauseTimer();

            EventManager.Instance.EndGame(true);
        }
    }

    private void SpawnVisualEffect(Character obj)
    {
        Instantiate(currentAbility.visualEffect, obj.effectPos.position, Quaternion.identity);
    }


}