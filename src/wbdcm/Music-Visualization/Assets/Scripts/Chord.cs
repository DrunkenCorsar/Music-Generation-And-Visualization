using AudioHelm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public partial class MainScript : MonoBehaviour
{
    public class Chord
    {
        public Chord(List<int> notesInChord)
        {
            this.NotesInChord = notesInChord;
        }

        public List<int> NotesInChord { get; set; }
    }
}
