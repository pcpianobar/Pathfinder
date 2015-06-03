using UnityEngine;
using System.Collections;

public class Cell01 : MonoBehaviour, System.IComparable<Cell01> {

	public enum Type
	{
		Clear,
		Wall,
		Start,
		Goal,
	}

	public Vector2 coordinate;
	public Type type;

	public int x;
	public int y;
	public Cell01[] neighbour = new Cell01[4];
	public Cell01 parent;

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
		type = Type.Start;
	}

	public void SetColorFlagGoal ()
	{
		GetComponent<Renderer>().material.color = Color.yellow;
		type = Type.Goal;
	}

	public void SetColorFlagClear ()
	{
		GetComponent<Renderer>().material.color = Color.white;
		type = Type.Clear;
	}

	public void SetColorFlagSearched ()
	{
		if (type != Type.Clear) return;
		GetComponent<Renderer>().material.color = Color.green;
	}

	public void SetColorFlagPath ()
	{
		if (type != Type.Clear) return;
		GetComponent<Renderer>().material.color = Color.gray;
	}

	public bool IsWalkable ()
	{
		return type == Type.Clear || type == Type.Start || type == Type.Goal;
	}

	public override int GetHashCode()
	{
		return x ^ y;
	}

	public override bool Equals (System.Object other)
	{
		if (other == null) return false;
		Cell01 otherCell = other as Cell01;
		if (otherCell == null) return false;

		return (x == otherCell.x) && (y == otherCell.y);
	}

	public bool Equals (Cell01 other)
	{
		return Equals ((System.Object)other);
	}

	public int CompareTo(Cell01 other)
	{
		return this.F.CompareTo (other.F);
	}
}
