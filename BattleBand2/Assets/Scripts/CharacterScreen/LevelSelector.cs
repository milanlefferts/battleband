using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelector : MonoBehaviour {

    public void SelectItem()
    {
        Time.timeScale = 1.0f; // return time to normal

        switch (name)
        {
            case "restartlevel":
                SceneController.Instance.LoadScene("Level_Rooftop");
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
            default:
                break;
        }
    }


}
