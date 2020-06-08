using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using General.ControllerInput;

public class CombatController : MonoBehaviour {
    
    // Allows remote access for unique instance
    public static CombatController Instance
    {
        get
        {
            return instance;
        }
    }
    private static CombatController instance;

    [Header("Stats")]
    public int power, powerMax;
    public int powerCharge;
    public int enemyHealth;
    private bool timerActive;

    private int specialCurrent = 0;
    private readonly int specialMax = 6;

    [Header("Turns")]
    public bool gameEnded;
    public bool combat;
    private int countdown;
    public float timerDuration;
    public float timerBeats;
    private float turnTime;
    private bool turnEnding;
    public bool playerTurn;
    public int enemyDeathCounter, friendlyDeathCounter;

    [Header("Character Selection")]
    public Character currentCharacter;
    public CharacterEnemy attacker;

    [Header("Targeting")]
    public List<CharacterFriendly> friendlyCharacters = new List<CharacterFriendly>();
    public List<CharacterEnemy> enemyCharacters = new List<CharacterEnemy>();
    private int currentTargetNr;
    IEnumerator deselectTimer = null;
    public bool targetingEnemy;
    private int currentEnemyNr;
    private List<int> targetingDirections = new List<int>();

    [Header("Current Ability")]
    public Ability currentAbility;

    [Header("Misc & Test")]
    public bool started;
    int beatCounter;
    public bool tutorial;
    public bool paused;
    public bool beat;

    [Header("Assigned")]
    public ControllerInput controllerInput;
    public Conductor conductor;

    // Audio
    private AudioSource audioSource;

    private void OnEnable()
    {
        EventManager.PauseCombatEvent += PauseTimer;
        EventManager.PauseCombatEvent += PauseCombat;
        EventManager.ResumeCombatEvent += ResumeCombat;

        EventManager.RhythmGameStartEvent += PauseTimer;
        EventManager.RhythmGameStopEvent += ContinueTimer;

        EventManager.DeathEvent += CharacterDeath;

        EventManager.EnemyAppearEvent += EnemyAppear;

        EventManager.BeatEvent += TimerBeat;

        EventManager.PauseGameEvent += PauseGame;
        EventManager.EndGameEvent += EndGame;
    }

    private void OnDisable()
    {
        EventManager.PauseCombatEvent -= PauseTimer;
        EventManager.PauseCombatEvent -= PauseCombat;
        EventManager.ResumeCombatEvent -= ResumeCombat;

        EventManager.RhythmGameStartEvent -= PauseTimer;
        EventManager.RhythmGameStopEvent -= ContinueTimer;

        EventManager.DeathEvent -= CharacterDeath;

        EventManager.EnemyAppearEvent -= EnemyAppear;

        EventManager.BeatEvent -= TimerBeat;

        EventManager.PauseGameEvent -= PauseGame;
        EventManager.EndGameEvent -= EndGame;
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

        Time.timeScale = 1.0f;
    }

