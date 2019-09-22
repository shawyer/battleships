
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
using SwinGameSDK;

/// <summary>
/// The DeploymentController controls the players actions
/// during the deployment phase.
/// </summary>
static class DeploymentController
{
	private const int ShipsTop = 98;
	private const int ShipsLeft = 20;
	private const int ShipsHeight = 90;

	private const int ShipsWidth = 300;
	private const int TopButtonsTop = 72;

	private const int TopReturnToMenu = 230;
	private const int ReturnToMenuWidth = 90;

	private const int TopButtonsHeight = 46;
	private const int PlayButtonLeft = 693;

	private const int PlayButtonWidth = 80;
	private const int UpDownButtonLeft = 410;

	private const int LeftRightButtonLeft = 350;
	private const int RandomButtonLeft = 547;

	private const int RandomButtonWidth = 51;

	private const int RandomButtonsWidth = 47;
	private const int TextOffset = 5;
	private static Direction _currentDirection = Direction.UpDown;

	private static ShipName _selectedShip = ShipName.Tug;
	/// <summary>
	/// Handles user input for the Deployment phase of the game.
	/// </summary>
	/// <remarks>
	/// Involves selecting the ships, deloying ships, changing the direction
	/// of the ships to add, randomising deployment, end then ending
	/// deployment
	/// </remarks>
	public static void HandleDeploymentInput()
	{
		if (SwinGame.KeyTyped(KeyCode.vk_ESCAPE)) {
			GameController.AddNewState(GameState.ViewingGameMenu);
		}

		if (SwinGame.KeyTyped(KeyCode.vk_UP) | SwinGame.KeyTyped(KeyCode.vk_DOWN)) {
			_currentDirection = Direction.UpDown;
		}
		if (SwinGame.KeyTyped(KeyCode.vk_LEFT) | SwinGame.KeyTyped(KeyCode.vk_RIGHT)) {
			_currentDirection = Direction.LeftRight;
		}

		if (SwinGame.KeyTyped(KeyCode.vk_r)) {
			GameController.HumanPlayer.RandomizeDeployment();
		}

		if (SwinGame.MouseClicked(MouseButton.LeftButton)) {
			ShipName selected = default(ShipName);
			selected = GetShipMouseIsOver();
			if (selected != ShipName.None) {
				_selectedShip = selected;
			} else {
				DoDeployClick();
			}

			if (GameController.HumanPlayer.ReadyToDeploy & UtilityFunctions.IsMouseInRectangle(PlayButtonLeft, TopButtonsTop, PlayButtonWidth, TopButtonsHeight)) {
				GameController.EndDeployment();
			} else if (UtilityFunctions.IsMouseInRectangle(UpDownButtonLeft, TopButtonsTop, RandomButtonsWidth, TopButtonsHeight)) {
				//_currentDirection = Direction.LeftRight;
                _currentDirection = Direction.UpDown;
			} else if (UtilityFunctions.IsMouseInRectangle(LeftRightButtonLeft, TopButtonsTop, RandomButtonsWidth, TopButtonsHeight)) {
				_currentDirection = Direction.LeftRight;
			} else if (UtilityFunctions.IsMouseInRectangle(RandomButtonLeft, TopButtonsTop, RandomButtonWidth, TopButtonsHeight)) {
				GameController.HumanPlayer.RandomizeDeployment();
				// order is int x 547, int y 72, int w 51, int h 46
			} else if (UtilityFunctions.IsMouseInRectangle(TopReturnToMenu, TopButtonsTop, ReturnToMenuWidth, TopButtonsHeight)) {
				GameController.SwitchState(GameController.GameState.ViewingMainMenu);
			}
		}
	}

