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
		bool keepsearching = true;
		bool pathExits = true;

		while (keepsearching && pathExits)
		{
			yield return new WaitForSeconds(0.1f);

			Cell currentNode = ExtractBestNodeFromOpenList ();
			if (currentNode == null)
			{
				pathExits = false;
				break;
			}
			closedList.Add (currentNode);

			if (end.Equals (currentNode))
			{
				keepsearching = false;
			}
			else
			{
				foreach (Cell neighbour in currentNode.neighbour)
				{
					if (neighbour == null) continue;
					if (!neighbour.IsWalkable ()) continue;
					if (closedList.Contains (neighbour)) continue;
					PrepareNewNodeFrom (currentNode, neighbour);
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
			}

			if (pathExits)
			{
				Cell n = end;
				while (n != null)
				{
					finalPath.Add (n);
					n.SetColorFlagPath ();
					n = n.parent;
					yield return new WaitForSeconds(0.05f);
				}
			}
		}
	}

	public List<int> PointsFromPath ()
	{
		List<int> points = new List<int> ();
		foreach (Cell n in finalPath)
		{
			points.Add (n.x);
			points.Add (n.y);
		}

		return points;
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

	void PrepareNewNodeFrom(Cell n, Cell neigbour) {
		neigbour.G = n.G + MovementCost(n, neigbour);
		neigbour.H = Heuristic(neigbour);
		neigbour.F = neigbour.G + neigbour.H;
		neigbour.parent = n;
	}
	
	float Heuristic (Cell n) {
		return Mathf.Sqrt((n.x - end.x)*(n.x - end.x) + (n.y - end.y)*(n.y - end.y));
	}
	
	float MovementCost(Cell a, Cell b) {
		return map [b.x, b.y].MovementCost ();
	}
}

