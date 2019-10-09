
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using System.IO;
using SwinGameSDK;

/// <summary>
/// Controls displaying and collecting high score data.
/// </summary>
/// <remarks>
/// Data is saved to a file.
/// </remarks>
static class HighScoreController
{
	private const int NameWidth = 3;

	private const int ScoresLeft = 490;
	/// <summary>
	/// The score structure is used to keep the name and
	/// score of the top players together.
	/// </summary>
	private struct Score : IComparable
	{
		public string name;

		public int value;
		/// <summary>
		/// Allows scores to be compared to facilitate sorting
		/// </summary>
		/// <param name="obj">the object to compare to</param>
		/// <returns>a value that indicates the sort order</returns>
		public int CompareTo(object obj)
		{
			if (obj is Score) {
				Score other = (Score)obj;

				return other.value - this.value;
			} else {
				return 0;
			}
		}
	}


	private static List<Score> _scores = new List<Score>();
	/// <summary>
	/// Loads the scores from the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNSSS
	/// 
	/// Where NNN is the name and SSS is the score
	/// </remarks>
	private static void LoadScores()
	{
		string filename = null;
		filename = SwinGame.PathToResource("highscores.txt");

		StreamReader input = default(StreamReader);
		input = new StreamReader(filename);

		//Read in the # of scores
		int numScores = 0;
		numScores = Convert.ToInt32(input.ReadLine());

		_scores.Clear();

		int i = 0;

		for (i = 1; i <= numScores; i++) {
			Score s = default(Score);
			string line = null;

			line = input.ReadLine();

			s.name = line.Substring(0, NameWidth);
			s.value = Convert.ToInt32(line.Substring(NameWidth));
			_scores.Add(s);
		}
		input.Close();
	}

	/// <summary>
	/// Saves the scores back to the highscores text file.
	/// </summary>
	/// <remarks>
	/// The format is
	/// # of scores
	/// NNNSSS
	/// 
	/// Where NNN is the name and SSS is the score
	/// </remarks>
	private static void SaveScores()
	{
		string filename = null;
		filename = SwinGame.PathToResource("highscores.txt");

		StreamWriter output = default(StreamWriter);
		output = new StreamWriter(filename);

		output.WriteLine(_scores.Count);

		foreach (Score s in _scores) {
			output.WriteLine(s.name + s.value);
		}

		output.Close();
	}

	/// <summary>
	/// Draws the high scores to the screen.
	/// </summary>
	public static void DrawHighScores()
	{
		const int ScoresHeading = 40;
		const int ScoresTop = 80;
		const int ScoreGap = 30;

		if (_scores.Count == 0)
			LoadScores();

		SwinGame.DrawText("   High Scores   ", Color.White, GameResources.GameFont("Courier"), ScoresLeft, ScoresHeading);

		//For all of the scores
		int i = 0;
		for (i = 0; i <= _scores.Count - 1; i++) {
			Score s = default(Score);

			s = _scores[i];

			//for scores 1 - 9 use 01 - 09
			if (i < 9) {
				SwinGame.DrawText(" " + (i + 1) + ":   " + s.name + "   " + s.value, Color.White, GameResources.GameFont("Courier"), ScoresLeft, ScoresTop + i * ScoreGap);
			} else {
				SwinGame.DrawText(i + 1 + ":   " + s.name + "   " + s.value, Color.White, GameResources.GameFont("Courier"), ScoresLeft, ScoresTop + i * ScoreGap);
			}
		}
	}

	/// <summary>
	/// Handles the user input during the top score screen.
	/// </summary>
	/// <remarks></remarks>
	public static void HandleHighScoreInput()
	{
		if (SwinGame.MouseClicked(MouseButton.LeftButton) || SwinGame.KeyTyped(KeyCode.vk_ESCAPE) || SwinGame.KeyTyped(KeyCode.vk_RETURN)) {
			GameController.EndCurrentState();
		}
	}

	/// <summary>
	/// Read the user's name for their highsSwinGame.
	/// </summary>
	/// <param name="value">the player's sSwinGame.</param>
	/// <remarks>
	/// This verifies if the score is a highsSwinGame.
	/// </remarks>
	public static void ReadHighScore(int value)
	{
		const int EntryTop = 500;

		if (_scores.Count == 0)
			LoadScores();

		//is it a high score
		if (value > _scores[_scores.Count - 1].value) {
			Score s = new Score();
			s.value = value;

			GameController.AddNewState(GameState.ViewingHighScores);

			int x = 0;
			x = ScoresLeft + SwinGame.TextWidth(GameResources.GameFont("Courier"), "Name: ");

			SwinGame.StartReadingText(Color.White, NameWidth, GameResources.GameFont("Courier"), x, EntryTop);

			//Read the text from the user
			while (SwinGame.ReadingText()) {
				SwinGame.ProcessEvents();

				UtilityFunctions.DrawBackground();
				DrawHighScores();
				SwinGame.DrawText("Name: ", Color.White, GameResources.GameFont("Courier"), ScoresLeft, EntryTop);
				SwinGame.RefreshScreen();
			}

			s.name = SwinGame.TextReadAsASCII();

			if (s.name.Length < 3) {
				s.name = s.name + new string(Convert.ToChar(" "), 3 - s.name.Length);
			}

			_scores.RemoveAt(_scores.Count - 1);
			_scores.Add(s);
			_scores.Sort();

			GameController.EndCurrentState();
		}
	}
}