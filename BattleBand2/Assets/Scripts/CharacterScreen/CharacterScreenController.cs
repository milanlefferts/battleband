using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using General.ControllerInput;
using TMPro;

public class CharacterScreenController : MonoBehaviour {
    // Allows remote access for unique instance
    public static CharacterScreenController Instance
    {
        get
        {
            return instance;
        }
    }
    private static CharacterScreenController instance;


    private int selection = 4; // Number of menus: char0, char1, char2, tour
    private int selected;

    private Vector3[] selectionPos, selectionRot;
    public ControllerInput controllerInput;

    private Coroutine openScreen;

    public Animator characterScreenAnim;

    public TextMeshProUGUI instrumentText;
    public Image[] instrumentsImages, instrumentSelectors;
    public Image techniqueStars, confidenceStars, staminaStars, difficultySkulls;
    public GameObject[] instruments;
    public TextMeshProUGUI characterName;
    public string[] characterNames;

    public Image fameFill;

    public TextMeshProUGUI abilityPointsText;

    // Instruments
    private int currentInstrumentNr;
    private Instrument[] currentCharInstruments;
    private string instrumentName, instrumentAbility, instrumentAbilityDescription, instrumentFlavor;

    // Menu interaction
    public MenuItem currentMenuItem;
    private int menuItemNr;
    private MenuItem[] currentMenuItemSet;
    public MenuItem[] menuItemSet_Level, menuItemSet_Stats;
    public AudioClip menuSelectSound, menuConfirmSound, menuAppearSound;
    private bool confirmed;

    public Animator levelScreenAnim;

    // Dpad
    bool canPress;
    float pressCooldown;

    // Difficulty
    private int difficultyCurrent, difficultyMax;
    public Image[] difficultyImages;

    // General Components
    private AudioSource audioSource;

    public TextMeshProUGUI left, right;

    // Dialogue
    public Animator dialogueArrow;
    public RuntimeAnimatorController[] dialogueArrowAnimators;

    private void OnEnable()
    {
        EventManager.SetControllerUIEvent += TempController;
    }

    private void OnDisable()
    {
        EventManager.SetControllerUIEvent -= TempController;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        controllerInput = new ControllerInput();
        controllerInput.SetupControls();
        SetSelectionPos();
        selected = 3;

        currentMenuItemSet = menuItemSet_Level;
        menuItemNr = 0;
        //currentMenuItem = currentMenuItemSet[menuItemNr];
        currentMenuItem.HighlightItem();

        difficultyMax = 3;
        difficultyCurrent = PlayerData.Instance.difficulty;
        if (difficultyCurrent < 1)
            difficultyCurrent = 1;
        difficultySkulls.fillAmount = 1 / 5f;

        SongManager.Instance.PlaySong("Kalax");

        pressCooldown = 0.1f;
        canPress = true;
        UpdateFameUI();

        levelScreenAnim.SetBool("Open", true);

        StartCoroutine(StartDialogue());
    }

    private void Update()
    {

        if (controllerInput.hasGamepad)
        {
            controllerInput.DpadInputCheck();
        }

        if (DialogueController.Instance.dialoguePlaying)
            return;

        if ((Input.GetKeyDown(controllerInput.buttonLeft) || Input.GetKeyDown(controllerInput.buttonRight) || Input.GetKeyDown(controllerInput.buttonUp)) && !confirmed)
        {
            confirmed = true;
            StartCoroutine(ConfirmWait());
        }
        // CHANGE MENU
        if (Input.GetKeyDown(controllerInput.buttonLeftTrigger))
        {
            SelectPrevious();
        }
        else if (Input.GetKeyDown(controllerInput.buttonRightTrigger))
        {
            SelectNext();
        }

        // INTERACT WITH MENU
        if (Input.GetKeyDown(controllerInput.dpadLeft) && canPress || controllerInput.left && canPress)
        {
            controllerInput.currentDirection = Dpad.None;
            StartCoroutine(DpadPause());

            audioSource.PlayOneShot(menuConfirmSound);
            LeftMenu();
        }
        else if (Input.GetKeyDown(controllerInput.dpadRight) && canPress || controllerInput.right && canPress)
        {

            controllerInput.currentDirection = Dpad.None;
            StartCoroutine(DpadPause());

            audioSource.PlayOneShot(menuConfirmSound);

            RightMenu();
        }
        else if (Input.GetKeyDown(controllerInput.dpadUp) && canPress || controllerInput.up && canPress)
        {
            controllerInput.currentDirection = Dpad.None;

            StartCoroutine(DpadPause());

            audioSource.PlayOneShot(menuSelectSound);
            DownMenu();
            controllerInput.up = false;
        }
        else if (Input.GetKeyDown(controllerInput.dpadDown) && canPress || controllerInput.down && canPress)
        {
            controllerInput.currentDirection = Dpad.None;

            StartCoroutine(DpadPause());

            audioSource.PlayOneShot(menuSelectSound);
            UpMenu();
            controllerInput.down = false;
        }
    }

