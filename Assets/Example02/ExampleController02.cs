using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleController02 : MonoBehaviour, AStar.IShortestPath<Cell02> {

	private Cell02[,] blockMap;
	public int width;
	public int height;
	public GameObject blockPrefab;
	[Range(0, 1)]
	public float debugSpeed = 0.1f;

	enum Command
	{
		Start,
		Goal,
		Road,
		Wall,
	}
	
	Cell02 startBlock;
	Cell02 goalBlock;
	Command currentCmd;
	AStar.PathFinder<Cell02> pathFinder;
	List<Cell02> traveledList = new List<Cell02> ();
	// Use this for initialization
	void Start () {
		pathFinder = new AStar.PathFinder<Cell02> (this);
		pathFinder.onTravelState = (state) => {
			traveledList.Add (state);
		};

		Generate ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButton (0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit) && hit.collider != null)
			{
				SetCellFlag (hit.collider.GetComponent<Cell02> ());
			}
		}
	}

	void SetCellFlag (Cell02 cell)
	{
		switch (currentCmd)
		{
		case Command.Start:
			if (startBlock != null) startBlock.SetColorFlagRoad ();
			cell.SetColorFlagStart ();
			startBlock = cell;
			break;
		case Command.Goal:
			if (goalBlock != null) goalBlock.SetColorFlagRoad ();
			cell.SetColorFlagGoal ();
			goalBlock = cell;
			break;
		case Command.Road:
			cell.SetColorFlagRoad ();
			break;
		case Command.Wall:
			cell.SetColorFlagWall ();
			break;
		}
	}
	
	void OnGUI ()
	{
		if (GUI.Button (new Rect (10, 50, 200, 50), "Select Start"))
		{
			currentCmd = Command.Start;
		}
		else if (GUI.Button (new Rect (10, 100, 200, 50), "Select Goal"))
		{
			currentCmd = Command.Goal;
		}
		else if (GUI.Button (new Rect (10, 150, 200, 50), "Wall"))
		{
			currentCmd = Command.Wall;
		}
		else if (GUI.Button (new Rect (10, 200, 200, 50), "Road"))
		{
			currentCmd = Command.Road;
		}
		else if (GUI.Button (new Rect (10, 300, 200, 50), "Clear"))
		{
			foreach (var cell in blockMap)
			{
				cell.Reset ();
			}
		}
		else if (GUI.Button (new Rect (10, 350, 200, 50), "Travel"))
		{
			foreach (var cell in blockMap)
			{
				if (cell.type == Cell02.Type.Start || cell.type == Cell02.Type.Goal || cell.type == Cell02.Type.Wall) continue;
				cell.Reset ();
			}

			traveledList.Clear ();
			List<Cell02> path = pathFinder.Travel (startBlock, goalBlock);
			if (path != null)
			{
				StartCoroutine (DrawDebug (path));
			}
		}
	}

	IEnumerator DrawDebug (List<Cell02> path)
	{
		foreach (Cell02 cell in traveledList)
		{
			yield return new WaitForSeconds (debugSpeed);
			cell.SetColorFlagSearched ();
		}
		foreach (Cell02 cell in path)
		{
			yield return new WaitForSeconds (debugSpeed);
			cell.SetColorFlagPath ();
		}
	}

	void Generate ()
	{
		blockMap = new Cell02[width, height];
		for (int y=0; y<height; y++)
		{
			for (int x=0; x<width; x++)
			{
				GameObject blockObject = Instantiate (blockPrefab) as GameObject;
				blockObject.name = string.Format ("Cell{0:00}", y*width+x+1);
				blockObject.transform.SetParent (transform);
				blockObject.transform.position = new Vector3 ((float)-width * 0.5f + x + 0.5f, (float)height * 0.5f - (y + 0.5f), 0);
				Cell02 block = blockObject.GetComponent<Cell02> ();
				block.x = x;
				block.y = y;
				blockMap[x, y] = block;
			}
		}

		startBlock = blockMap[0,0];
		goalBlock = blockMap[width-1,height-1];

		startBlock.SetColorFlagStart ();
		goalBlock.SetColorFlagGoal ();
	}

	public List<Cell02> Expand (Cell02 position)
	{
		List<Cell02> neighbourList = new List<Cell02> ();
		int[,] pathConsts = {
			{-1,0}, {1, 0}, {0, -1}, {0, 1}
		};
		for (int i=0; i<pathConsts.GetLength (0); i++)
		{
			int neighbourX = position.x+pathConsts[i,0];
			int neighbourY = position.y+pathConsts[i,1];
			if (0 > neighbourX || neighbourX >= width || 0 > neighbourY || neighbourY >= height) continue;

			Cell02 cell = blockMap[neighbourX, neighbourY];
			if (!cell.IsWalkable ()) continue;
			neighbourList.Add (cell);
		}

		return neighbourList;
	}

	public float Heuristic (Cell02 from, Cell02 to)
	{
//		return Mathf.Sqrt((from.x - to.x)*(from.x - to.x) + (from.y - to.y)*(from.y - to.y));
		return Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y);
	}

	public float ActualCost (Cell02 parent, Cell02 from, Cell02 to)
	{
		float cost = 1;
		if (parent != null)
		{
			if (parent.x != to.x && parent.y != to.y) cost = 10000;
		}
		return cost;
	}
}
