using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;
/// <summary>
/// This includes a number of utility methods for
/// drawing and interacting with the Mouse.
/// </summary>
static class UtilityFunctions
{
	public const int FieldTop = 122;
    public const int FieldLeft = 349;
	public const int FieldWidth = 418;

	public const int FieldHeight = 418;

	public const int MessageTop = 548;
	public const int CellWidth = 40;
	public const int CellHeight = 40;

	public const int CellGap = 2;

	public const int ShipGap = 3;
	private static readonly Color SmallSea = SwinGame.RGBAColor(6, 60, 94, 255);
	private static readonly Color SmallShip = Color.Gray;
	private static readonly Color SmallMiss = SwinGame.RGBAColor(1, 147, 220, 255);

	private static readonly Color SmallHit = SwinGame.RGBAColor(169, 24, 37, 255);
	private static readonly Color LargeSea = SwinGame.RGBAColor(6, 60, 94, 255);
	private static readonly Color LargeShip = Color.Gray;
	private static readonly Color LargeMiss = SwinGame.RGBAColor(1, 147, 220, 255);

	private static readonly Color LargeHit = SwinGame.RGBAColor(252, 2, 3, 255);
	private static readonly Color OutlineColor = SwinGame.RGBAColor(5, 55, 88, 255);
	private static readonly Color ShipFillColor = Color.Gray;
	private static readonly Color ShipOutlineColor = Color.White;

	private static readonly Color MessageColor = SwinGame.RGBAColor(2, 167, 252, 255);
	public const int AnimationCells = 7;

	public const int FramesPerCell = 8;
	/// <summary>
	/// Determines if the mouse is in a given rectangle.
	/// </summary>
	/// <param name="x">the x location to check</param>
	/// <param name="y">the y location to check</param>
	/// <param name="w">the width to check</param>
	/// <param name="h">the height to check</param>
	/// <returns>true if the mouse is in the area checked</returns>
	public static bool IsMouseInRectangle(int x, int y, int w, int h)
	{
		Point2D mouse = default(Point2D);
		bool result = false;

		mouse = SwinGame.MousePosition();

		//if the mouse is inline with the button horizontally
		if (mouse.X >= x & mouse.X <= x + w) {
			//Check vertical position
			if (mouse.Y >= y & mouse.Y <= y + h) {
				result = true;
			}
		}

		return result;
	}

	/// <summary>
	/// Draws a large field using the grid and the indicated player's ships.
	/// </summary>
	/// <param name="grid">the grid to draw</param>
	/// <param name="thePlayer">the players ships to show</param>
	/// <param name="showShips">indicates if the ships should be shown</param>
	public static void DrawField(ISeaGrid grid, Player thePlayer, bool showShips)
	{
		DrawCustomField(grid, thePlayer, false, showShips, FieldLeft, FieldTop, FieldWidth, FieldHeight, CellWidth, CellHeight,
		CellGap);
	}

	/// <summary>
	/// Draws a small field, showing the attacks made and the locations of the player's ships
	/// </summary>
	/// <param name="grid">the grid to show</param>
	/// <param name="thePlayer">the player to show the ships of</param>
	public static void DrawSmallField(ISeaGrid grid, Player thePlayer)
	{
		const int SmallFieldLeft = 39;
		const int SmallFieldTop = 373;
		const int SmallFieldWidth = 166;
		const int SmallFieldHeight = 166;
		const int SmallFieldCellWidth = 13;
		const int SmallFieldCellHeight = 13;
		const int SmallFieldCellGap = 4;

		DrawCustomField(grid, thePlayer, true, true, SmallFieldLeft, SmallFieldTop, SmallFieldWidth, SmallFieldHeight, SmallFieldCellWidth, SmallFieldCellHeight,
		SmallFieldCellGap);
	}

