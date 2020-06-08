using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class InstrumentAnimationChanger : MonoBehaviour {

    public string spriteSheetName;
    public int characterNr;
    private string loadedSpriteSheetName;
    private SpriteRenderer sRenderer;
    private Sprite[] subSprites;
    private bool isLoaded;

    private void Start()
    {
        sRenderer = GetComponent<SpriteRenderer>();
        StartCoroutine(Starting());
    }

    IEnumerator Starting ()
    {
        yield return new WaitForSeconds(1f);
        spriteSheetName = InstrumentManager.Instance.ReturnSpriteSheet(characterNr, PlayerData.Instance.currentInstruments[characterNr]);
        LoadSpriteSheet();
    }

    private void OnEnable()
    {
        EventManager.InstrumentSwitchEvent += LoadSpriteSheet;
    }

    private void OnDisable()
    {
        EventManager.InstrumentSwitchEvent -= LoadSpriteSheet;
    }
    
    void LateUpdate () {
        if (!isLoaded)
            return;

        string spriteName = sRenderer.sprite.name;

        var newSprite = Array.Find(subSprites, item => item.name == spriteName);
        //var newSprite = InstrumentManager.Instance.ReturnSpriteSheet(characterNr, PlayerData.Instance.currentInstruments[characterNr]);
        if (newSprite != sRenderer.sprite)
        {
            sRenderer.sprite = newSprite;
        }
    } 

    private void LoadSpriteSheet()
    {
        spriteSheetName = InstrumentManager.Instance.ReturnSpriteSheet(characterNr, PartyController.Instance.currentInstrumentSet[characterNr]);
        //loadedSpriteSheetName = spriteSheetName;
        //subSprites = Resources.LoadAll<Sprite>("Sprites/Characters/" + spriteSheetName);
        subSprites = Resources.LoadAll<Sprite>("Sprites/Characters/" + spriteSheetName);
        if (!isLoaded)
            isLoaded = true;
    }
}