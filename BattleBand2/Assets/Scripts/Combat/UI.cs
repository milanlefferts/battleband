using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using General.Colors;

public class UI : MonoBehaviour {

    [Header("Power")]
    public Image powerChargeCurrent;
    public Image powerBarFill;

    [Header("Special")]
    public Image specialBarFill;
    //public Animator specialBarAnim;

    [Header("Timer")]
    public Image[] cellBases, cellFills;
    private OnBeatSizeChange[] cellBaseScripts, cellFillScripts;
    public Animator friendlyTurnIndicator, enemyTurnIndicator;

    [Header("Hearts")]
    public Image heartFill;

    [Header("Scoring & Hits")]
    //public Text comboText;
    public TextMeshProUGUI scoreHits, scoreTotal;
    public Animator comboAnim;
    string lastScoreLevel = "";
   // bool scoreLevelRecentlyUpdated = false;
    public Image starMeter;
    public Image scoreBar;


    [Header("Popup text")]
    public Image popupSprite;
    public Sprite[] popupSprites;
    public GameObject noteStreakPopupObject;
    //private TextMeshProUGUI noteStreakPopupText;
    private float abilityUsedPopupDuration = 0.3f;

    [Header("Enemy Nameplates")]
    public GameObject nameEnemyObject, nameFriendlyObject;
    public TextMeshProUGUI nameEnemyAbilityText, nameEnemyText, nameFriendlyAbilityText, nameFriendlyText;

    [Header("Game Start")]
    public GameObject countdownTimer;
    public TextMeshProUGUI countdownText;

    [Header("Game Start/End")]
    public AudioClip[] countdownSounds;
    public TextMeshProUGUI endGameText;
    public Animator transitionScreenAnim;
    public AudioClip defeatSound, victorySound;
    public TextMeshProUGUI endScore, endCombo, endRank, endFame;

    [Header("Dialogue")]
    public Animator dialogueArrow; 
    public RuntimeAnimatorController[] dialogueArrowAnimators;
    public Animator[] menuAnimators;

    [Header("Ability Use")]
    public Image abilitySingerDirection, abilityDrummerDirection, abilityGuitaristDirection;
    private Animator abilitySingerDirectionAnim, abilityDrummerDirectionAnim, abilityGuitaristDirectionAnim, switchIconAnim;

    public Animator[] inputIconAnims, statBackgroundAnims;

    // Turns
    [Header("Lights")]
    public GameObject lightEnemyParty, lightFriendlyParty; // Lights

    [Header("Input")]
    public Image dialogueSkipText;
    public Sprite[] ps4ControllerUI, xboxControllerUI;
    public Sprite[] keyboardUI;

    [Header("Audio")]
    private AudioSource audioSource;

    [Header("Pause Menu")]
    public Image[] pauseMenuButtons;
    public TextMeshProUGUI[] pauseMenuTexts;

    [Header("Misc")]
    public GameObject[] characterUIs;
    public Image difficultyImage;