    private IEnumerator StartDialogue()
    {
        yield return new WaitForSeconds(1.5f);

        if (PlayerData.Instance.firstTime)
        {
            EventManager.Instance.Dialogue("garage_intro");
            PlayerData.Instance.firstTime = false;
            PlayerData.Instance.Save();
        }

        currentMenuItem = currentMenuItemSet[menuItemNr];
        currentMenuItem.HighlightItem();
    }

    // # Menu Controls
    private void UpMenu()
    {
        currentMenuItem.UnhighlightItem();
        menuItemNr = (menuItemNr + 1) > currentMenuItemSet.Length - 1 ? 0 : menuItemNr + 1;
        currentMenuItem = currentMenuItemSet[menuItemNr];
        currentMenuItem.HighlightItem();
    }

    private void DownMenu()
    {
        currentMenuItem.UnhighlightItem();
        menuItemNr = (menuItemNr - 1) < 0 ? currentMenuItemSet.Length - 1 : menuItemNr - 1;
        currentMenuItem = currentMenuItemSet[menuItemNr];
        currentMenuItem.HighlightItem();
    }

    private void LeftMenu()
    {
        if (currentMenuItemSet == menuItemSet_Level) // Select Difficulty
        {
            difficultyCurrent = (difficultyCurrent - 1) < 1 ? 1 : difficultyCurrent - 1;
            PlayerData.Instance.difficulty = difficultyCurrent;
            difficultySkulls.fillAmount = difficultyCurrent / 5f;
            //SetStat(difficultyImages, difficultyCurrent);
        }
        else if (menuItemNr == 3) // Select Instrument
        {
            PreviousInstrument();
        }
        else if (menuItemNr == 2) // Add Ability Point
        {
            PlayerData.Instance.RemoveStat(Stat.Stamina, selected);
            staminaStars.fillAmount = PlayerData.Instance.statStamina[selected] / 5f;

            //SetStat(levelStamina, PlayerData.Instance.statStamina[selected]);
            UpdateAbilityPoints();

        }
        else if (menuItemNr == 1) // Add Ability Point
        {
            PlayerData.Instance.RemoveStat(Stat.Confidence, selected);
            confidenceStars.fillAmount = PlayerData.Instance.statConfidence[selected] / 5f;

            //SetStat(levelConfidence, PlayerData.Instance.statConfidence[selected]);
            UpdateAbilityPoints();

        }
        else if (menuItemNr == 0) // Add Ability Point
        {
            PlayerData.Instance.RemoveStat(Stat.Technique, selected);
            techniqueStars.fillAmount = PlayerData.Instance.statTechnique[selected] / 5f;

            //SetStat(levelTechnique, PlayerData.Instance.statTechnique[selected]);
            UpdateAbilityPoints();

        }
    }

    private void RightMenu()
    {
        if (currentMenuItemSet == menuItemSet_Level) // Select Difficulty
        {
            difficultyCurrent = (difficultyCurrent + 1) > difficultyMax ? difficultyMax : difficultyCurrent + 1;
            PlayerData.Instance.difficulty = difficultyCurrent;
            difficultySkulls.fillAmount = difficultyCurrent / 5f;
            //SetStat(difficultyImages, difficultyCurrent);
        }
        else if (menuItemNr == 3) // Select Instrument
        {
            NextInstrument();
        }
        else if (menuItemNr == 2) // Add Ability Point
        {
            PlayerData.Instance.AddStat(Stat.Stamina, selected);
            staminaStars.fillAmount = PlayerData.Instance.statStamina[selected] / 5f;
            //SetStat(levelStamina, PlayerData.Instance.statStamina[selected]);
            UpdateAbilityPoints();

        }
        else if (menuItemNr == 1) // Add Ability Point
        {
            PlayerData.Instance.AddStat(Stat.Confidence, selected);
            confidenceStars.fillAmount = PlayerData.Instance.statConfidence[selected] / 5f;

            //SetStat(levelConfidence, PlayerData.Instance.statConfidence[selected]);
            UpdateAbilityPoints();

        }
        else if (menuItemNr == 0) // Add Ability Point
        {
            PlayerData.Instance.AddStat(Stat.Technique, selected);
            techniqueStars.fillAmount = PlayerData.Instance.statTechnique[selected] / 5f;

            //SetStat(levelTechnique, PlayerData.Instance.statTechnique[selected]);
            UpdateAbilityPoints();
        }
    }

