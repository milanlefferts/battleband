using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using General.ControllerInput;
using UnityEngine.UI;

public class CombatMenu : MonoBehaviour {

    public GameObject pauseMenu, endMenu;
    private bool opened;
    private ControllerInput controllerInput;

    public MenuItem currentMenuItem;
    private int menuItemNr;
    public MenuItem[] menuItems, menuItemsEnd;

    public Conductor conductor;

    private AudioSource audioSource;
    public AudioClip menuSelectSound, menuConfirmSound;

    private void OnEnable()
    {
        EventManager.EndGameEvent += EndGame;
    }

    private void OnDisable()
    {
        EventManager.EndGameEvent -= EndGame;
    }

    private void EndGame(bool irrelevant)
    {
        if (controllerInput == null)
        {
            controllerInput = CombatController.Instance.controllerInput;
        }
        currentMenuItem = menuItemsEnd[0];
        endMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }

    private void Start () {
        pauseMenu.SetActive(false);
        endMenu.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update () {

        if (Input.GetKeyDown(CombatController.Instance.controllerInput.buttonMenu) && !CombatController.Instance.gameEnded)
        {
            if (opened)
            {
                CloseMenu();
            } else
            {
                OpenMenu();
            }
        }

        if (pauseMenu.activeInHierarchy || endMenu.activeInHierarchy)
        {
            if (Input.GetKeyDown(controllerInput.buttonLeft))
            {
                audioSource.PlayOneShot(menuConfirmSound);

                currentMenuItem.combatMenu = this;
                currentMenuItem.SelectItem();
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
                if (CombatController.Instance.gameEnded)
                {
                    PreviousItemEnd();
                }
                else
                {
                    PreviousItem();
                }
                controllerInput.up = false;
            }
            else if (Input.GetKeyDown(controllerInput.dpadDown) || controllerInput.down)
            {
                audioSource.PlayOneShot(menuSelectSound);
                if (CombatController.Instance.gameEnded)
                {
                    NextItemEnd();
                }
                else
                {
                    NextItem();
                }
                controllerInput.down = false;
            }
        }
    }

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

    private void OpenMenu()
    {
        if (controllerInput == null)
        {
            controllerInput = CombatController.Instance.controllerInput;
        }

        pauseMenu.SetActive(true);
        PauseGame();
        opened = true;
    }

    public void CloseMenu()
    {
        pauseMenu.SetActive(false);
        opened = false;
        UnpauseGame();
    }

    private void PauseGame()
    {
        EventManager.Instance.PauseGame();
        Conductor.Instance.FadeoutSong();
        if (Time.timeScale == 1.0f)
            Time.timeScale = 0.0f;
    }

    private void UnpauseGame()
    {
        Conductor.Instance.FadeinSong();
        EventManager.Instance.PauseGame();
        if (Time.timeScale == 0.0f)
            Time.timeScale = 1.0f;
    }

    private void NextItemEnd()
    {
        currentMenuItem.UnhighlightItem();
        menuItemNr = (menuItemNr + 1) > menuItemsEnd.Length - 1 ? 0 : menuItemNr + 1;
        currentMenuItem = menuItemsEnd[menuItemNr];
        currentMenuItem.HighlightItem();
    }

    private void PreviousItemEnd()
    {
        currentMenuItem.UnhighlightItem();

        menuItemNr = (menuItemNr - 1) < 0 ? menuItemsEnd.Length - 1 : menuItemNr - 1;
        currentMenuItem = menuItemsEnd[menuItemNr];
        currentMenuItem.HighlightItem();
    }
}