    private void OnEnable()
    {
        // Combat Flow
        EventManager.SetupCombatEvent += SetupCombat;
        EventManager.ResumeCombatEvent += ResumeCombat;

        // Controller UI Setup
        EventManager.SetControllerUIEvent += SetControllerUI;
        
        // Stats
        EventManager.SetUIPowerValueEvent += UpdatePower;
        EventManager.SetPowerChargeValueEvent += UpdatePowerCharge;
        EventManager.UpdateScoreEvent += UpdateScore;
        EventManager.UpdateChainEvent += UpdateChain;
        EventManager.UpdateTimerEvent += UpdateTimer;
        EventManager.SetSpecialEvent += UpdateSpecial;
        EventManager.UpdateStarMeterEvent += UpdateStarMeter;

        // Rhythm Game
        EventManager.BeaterMissEvent += KeyMissed;
        EventManager.NoteHitEvent += NoteHit;
        EventManager.RhythmGameStopEvent += FriendlyNamePopupClose;
        EventManager.RhythmGameStartEvent += HideTimer;
        EventManager.RhythmGameStopEvent += RevealTimer;

        // Character Selection
        EventManager.InstrumentSwitchEvent += SetupAbilities;

        // Abilities
        EventManager.AbilityUsedEvent += AbilityPositiveFeedback;
        EventManager.PlayerAbilityMissEvent += AbilityNegativeFeedback;

        EventManager.SetEnemyNameEvent += EnemyNamePopup;
        EventManager.SetEnemyAbilityNameEvent += EnemyAbilityNamePopup;
        EventManager.SetFriendlyNameEvent += FriendlyNamePopup;
        EventManager.SetFriendlyAbilityNameEvent += FriendlyAbilityNamePopup;
        EventManager.CloseFriendlyNameEvent += FriendlyNamePopupClose;

        // Turns
        EventManager.PlayerTurnEvent += PlayerTurnStart;
        EventManager.EnemyTurnEvent += EnemyTurnStart;
        EventManager.AbilityIndicatorStatusEvent += ToggleFriendlyAbilities;

        EventManager.EnemyTurnEvent += TimerEnemy;
        EventManager.PlayerTurnEvent += TimerReset;
        EventManager.TutorialPlayerTurnEvent += TimerReset;

        // Death
        EventManager.DeathEvent += DeathPlayer;

        // Ending
        EventManager.CountdownEvent += CountdownTimer;
        EventManager.EndGameStateEvent += EndGameState;
        EventManager.EndGameEvent += EndGame;
        EventManager.SetEndComboEvent += SetEndCombo;
        EventManager.SetEndScoreEvent += SetEndScore;

    }

    private void OnDisable()
    {
        // Combat Flow
        EventManager.SetupCombatEvent -= SetupCombat;
        EventManager.ResumeCombatEvent -= ResumeCombat;

        // Controller UI Setup
        EventManager.SetControllerUIEvent -= SetControllerUI;

        // Stats
        EventManager.SetUIPowerValueEvent -= UpdatePower;
        EventManager.SetPowerChargeValueEvent -= UpdatePowerCharge;
        EventManager.UpdateScoreEvent -= UpdateScore;
        EventManager.UpdateChainEvent -= UpdateChain;
        EventManager.UpdateTimerEvent -= UpdateTimer;
        EventManager.SetSpecialEvent -= UpdateSpecial;
        EventManager.UpdateStarMeterEvent -= UpdateStarMeter;

        // Rhythm Game
        EventManager.BeaterMissEvent -= KeyMissed;
        EventManager.NoteHitEvent -= NoteHit;
        EventManager.RhythmGameStopEvent -= FriendlyNamePopupClose;
        EventManager.RhythmGameStartEvent -= HideTimer;
        EventManager.RhythmGameStopEvent -= RevealTimer;

        // Selecting Characters
        EventManager.InstrumentSwitchEvent -= SetupAbilities;

        // Abilities
        EventManager.AbilityUsedEvent -= AbilityPositiveFeedback;
        EventManager.PlayerAbilityMissEvent -= AbilityNegativeFeedback;

        EventManager.SetEnemyNameEvent -= EnemyNamePopup;
        EventManager.SetEnemyAbilityNameEvent -= EnemyAbilityNamePopup;
        EventManager.SetFriendlyNameEvent -= FriendlyNamePopup;
        EventManager.SetFriendlyAbilityNameEvent -= FriendlyAbilityNamePopup;
        EventManager.CloseFriendlyNameEvent -= FriendlyNamePopupClose;

        // Turns
        EventManager.PlayerTurnEvent -= PlayerTurnStart;
        EventManager.EnemyTurnEvent -= EnemyTurnStart;
        EventManager.AbilityIndicatorStatusEvent -= ToggleFriendlyAbilities;

        EventManager.EnemyTurnEvent -= TimerEnemy;
        EventManager.PlayerTurnEvent -= TimerReset;
        EventManager.TutorialPlayerTurnEvent -= TimerReset;

        // Death
        EventManager.DeathEvent -= DeathPlayer;

        // Ending
        EventManager.CountdownEvent -= CountdownTimer;
        EventManager.EndGameStateEvent -= EndGameState;
        EventManager.EndGameEvent -= EndGame;
        EventManager.SetEndComboEvent -= SetEndCombo;
        EventManager.SetEndScoreEvent -= SetEndScore;
    }

