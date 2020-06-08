using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using General.Colors;

public class MenuItem : MonoBehaviour {

    private Text thisText;
    public GameObject selectedArrow;
    public CombatMenu combatMenu;
    public Sprite selected, unselected;
    public Image thisImage;
    public Image arrowL, arrowR;

    private TextMeshProUGUI menuText;

    private void Start () {
        //thisText = GetComponent<Text>();
        menuText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void HighlightItem ()
    {
        menuText.color = Color.white;
        if (arrowL != null)
        {
            arrowL.color = Color.white;
            arrowR.color = Color.white;
        }
        if (selectedArrow != null)
            selectedArrow.SetActive(true);
    }

    public void UnhighlightItem ()
    {
        menuText.color = Colors.colorRed;
        if (arrowL != null)
        {
            arrowL.color = Colors.colorRed;
            arrowR.color = Colors.colorRed;
        }
        if (selectedArrow != null)
            selectedArrow.SetActive(false);
    }

    public void SelectItem ()
    {
        Time.timeScale = 1.0f; // return time to normal
    
        switch (name) {
            case "continue":
                if (combatMenu != null)
                    combatMenu.CloseMenu();
                break;
            case "restartlevel":
                SceneController.Instance.LoadScene("Level_Rooftop");
                break;
            case "characterscreen":
                SceneController.Instance.LoadScene("CharacterScreen");
                break;
            case "level1":
                SceneController.Instance.LoadScene("Level_Rooftop");
                break;
            case "tutorial":
                SceneController.Instance.LoadScene("Level_Tutorial");
                break;
            case "mainmenu":
                SceneController.Instance.LoadScene("MainMenu");
                break;
            case "exit":
                Application.Quit();
                break;
                
            // Levels
            case "TheGarage":
                SceneController.Instance.LoadScene("Level_Tutorial");
                break;
            case "TheRooftop":
                SceneController.Instance.LoadScene("Level_Rooftop");
                break;

            default:
                break;
        }
    }
}
