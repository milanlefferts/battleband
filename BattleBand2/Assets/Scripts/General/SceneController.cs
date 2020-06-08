using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class SceneController : MonoBehaviour
{
    public Animator transitionScreen;

    private bool isLoadingScene;

    public string currentScene = "Rooftop";  //TODO: Level specific

    private AsyncOperation async;

    public static SceneController Instance
    {
        get
        {
            return instance;

        }
    }
    public static SceneController instance;

    private float minLoadingDuration;
    public GameObject transitionScreenObject;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += LevelWasLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= LevelWasLoaded;

    }

    private void Awake()
    {
        if (!instance) // Singleton pattern
        {
            instance = this;
        } else
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        minLoadingDuration = 2f;
    }

    private void LevelWasLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject obj = Instantiate(transitionScreenObject);
        transitionScreen = obj.GetComponent<Animator>();
        obj.transform.SetAsLastSibling();
        //transitionScreen = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Animator>();
    }

    public void LoadScene(string sceneName)
    {
        if (isLoadingScene)
            return;

        isLoadingScene = true;
        PlayerData.Instance.Save();
        StartCoroutine(LoadSceneFlow(sceneName));
    }

    private IEnumerator LoadAsync(string sceneName)
    {
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        yield return async;
    }

    private IEnumerator LoadSceneFlow(string sceneName)
    {
        // Close screen
        transitionScreen.SetBool("CloseScreen", true);
        FadeOutMusic();
        yield return new WaitForSeconds(1f);

        // Load loading screen
        yield return SceneManager.LoadSceneAsync("LoadingScreen");

        yield return new WaitForSeconds(minLoadingDuration);

        // Load level async
        //yield return SceneManager.LoadSceneAsync(sceneName);
        //StartCoroutine(LoadAsync(sceneName));
        async = SceneManager.LoadSceneAsync(sceneName);
        async.allowSceneActivation = false;
        yield return async.isDone;

        // Fade loading animation
        StopLoading();

        yield return new WaitForSeconds(1f);

        async.allowSceneActivation = true; // Activates scene

        // Unload loading screen
        //SceneManager.UnloadSceneAsync("LoadingScreen");

        // Fade to new screen
        //transitionScreen.SetBool("CloseScreen", false);
        //yield return new WaitForSeconds(1f);
        isLoadingScene = false;
    }

    // Loading Events

    public static event Action FadeOutMusicEvent;
    public void FadeOutMusic()
    {
        FadeOutMusicEvent();
    }

    public static event Action FadeInMusicEvent;
    public void FadeInMusic()
    {
        FadeInMusicEvent();
    }

    public static event Action StopLoadingEvent;
    public void StopLoading()
    {
        StopLoadingEvent();
    }

}