    private IEnumerator ConfirmWait()
    {
        audioSource.PlayOneShot(menuConfirmSound);
        yield return new WaitForSeconds(0.5f);
        currentMenuItem.SelectItem();
    }

    // # Menu changes
    public void SelectNext()
    {
        selected = (selected + 1) > selection - 1 ? 0 : selected + 1;
        SelectMenu();
    }

    public void SelectPrevious()
    {

        selected = (selected - 1) < 0 ? selection - 1 : selected - 1;
        SelectMenu();
    }

    private void SelectMenu()
    {
        characterScreenAnim.SetBool("Open", false);
        levelScreenAnim.SetBool("Open", false);

        EventManager.Instance.MoveCameraSmooth(selectionPos[selected], selectionRot[selected]);

        if (selected != 3)
        {
            currentMenuItemSet = menuItemSet_Stats;
            SwitchCharacterScreen();
        }
        else
        {
            currentMenuItemSet = menuItemSet_Level;
            if (openScreen != null)
                StopCoroutine(openScreen);
            openScreen = StartCoroutine(OpenLevelSelectScreen());
        }

        currentMenuItem.UnhighlightItem();
        currentMenuItem = currentMenuItemSet[0];
        menuItemNr = 0;
        currentMenuItem.HighlightItem();


    }

    private IEnumerator OpenLevelSelectScreen()
    {
        yield return new WaitForSeconds(0.5F);
        audioSource.PlayOneShot(menuAppearSound);
        levelScreenAnim.SetBool("Open", true);
    }


    private void SetSelectionPos()
    {
        selectionPos = new Vector3[] {
                        new Vector3(4.5f,7.5f,-46.5f), // Singer
                        new Vector3(-10f,7.15f,-35.8f), // Drummer
                        new Vector3(19f,8f,-31.4f), // Guitarist
                        new Vector3(0f, 11.2f, -60f) // Front
        }; 

        selectionRot = new Vector3[] {
                        new Vector3(0f, 0f,0f),
                        new Vector3(0f,-5f,0f),
                        new Vector3(0f,5f,0f),
                        new Vector3(5f,0f,0f),};
    }

    private void SwitchCharacterScreen()
    {
        if (openScreen != null)
        {
            StopCoroutine(openScreen);
        }
        openScreen = StartCoroutine(SwitchChar());
    }

    private IEnumerator SwitchChar()
    {
        yield return new WaitForSeconds(0.25f);
        LoadCharacterData();
        yield return new WaitForSeconds(0.25f);
        audioSource.PlayOneShot(menuAppearSound);
        if (levelScreenAnim.GetBool("Open") == false)
            characterScreenAnim.SetBool("Open", true);

    }

    private void LoadCharacterData()
    {
        if (selected == 3)
        {
            return;
        }

        currentCharInstruments = InstrumentManager.Instance.instruments[selected];
        currentInstrumentNr = PlayerData.Instance.currentInstruments[selected];

        characterName.text = characterNames[selected];

        LoadInstrument(currentCharInstruments[currentInstrumentNr]);

        UpdateAbilityPoints();

        instrumentSelectors[currentInstrumentNr].enabled = false;
        instrumentSelectors[currentInstrumentNr].enabled = true;

        var i = 0;
        foreach (Instrument ins in currentCharInstruments)
        {
            instrumentsImages[i].sprite = currentCharInstruments[i].icon;
            i++;
        }

        SetInstruments(PlayerData.Instance.unlockedInstruments[selected]);

        staminaStars.fillAmount = PlayerData.Instance.statStamina[selected] / 5f;
        techniqueStars.fillAmount = PlayerData.Instance.statTechnique[selected] / 5f;
        confidenceStars.fillAmount = PlayerData.Instance.statConfidence[selected] / 5f;

        //SetStat(levelStamina, PlayerData.Instance.statStamina[selected]);
        //SetStat(levelConfidence, PlayerData.Instance.statConfidence[selected]);
        //SetStat(levelTechnique, PlayerData.Instance.statTechnique[selected]);

        EventManager.Instance.InstrumentSwitch();
    }

