using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Tutorial : MonoBehaviour {

    // Allows remote access for unique instance
    public static Tutorial Instance
    {
        get
        {
            return instance;
        }
    }
    private static Tutorial instance;

    public int currentPhase = 1;
    private bool nextPhase;

    public GameObject tutorialBar;
    public TextMeshProUGUI tutorialOverlay;
    public GameObject comboMeter, powerMeter, timer, abilityLeft, abilityRight, abilityUp, enemyName, heartMeter, specialMeter;
    public GameObject enemy1, enemy2, enemy3;
    private float transitionTime = 0.75f;

    private CombatController rhythmGameController;
    private TutorialRhythmGameController tutorialRhythmGameController;

    public GameObject friendlyPartyLights;

    // Controls
    private string[] overlayText;
    bool hasGamepad;

    private void OnEnable()
    {
        EventManager.TutorialNextPhaseEvent += NextPhase;
    }

    private void OnDisable()
    {
        EventManager.TutorialNextPhaseEvent -= NextPhase;
    }

    private void Awake()
    {
        instance = this;
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        CombatController.Instance.tutorial = true;
        TutorialRhythmGameController.Instance.tutorial = true;
        rhythmGameController = CombatController.Instance;
        tutorialRhythmGameController = TutorialRhythmGameController.Instance;

        StartCoroutine(TutorialFlow());

        SongManager.Instance.PlaySong("Job2");        // Sloan, Kalax, Morse, Necro

        DetermineOverlaytext();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            NextPhase();
        }

        if ((currentPhase == 4 || currentPhase == 11 || currentPhase == 16) & rhythmGameController.power == 1)
        {
            NextPhase();
        }
    }

    private IEnumerator TutorialFlow()
    {

        yield return new WaitForSeconds(2f);
        // # 1. Intro #
        EventManager.Instance.Dialogue("tutorial_0");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 2. Power meter appears #
        yield return new WaitForSeconds(transitionTime);
        AllowPowerMeter(); // Power meter appears
        AllowJamming(); // Player can jam
        SetOverlay(overlayText[7]);
        yield return new WaitUntil(()=> nextPhase);
        ResetPhase();

        currentPhase = 2;
        AllowJamming(); // Player can jam
        SetOverlay(overlayText[7]);
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        currentPhase = 2;
        AllowJamming(); // Player can jam
        SetOverlay(overlayText[7]);
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 3. Jamming: single #
        EventManager.Instance.Dialogue("tutorial_1");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 4. Jamming: bar #
        currentPhase = 4;
        SetOverlay(overlayText[0]);
        CombatController.Instance.power = 0; // reset power
        CombatController.Instance.powerCharge = 0;
        EventManager.Instance.SetUIPowerValue(0);
        yield return new WaitForSeconds(transitionTime);
        AllowJamming(); // Player can jam and fills up power
        // when bar full >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        currentPhase = 4;
        SetOverlay(overlayText[0]);
        CombatController.Instance.power = 0; // reset power
        EventManager.Instance.SetUIPowerValue(0);
        yield return new WaitForSeconds(transitionTime);
        AllowJamming(); // Player can jam and fills up power
        // when bar full >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        currentPhase = 4;
        SetOverlay(overlayText[8]);
        CombatController.Instance.power = 0; // reset power
        EventManager.Instance.SetUIPowerValue(0);
        yield return new WaitForSeconds(transitionTime);
        AllowJamming();
        AllowComboMeter();
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();


        // # 5. Enemy appears #
        AppearEnemy();
        CameraMove.Instance.ReturnCamera();
        yield return new WaitForSeconds(transitionTime);
        EventManager.Instance.Dialogue("tutorial_2");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        //______________________________________________________________________
        // # 6. Single target damage ability use #
        SetOverlay(overlayText[1]);
        EventManager.Instance.SetUIPowerValue(0);
        CombatController.Instance.power = 1; // reset power
        EventManager.Instance.SetUIPowerValue(1);
        yield return new WaitForSeconds(transitionTime);
        AllowDamage();
        EventManager.Instance.SetUIPowerValue(1);

        //button appears. when button is pressed >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 7. Single target damage ability targeting #
        SetOverlay(overlayText[2]);
        AllowJamming(); // Player can jam and fills up power
        // targeting appears. when target is selected and attack used >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        currentPhase = 6;
        EventManager.Instance.SetUIPowerValue(0);
        CombatController.Instance.power = 1; // reset power
        EventManager.Instance.SetUIPowerValue(1);
        // Single target damage ability use #
        SetOverlay(overlayText[1]);
        yield return new WaitForSeconds(transitionTime);
        AllowDamage();
        EventManager.Instance.SetUIPowerValue(1);

        //button appears. when button is pressed >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // Single target damage ability targeting #
        SetOverlay(overlayText[2]);
        AllowJamming(); // Player can jam and fills up power
        // targeting appears. when target is selected and attack used >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        currentPhase = 6;
        EventManager.Instance.SetUIPowerValue(0);
        CombatController.Instance.power = 1; // reset power
        EventManager.Instance.SetUIPowerValue(1);
        // Single target damage ability use #
        SetOverlay(overlayText[1]);
        yield return new WaitForSeconds(transitionTime);
        AllowDamage();
        EventManager.Instance.SetUIPowerValue(1);

        //button appears. when button is pressed >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // Single target damage ability targeting #
        SetOverlay(overlayText[2]);
        AllowJamming(); // Player can jam and fills up power
        // targeting appears. when target is selected and attack used >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();
        //______________________________________________________________________


        // # 8. Enemy taunt #
        CloseAbilities();
        yield return new WaitForSeconds(1f);
        EventManager.Instance.Dialogue("tutorial_3");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 9. Enemy timer #
        SetOverlay(overlayText[3]);
        // timer appears
        yield return new WaitForSeconds(transitionTime);
        EventManager.Instance.SetupCombat(); // set speed for timer
        AllowJamming();
        AllowTimer();
        heartMeter.SetActive(true);
        // when timer fills and enemy attacks >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();
        tutorialRhythmGameController.PauseTimer(); //StopTimer

        yield return new WaitForSeconds(1.5f); // enemy attack pause
        EventManager.Instance.TutorialPlayerTurn();
        rhythmGameController.playerTurn = true; // set player turn
        tutorialRhythmGameController.playerTurn = true; // set player turn
        enemyName.SetActive(false);
        CameraMove.Instance.ReturnCamera();
        friendlyPartyLights.SetActive(true);

        // # 10. Take damage #
        EventManager.Instance.Dialogue("tutorial_4");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 11. Heal: bar fill #
        SetOverlay(overlayText[0]);
        rhythmGameController.power = 0; // reset power
        EventManager.Instance.SetUIPowerValue(0); // update power
        yield return new WaitForSeconds(transitionTime);
        AllowJamming(); // Player can jam and fills up power
        // when bar full >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 12. Heal: ability used #
        SetOverlay(overlayText[4]);
        //when button is pressed >
        AllowHeal(); //button appears.
        EventManager.Instance.SetUIPowerValue(1);

        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        currentPhase = 12;
        SetOverlay(overlayText[4]);
        rhythmGameController.power = 1; // reset power
        EventManager.Instance.SetUIPowerValue(1); // update power
        //when button is pressed >
        AllowHeal(); //button appears.
        EventManager.Instance.SetUIPowerValue(1);

        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        currentPhase = 12;
        SetOverlay(overlayText[4]);
        rhythmGameController.power = 1; // reset power
        EventManager.Instance.SetUIPowerValue(1); // update power
        //when button is pressed >
        AllowHeal(); //button appears.
        EventManager.Instance.SetUIPowerValue(1);

        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 13. Conversation Heal and Reinforcements #
        CloseAbilities();
        EventManager.Instance.Dialogue("tutorial_5");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 14. Enemy reinforcements #
        AppearReinforcements(); // additional enemies appear
        enemy1.GetComponent<CharacterEnemy>().Heal(enemy2.GetComponent<Character>(), 100);
        // after two extra enemies appear >
        yield return new WaitForSeconds(1f);
        NextPhase();
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 15. Drummer dialogue (taunt) #
        EventManager.Instance.Dialogue("tutorial_6");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 16. AOE: bar fill #
        SetOverlay(overlayText[0]);
        AllowJamming(); // Player can jam and fills up power
        CombatController.Instance.power = 0; // reset power
        EventManager.Instance.SetUIPowerValue(0);
        // after bar fills up >
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 17. Drummer dialogue (ready) #
        EventManager.Instance.Dialogue("tutorial_7");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 18. AOE: ability used #
        SetOverlay(overlayText[5]);
        yield return new WaitForSeconds(transitionTime);
        AllowAOE(); //button appears. 
        EventManager.Instance.SetUIPowerValue(1);

        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 19. AOE: rhythm explanation #
        tutorialBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(300, -15);
        SetOverlay(overlayText[6]);
        //notes spawn. when enemies defeated
        yield return new WaitUntil(() => nextPhase); //
        ResetPhase();

        // # 20. Enemy dialogue (defeat) #
        yield return new WaitForSeconds(1.5f);
        EventManager.Instance.Dialogue("tutorial_8");
        CloseAbilities();
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 21. Enemy retreat #
        EnemyRetreat();
        // enemies move away >
        yield return new WaitForSeconds(1f);
        NextPhase();
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();

        // # 22. Party dialogue (conclusion) #
        CameraMove.Instance.MoveCameraSmooth(CameraMove.Instance.friendlyPartyZoomPos, CameraMove.Instance.baseRotationFriendly);

        EventManager.Instance.Dialogue("tutorial_9");
        yield return new WaitUntil(() => nextPhase);
        ResetPhase();
        Conductor.Instance.StopMusic(true);
        //endScreenAnim.SetBool("CloseScreen", true);
        yield return new WaitForSeconds(2f);
        //SceneManager.LoadScene("Level_Rooftop");
        SceneController.Instance.LoadScene("Level_Rooftop");
    }

    private void SetOverlay(string text)
    {
        tutorialBar.SetActive(true);
        tutorialOverlay.text = text;
    }

    public void NextPhase()
    {
        nextPhase = true;
    }

    private void ResetPhase()
    {
        rhythmGameController.combat = false;
        tutorialRhythmGameController.combat = false;
        tutorialBar.SetActive(false);
        nextPhase = false;
        currentPhase++;
    }

    private void DetermineOverlaytext()
    {
        string[] gamePads = Input.GetJoystickNames();
        if (gamePads.Length > 0)
        {
            foreach (string gamepad in gamePads)
            {
                if (gamepad == "Wireless Controller") // PS4 controls
                {
                    hasGamepad = true;
                    overlayText = overlayText_Playstation;
                    Debug.Log(gamepad + " detected. Setting PS4 controls.");
                    break;
                }
                else if (gamepad == "Controller (Xbox One For Windows)" || gamepad == "Controller (XBOX 360 For Windows)") // XBOX controls
                {
                    hasGamepad = true;
                    overlayText = overlayText_Xbox;
                    Debug.Log(gamepad + " detected. Setting XBOX 360 controls.");
                    break;
                }
            }
        }

        if (!hasGamepad)    // Set Keyboard Controls
        {
            overlayText = overlayText_Keyboard;
        }
    }

    // Down, Left, Right, Middle
    private string[] overlayText_Keyboard = new string[] {
    "Press <sprite index=1> on the beat to jam, filling the <color=purple>POWER</color> bar.",
    "Press <sprite index=3> on the beat to deal single-target <color=red>DAMAGE</color>. Each ability you use increases your <color=yellow>SPECIAL</color>. When full, press <color=yellow>R1</color> to use it.",
    "Press the correct <sprite index=12> on the beat to select a target.",
    "Your enemies will automatically attack when the <color=orange>timer</color> fills. Take too much damage and you lose a  <color=red>heart</color>.",
    "Lose all your <color=red>hearts</color> and it's game over! Press <sprite index=2>  on the beat to <color=yellow>HEAL</color>.",
    "Press <sprite index=0> on the beat to use your <color=green>Rhythm attack</color>.",
    "Hit the correct <sprite index=12> when the <sprite index=13> reaches the target to deal damage.",
    "Press <sprite index=1> on the beat. You can even jam in your enemy's turn!",
    "The <color=yellow>COMBO</color> meter indicates how well you are doing. Press <sprite index=1> on the beat!"
    };

    private string[] overlayText_Playstation = new string[] {
    "Press <sprite index=9> on the beat to jam, filling the <color=purple>POWER</color> bar.",
    "Press <sprite index=11> on the beat to deal single-target <color=red>DAMAGE</color>. Each ability you use increases your <color=yellow>SPECIAL</color>. When full, press <color=yellow>R1</color> to use it.",
    "Press the correct <sprite index=12> on the beat to select a target.",
    "Your enemies will automatically attack when the <color=orange>TIMER</color> fills.",
    "Press <sprite index=10>  on the beat to <color=yellow>HEAL</color>.",
    "Press <sprite index=8> on the beat to use your <color=green>RHYTHM</color> attack.",
    "Hit the correct <sprite index=12> when the <sprite index=13> reaches the target to deal damage.",
    "Press <sprite index=9> to jam on the beat. You can even jam in your enemy's turn!",
    "The <color=yellow>COMBO</color> meter indicates how well you are doing. Press <sprite index=9> on the beat!"
    };
    
    private string[] overlayText_Xbox = new string[] {
    "Press <sprite index=5> on the beat to jam, filling the <color=purple>POWER</color> bar.",
    "Press <sprite index=7> on the beat to deal single-target <color=red>DAMAGE</color>. Each ability you use increases your <color=yellow>SPECIAL</color>. When full, press <color=yellow>R1</color> to use it.",
    "Press the correct <sprite index=12> on the beat to select a target.",
    "Your enemies will automatically attack when the <color=orange>TIMER</color> fills.",
    "Press <sprite index=6> on the beat to <color=yellow>HEAL</color>.",
    "Press <sprite index=4> on the beat to use your <color=green>RHYTHM</color> attack.",
    "Hit the correct <sprite index=12> when the <sprite index=13> reaches the target to deal damage.",
    "Press <sprite index=5> on the beat.",
    "The <color=yellow>COMBO</color> meter indicates how well you are doing. Press <sprite index=5> on the beat!"
    };

    private void AllowJamming()
    {
        rhythmGameController.combat = true;
        tutorialRhythmGameController.combat = true;
    }

    private void AllowPowerMeter()
    {
        powerMeter.SetActive(true);
        CombatController.Instance.power = 0; // reset power
        EventManager.Instance.SetUIPowerValue(0);
    }

    private void AllowComboMeter()
    {
        comboMeter.SetActive(true);
    }

    private void AllowTimer()
    {
        tutorialRhythmGameController.timerDuration = 9f;
        timer.SetActive(true);
        rhythmGameController.combat = true;
        tutorialRhythmGameController.combat = true;

        tutorialRhythmGameController.StartTimer();
    }

    private void AllowHeal()
    {
        abilityLeft.SetActive(false);
        abilityUp.SetActive(false);
        abilityRight.SetActive(true);

        rhythmGameController.combat = true;
        tutorialRhythmGameController.combat = true;

        //EventManager.Instance.SetUIPowerValue(1);
    }

    private void AllowAOE()
    {
        abilityLeft.SetActive(false);
        abilityUp.SetActive(true);
        abilityRight.SetActive(false);
        rhythmGameController.combat = true;
        tutorialRhythmGameController.combat = true;

        //EventManager.Instance.SetUIPowerValue(1);
    }

    private void AllowDamage()
    {
        abilityLeft.SetActive(true);
        abilityUp.SetActive(false);
        abilityRight.SetActive(false);
        rhythmGameController.combat = true;
        tutorialRhythmGameController.combat = true;

        //EventManager.Instance.SetUIPowerValue(1);
    }

    private void CloseAbilities()
    {
        abilityLeft.SetActive(false);
        abilityUp.SetActive(false);
        abilityRight.SetActive(false);
    }

    private void AppearEnemy()
    {
        CharacterEnemy[] enemies = new CharacterEnemy[] { enemy1.GetComponent<CharacterEnemy>() };
        EventManager.Instance.EnemyAppear(enemies);
        tutorialRhythmGameController.EnemyAppear(enemies);
    }

    private void AppearReinforcements()
    {
        CharacterEnemy[] enemies = new CharacterEnemy[] { enemy2.GetComponent<CharacterEnemy>(), enemy3.GetComponent<CharacterEnemy>() };
        EventManager.Instance.EnemyAppear(enemies);
        tutorialRhythmGameController.EnemyAppear(enemies);
    }

    private void EnemyRetreat()
    {
        enemy1.SetActive(false);
        enemy2.SetActive(false);
        enemy3.SetActive(false);
    }

}
