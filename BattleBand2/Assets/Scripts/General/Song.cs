using UnityEngine;

public class Song  {
    public AudioClip audioFile;
    public string songId;
    public string songName;
    public int bpm;

    public Song (AudioClip sAudioFile, string sSongId, string sSongName, int sBpm)
    {
        audioFile = sAudioFile;
        songId = sSongId;
        songName = sSongName;
        bpm = sBpm;
    }

}