    private void Awake()
    {
        abilitySingerDirectionAnim = abilitySingerDirection.GetComponent<Animator>();
        abilityDrummerDirectionAnim = abilityDrummerDirection.GetComponent<Animator>();
        abilityGuitaristDirectionAnim = abilityGuitaristDirection.GetComponent<Animator>();
    }

    private void Start()
    {
        //switchIconAnim = switchIcon.GetComponent<Animator>();
        SetDifficultyImage();

        audioSource = GetComponent<AudioSource>();
        //noteStreakPopupText = noteStreakPopupObject.GetComponent<TextMeshProUGUI>();
        InitializeTimer();
        UpdatePower(0);
    }

    // #################
    // # Control Setup #
    // #################
    private void SetControllerUI(int val)
    {
        //OnBeatImageChange tempImage;

        if (val == 2) // Controller
        {
            EventManager.Instance.GetInputIcon(new Sprite[] { ps4ControllerUI[0], ps4ControllerUI[2], ps4ControllerUI[4] });

            abilitySingerDirection.sprite = ps4ControllerUI[0];

            abilityDrummerDirection.sprite = ps4ControllerUI[2];

            abilityGuitaristDirection.sprite = ps4ControllerUI[4];

            pauseMenuButtons[0].sprite = ps4ControllerUI[0];
            pauseMenuButtons[1].sprite = ps4ControllerUI[2];
            pauseMenuButtons[2].sprite = ps4ControllerUI[4];
            pauseMenuButtons[3].sprite = ps4ControllerUI[6];
            pauseMenuTexts[0].text = "L1";
            pauseMenuTexts[1].text = "R1";

            dialogueArrow.runtimeAnimatorController = dialogueArrowAnimators[2];
            dialogueSkipText.sprite = ps4ControllerUI[2];

            foreach (Animator anim in menuAnimators) {
                anim.runtimeAnimatorController = dialogueArrowAnimators[2];
            }
        }
        else if (val == 1)
        {
            EventManager.Instance.GetInputIcon(new Sprite[] { xboxControllerUI[0], xboxControllerUI[2], xboxControllerUI[4] });

            abilitySingerDirection.sprite = xboxControllerUI[0];/*
            tempImage = abilitySingerDirection.GetComponent<OnBeatImageChange>();
            tempImage.small = xboxControllerUI[0];
            tempImage.large = xboxControllerUI[1];*/

            abilityDrummerDirection.sprite = xboxControllerUI[2];/*
            tempImage = abilityDrummerDirection.GetComponent<OnBeatImageChange>();
            tempImage.small = xboxControllerUI[2];
            tempImage.large = xboxControllerUI[3];*/

            abilityGuitaristDirection.sprite = xboxControllerUI[4];


            pauseMenuButtons[0].sprite = xboxControllerUI[0];
            pauseMenuButtons[1].sprite = xboxControllerUI[2];
            pauseMenuButtons[2].sprite = xboxControllerUI[4];
            pauseMenuButtons[3].sprite = xboxControllerUI[6];
            pauseMenuTexts[0].text = "LB";
            pauseMenuTexts[1].text = "RB";

            dialogueArrow.runtimeAnimatorController = dialogueArrowAnimators[1];
            dialogueSkipText.sprite = xboxControllerUI[2];


            foreach (Animator anim in menuAnimators)
            {
                anim.runtimeAnimatorController = dialogueArrowAnimators[1];
            }
        }
        else if (val == 0) // Keyboard
        {
            EventManager.Instance.GetInputIcon(new Sprite[] { keyboardUI[0], keyboardUI[2], keyboardUI[4] });

            abilitySingerDirection.sprite = keyboardUI[0];/*
            tempImage = abilitySingerDirection.GetComponent<OnBeatImageChange>();
            tempImage.small = keyboardUI[0];
            tempImage.large = keyboardUI[1];
            */
            abilityDrummerDirection.sprite = keyboardUI[2];/*
            tempImage = abilityDrummerDirection.GetComponent<OnBeatImageChange>();
            tempImage.small = keyboardUI[2];
            tempImage.large = keyboardUI[3];*/

            abilityGuitaristDirection.sprite = keyboardUI[4];

            pauseMenuButtons[0].sprite = keyboardUI[0];
            pauseMenuButtons[1].sprite = keyboardUI[2];
            pauseMenuButtons[2].sprite = keyboardUI[4];
            pauseMenuButtons[3].sprite = keyboardUI[6];
            pauseMenuTexts[0].text = "L. Shift";
            pauseMenuTexts[1].text = "R. Shift";

            dialogueArrow.runtimeAnimatorController = dialogueArrowAnimators[0];
            dialogueSkipText.sprite = keyboardUI[2];

            foreach (Animator anim in menuAnimators)
            {
                anim.runtimeAnimatorController = dialogueArrowAnimators[0];
            }
        }

        abilitySingerDirection.SetNativeSize();
        abilityDrummerDirection.SetNativeSize();
        abilityGuitaristDirection.SetNativeSize();
        dialogueSkipText.SetNativeSize();
    }

