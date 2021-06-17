using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Policy;

public enum NoteDuration
{
	Whole,
	Half,
	Quarter,
	Eighth,
	Sixteenth
}

public class Tile
{
	int notePitchAbsolute;
	NoteDuration duration;
	float startsAt;
	bool determined;

	public Tile()
	{
		Determined = false;
		notePitchAbsolute = 0;
		duration = NoteDuration.Whole;
		startsAt = 0f;
	}

	public Tile(int notePitchAbsolute, NoteDuration duration, float startsAt)
    {
		this.notePitchAbsolute = notePitchAbsolute;
		this.duration = duration;
		this.startsAt = startsAt;

		if (notePitchAbsolute >= 0
			&& notePitchAbsolute <= 87
			&& duration != null
			&& startsAt > 0f)
			Determined = true;
		else
			Determined = false;
    }

    public bool Determined { get => determined; private set => determined = value; }
}
