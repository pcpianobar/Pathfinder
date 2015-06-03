using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleController02 : MonoBehaviour, AStar.IShortestPath<Cell02> {

	private Cell02[,] blockMap;
	public int width;
	public int height;
	public GameObject blockPrefab;
	
	enum Command
	{
		Start,
		Goal,
		Clear,
		Wall,
	}
	
	Cell02 startBlock;
	Cell02 goalBlock;
	Command currentCmd;
	AStar.PathFinder<Cell02> pathFinder;
	
	// Use this for initialization
	void Start () {
		pathFinder = new AStar.PathFinder<Cell02> (this);
		Generate ();
	}
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetMouseButton (0))
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast (ray, out hit))
			{
				if (hit.collider != null)
				{
					Cell02 pickedBlock = hit.collider.GetComponent<Cell02> ();
					switch (currentCmd)
					{
					case Command.Start:
						if (startBlock != null) startBlock.SetColorFlagClear ();
						pickedBlock.SetColorFlagStart ();
						startBlock = pickedBlock;
						break;
					case Command.Goal:
						if (goalBlock != null) goalBlock.SetColorFlagClear ();
						pickedBlock.SetColorFlagGoal ();
						goalBlock = pickedBlock;
						break;
					case Command.Clear:
						pickedBlock.SetColorFlagClear ();
						break;
					case Command.Wall:
						pickedBlock.SetColorFlagWall ();
						break;
					}
				}
			}
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
		else if (GUI.Button (new Rect (10, 200, 200, 50), "Clear"))
		{
			currentCmd = Command.Clear;
		}
		else if (GUI.Button (new Rect (10, 250, 200, 50), "Pathfinder"))
		{
			List<Cell02> path = pathFinder.Travel (startBlock, goalBlock);
			if (path != null)
			{
				StartCoroutine (DrawDebug (path));
			}
		}
	}

	IEnumerator DrawDebug (List<Cell02> path)
	{
		foreach (Cell02 cell in path)
		{
			yield return null;
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
				blockObject.name = string.Format ("Cell_{0}x{1}", x, y);
				blockObject.transform.SetParent (transform);
				blockObject.transform.position = new Vector3 ((float)-width * 0.5f + x + 0.5f, (float)-height * 0.5f + y + 0.5f, 0);
				Cell02 block = blockObject.GetComponent<Cell02> ();
				block.x = x;
				block.y = y;
				blockMap[x, y] = block;
			}
		}
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
			
			neighbourList.Add (blockMap[neighbourX, neighbourY]);
		}

		return neighbourList;
	}

	public float Heuristic (Cell02 from, Cell02 to)
	{
		return Mathf.Sqrt((from.x - to.x)*(from.x - to.x) + (from.y - to.y)*(from.y - to.y));
	}

	public float ActualCost (Cell02 from, Cell02 to)
	{
		float cost = 1;
//		if (from.parent != null)
//		{
//			if (from.parent.x != to.x && from.parent.y != to.y) cost = 10000;
//		}
		return cost;
	}
}