    // ##############
    // # Turn Setup #
    // ##############
    private void PlayerTurnStart()
    { 
        EnemyNamePopupClose();
        ToggleFriendlyAbilities(true);

        lightEnemyParty.SetActive(false);
        lightFriendlyParty.SetActive(true);

        friendlyTurnIndicator.SetBool("EnemyTurn", false);
        enemyTurnIndicator.SetBool("EnemyTurn", false);
    }

    private void EnemyTurnStart()
    {
        FriendlyNamePopupClose();

        ToggleFriendlyAbilities(false);

        lightEnemyParty.SetActive(true);
        lightFriendlyParty.SetActive(false);

        friendlyTurnIndicator.SetBool("EnemyTurn", true);
        enemyTurnIndicator.SetBool("EnemyTurn", true);

    }

    private void SetupCombat()
    {
        foreach (GameObject obj in characterUIs)
        {
            obj.SetActive(false);
        }
    }

    private void PauseCombat()
    {

    }

    private void ResumeCombat()
    {
        if (CombatController.Instance.playerTurn)
        {
            foreach (GameObject obj in characterUIs)
            {
                obj.SetActive(true);
                obj.GetComponentInParent<CharacterUI>().StartCharacterUI();

            }
        }
    }

    private void ToggleFriendlyAbilities(bool status)
    {
        if (status)
        {
            if (abilitySingerDirectionAnim.gameObject.activeSelf)
                abilitySingerDirectionAnim.SetInteger("Size", 1);
            if (abilityDrummerDirectionAnim.gameObject.activeSelf)
                abilityDrummerDirectionAnim.SetInteger("Size", 1);
            if (abilityGuitaristDirectionAnim.gameObject.activeSelf)
                abilityGuitaristDirectionAnim.SetInteger("Size", 1);
            foreach (Animator anim in inputIconAnims)
            {
                if (anim.gameObject.activeSelf)
                    anim.SetInteger("Size", 1);
            }
            foreach (Animator anim in statBackgroundAnims)
            {
                if (anim.gameObject.activeSelf)
                    anim.SetInteger("Size", 1);
            }
            
            /*
            if (switchIcon.gameObject.activeInHierarchy)
                switchIcon.GetComponent<Animator>().SetInteger("Size", 1);*/
        }
        else
        {
            if (abilitySingerDirectionAnim.gameObject.activeSelf)
                abilitySingerDirectionAnim.SetInteger("Size", 0);
            if (abilityDrummerDirectionAnim.gameObject.activeSelf)
                abilityDrummerDirectionAnim.SetInteger("Size", 0);
            if (abilityGuitaristDirectionAnim.gameObject.activeSelf)
                abilityGuitaristDirectionAnim.SetInteger("Size", 0);
            foreach (Animator anim in inputIconAnims)
            {
                if (anim.gameObject.activeSelf)
                    anim.SetInteger("Size", 0);
            }
            foreach (Animator anim in statBackgroundAnims)
            {
                if (anim.gameObject.activeSelf)
                    anim.SetInteger("Size", 0);
            }
            /*
            if (switchIcon.gameObject.activeInHierarchy)
                switchIcon.GetComponent<Animator>().SetInteger("Size", 0);*/
        }
    }

    // Enemy name popup
    private void EnemyNamePopup(string name)
    {
        nameEnemyText.text = name;
        nameEnemyObject.SetActive(true);
    }

