
using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
//using System.Data;
using System.Diagnostics;
/// <summary>
/// AttackResult gives the result after a shot has been made.
/// </summary>
public class AttackResult
{
	private ResultOfAttack _value;
	private Ship _ship;
	private string _text;
	private int _row;

	private int _column;
	/// <summary>
	/// The result of the attack
	/// </summary>
	/// <value>The result of the attack</value>
	/// <returns>The result of the attack</returns>
	public ResultOfAttack Value {
		get { return _value; }
	}

	/// <summary>
	/// The ship, if any, involved in this result
	/// </summary>
	/// <value>The ship, if any, involved in this result</value>
	/// <returns>The ship, if any, involved in this result</returns>
	public Ship Ship {
		get { return _ship; }
	}

	/// <summary>
	/// A textual description of the result.
	/// </summary>
	/// <value>A textual description of the result.</value>
	/// <returns>A textual description of the result.</returns>
	/// <remarks>A textual description of the result.</remarks>
	public string Text {
		get { return _text; }
	}

	/// <summary>
	/// The row where the attack occurred
	/// </summary>
	public int Row {
		get { return _row; }
	}

	/// <summary>
	/// The column where the attack occurred
	/// </summary>
	public int Column {
		get { return _column; }
	}

	/// <summary>
	/// Set the _value to the PossibleAttack value
	/// </summary>
	/// <param name="value">either hit, miss, destroyed, shotalready</param>
	public AttackResult(ResultOfAttack value, string text, int row, int column)
	{
		_value = value;
		_text = text;
		_ship = null;
		_row = row;
		_column = column;
	}

	/// <summary>
	/// Set the _value to the PossibleAttack value, and the _ship to the ship
	/// </summary>
	/// <param name="value">either hit, miss, destroyed, shotalready</param>
	/// <param name="ship">the ship information</param>
	public AttackResult(ResultOfAttack value, Ship ship, string text, int row, int column) : this(value, text, row, column)
	{
		_ship = ship;
	}

	/// <summary>
	/// Displays the textual information about the attack
	/// </summary>
	/// <returns>The textual information about the attack</returns>
	public override string ToString()
	{
		if (_ship == null) {
			return Text;
		}

		return Text + " " + _ship.Name;
	}
}

//=======================================================
//Service provided by Telerik (www.telerik.com)
//Conversion powered by NRefactory.
//Twitter: @telerik
//Facebook: facebook.com/telerik
//=======================================================
