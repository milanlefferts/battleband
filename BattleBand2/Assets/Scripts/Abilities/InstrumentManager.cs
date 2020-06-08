using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstrumentManager : MonoBehaviour
{
    // Allows remote access of unique instance
    public static InstrumentManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static InstrumentManager instance;

    public Dictionary<string, Instrument> instrumentDB = new Dictionary<string, Instrument>();
    public Sprite[] instrumentsGuitaristImages, instrumentsDrummerImages, instrumentsSingerImages;
    [HideInInspector]
    private Instrument[] instrumentsGuitarist, instrumentsDrummer, instrumentsSinger;
    public Instrument[][] instruments;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }
    }

    private void Start()
    {
        FillInstrumentDatabase();
        SetupInstruments();
    }

    private void AddInstrument(Instrument instrument)
    {
        instrumentDB.Add(instrument.name, instrument);
    }

    private void FillInstrumentDatabase()
    {
        instrumentDB.Clear();
        AddInstrument(new Instrument("Rusty Mic", "Soul Flare", "Single target. A fiery eruption of pure soul on a single enemy.", "How can a plastic mic even GET rusty?", instrumentsSingerImages[0], "Singer_Mic"));
        AddInstrument(new Instrument("Bone Caller", "Tinnitus", "All enemies. Unleash havoc upon your enemies' eardrums. Ouch!", "Made from real bone. As in, human bone.", instrumentsSingerImages[1], "Singer_Tambourine"));

        AddInstrument(new Instrument("Crummy Drumkit", "Thunderous Solo", "Rhythm attack. Hit all enemies with a jolt of lightning.", "Still better than a potlid!", instrumentsDrummerImages[0], "Drummer_DrumKit"));
        AddInstrument(new Instrument("Drunky Conga", "Sonic Boom", "Single target. Blast the wind out of an enemies sails, decreasing Attack.", "This conga has had a bit too much to drink last night.", instrumentsDrummerImages[1], "Drummer_Conga"));

        AddInstrument(new Instrument("Your Mom's Guitar", "Soothing Melody", "Party wide. Restore your allies' stamina, soothing their pain.", "Not sure if rust or feces..", instrumentsGuitaristImages[0], "Guitarist_Guitar"));
        AddInstrument(new Instrument("Chaos Riffer", "Crowd Pleaser", "Party wide. Increases everyone's attack through the power of MADNESS!", "Lacks power cables. Heart included.", instrumentsGuitaristImages[1], "Guitarist_Chaos"));
    }

    private void SetupInstruments()
    {
        instrumentsSinger = new Instrument[] { instrumentDB["Rusty Mic"], instrumentDB["Bone Caller"], instrumentDB["Bone Caller"] };
        instrumentsDrummer = new Instrument[] { instrumentDB["Crummy Drumkit"], instrumentDB["Drunky Conga"], instrumentDB["Drunky Conga"] };
        instrumentsGuitarist = new Instrument[] { instrumentDB["Your Mom's Guitar"], instrumentDB["Chaos Riffer"], instrumentDB["Chaos Riffer"] };
        instruments = new Instrument[][] { instrumentsSinger, instrumentsDrummer, instrumentsGuitarist };
    }

    public string ReturnSpriteSheet(int character, int instrument)
    {
        return instruments[character][instrument].sheet;
    }
}