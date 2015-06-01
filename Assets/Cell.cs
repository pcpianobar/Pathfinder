using UnityEngine;
using System.Collections;
using System;

public class Cell : MonoBehaviour, IEquatable<Cell> {

	public enum Type
	{
		Clear,
		Wall,
	}

	public Vector2 coordinate;
	public Type type;

	public int x;
	public int y;
	public Cell[] neighbour = new Cell[4];
	public Cell parent;

	public float F;
	public float G;
	public float H;

	public void Reset ()
	{
		F = G = H = 0;
		parent = null;
		if (type != Type.Wall) {
			SetColorFlagClear ();
		}
	}
	
	public void SetColorFlagWall ()
	{
		GetComponent<Renderer>().material.color = Color.red;
		type = Type.Wall;
	}

	public void SetColorFlagStart ()
	{
		GetComponent<Renderer>().material.color = Color.blue;
		type = Type.Clear;
	}

	public void SetColorFlagGoal ()
	{
		GetComponent<Renderer>().material.color = Color.yellow;
		type = Type.Clear;
	}

	public void SetColorFlagClear ()
	{
		GetComponent<Renderer>().material.color = Color.white;
		type = Type.Clear;
	}

	public void SetColorFlagSearched ()
	{
		GetComponent<Renderer>().material.color = Color.green;
	}

	public void SetColorFlagPath ()
	{
		GetComponent<Renderer>().material.color = Color.gray;
	}

	public bool IsWalkable ()
	{
		return type == Type.Clear;
	}

	public int MovementCost ()
	{
		return 1;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode ();
	}

	public override bool Equals (object other)
	{
		if (other == null) return false;
		Cell otherCell = other as Cell;
		if (otherCell == null) return false;

		return (x == otherCell.x) && (y == otherCell.y);
	}

	public bool Equals (Cell other)
	{
		return this.Equals (other);
	}
}
