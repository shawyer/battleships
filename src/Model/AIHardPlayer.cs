
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
/// <summary>
/// AIHardPlayer is a type of player. This AI will know directions of ships
/// when it has found 2 ship tiles and will try to destroy that ship. If that ship
/// is not destroyed it will shoot the other way. Ship still not destroyed, then
/// the AI knows it has hit multiple ships. Then will try to destoy all around tiles
/// that have been hit.
/// </summary>
public class AIHardPlayer : AIPlayer
{

	/// <summary>
	/// Target allows the AI to know more things, for example the source of a
	/// shot target
	/// </summary>
	protected class Target
	{
		private readonly Location _shotAt;

		private readonly Location _source;
		/// <summary>
		/// The target shot at
		/// </summary>
		/// <value>The target shot at</value>
		/// <returns>The target shot at</returns>
		public Location ShotAt {
			get { return _shotAt; }
		}

		/// <summary>
		/// The source that added this location as a target.
		/// </summary>
		/// <value>The source that added this location as a target.</value>
		/// <returns>The source that added this location as a target.</returns>
		public Location Source {
			get { return _source; }
		}

		internal Target(Location shootat, Location source)
		{
			_shotAt = shootat;
			_source = source;
		}

		/// <summary>
		/// If source shot and shootat shot are on the same row then 
		/// give a boolean true
		/// </summary>
		public bool SameRow {
			get { return _shotAt.Row == _source.Row; }
		}

		/// <summary>
		/// If source shot and shootat shot are on the same column then 
		/// give a boolean true 
		/// </summary>
		public bool SameColumn {
			get { return _shotAt.Column == _source.Column; }
		}
	}

	/// <summary>
	/// Private enumarator for AI states. currently there are two states,
	/// the AI can be searching for a ship, or if it has found a ship it will
	/// target the same ship
	/// </summary>
	private enum AIStates
	{
		/// <summary>
		/// The AI is searching for its next target
		/// </summary>
		Searching,

		/// <summary>
		/// The AI is trying to target a ship
		/// </summary>
		TargetingShip,

		/// <summary>
		/// The AI is locked onto a ship
		/// </summary>
		HittingShip
	}

	private AIStates _currentState = AIStates.Searching;
	private Stack<Target> _targets = new Stack<Target>();
	private List<Target> _lastHit = new List<Target>();

	private Target _currentTarget;
	public AIHardPlayer(BattleShipsGame game) : base(game)
	{
	}

	/// <summary>
	/// GenerateCoords will call upon the right methods to generate the appropriate shooting
	/// coordinates
	/// </summary>
	/// <param name="row">the row that will be shot at</param>
	/// <param name="column">the column that will be shot at</param>
	protected override void GenerateCoords(ref int row, ref int column)
	{
		do {
			_currentTarget = null;

			//check which state the AI is in and uppon that choose which coordinate generation
			//method will be used.
			switch (_currentState) {
				case AIStates.Searching:
					SearchCoords(ref row, ref column);
					break;
				case AIStates.TargetingShip:
				case AIStates.HittingShip:
					TargetCoords(ref row, ref column);
					break;
				default:
					throw new ApplicationException("AI has gone in an invalid state");
			}

		} while ((row < 0 || column < 0 || row >= EnemyGrid.Height || column >= EnemyGrid.Width || EnemyGrid[row, column] != TileView.Sea));
		//while inside the grid and not a sea tile do the search
	}

	/// <summary>
	/// TargetCoords is used when a ship has been hit and it will try and destroy
	/// this ship
	/// </summary>
	/// <param name="row">row generated around the hit tile</param>
	/// <param name="column">column generated around the hit tile</param>
	private void TargetCoords(ref int row, ref int column)
	{
		Target t = null;
		t = _targets.Pop();

		row = t.ShotAt.Row;
		column = t.ShotAt.Column;
		_currentTarget = t;
	}

	/// <summary>
	/// SearchCoords will randomly generate shots within the grid as long as its not hit that tile already
	/// </summary>
	/// <param name="row">the generated row</param>
	/// <param name="column">the generated column</param>
	private void SearchCoords(ref int row, ref int column)
	{
		row = _Random.Next(0, EnemyGrid.Height);
		column = _Random.Next(0, EnemyGrid.Width);
		_currentTarget = new Target(new Location(row, column), null);
	}

	/// <summary>
	/// ProcessShot is able to process each shot that is made and call the right methods belonging
	/// to that shot. For example, if its a miss = do nothing, if it's a hit = process that hit location
	/// </summary>
	/// <param name="row">the row that was shot at</param>
	/// <param name="col">the column that was shot at</param>
	/// <param name="result">the result from that hit</param>
	protected override void ProcessShot(int row, int col, AttackResult result)
	{
		switch (result.Value) {
			case ResultOfAttack.Miss:
				_currentTarget = null;
				break;
			case ResultOfAttack.Hit:
				ProcessHit(row, col);
				break;
			case ResultOfAttack.Destroyed:
				ProcessDestroy(row, col, result.Ship);
				break;
			case ResultOfAttack.ShotAlready:
				throw new ApplicationException("Error in AI");
		}

		if (_targets.Count == 0)
			_currentState = AIStates.Searching;
	}

