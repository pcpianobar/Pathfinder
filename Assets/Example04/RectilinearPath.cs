using System;
using System.Collections.Generic;

namespace RectilinearPath
{
	public interface IShortestPath<State> 
	{
		List<State> Expand (State position);
		int ActualCost (State parent, State from, State to);
	}
	
	public class PathFinder<State> {
		class Node<T> {
			public Node<T> parent;
			public T state;
			public int cost; // cost
			
			public Node(Node<T> parent, int cost, T state)
			{
				this.parent = parent;
				this.cost = cost;
				this.state = state;
			}
				
			public override string ToString ()
			{
				if (parent == null) return string.Format ("{0}({1}) cost({1})", state, null, cost);
				return string.Format ("{0}({1}) cost({2})", state, parent.state, cost);
			}
		}
		
		public Action<State> onTravelState;
		private IShortestPath<State> info;
		public PathFinder(IShortestPath<State> info){
			this.info = info;
		}

//		public List<State> Travel (State fromState, State toState)
//		{
//			List<State> closedList = new List<State> ();
//			Queue<Node<State>> openList = new Queue<Node<State>>();
//			Node<State> startNode = new Node<State>(null,1,fromState);
//
//			openList.Enqueue (startNode);
//			while (openList.Count > 0)
//			{
//				Node<State> node = openList.Dequeue ();
//				if (node.state.Equals (toState)) return BuildShortestPath (node);
//				
//				closedList.Add (node.state);
//				foreach (State neighbourState in info.Expand(node.state))
//				{
//					if (closedList.Contains (neighbourState)) continue;;
//					
//					Node<State> searchNode = CreateNode(node, neighbourState, toState);
//
//					if (searchNode.cost > 3) return null;
//
//					openList.Enqueue (searchNode);
//									
//					if (onTravelState != null) onTravelState (neighbourState);
//				}
//			}
//			
//			return null;
//		}

		Stack<State> closedList = new Stack<State> ();
		public List<State> Travel (State fromState, State toState)
		{
			closedList.Clear ();
			Node<State> startNode = new Node<State>(null,1,fromState);
			Node<State> findNode = Travel2 (startNode, toState);
			if (findNode != null)
			{
				return BuildShortestPath (findNode);
			}

			return null;
		}

		private Node<State> Travel2 (Node<State> node, State toState)
		{
			if (node.cost > 3) return null;
			if (node.state.Equals(toState)) return node;
			
			closedList.Push (node.state);
			foreach (State neighbourState in info.Expand(node.state))
			{
				if (closedList.Contains (neighbourState)) continue;;
				
				Node<State> searchNode = CreateNode(node, neighbourState, toState);
				Node<State> findNode = Travel2 (searchNode, toState);
				if (findNode != null)
				{
					return findNode;
				}

				if (onTravelState != null) onTravelState (neighbourState);
			}
			closedList.Pop ();

			return null;
		}
		
		private Node<State> CreateNode (Node<State> node, State child, State toState)
		{
			State parent = (node.parent != null) ? node.parent.state : default(State);
			int cost = info.ActualCost(parent, node.state, child);

			return new Node<State>(node, node.cost + cost, child);
		}
		
		private List<State> BuildShortestPath(Node<State> seachNode)
		{
			List<State> list = new List<State>();
			while (seachNode != null)
			{
				list.Insert(0, seachNode.state);
				seachNode = seachNode.parent;
			}
			
			return list;
		}
	}
	
	public class PriorityQueue<P, V>
	{
		private SortedDictionary<P, LinkedList<V>> list = new SortedDictionary<P, LinkedList<V>>();
		
		public void Enqueue(V value, P priority)
		{
			LinkedList<V> q;
			if (!list.TryGetValue(priority, out q))
			{
				q = new LinkedList<V>();
				list.Add(priority, q);
			}
			q.AddLast(value);
		}
		
		public V Dequeue()
		{
			// will throw exception if there isn’t any first element!
			SortedDictionary<P, LinkedList<V>>.KeyCollection.Enumerator enume = list.Keys.GetEnumerator();
			enume.MoveNext();
			P key = enume.Current;
			LinkedList<V> v = list[key];
			V res = v.First.Value;
			v.RemoveFirst();
			if (v.Count == 0){ // nothing left of the top priority.
				list.Remove(key);
			}
			return res;
		}
		
		public void Replace(V oldValue, V newValue, P oldPriority, P newPriority){
			LinkedList<V> v = list[oldPriority];
			v.Remove(oldValue);
			
			if (v.Count == 0){ // nothing left of the top priority.
				list.Remove(oldPriority);
			}
			
			Enqueue(newValue, newPriority);
		}
		
		public bool IsEmpty
		{
			get { return list.Count==0; }
		}
		
		public override string ToString() {
			string res = "";
			foreach (P key in list.Keys){
				foreach (V val in list[key]){
					res += val +", ";
				}
			}
			return res;
		}
	}
}