    private void EnemyAbilityNamePopup(string name)
    {
        nameEnemyAbilityText.text = name;
    }

    private void EnemyNamePopupClose()
    {
        nameEnemyObject.GetComponent<Animator>().SetTrigger("Close");
        nameEnemyText.text = "";
        nameEnemyAbilityText.text = "";
    }

    // Friendly name popup
    private void FriendlyNamePopupClose()
    {
        if (nameFriendlyObject.activeInHierarchy)
        {
            nameFriendlyObject.GetComponent<Animator>().SetTrigger("Close");
        }
        nameFriendlyText.text = "";
        nameFriendlyAbilityText.text = "";
    }

    private void FriendlyNamePopup(string name)
    {
        nameFriendlyText.text = name;
        nameFriendlyObject.SetActive(true);
    }

    private void FriendlyAbilityNamePopup(string name)
    {
        nameFriendlyAbilityText.text = name;
    }

    // Timer

    private void InitializeTimer()
    {
        cellBaseScripts = new OnBeatSizeChange[cellBases.Length];
        cellFillScripts = new OnBeatSizeChange[cellFills.Length];

        for (int i = 0; i < cellBases.Length; i++)
        {
            cellBaseScripts[i] = cellBases[i].GetComponent<OnBeatSizeChange>();
            cellFillScripts[i] = cellFills[i].GetComponent<OnBeatSizeChange>();

        }
    }

    private void UpdateTimer(float time)
    {
        if (time > cellBases.Length - 1)
        {
            return;
        }
        cellFills[(int)time].enabled = false;
    }
    
    private void TimerEnemy()
    {
        for (int i = 0; i < cellBases.Length; i++)
        {
            cellFills[i].enabled = true;

        }
    }

    private void TimerReset()
    {
        for(int i = 0; i < cellBases.Length; i++)
        {
            cellFills[i].enabled = true;
        }
    }

    private void CountdownTimer(int countdown)
    {
        if (countdown == 3)
        {
            countdownTimer.SetActive(true);
            countdownText.text = countdown.ToString();
            //countdownImage.sprite = countdownNumbers[countdown - 1];
            audioSource.PlayOneShot(countdownSounds[countdown - 1]);
        }
        if (countdown == 0)
        {
            //countdownImage.sprite = countdownNumbers[3];
            countdownText.text = "ROCK!";
            audioSource.PlayOneShot(countdownSounds[3]);
        }
        else if (countdown < 0)
        {
            countdownTimer.SetActive(false);
            EventManager.CountdownEvent -= CountdownTimer;
        } else
        {
            countdownText.text = countdown.ToString();
            //countdownImage.sprite = countdownNumbers[countdown - 1];
            audioSource.PlayOneShot(countdownSounds[countdown - 1]);
        }
        countdownTimer.GetComponent<Animator>().SetTrigger("Countdown");
    }

    private void HideTimer()
    {
        //turnIndicator.SetActive(false);

        //timerFill.enabled = false;
        //timerBase.enabled = false;
        //timerIcon.enabled = false;
    }

    private void RevealTimer()
    {
        //turnIndicator.SetActive(true);

        //timerFill.enabled = true;
        //timerBase.enabled = true;
        //timerIcon.enabled = true;
    }

    // ###############
    // # Ability Use #
    // ###############
    private void SetupAbilities()
    {
        //Ability[] tempAbilities = PartyController.Instance.GetAbilities();
        /*
        abilitySingerName.text = tempAbilities[0].GetName();
        abilitySingerIcon.sprite = tempAbilities[0].icon;
        abilityDrummerName.text = tempAbilities[1].GetName();
        abilityDrummerIcon.sprite = tempAbilities[1].icon;
        abilityGuitaristName.text = tempAbilities[2].GetName();
        abilityGuitaristIcon.sprite = tempAbilities[2].icon;
        */
        // Update UI
        UpdatePower(CombatController.Instance.power);
    }

    private void KeyMissed()
    {
        SetNotestreak("Miss", Colors.colorRedLight);
    }