	/// <summary>
	/// Draws the player's grid and ships.
	/// </summary>
	/// <param name="grid">the grid to show</param>
	/// <param name="thePlayer">the player to show the ships of</param>
	/// <param name="small">true if the small grid is shown</param>
	/// <param name="showShips">true if ships are to be shown</param>
	/// <param name="left">the left side of the grid</param>
	/// <param name="top">the top of the grid</param>
	/// <param name="width">the width of the grid</param>
	/// <param name="height">the height of the grid</param>
	/// <param name="cellWidth">the width of each cell</param>
	/// <param name="cellHeight">the height of each cell</param>
	/// <param name="cellGap">the gap between the cells</param>
	private static void DrawCustomField(ISeaGrid grid, Player thePlayer, bool small, bool showShips, int left, int top, int width, int height, int cellWidth, int cellHeight,
	int cellGap)
	{
		//SwinGame.FillRectangle(Color.Blue, left, top, width, height)

		int rowTop = 0;
		int colLeft = 0;

		//Draw the grid
		for (int row = 0; row <= 9; row++) {
			rowTop = top + (cellGap + cellHeight) * row;

			for (int col = 0; col <= 9; col++) {
				colLeft = left + (cellGap + cellWidth) * col;

				Color fillColor = default(Color);
				bool draw = false;

				draw = true;

				switch (grid[row, col]) {
					//case TileView.Ship:
					//	draw = false;
					//	break;
					//If small Then fillColor = _SMALL_SHIP Else fillColor = _LARGE_SHIP
					case TileView.Miss:
						if (small)
							fillColor = SmallMiss;
						else
							fillColor = LargeMiss;
						break;
					case TileView.Hit:
						if (small)
							fillColor = SmallHit;
						else
							fillColor = LargeHit;
						break;
					case TileView.Sea:
					case TileView.Ship:
						if (small)
							fillColor = SmallSea;
						else
							draw = false;
						break;
				}

				if (draw) {
					SwinGame.FillRectangle(fillColor, colLeft, rowTop, cellWidth, cellHeight);
					if (!small) {
						SwinGame.DrawRectangle(OutlineColor, colLeft, rowTop, cellWidth, cellHeight);
					}
				}
			}
		}

		if (!showShips) {
			return;
		}

		int shipHeight = 0;
		int shipWidth = 0;
		string shipName = null;

		//Draw the ships
		foreach (Ship s in thePlayer) {
			if (s == null || !s.IsDeployed)
				continue;
			rowTop = top + (cellGap + cellHeight) * s.Row + ShipGap;
			colLeft = left + (cellGap + cellWidth) * s.Column + ShipGap;

			if (s.Direction == Direction.LeftRight) {
				shipName = "ShipLR" + s.Size;
				shipHeight = cellHeight - (ShipGap * 2);
				shipWidth = (cellWidth + cellGap) * s.Size - (ShipGap * 2) - cellGap;
			} else {
				//Up down
				shipName = "ShipUD" + s.Size;
				shipHeight = (cellHeight + cellGap) * s.Size - (ShipGap * 2) - cellGap;
				shipWidth = cellWidth - (ShipGap * 2);
			}

			if (!small) {
				SwinGame.DrawBitmap(GameResources.GameImage(shipName), colLeft, rowTop);
			} else {
				SwinGame.FillRectangle(ShipFillColor, colLeft, rowTop, shipWidth, shipHeight);
				SwinGame.DrawRectangle(ShipOutlineColor, colLeft, rowTop, shipWidth, shipHeight);
			}
		}
	}


	private static string _message;
	/// <summary>
	/// The message to display
	/// </summary>
	/// <value>The message to display</value>
	/// <returns>The message to display</returns>
	public static string Message {
		get { return _message; }
		set { _message = value; }
	}

	/// <summary>
	/// Draws the message to the screen
	/// </summary>
	public static void DrawMessage()
	{
		SwinGame.DrawText(Message, MessageColor, GameResources.GameFont("Courier"), FieldLeft, MessageTop);
	}

	/// <summary>
	/// Draws the background for the current state of the game
	/// </summary>

	public static void DrawBackground()
	{
		switch (GameController.CurrentState) {
			case GameState.ViewingMainMenu:
			case GameState.ViewingGameMenu:
			case GameState.AlteringSettings:
			case GameState.ViewingHighScores:
				SwinGame.DrawBitmap(GameResources.GameImage("Menu"), 0, 0);
				break;
			case GameState.Discovering:
			case GameState.EndingGame:
				SwinGame.DrawBitmap(GameResources.GameImage("Discovery"), 0, 0);
				break;
			case GameState.Deploying:
				SwinGame.DrawBitmap(GameResources.GameImage("Deploy"), 0, 0);
				break;
			default:
				SwinGame.ClearScreen();
				break;
		}

		SwinGame.DrawFramerate(675, 585, GameResources.GameFont("CourierSmall"));
	}

	public static void AddExplosion(int row, int col)
	{
		AddAnimation(row, col, "Splash");
	}

	public static void AddSplash(int row, int col)
	{
		AddAnimation(row, col, "Splash");
	}


	private static List<Sprite> _Animations = new List<Sprite>();
	private static void AddAnimation(int row, int col, string image)
	{
		Sprite s = default(Sprite);
		Bitmap imgObj = default(Bitmap);

		imgObj = GameResources.GameImage(image);
		imgObj.SetCellDetails(40, 40, 3, 3, 7);

		AnimationScript animation = default(AnimationScript);
		animation = SwinGame.LoadAnimationScript("splash.txt");

		s = SwinGame.CreateSprite(imgObj, animation);
		s.X = FieldLeft + col * (CellWidth + CellGap);
		s.Y = FieldTop + row * (CellHeight + CellGap);

		s.StartAnimation("splash");
		_Animations.Add(s);
	}

	public static void UpdateAnimations()
	{
		List<Sprite> ended = new List<Sprite>();
		foreach (Sprite s in _Animations) {
			SwinGame.UpdateSprite(s);
			if (s.AnimationHasEnded) {
				ended.Add(s);
			}
		}

		foreach (Sprite s in ended) {
			_Animations.Remove(s);
			SwinGame.FreeSprite(s);
		}
	}

	public static void DrawAnimations()
	{
		foreach (Sprite s in _Animations) {
			SwinGame.DrawSprite(s);
		}
	}

	public static void DrawAnimationSequence()
	{
		int i = 0;
		for (i = 1; i <= AnimationCells * FramesPerCell; i++) {
			UpdateAnimations();
			GameController.DrawScreen();
		}
	}
}