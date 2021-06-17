using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public partial class MainScript : MonoBehaviour
{
    /// <summary>
    /// Class that represents whole theme stave
    /// </summary>
    public partial class Stave
    {
        int baseNote = -1;
        List<int> allowedTiles;
        List<Chord> progression;
        public Dictionary<int, float> NoteProbabilities { get; set; }

        /// <summary>
        /// Method that fills list of tiles and generates new theme by doing it
        /// </summary>
        private void GenerateTiles()
        {
            bool rightScale = false;
            int tries = 0;
            allowedTiles = new List<int>();

            while (!rightScale && tries < 100)
            {
                allowedTiles = new List<int>();

                ChooseScale();

                if (ChooseChordProgression())
                {
                    float currentTime = 2 * FloatDuration(NoteDuration.Whole);
                    float currentStep = FloatDuration(NoteDuration.Half);
                    int currentChordNum = 0;

                    int stairsMode = (new System.Random()).Next(0, 3);

                    for (int i = 0; i < 52; i++)
                    {
                        switch (stairsMode)
                        {
                            case 0:
                                for (int j = 0; j < progression[currentChordNum].NotesInChord.Count; j++)
                                {
                                    tiles.Add(new Tile(progression[currentChordNum].NotesInChord[j], NoteDuration.Half, currentTime, NoteGroup.Chord));
                                }
                                break;
                            case 1:
                                for (int j = 0; j < progression[currentChordNum].NotesInChord.Count; j++)
                                {
                                    tiles.Add(new Tile(progression[currentChordNum].NotesInChord[j], NoteDuration.Eighth, currentTime + (j * FloatDuration(NoteDuration.Eighth)), NoteGroup.Chord));
                                }
                                break;
                            default:
                                for (int j = 0; j < progression[currentChordNum].NotesInChord.Count + 1; j++)
                                {
                                    if (j < progression[currentChordNum].NotesInChord.Count)
                                    {
                                        tiles.Add(new Tile(progression[currentChordNum].NotesInChord[j], NoteDuration.Eighth, currentTime + (j * FloatDuration(NoteDuration.Eighth)), NoteGroup.Chord));
                                    }
                                    else
                                    {
                                        tiles.Add(new Tile(progression[currentChordNum].NotesInChord[j - 2], NoteDuration.Eighth, currentTime + (j * FloatDuration(NoteDuration.Eighth)), NoteGroup.Chord));
                                    }
                                }
                                break;
                        }

                        currentChordNum++;
                        if (currentChordNum >= progression.Count)
                        {
                            currentChordNum = 0;
                        }
                        currentTime += currentStep;
                    }

                    rightScale = true;

                    GenerateMelody();
                }
                else
                {

                }

                tries++;
                
                //-- COMMENT THIS IF WANNA GENERATE --
                // LoadTemplateMelody(); // <------------
                // -----------------------------------
            }
        }

        /// <summary>
        /// Method that generates new melody based on global variables from Stave and currently set variables in this class instance
        /// </summary>
        private void GenerateMelody()
        {
            var baseNoteMelody = baseNote + 28;

            if (NoteProbabilities == null)
            {
                NoteProbabilities = new Dictionary<int, float>();

                NoteProbabilities.Add(1, 0.2f);
                NoteProbabilities.Add(2, 0.2f);
                NoteProbabilities.Add(3, 0.2f);
                NoteProbabilities.Add(4, 0.2f);
                NoteProbabilities.Add(5, 0.2f);
            }

            List<int> coupletPoints = new List<int>();

            for (int i = 0; i < 4; i++)
            {
                coupletPoints.Add(GetClosestNoteInScale(baseNoteMelody + (new System.Random()).Next(0, 12)));
            }

            bool forwardStairing = (UnityEngine.Random.value > 0.5);
            float forwardChance = 0.6f;

            List<int> notesFirstLineTemplate = new List<int>();
            notesFirstLineTemplate.Add(coupletPoints[0]);

            if (forwardStairing)
            {
                forwardChance *= -1;
                forwardChance += 1;
            }

            int notesCount = (new System.Random()).Next(11, 15);
            for (int i = 1; i < notesCount; i++)
            {
                float currentIntervalDelta = UnityEngine.Random.value;

                float forwardDeltaBalancer = 1.0f;
                if (Math.Abs(coupletPoints[0] - notesFirstLineTemplate[i - 1]) > 8)
                {
                    float distanceFromBase = Math.Abs(coupletPoints[0] - notesFirstLineTemplate[i - 1]);
                    for (int yu = 0; yu < distanceFromBase - 8; yu++)
                    {
                        forwardDeltaBalancer *= 0.95f;
                    }
                }

                int currentinterval = 0;
                foreach (var j in NoteProbabilities)
                {
                    currentIntervalDelta -= j.Value;
                    if (currentIntervalDelta <= 0)
                    {
                        currentinterval = j.Key;
                        break;
                    }
                }

                int forwardMultiplier = 1;
                float realChance = forwardChance;
                if (notesFirstLineTemplate[i - 1] > coupletPoints[0])
                {
                    realChance *= forwardDeltaBalancer;
                }
                else
                {
                    realChance += (1 - realChance) * (1 - forwardDeltaBalancer);
                }

                if ((UnityEngine.Random.value > realChance))
                {
                    forwardMultiplier *= -1;
                }

                notesFirstLineTemplate.Add(notesFirstLineTemplate[i - 1] 
                    + forwardMultiplier * (currentinterval));
            }

            forwardChance *= -1;
            forwardChance += 1;

            List<int> notesSecondLineTemplate = new List<int>();
            notesSecondLineTemplate.Add(coupletPoints[1]);

            notesCount = (new System.Random()).Next(11, 15);
            for (int i = 1; i < notesCount; i++)
            {
                float currentIntervalDelta = UnityEngine.Random.value;

                float forwardDeltaBalancer = 1.0f;
                if (Math.Abs(coupletPoints[1] - notesSecondLineTemplate[i - 1]) > 8)
                {
                    float distanceFromBase = Math.Abs(coupletPoints[1] - notesSecondLineTemplate[i - 1]);
                    for (int yu = 0; yu < distanceFromBase - 8; yu++)
                    {
                        forwardDeltaBalancer *= 0.95f;
                    }
                }

                int currentinterval = 0;
                foreach (var j in NoteProbabilities)
                {
                    currentIntervalDelta -= j.Value;
                    if (currentIntervalDelta <= 0)
                    {
                        currentinterval = j.Key;
                        break;
                    }
                }

                int forwardMultiplier = 1;
                float realChance = forwardChance;
                if (notesSecondLineTemplate[i - 1] > coupletPoints[1])
                {
                    realChance *= forwardDeltaBalancer;
                }
                else
                {
                    realChance += (1 - realChance) * (1 - forwardDeltaBalancer);
                }

                if ((UnityEngine.Random.value > realChance))
                {
                    forwardMultiplier *= -1;
                }

                notesSecondLineTemplate.Add(notesSecondLineTemplate[i - 1]
                    + forwardMultiplier * (currentinterval));
            }

            List<int> notesThirdLineTemplate = notesFirstLineTemplate;
            int deltaInterval = coupletPoints[2] - coupletPoints[0];
            for (int i = 0; i < notesThirdLineTemplate.Count; i++)
            {
                notesThirdLineTemplate[i] += deltaInterval;
            }

            List<int> notesFourthLineTemplate = notesSecondLineTemplate;
            deltaInterval = coupletPoints[3] - coupletPoints[1];
            for (int i = 0; i < notesFourthLineTemplate.Count; i++)
            {
                notesFourthLineTemplate[i] += deltaInterval;
            }

            for (int j = 0; j < 2; j++)
            {
                float startingTime = (FloatDuration(NoteDuration.Half) * 8f)
                    + (j * (FloatDuration(NoteDuration.Half) * 32f));

                for (int i = 0; i < notesFirstLineTemplate.Count; i++)
                {
                    float probabilityCase = UnityEngine.Random.value;
                    if (probabilityCase > 0.3f)
                    {
                        tiles.Add(new Tile(notesFirstLineTemplate[i], NoteDuration.Eighth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                    }
                    else if (probabilityCase > 0.15f)
                    {
                        tiles.Add(new Tile(notesFirstLineTemplate[i], NoteDuration.Quarter,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        i++;
                    }
                    else
                    {
                        tiles.Add(new Tile(notesFirstLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        tiles.Add(new Tile(notesFirstLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth)) + FloatDuration(NoteDuration.Sixteenth)));
                    }
                }

                startingTime += FloatDuration(NoteDuration.Half) * 4f;
                for (int i = 0; i < notesSecondLineTemplate.Count; i++)
                {
                    float probabilityCase = UnityEngine.Random.value;
                    if (probabilityCase > 0.3f)
                    {
                        tiles.Add(new Tile(notesSecondLineTemplate[i], NoteDuration.Eighth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                    }
                    else if (probabilityCase > 0.15f)
                    {
                        tiles.Add(new Tile(notesSecondLineTemplate[i], NoteDuration.Quarter,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        i++;
                    }
                    else
                    {
                        tiles.Add(new Tile(notesSecondLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        tiles.Add(new Tile(notesSecondLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth)) + FloatDuration(NoteDuration.Sixteenth)));
                    }
                }

                startingTime += FloatDuration(NoteDuration.Half) * 4f;
                for (int i = 0; i < notesThirdLineTemplate.Count; i++)
                {
                    float probabilityCase = UnityEngine.Random.value;
                    if (probabilityCase > 0.3f)
                    {
                        tiles.Add(new Tile(notesThirdLineTemplate[i], NoteDuration.Eighth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                    }
                    else if (probabilityCase > 0.15f)
                    {
                        tiles.Add(new Tile(notesThirdLineTemplate[i], NoteDuration.Quarter,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        i++;
                    }
                    else
                    {
                        tiles.Add(new Tile(notesThirdLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        tiles.Add(new Tile(notesThirdLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth)) + FloatDuration(NoteDuration.Sixteenth)));
                    }
                }

                startingTime += FloatDuration(NoteDuration.Half) * 4f;
                for (int i = 0; i < notesFourthLineTemplate.Count; i++)
                {
                    float probabilityCase = UnityEngine.Random.value;
                    if (probabilityCase > 0.3f)
                    {
                        tiles.Add(new Tile(notesFourthLineTemplate[i], NoteDuration.Eighth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                    }
                    else if (probabilityCase > 0.15f)
                    {
                        tiles.Add(new Tile(notesFourthLineTemplate[i], NoteDuration.Quarter,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        i++;
                    }
                    else
                    {
                        tiles.Add(new Tile(notesFourthLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth))));
                        tiles.Add(new Tile(notesFourthLineTemplate[i], NoteDuration.Sixteenth,
                            startingTime + (i * FloatDuration(NoteDuration.Eighth)) + FloatDuration(NoteDuration.Sixteenth)));
                    }
                }
            }
        }

        /// <summary>
        /// Private method for easely getting closest note that is in our scale
        /// </summary>
        /// <param name="note">Number of note that shouldnt be in scale</param>
        /// <returns></returns>
        private int GetClosestNoteInScale(int note)
        {
            int noteRet = note;

            int i = 0;
            while (true)
            {
                noteRet += i;
                if (allowedTiles.Contains(noteRet))
                {
                    return noteRet;
                }

                noteRet -= 2 * i;
                if (allowedTiles.Contains(noteRet))
                {
                    return noteRet;
                }

                noteRet += i;
                i++;
            }
        }

        /// <summary>
        /// Method that choses scale
        /// </summary>
        private void ChooseScale()
        {
            var scaleType = UnityEngine.Random.Range(0, StandartScales.Count);
            var note = UnityEngine.Random.Range(0, 12);

            var allowedNotesInOneOctave = StandartScales[scaleType].NotesInScale;
            for (var i = 0; i < allowedNotesInOneOctave.Count; i++)
            {
                allowedNotesInOneOctave[i] += note + 3;
                allowedNotesInOneOctave[i] %= 12;
            }

            baseNote = 3 + note;

            var infoString = "Allowed notes at keyboard: ";
            for (int i = 0; i < TOTAL_KEYS_AT_KEYBOARD; i++)
            {
                if (allowedNotesInOneOctave.IndexOf(i % 12) != -1)
                {
                    allowedTiles.Add(i);
                    infoString += " " + i.ToString();
                }
            }

            Debug.Log("Chosen scale: " + Enum.GetName(typeof(NoteName), note) + " " + Enum.GetName(typeof(ScaleType), scaleType));
            Debug.Log(infoString);
            Debug.Log("Base note: " + baseNote);
        }

        /// <summary>
        /// Method that choses chord progression
        /// </summary>
        /// <returns></returns>
        private bool ChooseChordProgression()
        {
            bool progressionFound = false; 
            var counter = 10000;

            while (!progressionFound && counter > 0)
            {
                var currentProgression = StandartProgressions[UnityEngine.Random.Range(0, StandartProgressions.Count)];
                var chordsInProgressionString = new List<string>(currentProgression.Split('-'));
                var chordsInProgression = new List<Chord>();

                for (int i = 0; i < chordsInProgressionString.Count; i++)
                {
                    chordsInProgression.Add(new Chord(new List<int>(GetFromProgressionLabel(chordsInProgressionString[i]).NotesInChord)));
                }

                bool mistakeFound = false;
                for (int i = 0; i < chordsInProgression.Count; i++)
                {
                    for (int j = 0; j < chordsInProgression[i].NotesInChord.Count; j++)
                    {
                        if (allowedTiles.IndexOf(chordsInProgression[i].NotesInChord[j]) == -1)
                        {
                            mistakeFound = true;
                        }
                    }
                }

                if (!mistakeFound)
                {
                    progressionFound = true;
                    progression = chordsInProgression;
                    Debug.Log("Chords progression found: " + currentProgression);
                }
                else
                {
                    counter--;
                }
            }

            if (!progressionFound)
            {
                Debug.LogWarning("Can't find right chord progrression");
            }

            return progressionFound;
        }

        /// <summary>
        /// Method that parses chord label to real chord based on selected scale
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        private Chord GetFromProgressionLabel(string label)
        {
            int numberOfFirstNote = -1;
            switch (label)
            {
                case "I":
                case "i":
                    numberOfFirstNote = 0;
                        break;
                case "II":
                case "ii":
                    numberOfFirstNote = 1;
                    break;
                case "III":
                case "iii":
                    numberOfFirstNote = 2;
                    break;
                case "IV":
                case "iv":
                    numberOfFirstNote = 3;
                    break;
                case "V":
                case "v":
                    numberOfFirstNote = 4;
                    break;
                case "VI":
                case "vi":
                    numberOfFirstNote = 5;
                    break;
                case "VII":
                case "vii":
                    numberOfFirstNote = 6;
                    break;
            }
            if (numberOfFirstNote == -1)
            {
                Debug.LogError("There is no such chord as " + label);
            }

            int firstNote = baseNote;

            for (int i = 0; i < numberOfFirstNote; )
            {
                firstNote++;
                if (allowedTiles.IndexOf(firstNote) != -1)
                {
                    i++;
                }
            }

            int chordTypeIndex = -1;
            switch(label)
            {
                case "I": case "II": case "III": case "IV": case "V": case "VI": case "VII":
                    chordTypeIndex = (int)ChordType.Major;
                    break;
                case "i": case "ii": case "iii": case "iv": case "v": case "vi": case "vii":
                    chordTypeIndex = (int)ChordType.Minor;
                    break;
            }

            Chord chord = new Chord(new List<int>(StandartChords[chordTypeIndex].NotesInChord));
            for (int i = 0; i < chord.NotesInChord.Count; i++)
            {
                chord.NotesInChord[i] += firstNote;
            }
            return chord;
        }

        /// <summary>
        /// Method that harcodes prepared melody. Used for testing
        /// </summary>
        void LoadTemplateMelody()
        {
            tiles.Clear();

            tiles.Add(new Tile(22, NoteDuration.Whole, 3f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Whole, 3f, NoteGroup.Chord));


            tiles.Add(new Tile(18, NoteDuration.Half, 5f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Half, 5f, NoteGroup.Chord));

            tiles.Add(new Tile(17, NoteDuration.Half, 6f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Half, 6f, NoteGroup.Chord));

            // \\\

            tiles.Add(new Tile(22, NoteDuration.Whole, 7f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Whole, 7f, NoteGroup.Chord));


            tiles.Add(new Tile(18, NoteDuration.Half, 9f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Half, 9f, NoteGroup.Chord));


            tiles.Add(new Tile(17, NoteDuration.Half, 10f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Half, 10f, NoteGroup.Chord));

            // \\\

            tiles.Add(new Tile(22, NoteDuration.Whole, 11f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Whole, 11f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 11f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 11.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 11.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 11.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 12f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 12.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 12.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 12.75f));

            tiles.Add(new Tile(18, NoteDuration.Half, 13f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Half, 13f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 13f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 13.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 13.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 13.75f));

            tiles.Add(new Tile(17, NoteDuration.Half, 14f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Half, 14f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 14f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 14.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 14.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 14.75f));

            // \\\

            tiles.Add(new Tile(22, NoteDuration.Whole, 15f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Whole, 15f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 15f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 15.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 15.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 15.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 16f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 16.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 16.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 16.75f));

            tiles.Add(new Tile(18, NoteDuration.Half, 17f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Half, 17f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 17f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 17.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 17.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 17.75f));

            tiles.Add(new Tile(17, NoteDuration.Half, 18f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Half, 18f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 18f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 18.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 18.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 18.75f));

            // \\\

            tiles.Add(new Tile(22, NoteDuration.Quarter, 19f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 20f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 19f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 20f, NoteGroup.Chord));

            tiles.Add(new Tile(22, NoteDuration.Quarter, 19.5f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 20.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 19.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 20.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 19f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 19.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 19.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 19.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 20f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 20.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 20.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 20.75f));

            tiles.Add(new Tile(18, NoteDuration.Quarter, 21f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 21f, NoteGroup.Chord));
            tiles.Add(new Tile(18, NoteDuration.Quarter, 21.5f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 21.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 21f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 21.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 21.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 21.75f));

            tiles.Add(new Tile(17, NoteDuration.Quarter, 22f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 22f, NoteGroup.Chord));
            tiles.Add(new Tile(17, NoteDuration.Quarter, 22.5f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 22.5f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 22f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 22.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 22.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 22.75f));

            // \\\

            tiles.Add(new Tile(22, NoteDuration.Quarter, 23f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 24f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 23f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 24f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 23.5f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 24.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 23.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 24.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 23f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 23.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 23.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 23.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 24f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 24.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 24.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 24.75f));

            tiles.Add(new Tile(18, NoteDuration.Quarter, 25f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 25f, NoteGroup.Chord));
            tiles.Add(new Tile(18, NoteDuration.Quarter, 25.5f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 25.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 25.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 25.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 25.75f));

            tiles.Add(new Tile(17, NoteDuration.Quarter, 26f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 26f, NoteGroup.Chord));
            tiles.Add(new Tile(17, NoteDuration.Quarter, 26.5f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 26.5f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 26f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 26.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 26.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 26.75f));

            // \\\
            tiles.Add(new Tile(22, NoteDuration.Quarter, 27f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 28f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 27f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 28f, NoteGroup.Chord));

            tiles.Add(new Tile(22, NoteDuration.Quarter, 27.5f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 28.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 27.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 28.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 27f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 27.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 27.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 27.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 28f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 28.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 28.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 28.75f));

            tiles.Add(new Tile(18, NoteDuration.Quarter, 29f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 29f, NoteGroup.Chord));
            tiles.Add(new Tile(18, NoteDuration.Quarter, 29.5f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 29.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 29f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 29.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 29.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 29.75f));

            tiles.Add(new Tile(17, NoteDuration.Quarter, 30f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 30f, NoteGroup.Chord));
            tiles.Add(new Tile(17, NoteDuration.Quarter, 30.5f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 30.5f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 30f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 30.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 30.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 30.75f));

            tiles.Add(new Tile(49, NoteDuration.Eighth, 27f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 27.25f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 27.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 27.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 28f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 28.125f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 28.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 28.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 29f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 29.125f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 29.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 29.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 30f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 30.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 30.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 30.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 30.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 30.625f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 30.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 30.875f, NoteGroup.Secondary));

            //

            tiles.Add(new Tile(22, NoteDuration.Quarter, 31f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 32f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 31f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 32f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 31.5f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 32.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 31.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 32.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 31f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 31.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 31.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 31.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 32f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 32.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 32.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 32.75f));

            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 31f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 31.125f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 31.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 31.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 32f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 32.125f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 32.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 32.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 33f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 33.125f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 33.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 33.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 34f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 34.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 34.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 34.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 34.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 34.625f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 34.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 34.875f, NoteGroup.Secondary));

            tiles.Add(new Tile(18, NoteDuration.Quarter, 33f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 33f, NoteGroup.Chord));
            tiles.Add(new Tile(18, NoteDuration.Quarter, 33.5f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 33.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 33f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 33.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 33.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 33.75f));

            tiles.Add(new Tile(17, NoteDuration.Quarter, 34f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 34f, NoteGroup.Chord));
            tiles.Add(new Tile(17, NoteDuration.Quarter, 34.5f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 34.5f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 34f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 34.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 34.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 34.75f));

            // 

            tiles.Add(new Tile(22, NoteDuration.Quarter, 35f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 36f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 35f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 36f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 35.5f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 36.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 35.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 36.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 35f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 35.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 35.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 35.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 36f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 36.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 36.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 36.75f));

            tiles.Add(new Tile(49, NoteDuration.Eighth, 35f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 35.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 35.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 35.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 35.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 36f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 36.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 36.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 36.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 36.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 36.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 36.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 37f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 37.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 37.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 37.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 37.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 37.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 37.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 38f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 38.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 38.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 38.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 38.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 38.625f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 38.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 38.875f, NoteGroup.Secondary));

            tiles.Add(new Tile(18, NoteDuration.Quarter, 37f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 37f, NoteGroup.Chord));
            tiles.Add(new Tile(18, NoteDuration.Quarter, 37.5f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 37.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 37f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 37.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 37.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 37.75f));

            tiles.Add(new Tile(17, NoteDuration.Quarter, 38f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 38f, NoteGroup.Chord));
            tiles.Add(new Tile(17, NoteDuration.Quarter, 38.5f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 38.5f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 38f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 38.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 38.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 38.75f));

            // 

            tiles.Add(new Tile(22, NoteDuration.Quarter, 39f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 40f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 39f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 40f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 39.5f, NoteGroup.Chord));
            tiles.Add(new Tile(22, NoteDuration.Quarter, 40.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 39.5f, NoteGroup.Chord));
            tiles.Add(new Tile(10, NoteDuration.Quarter, 40.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 39f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 39.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 39.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 39.75f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 40f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 40.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 40.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 40.75f));

            tiles.Add(new Tile(49, NoteDuration.Eighth, 39f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 39.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 39.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 39.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 39.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 40f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 40.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 40.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 40.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 40.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 40.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 40.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 41f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 41.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 41.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 41.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 41.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 41.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 41.875f, NoteGroup.Secondary));
            tiles.Add(new Tile(46, NoteDuration.Sixteenth, 42f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 42.125f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 42.25f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 42.375f, NoteGroup.Secondary));
            tiles.Add(new Tile(48, NoteDuration.Sixteenth, 42.5f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 42.625f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 42.75f, NoteGroup.Secondary));
            tiles.Add(new Tile(49, NoteDuration.Sixteenth, 42.875f, NoteGroup.Secondary));

            tiles.Add(new Tile(18, NoteDuration.Quarter, 41f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 41f, NoteGroup.Chord));
            tiles.Add(new Tile(18, NoteDuration.Quarter, 41.5f, NoteGroup.Chord));
            tiles.Add(new Tile(6, NoteDuration.Quarter, 41.5f, NoteGroup.Chord));

            tiles.Add(new Tile(37, NoteDuration.Eighth, 41f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 41.25f));
            tiles.Add(new Tile(34, NoteDuration.Eighth, 41.5f));
            tiles.Add(new Tile(29, NoteDuration.Eighth, 41.75f));

            tiles.Add(new Tile(17, NoteDuration.Quarter, 42f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 42f, NoteGroup.Chord));
            tiles.Add(new Tile(17, NoteDuration.Quarter, 42.5f, NoteGroup.Chord));
            tiles.Add(new Tile(5, NoteDuration.Quarter, 42.5f, NoteGroup.Chord));

            tiles.Add(new Tile(39, NoteDuration.Eighth, 42f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 42.25f));
            tiles.Add(new Tile(36, NoteDuration.Eighth, 42.5f));
            tiles.Add(new Tile(37, NoteDuration.Eighth, 42.75f));

            // ---

            tiles.Add(new Tile(22, NoteDuration.Whole, 43f, NoteGroup.Chord));
            tiles.Add(new Tile(34, NoteDuration.Whole, 43f, NoteGroup.Chord));
            tiles.Add(new Tile(58, NoteDuration.Whole, 43f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Eighth, 43f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 43.25f));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 43.5f));
            tiles.Add(new Tile(41, NoteDuration.Eighth, 43.75f));
            tiles.Add(new Tile(49, NoteDuration.Eighth, 44f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 44.25f));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 44.5f));
            tiles.Add(new Tile(41, NoteDuration.Eighth, 44.75f));

            tiles.Add(new Tile(18, NoteDuration.Half, 45f, NoteGroup.Chord));
            tiles.Add(new Tile(30, NoteDuration.Half, 45f, NoteGroup.Chord));
            tiles.Add(new Tile(54, NoteDuration.Half, 45f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Eighth, 45f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 45.25f));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 45.5f));
            tiles.Add(new Tile(41, NoteDuration.Eighth, 45.75f));

            tiles.Add(new Tile(17, NoteDuration.Half, 46f, NoteGroup.Chord));
            tiles.Add(new Tile(29, NoteDuration.Half, 46f, NoteGroup.Chord));
            tiles.Add(new Tile(53, NoteDuration.Half, 46f, NoteGroup.Secondary));

            tiles.Add(new Tile(51, NoteDuration.Eighth, 46f));
            tiles.Add(new Tile(49, NoteDuration.Eighth, 46.25f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 46.5f));
            tiles.Add(new Tile(49, NoteDuration.Eighth, 46.75f));


            // \\\

            tiles.Add(new Tile(22, NoteDuration.Whole, 47f, NoteGroup.Chord));
            tiles.Add(new Tile(34, NoteDuration.Whole, 47f, NoteGroup.Chord));
            tiles.Add(new Tile(58, NoteDuration.Whole, 47f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Eighth, 47f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 47.25f));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 47.5f));
            tiles.Add(new Tile(41, NoteDuration.Eighth, 47.75f));
            tiles.Add(new Tile(49, NoteDuration.Eighth, 48f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 48.25f));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 48.5f));
            tiles.Add(new Tile(41, NoteDuration.Eighth, 48.75f));

            tiles.Add(new Tile(18, NoteDuration.Half, 49f, NoteGroup.Chord));
            tiles.Add(new Tile(30, NoteDuration.Half, 49f, NoteGroup.Chord));
            tiles.Add(new Tile(54, NoteDuration.Half, 49f, NoteGroup.Secondary));

            tiles.Add(new Tile(49, NoteDuration.Eighth, 49f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 49.25f));
            tiles.Add(new Tile(46, NoteDuration.Eighth, 49.5f));
            tiles.Add(new Tile(41, NoteDuration.Eighth, 49.75f));

            tiles.Add(new Tile(17, NoteDuration.Half, 50f, NoteGroup.Chord));
            tiles.Add(new Tile(29, NoteDuration.Half, 50f, NoteGroup.Chord));
            tiles.Add(new Tile(53, NoteDuration.Half, 50f, NoteGroup.Secondary));

            tiles.Add(new Tile(51, NoteDuration.Eighth, 50f));
            tiles.Add(new Tile(49, NoteDuration.Eighth, 50.25f));
            tiles.Add(new Tile(48, NoteDuration.Eighth, 50.5f));
            tiles.Add(new Tile(49, NoteDuration.Eighth, 50.75f));

            // \\\

            tiles.Add(new Tile(22, NoteDuration.Whole, 51f, NoteGroup.Chord));
            tiles.Add(new Tile(34, NoteDuration.Whole, 51f, NoteGroup.Chord));
            tiles.Add(new Tile(46, NoteDuration.Whole, 51f));
            tiles.Add(new Tile(58, NoteDuration.Whole, 51f, NoteGroup.Secondary));
        }
    }
}
