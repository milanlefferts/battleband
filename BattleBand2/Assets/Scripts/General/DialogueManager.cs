using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

    public static Dictionary<string, Dialogue> dialogueDB = new Dictionary<string, Dialogue>();

    public AudioClip[] garageIntro;
    public AudioClip[] rooftopIntro, rooftopOuttro;
    public AudioClip[] tutorial0, tutorial1, tutorial2, tutorial3, tutorial4, tutorial5, tutorial6, tutorial7, tutorial8, tutorial9;

    // Allows remote access for unique instance
    public static DialogueManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static DialogueManager instance;

    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        FillDialogueDatabase();
    }

    private void AddDialogue(Dialogue dialogue)
    {
        dialogueDB.Add(dialogue.id, dialogue);
    }

    private void FillDialogueDatabase()
    {
        dialogueDB.Clear();

        // Garage
        AddDialogue(new Dialogue("garage_intro", garageIntro_Speakers, garageIntro_Speeches, garageIntro));

        // Tutorial
        AddDialogue(new Dialogue("tutorial_0", speakersTutorial0, speechesTutorial0, tutorial0));
        AddDialogue(new Dialogue("tutorial_1", speakersTutorial1, speechesTutorial1, tutorial1));
        AddDialogue(new Dialogue("tutorial_2", speakersTutorial2, speechesTutorial2, tutorial2));
        AddDialogue(new Dialogue("tutorial_3", speakersTutorial3, speechesTutorial3, tutorial3));
        AddDialogue(new Dialogue("tutorial_4", speakersTutorial4, speechesTutorial4, tutorial4));
        AddDialogue(new Dialogue("tutorial_5", speakersTutorial5, speechesTutorial5, tutorial5));
        AddDialogue(new Dialogue("tutorial_6", speakersTutorial6, speechesTutorial6, tutorial6));
        AddDialogue(new Dialogue("tutorial_7", speakersTutorial7, speechesTutorial7, tutorial7));
        AddDialogue(new Dialogue("tutorial_8", speakersTutorial8, speechesTutorial8, tutorial8));
        AddDialogue(new Dialogue("tutorial_9", speakersTutorial9, speechesTutorial9, tutorial9));

        // Rooftop
        AddDialogue(new Dialogue("rooftop_intro", rooftopSpeakersIntro, rooftopSpeechesIntro, rooftopIntro));
        AddDialogue(new Dialogue("rooftop_outtro", rooftopSpeakersOuttro, rooftopSpeechesOuttro, rooftopOuttro));
    }

    public Dialogue FindDialogue(string id)
    {
        return (dialogueDB[id]);
    }

    // Garage
    private string[] garageIntro_Speakers = new string[] { "Guitarist", "Guitarist", "Guitarist", "Singer", "Drummer", "Guitarist" };
    private string[] garageIntro_Speeches = new string[] { "Welcome to the <color=#D10010>DEMO</color>, dude! ...Or dudette!", "The <color=#D10010>GARAGE</color> is where we hang back, rehearse, relax, you know. All that jazz.", " Or rock, for that matter. Or metal, or salsa. Or whatever we're into that week, man.", "Here you can choose a <color=#D10010>VENUES</color> for us to play, and change the <color=#D10010>DIFFICULTY</color> of the gigs.", "Also, you can select a bandmember with the shoulderbuttons to change their <color=#D10010>INSTRUMENTS</color> and attributes.", "Oh yeah, this game is NOT easy, so I suggest you play the <color=#D10010>TUTORIAL</color> first, man. See ya around!" };
    
    // Tutorial
    private string[] speakersTutorial0 = new string[] { "Guitarist", "Guitarist" };
    private string[] speechesTutorial0 = new string[] { "Wow! Wanna lay down some <color=blue>JAM</color>s?", "*Guitar sound*" };

    private string[] speakersTutorial1 = new string[] { "Guitarist", "Singer" };
    private string[] speechesTutorial1 = new string[] { "Radical! It seems we can generate some kind of <color=purple>POWER</color> if we <color=blue>JAM</color> on the beat!", "Let's try." };

    private string[] speakersTutorial2 = new string[] { "TheSuit", "TheSuit", "Drummer", "Singer" };
    private string[] speechesTutorial2 = new string[] { "Those chords.. is that MUSIC?!", "Lay down yer instruments, or face the consequences!.", "No way, suit! Show 'em what you got, Andre.", "I.. I can feel the <color=purple>POWER</color> of music surging through me!" };

    private string[] speakersTutorial3 = new string[] { "TheSuit" };
    private string[] speechesTutorial3 = new string[] { "You kids asked for it. I'll take care of you and this tainted free-spirited 'sound'." };

    private string[] speakersTutorial4 = new string[] { "Drummer", "Guitarist" };
    private string[] speechesTutorial4 = new string[] { "Ouch, that hurt!", "Don't worry guys, I've got you!" };

    private string[] speakersTutorial5 = new string[] { "Singer", "TheSuit" };
    private string[] speechesTutorial5 = new string[] { "Appreciated, thanks Max.", "Their music is too powerful. Assist me, minions!" };

    private string[] speakersTutorial6 = new string[] { "Drummer" };
    private string[] speechesTutorial6 = new string[] { "Ready? Let's charge it up." };

    private string[] speakersTutorial7 = new string[] { "Drummer", "Drummer" };
    private string[] speechesTutorial7 = new string[] { "I'll finish this!", "Let's drop the bass" };

    private string[] speakersTutorial8 = new string[] { "TheSuit", "TheSuit" };
    private string[] speechesTutorial8 = new string[] { "How can this be, I thought we banished those infernal <color=purple>Instruments</color>.", "I have to report this to Management!" };

    private string[] speakersTutorial9 = new string[] { "Guitarist", "Singer", "Drummer" };
    private string[] speechesTutorial9 = new string[] { "What's the suit talking about, man?", "No idea, but perhaps we should find out.", "Quickly, after them!" };
    
    // _____________________________________
    private string[] rooftopSpeakersIntro = new string[] { "Drummer", "Singer", "FameLeech", "FameLeech", "Guitarist", "Singer", "Guitarist", "FameLeech", "FameLeech", "Drummer", "FameLeech", "TheSuit", "DoggyGuard" };
    private string[] rooftopSpeechesIntro = new string[] { "Interesting.. How did we get up here with all our instruments?", "Probably magic or something. Just don't think about it.", "O. M. G.. Where did YOU come from, with THOSE outfits?", "Were you even on the guest list?", "No, but we're totally on the BEST list!", "Good one, Max.", "Thanks, dude!", "So are you guys, like, totally famous or something?", "Let's take a selfie!", "Eh, no thanks, creep.", "Oh. My. God. I can't even..", "What's going up there? Guards, check it out!", "Rrrrrr!" };

    private string[] rooftopSpeakersOuttro = new string[] { "TheSuit", "TheSuit", "TheSuit", "Singer", "Guitarist", "Drummer", "Singer", "Drummer", "Guitarist", "Guitarist", "Drummer", "Singer" };
    private string[] rooftopSpeechesOuttro = new string[] { "It's too late for ye. I already warned <color=red>Management</color>.", "They will never allow ye to find the rest of the <color=purple>Instruments</color>.", "I'll take their location to my grave.. *fainting*", "Maybe we should look for clues!", "Wow. What a coincidence! He dropped a note with a location on it.", "What villains keeps their plans in their front pocket?", "A forgetful one, perhaps?", "So, what does it say?", "'Make sure they don't get to the <color=red>Bodega</color>.'", "'They could free the <color=red>Salsa God</color> and ru-ruin our plans.'", "Isn't that a bit TOO obvious?", "We can't just end the game here now, can we? To the <color=red>Bodega</color>!" };
}