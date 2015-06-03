using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AStarPathFinder : MonoBehaviour
{
	List<Cell01> openList;
	List<Cell01> closedList;
	List<Cell01> finalPath;

	Cell01 start;
	Cell01 end;

	public AStarPathFinder ()
	{
		openList = new List<Cell01> ();
		closedList = new List<Cell01> ();
		finalPath = new List<Cell01>();
	}

	public void StartFindPath (Cell01 startCell, Cell01 goalCell, Cell01[,] map, bool targetCellMustBeFree)
	{
		openList.Clear ();
		closedList.Clear ();
		finalPath.Clear ();

		foreach (var cell in map)
		{
			if (startCell.Equals (cell) || goalCell.Equals (cell)) continue;
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
		Cell01 currentNode = null;
		while (openList.Count > 0)
		{
			yield return new WaitForSeconds (0.1f);

			currentNode = EnqueueCellFromOpenList ();
			closedList.Add (currentNode);

			if (end.Equals (currentNode))
			{
				BuildPath (currentNode);
				break;
			}

			foreach (Cell01 neighbour in currentNode.neighbour)
			{
				if (neighbour == null) continue;
				if (!neighbour.IsWalkable ()) continue;
				if (closedList.Contains (neighbour)) continue;
				neighbour.SetColorFlagSearched ();

				if (!openList.Contains (neighbour)) 
				{
					CalcCost (currentNode, neighbour);
					DequeueCellToOpenList (neighbour);
				}
				else
				{
					float g = DistBetween (currentNode, neighbour);
					if (g < neighbour.G) 
					{
						CalcCost (currentNode, neighbour);
					}
				}
			}
		}
	}

	void BuildPath (Cell01 cell)
	{
		while (cell != null)
		{
			finalPath.Add (cell);
			cell.SetColorFlagPath ();
			cell = cell.parent;
		}
	}

	Cell01 EnqueueCellFromOpenList ()
	{
		Cell01 cell = null;
		if (openList.Count > 0)
		{
			cell = openList[0];
			openList.RemoveAt (0);
		}

		return cell;
	}

	void DequeueCellToOpenList (Cell01 cell)
	{
		if (!openList.Contains (cell)) 
		{
			openList.Add (cell);
			openList.Sort ();
		}
	}

	void CalcCost (Cell01 from, Cell01 to) 
	{
		to.G = DistBetween (from, to);
		to.H = Heuristic(to, end);
		to.F = to.G + to.H;
		to.parent = from;
	}

	float DistBetween (Cell01 from, Cell01 to)
	{
		float G = from.G + 1;
		if (from.parent != null)
		{
			if (from.parent.x != to.x && from.parent.y != to.y) G = from.G + 10000;
		}
		return G;
	}

	float Heuristic (Cell01 n, Cell01 goal) {
		return Mathf.Sqrt((n.x - goal.x)*(n.x - goal.x) + (n.y - goal.y)*(n.y - goal.y));
	}
}

