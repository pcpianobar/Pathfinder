using UnityEngine;
using System.Collections;

public class Cell02 : MonoBehaviour {

	public enum Type
	{
		Clear,
		Wall,
		Start,
		Goal,
	}

	public Type type;

	public int x;
	public int y;

	public void Reset ()
	{
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
}