	/// <summary>
	/// ProcessDetroy is able to process the destroyed ships targets and remove _lastHit targets.
	/// It will also call RemoveShotsAround to remove targets that it was going to shoot at
	/// </summary>
	/// <param name="row">the row that was shot at and destroyed</param>
	/// <param name="col">the row that was shot at and destroyed</param>
	/// <param name="ship">the row that was shot at and destroyed</param>
	private void ProcessDestroy(int row, int col, Ship ship)
	{
		bool foundOriginal = false;
		Location source = null;
		Target current = null;
		current = _currentTarget;

		foundOriginal = false;

		//i = 1, as we dont have targets from the current hit...
		int i = 0;

		for (i = 1; i <= ship.Hits - 1; i++) {
			if (!foundOriginal) {
				source = current.Source;
				//Source is nnothing if the ship was originally hit in
				// the middle. This then searched forward, rather than
				// backward through the list of targets
				if (source == null) {
					source = current.ShotAt;
					foundOriginal = true;
				}
			} else {
				source = current.ShotAt;
			}

			//find the source in _lastHit
			foreach (Target t in _lastHit) {
				if ((!foundOriginal && t.ShotAt == source) || (foundOriginal & t.Source == source)) {
					current = t;
					_lastHit.Remove(t);
					break; // TODO: might not be correct. Was : Exit For
				}
			}

			RemoveShotsAround(current.ShotAt);
		}
	}

	/// <summary>
	/// RemoveShotsAround will remove targets that belong to the destroyed ship by checking if 
	/// the source of the targets belong to the destroyed ship. If they don't put them on a new stack.
	/// Then clear the targets stack and move all the targets that still need to be shot at back 
	/// onto the targets stack
	/// </summary>
	/// <param name="toRemove"></param>
	private void RemoveShotsAround(Location toRemove)
	{
		Stack<Target> newStack = new Stack<Target>();
		//create a new stack

		//check all targets in the _targets stack

		foreach (Target t in _targets) {
			//if the source of the target does not belong to the destroyed ship put them on the newStack
			if (!object.ReferenceEquals(t.Source, toRemove))
				newStack.Push(t);
		}

		_targets.Clear();
		//clear the _targets stack

		//for all the targets in the newStack, move them back onto the _targets stack
		foreach (Target t in newStack) {
			_targets.Push(t);
		}

		//if the _targets stack is 0 then change the AI's state back to searching
		if (_targets.Count == 0)
			_currentState = AIStates.Searching;
	}

	/// <summary>
	/// ProcessHit gets the last hit location coordinates and will ask AddTarget to
	/// create targets around that location by calling the method four times each time with
	/// a new location around the last hit location.
	/// It will then set the state of the AI and if it's not Searching or targetingShip then 
	/// start ReOrderTargets.
	/// </summary>
	/// <param name="row"></param>
	/// <param name="col"></param>
	private void ProcessHit(int row, int col)
	{
		_lastHit.Add(_currentTarget);

		//Uses _currentTarget as the source
		AddTarget(row - 1, col);
		AddTarget(row, col - 1);
		AddTarget(row + 1, col);
		AddTarget(row, col + 1);

		if (_currentState == AIStates.Searching) {
			_currentState = AIStates.TargetingShip;
		} else {
			//either targetting or hitting... both are the same here
			_currentState = AIStates.HittingShip;

			ReOrderTargets();
		}
	}

	/// <summary>
	/// ReOrderTargets will optimise the targeting by re-orderin the stack that the targets are in.
	/// By putting the most important targets at the top they are the ones that will be shot at first.
	/// </summary>

	private void ReOrderTargets()
	{
		//if the ship is lying on the same row, call MoveToTopOfStack to optimise on the row
		if (_currentTarget.SameRow) {
			MoveToTopOfStack(_currentTarget.ShotAt.Row, -1);
		} else if (_currentTarget.SameColumn) {
			//else if the ship is lying on the same column, call MoveToTopOfStack to optimise on the column
			MoveToTopOfStack(-1, _currentTarget.ShotAt.Column);
		}
	}

	/// <summary>
	/// MoveToTopOfStack will re-order the stack by checkin the coordinates of each target
	/// If they have the right column or row values it will be moved to the _Match stack else 
	/// put it on the _NoMatch stack. Then move all the targets from the _NoMatch stack back on the 
	/// _targets stack, these will be at the bottom making them less important. The move all the
	/// targets from the _Match stack on the _targets stack, these will be on the top and will there
	/// for be shot at first
	/// </summary>
	/// <param name="row">the row of the optimisation</param>
	/// <param name="column">the column of the optimisation</param>
	private void MoveToTopOfStack(int row, int column)
	{
		Stack<Target> _NoMatch = new Stack<Target>();
		Stack<Target> _Match = new Stack<Target>();

		Target current = null;

		while (_targets.Count > 0) {
			current = _targets.Pop();
			if (current.ShotAt.Row == row || current.ShotAt.Column == column) {
				_Match.Push(current);
			} else {
				_NoMatch.Push(current);
			}
		}

		foreach (Target t in _NoMatch) {
			_targets.Push(t);
		}
		foreach (Target t in _Match) {
			_targets.Push(t);
		}
	}

	/// <summary>
	/// AddTarget will add the targets it will shoot onto a stack
	/// </summary>
	/// <param name="row">the row of the targets location</param>
	/// <param name="column">the column of the targets location</param>

	private void AddTarget(int row, int column)
	{

		if ((row >= 0 && column >= 0 && row < EnemyGrid.Height && column < EnemyGrid.Width && EnemyGrid[row, column] == TileView.Sea)) {
			_targets.Push(new Target(new Location(row, column), _currentTarget.ShotAt));
		}
	}

}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
