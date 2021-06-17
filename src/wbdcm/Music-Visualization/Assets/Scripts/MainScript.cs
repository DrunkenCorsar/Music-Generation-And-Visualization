using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;
using SFB;
using MidiParser;
using System;

public partial class MainScript : MonoBehaviour
{
    public AudioHelm.HelmController helmController;
    public bool playing = false;
    public Sprite iconPause;
    public Sprite iconPlay;

    private Dictionary<int, int> noteCounts;
    private Dictionary<int, float> noteProbabilities;

    Stave stave;

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        stave = new Stave(helmController);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (playing)
        {
            stave.Move(Time.deltaTime);
        }
    }

    /// <summary>
    /// Mathod that updates general volume in scene
    /// </summary>
    /// <param name="slider">Reference to UnitySlider in scene, from which we supposted to take new value.</param>
    public void ChangeVolume(Slider slider)
    {
        GameObject synthesizer = GameObject.Find("Synthesizer");
        AudioSource audS = synthesizer.GetComponent<AudioSource>();
        audS.volume = slider.value;
    }

    /// <summary>
    /// Method that pauses ot resumes playing current melody. Action depends on current playing state
    /// </summary>
    public void PauseOrResume()
    {
        playing = !playing;

        GameObject startButton = GameObject.Find("StartButton");
        Image startButtonImage = startButton.GetComponent(typeof(Image)) as Image;

        if (playing)
        {
            startButtonImage.sprite = iconPause;
        }
        else
            startButtonImage.sprite = iconPlay;
    }

    /// <summary>
    /// Updates duration of next created theme
    /// </summary>
    public void ChangeDuration()
    {
        stave.ChangeLoopDuration();
    }

    /// <summary>
    /// Uploades MIDI or MID file that user will select via File Browser.
    /// </summary>
    public void UploadMidiFile()
    {
        var extensions = new[] {
            new ExtensionFilter("MIDI files", "midi", "mid" ),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);
        Debug.Log("EXTRACTED PATH: " + paths[0]);

        string songPath = paths[0];

        const int CMaxInterval = 12;

        noteCounts = new Dictionary<int, int>();
        noteProbabilities = new Dictionary<int, float>();

        for (int i = 1; i <= CMaxInterval; i++)
        {
            noteCounts.Add(i, 0);
            noteProbabilities.Add(i, 0f);
        }

        var midiFile = new MidiFile(songPath);

        int lastNote = 0;
        int countNotes = 0;

        foreach (var track in midiFile.Tracks)
        {
            Debug.Log("TRACK ");
            foreach (var midiEvent in track.MidiEvents)
            {
                if (midiEvent.MidiEventType == MidiEventType.NoteOn)
                {
                    var channel = midiEvent.Channel;
                    var note = midiEvent.Note;
                    var velocity = midiEvent.Velocity;

                    Debug.Log("CHANNEL " + channel + "  NOTE " + note + "  VELOCITY " + velocity + "  TIME " + midiEvent.Time);

                    int currentInterval = Math.Abs(lastNote - note);
                    if (currentInterval > 0 && currentInterval <= CMaxInterval)
                    {
                        countNotes++;
                        noteCounts[currentInterval]++;
                    }

                    lastNote = note;
                }
            }
        }

        for (int i = 1; i <= CMaxInterval; i++)
        {
            noteProbabilities[i] = (float)noteCounts[i] / countNotes;
        }

        stave.NoteProbabilities = noteProbabilities;
    }

    /// <summary>
    /// Update music playing speed
    /// </summary>
    /// <param name="slider">Is reference to slider in the scene</param>
    public void ChangeSpeed(Slider slider)
    {
        stave.ChangeSpeed(slider.value);
    }

    /// <summary>
    /// Resets all of objects at scene to their initial places, regenerates theme
    /// </summary>
    public void ResetGame()
    {
        playing = false;
        GameObject startButton = GameObject.Find("StartButton");
        Image startButtonImage = startButton.GetComponent(typeof(Image)) as Image;
        startButtonImage.sprite = iconPlay;
        stave.Reset();
    }
}
