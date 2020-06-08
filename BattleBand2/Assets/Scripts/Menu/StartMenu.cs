using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.ControllerInput;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    private ControllerInput controllerInput;

    public MenuItem currentMenuItem;
    private int menuItemNr;
    public MenuItem[] menuItems;

    private bool opened;
    private bool confirmed;

    // Creator screen
    public GameObject creatorScreen;
    private bool creatorScreenClosed;

    // Opening screen
    private bool started;
    private bool starting;
    public Animator openingScreen, logo;

    // Sounds
    private AudioSource audioSource;
    public AudioClip menuSelectSound, menuConfirmSound, titleAnnouncerSound, riffSound, reverbSound;

    // lol
    public GameObject lights, menu;

    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    private void Start()
    {
        Cursor.visible = false;
        controllerInput = new ControllerInput();
        controllerInput.SetupControls();

        audioSource = GetComponent<AudioSource>();
        StartCoroutine(CreatorScreen());
    }

    private IEnumerator CreatorScreen()
    {
        yield return new WaitForSeconds(3f);
        audioSource.PlayOneShot(reverbSound); // Sound for "by Milan Lefferts" screen
        yield return new WaitForSeconds(2.5f);
        creatorScreen.GetComponent<Animator>().SetTrigger("FadeOut");
        creatorScreenClosed = true;
        audioSource.PlayOneShot(riffSound);
    }

    private IEnumerator OpenScreen()
    {
        audioSource.PlayOneShot(titleAnnouncerSound);
        logo.SetTrigger("Select");

        SongManager.Instance.PlaySong("Job1");

        openingScreen.SetTrigger("PressKey");

        yield return new WaitForSeconds(0.5f);
        lights.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        menu.SetActive(true);

        started = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(0);
        }

        if (controllerInput.hasGamepad)
        {
            controllerInput.DpadInputCheck();
        }

        if (!creatorScreenClosed)
        {
            return;
        }

        if (!starting & Input.anyKeyDown)
        {
            starting = true;
            StartCoroutine(OpenScreen());
        }

        if (!started || confirmed)
        {
            return;
        }

        if (Input.GetKeyDown(controllerInput.buttonLeft))
        {
            confirmed = true;
            StartCoroutine(ConfirmWait());
        }

        else if (Input.GetKeyDown(controllerInput.dpadLeft) || controllerInput.left)
        {
            controllerInput.left = false;
        }
        else if (Input.GetKeyDown(controllerInput.dpadRight) || controllerInput.right)
        {
            controllerInput.right = false;
        }
        else if (Input.GetKeyDown(controllerInput.dpadUp) || controllerInput.up)
        {
            audioSource.PlayOneShot(menuSelectSound);
            PreviousItem();
            controllerInput.up = false;
        }
        else if (Input.GetKeyDown(controllerInput.dpadDown) || controllerInput.down)
        {
            audioSource.PlayOneShot(menuSelectSound);
            NextItem();
            controllerInput.down = false;
        }
    }

    // # Menu Controls
    private void NextItem()
    {
        currentMenuItem.UnhighlightItem();
        menuItemNr = (menuItemNr + 1) > menuItems.Length - 1 ? 0 : menuItemNr + 1;
        currentMenuItem = menuItems[menuItemNr];
        currentMenuItem.HighlightItem();
    }

    private void PreviousItem()
    {
        currentMenuItem.UnhighlightItem();

        menuItemNr = (menuItemNr - 1) < 0 ? menuItems.Length - 1 : menuItemNr - 1;
        currentMenuItem = menuItems[menuItemNr];
        currentMenuItem.HighlightItem();
    }

    private IEnumerator ConfirmWait()
    {
        audioSource.PlayOneShot(menuConfirmSound);
        yield return new WaitForSeconds(0.5f);
        currentMenuItem.SelectItem();
    }
}