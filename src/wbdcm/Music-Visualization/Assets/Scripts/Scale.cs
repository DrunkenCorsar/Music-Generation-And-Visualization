using AudioHelm;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public partial class MainScript : MonoBehaviour
{
    public class Scale
    {
        public Scale(List<int> notesInScale)
        {
            this.NotesInScale = notesInScale;
        }

        public List<int> NotesInScale { get; set; }
    }
}
