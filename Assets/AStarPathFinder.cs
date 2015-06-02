using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarPathFinder : MonoBehaviour
{
	List<Cell> openList;
	List<Cell> closedList;
	List<Cell> finalPath;
	Cell start;
	Cell end;

	Cell[,] map;

	public AStarPathFinder ()
	{
		openList = new List<Cell> ();
		closedList = new List<Cell> ();
		finalPath = new List<Cell>();
	}

	public void StartFindPath (Cell startCell, Cell goalCell, Cell[,] map, bool targetCellMustBeFree)
	{
		openList.Clear ();
		closedList.Clear ();
		finalPath.Clear ();

		this.map = map;
		foreach (var cell in map)
		{
			cell.Reset ();
		}
		
		start = startCell;
		end = goalCell;

		openList.Add (start);

		StopCoroutine ("FindPath");
		StartCoroutine ("FindPath", targetCellMustBeFree);
	}

	IEnumerator FindPath (bool targetCellMustBeFree)
	{
		Cell currentNode = null;
		while (openList.Count > 0)
		{
			yield return new WaitForSeconds(0.1f);

			currentNode = ExtractBestNodeFromOpenList ();
			closedList.Add (currentNode);

			if (end.Equals (currentNode)) break;

			foreach (Cell neighbour in currentNode.neighbour)
			{
				if (neighbour == null) continue;
				if (!neighbour.IsWalkable ()) continue;
				if (closedList.Contains (neighbour)) continue;
				CalcCost (currentNode, neighbour);
				neighbour.SetColorFlagSearched ();

				if (!openList.Contains (neighbour)) 
				{
					openList.Add (neighbour);
				}
				else
				{
//						if (neighbour.G < inOpenList.G) 
//						{
//							inOpenList.G = neighbour.G;
//							inOpenList.F = inOpenList.G + inOpenList.H;
//							inOpenList.parent = currentNode;
//						}
				}
			}

			currentNode = null;
		}

		if (currentNode != null)
		{
			Cell n = end;
			while (n != null)
			{
				finalPath.Add (n);
				n.SetColorFlagPath ();
				n = n.parent;
			}
		}
	}

	Cell ExtractBestNodeFromOpenList ()
	{
		float minF = float.MaxValue;
		Cell bestOne = null;
		foreach (Cell n in openList)
		{
			if (n.F < minF)
			{
				minF = n.F;
				bestOne = n;
			}
		}

		if (bestOne != null)
		{
			openList.Remove (bestOne);
		}

		return bestOne;
	}

	void CalcCost (Cell n, Cell neigbour) {
		neigbour.G = n.G + neigbour.MovementCost ();
		if (n.parent != null)
		{
			if (n.parent.x != neigbour.x) neigbour.G += 10000;
			if (n.parent.y != neigbour.y) neigbour.G += 10000;
		}
		neigbour.H = Heuristic(neigbour);
		neigbour.F = neigbour.G + neigbour.H;
		neigbour.parent = n;
	}
	
	float Heuristic (Cell n) {
		return Mathf.Sqrt((n.x - end.x)*(n.x - end.x) + (n.y - end.y)*(n.y - end.y));
	}
}