    private void NoteHit()
    {
        if (CombatController.Instance.playerTurn)
        {
            SetNotestreak("Hit", Color.white);
        } else
        {
            SetNotestreak("Block", Colors.colorBlueLight);
        }
    }

    private void SetNotestreak(string text, Color color)
    {
        StopCoroutine("NotestreakPopup");
        noteStreakPopupObject.SetActive(false);
        //noteStreakPopupText.text = text;
        popupSprite.sprite = TextPopupImageSelector(text);
        popupSprite.color = color;
        popupSprite.SetNativeSize();
        StartCoroutine("NotestreakPopup");
    }

    private IEnumerator NotestreakPopup()
    {
        noteStreakPopupObject.SetActive(true);
        yield return new WaitForSeconds(abilityUsedPopupDuration);
        noteStreakPopupObject.SetActive(false);
    }

    private Sprite TextPopupImageSelector(string text)
    {
        Sprite temp = popupSprites[0];
        switch (text)
        {
            case "Hit":
                temp = popupSprites[0];
                break;
            case "Miss":
                temp = popupSprites[1];
                break;
            case "Jam":
                temp = popupSprites[2];
                break;
            case "Switch":
                temp = popupSprites[3];
                break;/*
            case "Block":
                temp = popupSprites[4];
                break;*/
            default:
                break;
        }
        return temp;
    }

    private void AbilityPositiveFeedback(string ability)
    {
        switch (ability)
        {
            case "Ability1":
                abilitySingerDirectionAnim.SetTrigger("Hit");
                break;
            case "Ability2":
                abilityDrummerDirectionAnim.SetTrigger("Hit");
                break;
            case "Ability3":
                abilityGuitaristDirectionAnim.SetTrigger("Hit");
                break;
            case "Jam":
                //StartCoroutine(BlinkAbility(jamIcon, Colors.colorYellow));
                //StartCoroutine(AbilityUsePopup(jamFeedbackText));
                break;
            case "Switch":
                //switchIconAnim.SetTrigger("Hit");
                //StartCoroutine(AbilityUsePopup(switchFeedbackText));
                break;
            default:
                break;
        }
    }

    private IEnumerator AbilityUsePopup(GameObject obj)
    {
        obj.SetActive(true);
        yield return new WaitForSeconds(abilityUsedPopupDuration);
        obj.SetActive(false);
    }

    private void AbilityNegativeFeedback(string ability)
    {
        switch (ability)
        {
            case "Ability1":
                abilitySingerDirectionAnim.SetTrigger("Miss");
                break;
            case "Ability2":
                abilityDrummerDirectionAnim.SetTrigger("Miss");
                break;
            case "Ability3":
                abilityGuitaristDirectionAnim.SetTrigger("Miss");
                break;
            case "Jam":
                //StartCoroutine(BlinkAbility(jamIcon, Colors.colorRedLight));
                break;
            case "Switch":
                //switchIconAnim.SetTrigger("Miss");
                break;
            default:
                break;
        }
    }

    private IEnumerator BlinkAbility(Image img, Color color)
    {
        Color tempColor = img.color;
        img.color = color;
        yield return new WaitForSeconds(0.1f);
        img.color = tempColor;
    }

    // ################
    // # Stat Updates #
    // ################
    private void UpdatePowerCharge(int charge)
    {
        float baseFill = 0;
        switch (CombatController.Instance.power)
        {
            case 0:
                baseFill = 0;
                break;
            case 1:
                baseFill = 0.33f;
                break;
            case 2:
                baseFill = 0.66f;
                break;
            case 3:
                baseFill = 1f;
                break;
            default:
                baseFill = 0;
                break;
        }
        if (baseFill == 1f)
        {
            powerBarFill.fillAmount = baseFill;
        }
        else
        {
            powerBarFill.fillAmount = baseFill + ((charge / 100f) * (1f/3f));
        }
    }

