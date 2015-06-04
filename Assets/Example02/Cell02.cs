using UnityEngine;
using System.Collections;

public class Cell02 : MonoBehaviour {

	public enum Type
	{
		Road,
		Wall,
		Start,
		Goal,
	}

	public Type type;

	public int x;
	public int y;

	public void Reset ()
	{
		SetColorFlagRoad ();
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

	public void SetColorFlagRoad ()
	{
		GetComponent<Renderer>().material.color = Color.white;
		type = Type.Road;
	}

	public void SetColorFlagSearched ()
	{
		if (type != Type.Road) return;
		GetComponent<Renderer>().material.color = Color.green;
	}

	public void SetColorFlagPath ()
	{
		if (type != Type.Road) return;
		GetComponent<Renderer>().material.color = Color.gray;
	}

	public bool IsWalkable ()
	{
		return type == Type.Road || type == Type.Start || type == Type.Goal;
	}

	public override string ToString ()
	{
		return gameObject.name;
	}
}
