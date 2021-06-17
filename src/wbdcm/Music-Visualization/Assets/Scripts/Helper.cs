using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public partial class MainScript : MonoBehaviour
{
	/// <summary>
	/// Describes all user noteSurations
	/// </summary>
	public enum NoteDuration
	{
		Whole,
		Half,
		Quarter,
		Eighth,
		Sixteenth
	}

	/// <summary>
	/// Describes all used notegroups
	/// </summary>
	public enum NoteGroup
	{
		Main,
		Chord,
		Secondary
	}


	/// <summary>
	/// Describes all used standart scales
	/// </summary>
	public static List<Scale> StandartScales = new List<Scale>()
	{
		new Scale(new List<int>() { 0, 2, 4, 5, 7, 9, 11 } ), // major natural
		new Scale(new List<int>() { 0, 2, 3, 5, 7, 8, 10 } ), // minor natural
		new Scale(new List<int>() { 0, 2, 4, 5, 7, 8, 11 } ), // major harmonic
		new Scale(new List<int>() { 0, 2, 3, 5, 7, 8, 11 } ), // minor harmonic
		new Scale(new List<int>() { 0, 2, 4, 5, 7, 8, 10 } ), // major melodic
		new Scale(new List<int>() { 0, 2, 3, 5, 7, 9, 11 } )  // minor melodic
	};

	/// <summary>
	/// Describes standart scale type
	/// </summary>
	public enum ScaleType
	{
		MajorNatural,
		MinorNatural,
		MajorHarmonic,
		MinorHarmonic,
		MajorMelodic,
		MinorMelodic
	}

	/// <summary>
	/// All standart chordes used
	/// </summary>
	public static List<Chord> StandartChords = new List<Chord>()
	{
		new Chord(new List<int>() { 0, 4, 7 }), // major
		new Chord(new List<int>() { 0, 3, 7 }), // minor
		new Chord(new List<int>() { 0, 4, 8 }), // augmented
		new Chord(new List<int>() { 0, 3, 6 }), // diminished
		new Chord(new List<int>() { 0, 2, 7 }) // suspended
	};

	/// <summary>
	/// Names of chord types used
	/// </summary>
	public enum ChordType
    {
		Major,
		Minor,
		Augmented,
		Diminished,
		Suspended
    }

	/// <summary>
	/// List of chord progressions
	/// </summary>
	public static List<string> StandartProgressions = new List<string>()
	{
        "I-V-vi-IV",
        "I-III-VI-IV",
        "I-III-vi-IV",
        "I-I-vi-IV",
        "I-II-vi-IV",
        "I-IV-vi-IV",
		// -----------
		"vi-IV-I-V",
        "IV-I-V-vi",
        "V-vi-IV-I",
		// -----------
		"I-vi-IV-V",
		// -----------
		"I-IV-I-I-IV-IV-I-I-V-IV-I-V",
		// -----------
		"I-IV-V-I",
        "I-IV-V-IV",
        "I-V-IV-V",
		// -----------
		"I-VI-II-V"

        // "V-VI-V-II"
	};

	/// <summary>
	/// All note names
	/// </summary>
	public enum NoteName
	{
		C,
		Csh,
		D,
		Dsh,
		E,
		F,
		Fsh,
		G,
		Gsh,
		A,
		Ash,
		B
	}

	// TBD
	//public bool ScaleContainsNote()
	//{
	//	return false;
	//}

	/// <summary>
	/// Method that converts enum float duration to float value
	/// </summary>
	/// <param name="duration">Given parameter that represents note duration in enum value</param>
	/// <returns>Float interpratation of current duration</returns>
	public static float FloatDuration(NoteDuration duration)
    {
		float dur = 0f;
		switch (duration)
		{
			case NoteDuration.Whole:
				dur = 2f;
				break;
			case NoteDuration.Half:
				dur = 1f;
				break;
			case NoteDuration.Quarter:
				dur = 0.5f;
				break;
			case NoteDuration.Eighth:
				dur = 0.25f;
				break;
			case NoteDuration.Sixteenth:
				dur = 0.125f;
				break;
		}
		return dur;
	}
}
