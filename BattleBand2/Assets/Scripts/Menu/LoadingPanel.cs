using UnityEngine;

public class LoadingPanel : MonoBehaviour {
    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    void Start () {
        SceneController.Instance.transitionScreen = GetComponent<Animator>();
    }
}