    private void UpdatePower(int power)
    {
        powerChargeCurrent.GetComponent<Animator>().SetInteger("Power", power);
        UpdatePowerCharge(CombatController.Instance.powerCharge);

        if (!CombatController.Instance.playerTurn)
        {
            return;
        }

        if (power >= 1)
        {
            if (abilitySingerDirectionAnim.gameObject.activeSelf)
                abilitySingerDirectionAnim.SetInteger("Color", 1);
            //abilitySinger.GetComponent<ReactToBeat>().enabled = true;
            if (abilityDrummerDirectionAnim.gameObject.activeSelf)
                abilityDrummerDirectionAnim.SetInteger("Color", 1);
            //abilityDrummer.GetComponent<ReactToBeat>().enabled = true;
            if (abilityGuitaristDirectionAnim.gameObject.activeSelf)
                abilityGuitaristDirectionAnim.SetInteger("Color", 1);
            //abilityGuitarist.GetComponent<ReactToBeat>().enabled = true;

            foreach (Animator anim in inputIconAnims)
            {
                if (anim.gameObject.activeSelf)
                    anim.SetInteger("Color", 1);
            }

        }
        else
        {
            if (abilitySingerDirectionAnim.gameObject.activeSelf)
                abilitySingerDirectionAnim.SetInteger("Color", 0);
            //abilitySinger.GetComponent<ReactToBeat>().enabled = false;
            if (abilityDrummerDirectionAnim.gameObject.activeSelf)
                abilityDrummerDirectionAnim.SetInteger("Color", 0);
            //abilityDrummer.GetComponent<ReactToBeat>().enabled = false;
            if (abilityGuitaristDirectionAnim.gameObject.activeSelf)
                abilityGuitaristDirectionAnim.SetInteger("Color", 0);
            //abilityGuitarist.GetComponent<ReactToBeat>().enabled = false;

            foreach (Animator anim in inputIconAnims)
            {
                if (anim.gameObject.activeSelf)
                    anim.SetInteger("Color", 0);
            }
        }
    }

    private IEnumerator AbilityAvailableFlash(Image flash, Color color)
    {
        flash.color = color;
        yield return new WaitForSeconds(0.1f);
        flash.color = Color.white;
    }

    // ###########
    // # Special #
    // ###########
    private void UpdateSpecial(float val)
    {
        specialBarFill.fillAmount = val;
    }

    // #########
    // # Score #
    // #########
    private void UpdateStarMeter(float score, int comboLevel)
    {
        //comboText.text = string.Format("{0}", score);

        scoreBar.fillAmount = score;
        starMeter.fillAmount = comboLevel / 5f;

        /*
        if (comboLevel != lastScoreLevel && !scoreLevelRecentlyUpdated)
        {
            //StartCoroutine(ScoreLevelChangeTimer());
            SetScoreStreak(comboLevel, Colors.colorYellow);
        }*/
    }

    private void UpdateChain(int hits)
    {
        scoreHits.text = "\n\n" + hits.ToString();
    }

    private void UpdateScore(int score)
    {
        scoreTotal.text = "\n" + score.ToString();
    }

    // #############
    // # End game #
    // #############
    private void EndGame(bool victory)
    {
        if (victory) // Win
        {
            endGameText.text = "VICTORY! :D";
            audioSource.PlayOneShot(victorySound);
        }
        else // Loss
        {
            endGameText.text = "DEFEAT :(";
            audioSource.PlayOneShot(defeatSound);
        }
        EventManager.EndGameEvent -= EndGame;
        transitionScreenAnim.SetBool("CloseScreen", true);
    }

    private void EndGameState() // TODO: make this generic (and not only function for this level)
    {
        foreach (GameObject obj in characterUIs)
        {
            obj.SetActive(false);
        }
    }
    private void SetEndCombo(int val)
    {
        endCombo.text = val.ToString();
    }

    private void SetEndScore(int val)
    {
        endScore.text = val.ToString();
        endFame.text = "50 <color=blue>+" + (val).ToString() + "bonus</color>";
        if (lastScoreLevel != "")
        {
            endRank.text = lastScoreLevel;
        }
        else
        {
            endRank.text = "none";
        }
    }

    private void DeathPlayer(Character character)
    {
        if (character.isFriendlyCharacter)
        {
            heartFill.fillAmount -= 0.33f;
        }
    }

    // ##############
    // # Difficulty #
    // ##############
    private void SetDifficultyImage()
    {
        if (difficultyImage != null)
            difficultyImage.fillAmount = PlayerData.Instance.difficulty / 5f;
    }
}