    private void Start () {
        Cursor.visible = false;
        controllerInput = new ControllerInput();
        controllerInput.SetupControls();

        audioSource = GetComponent<AudioSource>();

        countdown = 3;

        power = 0;
        powerMax = 3;

        enemyHealth = 0;
        powerCharge = 0;
        playerTurn = true;

        timerDuration = 9f;

        AdjustPower(0);
        AdjustFavorBar(0);
        AddSpecial(0);
        //AddSpecial(0);

        if (!tutorial)
        {
            StartCoroutine(Intro());
        }
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

        if (tutorial)
        {
            return;
        }

        if (!combat || paused)
        {
            return;
        }

        if (conductor.beatPressWindow && !conductor.abilityUsed && !conductor.rhythmGameActive)
        {
            // Jam
            if (Input.GetKeyDown(controllerInput.buttonDown))
            {
                EventManager.Instance.BeaterHit();
                AdjustFavorBar(20);
                EventManager.Instance.AbilityUsed("Jam");
               // Instantiate(beatPulse);
            }

            // Ability0
            if ((Input.GetKeyDown(controllerInput.buttonLeft)) && power >= 1 && !targetingEnemy && playerTurn)
            {
                CheckAbilityUse(0);
                controllerInput.left = false;
            }
            // # Ability1 #
            else if ((Input.GetKeyDown(controllerInput.buttonUp)) && power >= 1 && !targetingEnemy && playerTurn)
            {
                CheckAbilityUse(1);
                controllerInput.up = false;
            }
            // # Ability2 #
            else if ((Input.GetKeyDown(controllerInput.buttonRight)) && power >= 1 && !targetingEnemy && playerTurn)
            {
                CheckAbilityUse(2);
                controllerInput.right = false;
            }
            
            // # Switch #
            else if ((Input.GetKeyDown(controllerInput.buttonLeftTrigger)) && !targetingEnemy && playerTurn)
            {
                EventManager.Instance.SwitchAbilities();
                EventManager.Instance.BeaterHit();
                EventManager.Instance.AbilityUsed("Switch");
                
            }

            // Special
            else if ((Input.GetKeyDown(controllerInput.buttonRightTrigger)) && (specialCurrent == specialMax) && !targetingEnemy && playerTurn)
            {
                CheckAbilityUse(3);
            }

            else if ((Input.GetKeyDown(controllerInput.buttonLeft)) && power < 1)
            {
                controllerInput.left = false;
                AbilityMiss("Ability1");
            }
            else if ((Input.GetKeyDown(controllerInput.buttonUp)) && power < 1)
            {
                controllerInput.up = false;
                AbilityMiss("Ability2");
            }
            else if ((Input.GetKeyDown(controllerInput.buttonRight)) && power < 1)
            {
                controllerInput.right = false;
                AbilityMiss("Ability3");
            }

            // Selecting Targets
            if (targetingEnemy && (Input.GetKeyDown(controllerInput.dpadLeft) || controllerInput.left) && targetingDirections.Contains(0) && IsTargetAlive(0))
            {
                controllerInput.left = false;
                ConfirmEnemyTarget(0);
                EventManager.Instance.BeaterHit();
            }
            else if (targetingEnemy && (Input.GetKeyDown(controllerInput.dpadRight) || controllerInput.right) && targetingDirections.Contains(1) && IsTargetAlive(1))
            {
                controllerInput.right = false;
                ConfirmEnemyTarget(1);
                EventManager.Instance.BeaterHit();
            }
            else if (targetingEnemy && (Input.GetKeyDown(controllerInput.dpadUp) || controllerInput.up) && targetingDirections.Contains(2) && IsTargetAlive(2))
            {
                controllerInput.up = false;
                ConfirmEnemyTarget(2);
                EventManager.Instance.BeaterHit();
            }
            else if (targetingEnemy && (Input.GetKeyDown(controllerInput.dpadDown) || controllerInput.down) && targetingDirections.Contains(3) && IsTargetAlive(3))
            {
                controllerInput.down = false;
                ConfirmEnemyTarget(3);
                EventManager.Instance.BeaterHit();
            }
        }

        // Jam (rhythmgame)
        else if (Input.GetKeyDown(controllerInput.buttonDown) && conductor.rhythmGameActive && combat && conductor.beatPressWindow && !targetingEnemy)
        {
            EventManager.Instance.BeaterHit();
            AdjustFavorBar(20);
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

            }/*
            else if (Input.GetKeyDown(controllerInput.buttonSwitch))
            {
                AbilityMiss("Switch");
            }*/
        }
    }

    // ##########
    // # Combat #
    // ##########
    private IEnumerator Intro()
    {
        yield return null;
        EventManager.Instance.SetupCombat();
        SetupCharacterList();

        yield return new WaitForSeconds(1.5f);

        switch (PlayerData.Instance.difficulty)
        {
            case 0:
                //SongManager.Instance.PlaySong("Kalax");
                //SongManager.Instance.PlaySong("Job1");
                SongManager.Instance.PlaySong("Frank1");
                break;
            case 1:
                //SongManager.Instance.PlaySong("Carp");
                //SongManager.Instance.PlaySong("Job1");
                SongManager.Instance.PlaySong("Frank1");
                break;
            case 2:
                //SongManager.Instance.PlaySong("Waves");
                //SongManager.Instance.PlaySong("Job1");
                SongManager.Instance.PlaySong("Frank2");
                break;
            case 3:
                //SongManager.Instance.PlaySong("Waves");
                //SongManager.Instance.PlaySong("Job1");
                SongManager.Instance.PlaySong("Frank3");
                break;
            default:
                //SongManager.Instance.PlaySong("Kalax");
                SongManager.Instance.PlaySong("Job1");


                break;
        }
        //SongManager.Instance.PlayRandomSong();
        //SongManager.Instance.PlaySong("Carp"); // Sloan, Kalax, Morse, Necro, Waves, Kill
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
        if (!started)
        {
            started = true;
            EventManager.BeatEvent += Countdown;
        } else
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
            playerTurn = false;
            CameraMove.Instance.EnemyTurn();
            EventManager.Instance.EnemyTurn();

            yield return new WaitForSeconds(1f);

            EnemyTurn();
        }

        // Player Turn
        else
        {
            playerTurn = true;
            CameraMove.Instance.ReturnCamera();
            EventManager.Instance.PlayerTurn();
            StartTimer(); // starts enemy turn countdown
        }
    }

    private void TimerBeat()
    {
        if (timerActive && playerTurn && !paused)
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
        //print(currentAbility.GetName());

        switch (currentAbility.targets)
        {
            case "EnemyRandom": // Instant
                EnemyAbilityRandom(false);
                break;
            case "EnemyRhythm": // Rhythm
                EnemyAbilityRhythm();
                break;
            case "FriendlyRandom":
                EnemyAbilityRandom(true);
                break;
            default:
                break;
        }
    }

    private CharacterEnemy NextEnemy()
    {
        CharacterEnemy nextEnemy = null;

        IncrementEnemyNr();

        for (int i = 0; i <= enemyCharacters.Count - 1; i++)
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
        {
            nextEnemy = enemyCharacters[0];
        }
        return nextEnemy;
    }

    private void IncrementEnemyNr()
    {
        currentEnemyNr = (currentEnemyNr + 1) > enemyCharacters.Count - 1 ? 0 : currentEnemyNr + 1;
    }

    private void EnemyAbilityRhythm()
    {
        EventManager.Instance.SetFriendlyAsSoloTarget(true); // Sets friendlies as target
        enemyCharacters[currentEnemyNr].SetAsSoloUser(true);

        EventManager.Instance.SetEnemyAbilityName(currentAbility.GetName()); // Set UI
        //attacker.AttackAnimation(currentAbility);
        EventManager.Instance.RhythmGameStart();
        EventManager.RhythmGameStopEvent += EnemyRhythmEnd;
    }

    private void EnemyRhythmEnd()
    {
        SwitchTurn();

        enemyCharacters[currentEnemyNr].CharacterDeselect();

        EventManager.RhythmGameStopEvent -= EnemyRhythmEnd;
    }

    private void EnemyAbilityRandom(bool friendly)
    {
        StartCoroutine(EnemyAbilityInstantTime(friendly));
    }


    private IEnumerator EnemyAbilityInstantTime(bool friendly)
    {
        EventManager.Instance.SetEnemyAbilityName(currentAbility.GetName()); // Set UI

        // Attacker
        attacker.AttackAnimation(currentAbility);

        yield return new WaitForSeconds(0.5f);

        // Select target
        Character target = SelectRandomTarget(friendly);

        if (currentAbility.sound != null && !audioSource.isPlaying) // Play sound, if possible
            audioSource.PlayOneShot(currentAbility.sound);

        currentAbility.effect(target, attacker);

        //EventManager.Instance.AbilityUsed(attacker.abilities[0].GetName());

        if (currentAbility.visualEffect != null) // Use visual effect, if possible
            SpawnVisualEffect(target);

        yield return new WaitForSeconds(1f);

        enemyCharacters[currentEnemyNr].CharacterDeselect(); // Deselect character

        SwitchTurn(); // Enemy Turn end
    }

    private Character SelectRandomTarget(bool friendly)
    {
        Character target = null;
        int rand;

        while (target == null)
        {
            if (friendly)
            {
                rand = Random.Range(0, enemyCharacters.Count);
                if (enemyCharacters[rand].isAlive)
                    target = enemyCharacters[rand];
            } else
            {
                rand = Random.Range(0, friendlyCharacters.Count);
                if (friendlyCharacters[rand].isAlive)
                    target = friendlyCharacters[rand];
            }

        }
        return target;
    }

    private void EnemyAppear(CharacterEnemy[] enemies)
    {
        foreach (CharacterEnemy enemy in enemies)
        {
            enemyCharacters.Add(enemy);
            enemy.gameObject.SetActive(true);
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
        AdjustFavorBar(-15);
    }

    bool special = false;

    // Checks the ability's target and performs its effects if applicable
    private void CheckAbilityUse(int charNr)
    {
        EventManager.Instance.BeaterHit();
        EventManager.Instance.AbilityUsed("Ability" + (charNr + 1)); // UI feedback

        currentAbility = PartyController.Instance.abilities[charNr]; // select ability
        EventManager.Instance.SetFriendlyAbilityName(currentAbility.GetName());


        if (charNr > 2) // Special Attack
        {
            special = true;

            charNr = 0;
            EventManager.Instance.SetFriendlyName("");
            EventManager.Instance.Summon(currentAbility.GetName());
            foreach (CharacterFriendly character in friendlyCharacters)
            {
                character.CharacterSelect();
                character.AttackAnimation(currentAbility);
            }
        }
        else // Normal Attack
        {
            special = false;
            currentCharacter = friendlyCharacters[charNr];
            EventManager.Instance.SetFriendlyName(currentCharacter.characterName);
            currentCharacter.CharacterSelect();
            currentCharacter.AttackAnimation(currentAbility);
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
                if (special)
                {
                    friendlyCharacters[charNr].SetAsSoloUser(true);// TODO: make multiple
                }
                else
                {
                    friendlyCharacters[charNr].SetAsSoloUser(true); 
                }

                EventManager.Instance.AbilityUse(currentAbility, enemyCharacters[0], currentCharacter);
                AbilityResourceChange(-1);
                break;

            case "FriendlyRhythm":
                CameraMove.Instance.PartyBottomView();
                EventManager.Instance.RhythmGameStart();
                EventManager.Instance.SetFriendlyAsSoloTarget(true); // Sets friendly as target
                if (special)
                {
                    friendlyCharacters[charNr].SetAsSoloUser(true);// TODO: make multiple
                }
                else
                {
                    friendlyCharacters[charNr].SetAsSoloUser(true);
                }
                EventManager.Instance.AbilityUse(currentAbility, friendlyCharacters[0], currentCharacter);
                AbilityResourceChange(-1);
                break;

            case "FriendlyAll":
                CameraMove.Instance.PartyFocus();
                //CameraMove.Instance.PartyZoom();
                audioSource.PlayOneShot(currentAbility.sound);
                currentAbility.effect(friendlyCharacters[0], currentCharacter);
                EventManager.Instance.AbilityUse(currentAbility, friendlyCharacters[0], currentCharacter);

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
                EventManager.Instance.AbilityUse(currentAbility, enemyCharacters[0], currentCharacter);

                foreach (CharacterEnemy enemy in enemyCharacters)
                {
                    if (enemy.isAlive)
                        SpawnVisualEffect(enemy);
                }
                AbilityUseReset();
                break;

            case "Self":
                audioSource.PlayOneShot(currentAbility.sound);
                currentAbility.effect(currentCharacter, currentCharacter);
                EventManager.Instance.AbilityUse(currentAbility, currentCharacter, currentCharacter);
                SpawnVisualEffect(currentCharacter);
                AbilityUseReset();
                break;
        }
    }

    private void AbilityUseReset()
    {
        AbilityResourceChange(-1);
        if (deselectTimer != null)
            StopCoroutine(deselectTimer);
        deselectTimer = DeselectTimer((CharacterFriendly)currentCharacter);
        StartCoroutine(deselectTimer);
    }

    private void AbilityResourceChange(int change)
    {
        if (special)
        {
            ResetSpecial();
        } else
        {
            AdjustPower(change);
            AddSpecial(1);
        }
    }

    private void ResetSpecial()
    {
        specialCurrent = 0;
        EventManager.Instance.SetSpecial(0);
    }

    private void AddSpecial(int change)
    {
        if (specialCurrent < specialMax)
        {
            specialCurrent += change;
        }
        EventManager.Instance.SetSpecial((float)specialCurrent / specialMax);
    }

    private IEnumerator DeselectTimer(CharacterFriendly character)
    {
        //currentCharacter = null;
        yield return new WaitForSeconds(0.5f);
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
        if (currentCharacter != null)
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
        EventManager.Instance.AbilityUse(currentAbility, tar, currentCharacter);

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
        //enemyCharacters = enemyCharacters.OrderBy(a => a.name).ToList();
        //enemyCharacters = enemyCharacters.OrderBy(a => a.pos).ToList();
        enemyCharacters = enemyCharacters.OrderByDescending(a => a.transform.position.x).ToList();

        AssignEnemyTargetDirections();
    }


    private void AssignEnemyTargetDirections()
    {
        int direction = 0;
        int[] targetDirs = new int[] { 1, 2, 0, 3 }; // Right Up Left Down 
        targetingDirections.Clear();

        // determine amount of characters alive
        int living = 0;
        foreach (CharacterEnemy enemy in enemyCharacters)
        {
            if (enemy.isAlive)
            {
                living++;
            }
        }
        
        if (living == 2) {
            targetDirs = new int[] { 1,0 }; // Right Left 
        } else if (living == 3)
        {
            targetDirs = new int[] { 1, 2, 0 }; // Right Up Left 
        } else
        {
            //nothing happens
        }

        // divide targeting reticles based on # of living characters
        foreach (CharacterEnemy enemy in enemyCharacters)
        {
            if (enemy.isAlive)
            {
                enemy.SetTargetingDirection(targetDirs[direction]);
                targetingDirections.Add(enemy.targetingDirection);
                direction++;
            }

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
            if (enemy.isAlive && enemy.targetingDirection == dir)
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
        power += change;

        if (power >= powerMax)
        {
            power = powerMax;
        }

        if (power < 0)
        {
            power = 0;
        }
        EventManager.Instance.SetUIPowerValue(power);
    }

    private void AdjustFavorBar (int change)
    {
        if (power >= powerMax)
        {
            EventManager.Instance.SetPowerChargeValue(powerCharge);
            return;
        }

        powerCharge += change;
        
        if (powerCharge >= 100)
        {
               EventManager.Instance.SetPowerChargeValue(powerCharge);
            if (power >= powerMax) {
                powerCharge = 100;
                EventManager.Instance.SetPowerChargeValue(powerCharge);
            }
            else
            {
                powerCharge = 0;
                AdjustPower(1);
            }            
        } else if (powerCharge < 0)
        {
            powerCharge = 0;
            EventManager.Instance.SetPowerChargeValue(powerCharge);
        }
        else
        {
            EventManager.Instance.SetPowerChargeValue(powerCharge);
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
            //friendlyDeathCounter++;
            //friendlyCharacters.Remove((CharacterFriendly)character);
        }
        else // if enemy character
        {
            enemyDeathCounter++;
            //enemyCharacters.Remove((CharacterEnemy)character);
        }
        //CheckForGameEnding();
        StartCoroutine(CheckForGameEndingWait());
    }

    private IEnumerator CheckForGameEndingWait()
    {
        yield return new WaitForSeconds(1f);
        CheckForGameEnding();
    }

    public void CheckForGameEnding()
    {
        if (friendlyCharacters.Count <= friendlyDeathCounter) // if all friendlies are defeated, lose game
        {
           // EndGame(false);

            EventManager.Instance.EndGame(false);
        }
        else if (enemyCharacters.Count <= enemyDeathCounter) // if all enemies are defeated, win game
        {
            EndGame(true);

            EventManager.ResumeCombatEvent -= ResumeCombat;
            EventManager.ResumeCombatEvent += ContinueOuttro;

            EventManager.Instance.Dialogue("rooftop_outtro");
        }
    }

    private void EndGame (bool victory)
    {
        gameEnded = true;
        EventManager.Instance.EndGameState();
        PauseCombat();
        PauseTimer();
        EventManager.Instance.RhythmGameStop();
    }

    private void ContinueOuttro()
    {
        EventManager.ResumeCombatEvent -= ContinueOuttro;
        EventManager.Instance.EndGame(true);
    }


    // ########
    // # Misc #
    // ########
    private void PauseGame()
    {
        if (paused)
        {
            StartCoroutine(UnpauseWait());
        }
        else
        {
            paused = true;
        }
    }

    private IEnumerator UnpauseWait()
    {
        yield return new WaitForSeconds(0.5f);
        paused = false;

    }

    private void SpawnVisualEffect(Character obj)
    {
            Instantiate(currentAbility.visualEffect, obj.effectPos.position, Quaternion.identity);
    }
}