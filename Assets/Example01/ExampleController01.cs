using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExampleController01 : MonoBehaviour {

	private Cell01[,] blockMap;
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

	Cell01 startBlock;
	Cell01 goalBlock;
	Command currentCmd;
	AStarPathFinder pathFinder;

	// Use this for initialization
	void Start () {
		pathFinder = GetComponent<AStarPathFinder> ();
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
					Cell01 pickedBlock = hit.collider.GetComponent<Cell01> ();
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
			pathFinder.StartFindPath (startBlock, goalBlock, blockMap, false);
		}
	}

	void Generate ()
	{
		blockMap = new Cell01[width, height];
		for (int y=0; y<height; y++)
		{
			for (int x=0; x<width; x++)
			{
				GameObject blockObject = Instantiate (blockPrefab) as GameObject;
				blockObject.name = string.Format ("Cell_{0}x{1}", x, y);
				blockObject.transform.SetParent (transform);
				blockObject.transform.position = new Vector3 ((float)-width * 0.5f + x + 0.5f, (float)-height * 0.5f + y + 0.5f, 0);
				Cell01 block = blockObject.GetComponent<Cell01> ();
				block.coordinate = new Vector2 (x, y);
				block.x = x;
				block.y = y;
				blockMap[x, y] = block;
			}
		}

		int[,] pathConsts = {
			{-1,0}, {1, 0}, {0, -1}, {0, 1}
		};
		foreach (Cell01 block in blockMap)
		{
			for (int i=0; i<pathConsts.GetLength (0); i++)
			{
				int neighbourX = block.x+pathConsts[i,0];
				int neighbourY = block.y+pathConsts[i,1];
				if (0 > neighbourX || neighbourX >= width || 0 > neighbourY || neighbourY >= height) continue;

				block.neighbour[i] = blockMap[neighbourX, neighbourY];
			}
		}
	}
}