    private void UpdateAbilityPoints ()
    {
        if (PlayerData.Instance.abilityPointsLeft[selected] > 0)
        {
            abilityPointsText.text = PlayerData.Instance.abilityPointsLeft[selected].ToString();
        }
        else
        {
            abilityPointsText.text = "0";
        }
    }

    private void LoadInstrument(Instrument ins)
    {
        instrumentName = ins.name;
        instrumentAbility = ins.ability;
        instrumentAbilityDescription = ins.abilityText;
        instrumentFlavor = ins.flavorText;

        var s = string.Format("<b><color=red>{0}</color>\n\n</b>Grants <color=#D10010> {1} </color>\n\n{2}\n\n<i>'{3}'</i> ",
                        instrumentName, instrumentAbility, instrumentAbilityDescription, instrumentFlavor);

        instrumentText.text = s;
    }

    private void NextInstrument()
    {
        instrumentSelectors[currentInstrumentNr].enabled = false;
        currentInstrumentNr = (currentInstrumentNr + 1) > PlayerData.Instance.unlockedInstruments[selected] ? 0 : currentInstrumentNr + 1;
        LoadInstrument(currentCharInstruments[currentInstrumentNr]);
        PlayerData.Instance.currentInstruments[selected] = currentInstrumentNr;

        // TODO: Make this less omslachtig.
        PartyController.Instance.ChangeAbility(selected, 1, AbilityManager.Instance.abilityDB[currentCharInstruments[currentInstrumentNr].ability]);

        instrumentSelectors[currentInstrumentNr].enabled = true;
        EventManager.Instance.InstrumentSwitch();

    }

    private void PreviousInstrument()
    {
        instrumentSelectors[currentInstrumentNr].enabled = false;
        currentInstrumentNr = (currentInstrumentNr - 1) < 0 ? PlayerData.Instance.unlockedInstruments[selected] : currentInstrumentNr - 1;
        LoadInstrument(currentCharInstruments[currentInstrumentNr]);
        PlayerData.Instance.currentInstruments[selected] = currentInstrumentNr;

        // TODO: Make this less omslachtig.
        PartyController.Instance.ChangeAbility(selected, 1, AbilityManager.Instance.abilityDB[currentCharInstruments[currentInstrumentNr].ability]);

        instrumentSelectors[currentInstrumentNr].enabled = true;
        EventManager.Instance.InstrumentSwitch();

    }

    private void SetStat(Image[] statImg, int stat)
    {
        for (int i = 0; i < statImg.Length; i++)
        {
            if (stat == 0)
            {
                statImg[i].color = Color.black;
            }
            else if (i < stat)
            {
                statImg[i].color = Color.white;
            }
            else
            {
                statImg[i].color = Color.black;
            }
        }
    }

    private void SetInstruments(int unlocked)
    {
        for (int i = 0; i < instruments.Length; i++)
        {
            if (i <= unlocked)
            {
                instruments[i].SetActive(true);
            }
            else
            {
                instruments[i].SetActive(false);
            }
        }
    }

    private void UpdateFameUI()
    {
        if (PlayerData.Instance.playerFame == 0)
        {
            fameFill.fillAmount = 0f;

        }
        else if (PlayerData.Instance.playerLevel == 1)
        {
            fameFill.fillAmount = PlayerData.Instance.playerFame / 100f;
        }
        else
        {
            fameFill.fillAmount = (PlayerData.Instance.playerFame - (PlayerData.Instance.playerLevel * 100f) /100f);
        }
    }

    // MISC
    private void TempController(int controller)
    {
        dialogueArrow.runtimeAnimatorController = dialogueArrowAnimators[controller];

        switch (controller)
        {
            case 0: // KEYBOARD
                left.text = "L.SHIFT";
                right.text = "R. SHIFT";
                
                break;

            case 1: // XBOX
                left.text = "L1";
                right.text = "R1";
                break;

            case 2: // PS
                left.text = "L1";
                right.text = "R1";
                break;

            default:
                left.text = "L. SHIFT";
                right.text = "R. SHIFT";
                break;
        }

    }

    // CONTROLLER
    private IEnumerator DpadPause()
    {
        canPress = false;
        yield return new WaitForSeconds(pressCooldown);
        canPress = true;
    }
}