	/// <summary>
	/// The user has clicked somewhere on the screen, check if its is a deployment and deploy
	/// the current ship if that is the case.
	/// </summary>
	/// <remarks>
	/// If the click is in the grid it deploys to the selected location
	/// with the indicated direction
	/// </remarks>
	private static void DoDeployClick()
	{
		Point2D mouse = default(Point2D);

		mouse = SwinGame.MousePosition();

		//Calculate the row/col clicked
		int row = 0;
		int col = 0;

        //row = Convert.ToInt32(Math.Floor((mouse.Y) / (UtilityFunctions.CELL_HEIGHT + UtilityFunctions.CELL_GAP)));
        row = Convert.ToInt32(Math.Floor((mouse.Y - UtilityFunctions.FieldTop) / (UtilityFunctions.CellHeight + UtilityFunctions.CellGap)));
        col = Convert.ToInt32(Math.Floor((mouse.X - UtilityFunctions.FieldLeft) / (UtilityFunctions.CellWidth + UtilityFunctions.CellGap)));

		if (row >= 0 & row < GameController.HumanPlayer.PlayerGrid.Height) {
			if (col >= 0 & col < GameController.HumanPlayer.PlayerGrid.Width) {
				//if in the area try to deploy
				try {
					GameController.HumanPlayer.PlayerGrid.MoveShip(row, col, _selectedShip, _currentDirection);
				} catch (Exception ex) {
					Audio.PlaySoundEffect(GameResources.GameSound("Error"));
					UtilityFunctions.Message = ex.Message;
				}
			}
		}
	}

    /// <summary>
    /// Draws the deployment screen showing the field and the ships
    /// that the player can deploy.
    /// </summary>
    public static void DrawDeployment()
    {
        UtilityFunctions.DrawField(GameController.HumanPlayer.PlayerGrid, GameController.HumanPlayer, true);

        //Draw the Left/Right and Up/Down buttons
        if (_currentDirection == Direction.LeftRight) {
            SwinGame.DrawBitmap(GameResources.GameImage("LeftRightButton"), LeftRightButtonLeft, TopButtonsTop);
            SwinGame.DrawText("U/D", Color.Gray, GameFont("Menu"), UpDownButtonLeft, TopButtonsTop);
            SwinGame.DrawText("L/R", Color.White, GameFont("Menu"), LeftRightButtonLeft, TopButtonsTop);
        } else {
            SwinGame.DrawBitmap(GameResources.GameImage("UpDownButton"), LeftRightButtonLeft, TopButtonsTop);
            SwinGame.DrawText("U/D", Color.White, GameFont("Menu"), UpDownButtonLeft, TopButtonsTop);
            SwinGame.DrawText("L/R", Color.Gray, GameFont("Menu"), LeftRightButtonLeft, TopButtonsTop);
        }

		//DrawShips
		foreach (ShipName sn in Enum.GetValues(typeof(ShipName))) {
			int i = 0;
			i = ((int)sn) - 1;
			if (i >= 0) {
				if (sn == _selectedShip) {
					SwinGame.DrawBitmap(GameResources.GameImage("SelectedShip"), ShipsLeft, ShipsTop + i * ShipsHeight);
					//    SwinGame.FillRectangle(Color.LightBlue, ShipsLeft, ShipsTop + i * ShipsHeight, ShipsWidth, ShipsHeight)
					//Else
					//    SwinGame.FillRectangle(Color.Gray, ShipsLeft, ShipsTop + i * ShipsHeight, ShipsWidth, ShipsHeight)
				}

				//SwinGame.DrawRectangle(Color.Black, ShipsLeft, ShipsTop + i * ShipsHeight, ShipsWidth, ShipsHeight)
				//SwinGame.DrawText(sn.ToString(), Color.Black, GameFont("Courier"), ShipsLeft + TextOffset, ShipsTop + i * ShipsHeight)

			}
		}

		if (GameController.HumanPlayer.ReadyToDeploy) {
			SwinGame.DrawBitmap(GameResources.GameImage("PlayButton"), PlayButtonLeft, TopButtonsTop);
			// SwinGame.FillRectangle(Color.LightBlue, PlayButtonLeft, PLAY_BUTTON_TOP, PLAY_BUTTON_WIDTH, PLAY_BUTTON_HEIGHT)
			// SwinGame.DrawText("PLAY", Color.Black, GameFont("Courier"), PlayButtonLeft + TEXT_OFFSET, PLAY_BUTTON_TOP)
		}

		UtilityFunctions.DrawMessage();
	}

	/// <summary>
	/// Gets the ship that the mouse is currently over in the selection panel.
	/// </summary>
	/// <returns>The ship selected or none</returns>
	private static ShipName GetShipMouseIsOver()
	{
		foreach (ShipName sn in Enum.GetValues(typeof(ShipName))) {
			int i = 0;
			i =((int)sn) - 1;

			if (UtilityFunctions.IsMouseInRectangle(ShipsLeft, ShipsTop + i * ShipsHeight, ShipsWidth, ShipsHeight)) {
				return sn;
			}
		}

		return ShipName.None;
	}
}
