using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SongManager : MonoBehaviour {
    // Allows remote access for unique instance
    public static SongManager Instance
    {
        get
        {
            return instance;
        }
    }
    private static SongManager instance;

    public static Dictionary<string, Song> songDB = new Dictionary<string, Song>();

    public AudioClip[] songs;

    void Awake()
    {
        instance = this;
        FillSongDatabase();
    }

    private void AddSong(Song song)
    {
        songDB.Add(song.songName, song);
    }

    private void FillSongDatabase()
    {
        songDB.Clear();

        // Placeholder Song
        AddSong(new Song(songs[0], "0_NecroDancer_Normal", "Necro", 115));
        AddSong(new Song(songs[1], "1_Synthwave_Slow", "Kalax", 108));
        AddSong(new Song(songs[2], "2_Synthwave_Medium", "Carp", 120));
        AddSong(new Song(songs[3], "3_Synthwave_MediumFast", "Waves", 125));
        AddSong(new Song(songs[4], "4_Synthwave_Fast", "Sloan", 130));
        AddSong(new Song(songs[5], "5_Chiptune_VeryFast", "Morse", 136));

        // Real Songs
        AddSong(new Song(songs[6], "6_Synth_Job", "Job1", 100));
        AddSong(new Song(songs[7], "7_SynthHiphop_Job", "Job2", 100));
        AddSong(new Song(songs[8], "8_Tubercrawler_Frank", "Frank1", 110));
        AddSong(new Song(songs[9], "9_Tubercrawler_Frank", "Frank2", 120));
        AddSong(new Song(songs[10], "10_Tubercrawler_Frank", "Frank3", 130));


    }

    public Song FindSong(string name)
    {
        return (songDB[name]);
    }

    public void PlaySong(string name)
    {
        Song tempSong = FindSong(name);
        EventManager.Instance.LoadSong(tempSong.audioFile, tempSong.bpm);
    }

    public void PlayRandomSong()
    {
        int rand = Random.Range(0, songDB.Count);
        string randKey = songDB.Keys.ElementAt(rand);
        Song randSong = songDB[randKey];
        EventManager.Instance.LoadSong(randSong.audioFile, randSong.bpm);
    }

}
