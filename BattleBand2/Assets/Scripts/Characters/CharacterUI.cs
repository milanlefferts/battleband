using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(CharacterFriendly))]
public class CharacterUI : MonoBehaviour {

    private CharacterFriendly character;
    public Image abilityIcon1, abilityIcon2, inputIcon;
    public GameObject musicNote;
    public Transform notePosition, noteParent;


    public TextMeshProUGUI characterName;
    private int currentInstrument;
    private Animator icon1anim, icon2anim;

    private void OnEnable()
    {
        EventManager.GetInputIconEvent += SetupUI;
        EventManager.InstrumentSwitchEvent += SwitchAbilities;
        EventManager.AbilityUsedEvent += SpawnNotes2;
        EventManager.NoteHitEvent += SpawnNotes;

    }

    private void OnDisable()
    {
        EventManager.GetInputIconEvent -= SetupUI;
        EventManager.InstrumentSwitchEvent -= SwitchAbilities;
        EventManager.AbilityUsedEvent -= SpawnNotes2;
        EventManager.NoteHitEvent -= SpawnNotes;
    }

    private void Start () {
        icon1anim = abilityIcon1.GetComponent<Animator>();
        icon2anim = abilityIcon2.GetComponent<Animator>();

        character = GetComponent<CharacterFriendly>();
    }

    public void StartCharacterUI ()
    {
        icon1anim.SetInteger("ColorBW", 1);
        icon2anim.SetInteger("ColorBW", 0);
    }

    private void SetupUI (Sprite[] sprites)
    {
        if (character == null)
        {
            character = GetComponent<CharacterFriendly>();
            icon1anim = abilityIcon1.GetComponent<Animator>();
            icon2anim = abilityIcon2.GetComponent<Animator>();
        }

        inputIcon.sprite = sprites[character.characterNumber];
        characterName.text = character.characterName;
        abilityIcon1.sprite = PartyController.Instance.abilitySet1[character.characterNumber].icon;
        abilityIcon1.SetNativeSize();
        icon1anim.SetInteger("ColorBW", 1);
        abilityIcon2.sprite = PartyController.Instance.abilitySet2[character.characterNumber].icon;
        abilityIcon2.SetNativeSize();
        icon2anim.SetInteger("ColorBW", 0);
    }

    private void SwitchAbilities()
    {
        if (!CombatController.Instance.combat)
            return;

        if (PartyController.Instance.abilitySet == 0)
        {
            icon1anim.SetInteger("ColorBW", 1);
            icon2anim.SetInteger("ColorBW", 0);
        }
        else
        {
            icon1anim.SetInteger("ColorBW", 0);
            icon2anim.SetInteger("ColorBW", 1);
        }
    }

    private void SpawnNotes ()
    {

        var wantedPos = Camera.main.WorldToScreenPoint(notePosition.position);

        int nr = Random.Range(1, 2); // defaults to 1 now
        for (int i = 0; i < nr; i++)
        {
            GameObject tempNote = Instantiate(musicNote, wantedPos, Quaternion.identity);
            tempNote.transform.SetParent(noteParent, true);
        }


        //musicNote.SetActive(true())
    }

    private void SpawnNotes2(string str)
    {
        if (str == "Switch")
            return;

        SpawnNotes();
    }
}