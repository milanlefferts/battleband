using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using General.ControllerInput;

public class DialogueController : MonoBehaviour {

    // Allows remote access for unique instance
    public static DialogueController Instance
    {
        get
        {
            return instance;
        }
    }
    private static DialogueController instance;

    private TextMeshProUGUI dialogueText;
    public GameObject dialogueRight, dialogueLeft;
    public Animator topPanel, bottomPanel;
    public GameObject dialogueArrow, skipText;

    private IEnumerator currentVoice, moveLips;

    private AudioSource audioSource;

    public Animator speakerRight, speakerLeft;
    public RuntimeAnimatorController[] animators; // Singer, Drummer, Guitarist, TheSuit
    public AudioSource music;
    private Animator speaker;

    public bool dialoguePlaying;
    Coroutine dialogueRoutine;

    private void OnEnable()
    {
        EventManager.DialogueEvent += StartDialogue;
    }

    private void OnDisable()
    {
        EventManager.DialogueEvent -= StartDialogue;
    }

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    
    private void Update()
    {
        if (CombatController.Instance != null)
        {
            if (dialoguePlaying && Input.GetKeyDown(CombatController.Instance.controllerInput.buttonUp) && !CombatController.Instance.tutorial)
            {
                CloseDialogue();
            }
        } else
        {
            // TODO: skip dialogue
        }

    }

    public void StartDialogue(string id)
    {

        // Look up dialogue (based on level and id)
        Dialogue tempDialogue = DialogueManager.Instance.FindDialogue(id);

        // Start dialogue
        dialogueRoutine = StartCoroutine(Dialogue(tempDialogue.speakers, tempDialogue.speeches, tempDialogue.voices));
    }

    public IEnumerator Dialogue(string[] spkr, string[] speech, AudioClip[] voice)
    {
        dialoguePlaying = true;

        if (CombatController.Instance != null)
            EventManager.Instance.PauseCombat();
        EventManager.Instance.DampenMusic();

        speaker = SetSpeaker(spkr[0]);
        speaker.SetBool("DialogueActive", true);

        topPanel.SetBool("DialogueActive", true);
        bottomPanel.SetBool("DialogueActive", true);

        dialogueArrow.SetActive(true);
        if (CombatController.Instance != null && !CombatController.Instance.tutorial)
            skipText.SetActive(true);

        int currentText = 0;

        yield return new WaitForSeconds(0.5f);

        while (currentText < speech.Length)
        {
            yield return null;

            speaker = SetSpeaker(spkr[currentText]);

            currentVoice = PlayVoice(voice[currentText]);

            StartCoroutine(currentVoice); // Play Voice
            moveLips = MoveLips(speaker); 

            dialogueText.text = speech[currentText]; // Set Text
            if (CombatController.Instance)
            {
                while (!(Input.GetKeyDown(CombatController.Instance.controllerInput.buttonLeft) && !CombatController.Instance.paused))
                {
                    yield return null;
                }
            } else
            {
                while (!(Input.GetKeyDown(CharacterScreenController.Instance.controllerInput.buttonLeft)))
                {
                    yield return null;
                }
            }


            dialogueText.text = "";
            StopCoroutine(moveLips); // Stop Lips
            audioSource.Stop(); // Stop Voice
            currentText++; 
        }

        CloseDialogue();
    }

    private void CloseDialogue()
    {
        SceneController.Instance.FadeInMusic();

        audioSource.Stop();

        speaker.SetBool("DialogueActive", false);
        topPanel.SetBool("DialogueActive", false);
        bottomPanel.SetBool("DialogueActive", false);
        dialogueText.text = ""; // Set Text
        dialogueArrow.SetActive(false);
        skipText.SetActive(false);

        StopCoroutine(dialogueRoutine);
        dialoguePlaying = false;

        if (CombatController.Instance)
        {
            if (CombatController.Instance.tutorial)
            {
                EventManager.Instance.TutorialNextPhase();
            }
            else
            {
                EventManager.Instance.ResumeCombat();
            }
        }
    }

    private IEnumerator PlayVoice(AudioClip sample)
    {
        if (sample == null)
            yield break;
        yield return null;
        audioSource.clip = sample;
        audioSource.Play();
        StartCoroutine(moveLips); // Move Lips

    }

    private IEnumerator MoveLips(Animator spkr)
    {
        spkr.SetBool("Talking", true);
        yield return new WaitUntil(() => !audioSource.isPlaying);
        spkr.SetBool("Talking", false);
    }

    private Animator SetSpeaker(string speaker)
    {
        Animator tempSpeaker;
        switch (speaker)
        {
            case "Singer": // Left
                tempSpeaker = speakerRight;
                SetSpeakerRight(0);
                break;
            case "Drummer": // Left
                tempSpeaker = speakerRight;
                SetSpeakerRight(1);
                break;
            case "Guitarist": // Left
                tempSpeaker = speakerRight;
                SetSpeakerRight(2);
                break;
            case "TheSuit": // Right
                tempSpeaker = speakerLeft;
                SetSpeakerLeft(3);
                break;
            case "DoggyGuard": // Right
                tempSpeaker = speakerLeft;
                SetSpeakerLeft(4);
                break;
            case "FameLeech": // Right
                tempSpeaker = speakerLeft;
                SetSpeakerLeft(5);
                break;
            default:
                tempSpeaker = speakerRight;
                break;
        }
        return tempSpeaker;
    }

    private void SetSpeakerRight(int pos)
    {
        if (speakerRight.runtimeAnimatorController != animators[pos])
            speakerRight.runtimeAnimatorController = animators[pos];
        dialogueRight.SetActive(true);
        dialogueLeft.SetActive(false);
        dialogueText = dialogueRight.GetComponentInChildren<TextMeshProUGUI>();
        speakerRight.SetBool("DialogueActive", true);
    }

    private void SetSpeakerLeft(int pos)
    {
        if (speakerLeft.runtimeAnimatorController != animators[pos])
            speakerLeft.runtimeAnimatorController = animators[pos];
        dialogueLeft.SetActive(true);
        dialogueRight.SetActive(false);
        dialogueText = dialogueLeft.GetComponentInChildren<TextMeshProUGUI>();
        speakerLeft.SetBool("DialogueActive", true);
    }
}