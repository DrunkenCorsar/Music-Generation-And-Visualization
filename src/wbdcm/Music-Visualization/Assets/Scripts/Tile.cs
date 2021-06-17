using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public partial class MainScript : MonoBehaviour
{
	public class Tile
	{
		public bool Determined { get; private set; }
		public NoteDuration Duration { get; private set; }
		public int NotePitchAbsolute { get; private set; }
		public float FloatDuration { get; private set; }
		public NoteGroup Group { get; private set; }
		public float StartsAt { get; private set; }

		public Tile()
		{
			Determined = false;
			NotePitchAbsolute = 0;
			Duration = NoteDuration.Whole;
			StartsAt = 0f;
		}

		public Tile(int notePitchAbsolute, NoteDuration duration, float startsAt, NoteGroup group)
		{
			NotePitchAbsolute = notePitchAbsolute;
			Duration = duration;
			StartsAt = startsAt;
			Group = group;

			if (notePitchAbsolute >= 0
				&& notePitchAbsolute < TOTAL_KEYS_AT_KEYBOARD
				&& startsAt > 0f)
				Determined = true;
			else
				Determined = false;

			FloatDuration = FloatDuration(duration);
		}

		public Tile(int notePitchAbsolute, NoteDuration duration, float startsAt) :
			this(notePitchAbsolute, duration, startsAt, NoteGroup.Main) { }

		public bool IsThin
		{
			get
			{
				int tmpPitch = NotePitchAbsolute % 12;
				bool thin = false;
				if (new int[] { 1, 4, 6, 9, 11 }.Contains(tmpPitch))
					thin = true;
				return thin;
			}
		}

		public float DeltaYPosition
        {
			get
            {
				float delta = 0f;
				switch (Duration)
                {
					case NoteDuration.Whole:
						delta = 0f + (GRID_UNIT_HEIGHT / 2f);
						break;
					case NoteDuration.Half:
						delta = 0f;
						break;
					case NoteDuration.Quarter:
						delta = (-0.25f * GRID_UNIT_HEIGHT) + ((StartsAt % 1f) * GRID_UNIT_HEIGHT);
						break;
					case NoteDuration.Eighth:
						delta = ((-3f / 8f) * GRID_UNIT_HEIGHT) + ((StartsAt % 1f) * GRID_UNIT_HEIGHT);
						break;
					case NoteDuration.Sixteenth:
						delta = ((-7f / 16f) * GRID_UNIT_HEIGHT) + ((StartsAt % 1f) * GRID_UNIT_HEIGHT);
						break;
                }
				return delta;
            }
        }

		public float XPosition
        {
			get
            {
				float pos = 0f;
				if (!IsThin)
                {
					// всего 47 белых
					// -9.38 - первая белая
					// 9.38 - последняя белая
					int whiteNumber = 0;
					whiteNumber += (NotePitchAbsolute / 12) * 7;
					switch (NotePitchAbsolute % 12)
                    {
						case 0:
							whiteNumber += 1;
							break;
						case 2:
							whiteNumber += 2;
							break;
						case 3:
							whiteNumber += 3;
							break;
						case 5:
							whiteNumber += 4;
							break;
						case 7:
							whiteNumber += 5;
							break;
						case 8:
							whiteNumber += 6;
							break;
						case 10:
							whiteNumber += 7;
							break;
					}
					whiteNumber -= 1;
					pos = -9.38f + (whiteNumber * ((9.38f - (-9.38f)) / (46)));
                }
				else
                {
					// -9.17 - первая чёрная
					// 9.18 - последняя чёрная
					// всего чёрных - 46
					int blacknumber = 0;
					blacknumber += (NotePitchAbsolute / 12) * 7;
					switch (NotePitchAbsolute % 12)
                    {
						case 1:
							blacknumber += 1;
							break;
						case 4:
							blacknumber += 3;
							break;
						case 6:
							blacknumber += 4;
							break;
						case 9:
							blacknumber += 6;
							break;
						case 11:
							blacknumber += 7;
							break;
					}
					blacknumber -= 1;
					pos = -9.17f + (blacknumber * ((9.18f - (-9.17f)) / 45));
				}
				return pos;
            }
        }

		public bool BelongsToGrid(int gridNumber)
        {
			return (StartsAt >= gridNumber - 1 && StartsAt < gridNumber);
        }

		public bool IsPlayingAt(float time)
		{
			bool returned = false;
			if (StartsAt <= time && StartsAt + FloatDuration >= time)
				returned = true;
			return returned;
		}

		public string HighlightTemplateName
        {
			get
            {
				string name = "";
				switch (NotePitchAbsolute % 12)
                {
					case 3:
					case 8:
						name = "HighlightLeftTemplate";
						break;
					case 0:
					case 5:
					case 10:
						name = "HighlightMiddleTemplate";
						break;
					case 2:
					case 7:
						name = "HighlightRightTemplate";
						break;
					case 1:
					case 4:
					case 6:
					case 9:
					case 11:
						name = "HighlightThinTemplate";
						break;
				}
				if (NotePitchAbsolute == 0)
                {
					name = "HighlightLeftTemplate";
				}
				if (Group == NoteGroup.Chord)
					name += "Chord";
				else if (Group == NoteGroup.Secondary)
					name += "Secondary";
				return name;
            }
        }

		public string TileObjectTemplateName
		{
			get
			{
				string returned = "";
				if (IsThin)
				{
					switch (Duration)
					{
						case NoteDuration.Whole:
							returned = "NoteWholeTemplateThin";
							break;
						case NoteDuration.Half:
							returned = "NoteHalfTemplateThin";
							break;
						case NoteDuration.Quarter:
							returned = "NoteQuarterTemplateThin";
							break;
						case NoteDuration.Eighth:
							returned = "NoteEighthTemplateThin";
							break;
						case NoteDuration.Sixteenth:
							returned = "NoteSixteenthTemplateThin";
							break;
					}
				}
				else
				{
					switch (Duration)
					{
						case NoteDuration.Whole:
							returned = "NoteWholeTemplate";
							break;
						case NoteDuration.Half:
							returned = "NoteHalfTemplate";
							break;
						case NoteDuration.Quarter:
							returned = "NoteQuarterTemplate";
							break;
						case NoteDuration.Eighth:
							returned = "NoteEighthTemplate";
							break;
						case NoteDuration.Sixteenth:
							returned = "NoteSixteenthTemplate";
							break;
					}
				}
				if (Group == NoteGroup.Chord)
					returned += "Chord";
				else if (Group == NoteGroup.Secondary)
					returned += "Secondary";
				return returned;
			}
		}
	}
}