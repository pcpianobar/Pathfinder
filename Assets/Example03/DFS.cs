using System;
using System.Collections.Generic;

namespace DFS
{
	public interface IShortestPath<State> 
	{
		List<State> Expand (State position);
		float Heuristic (State from, State to);
		float ActualCost (State parent, State from, State to);
	}
	
	public class PathFinder<State> {
		class Node<T> : IComparable< Node<T>> {
			public Node<T> parent;
			public T state;
			public float g; // cost
			public float f; // estimate
			
			public Node(Node<T> parent, float g, float f, T state)
			{
				this.parent = parent;
				this.g = g;
				this.f = f;
				this.state = state;
			}
			// Reverse sort order (smallest numbers first)
			public int CompareTo(Node<T> other)
			{
				return this.f.CompareTo (other.f);
			}
			
			public override string ToString ()
			{
				if (parent == null) return string.Format ("{0}({1} g({2}), f({3})", state, null, g, f);
				return string.Format ("{0}({1}) g({2}), f({3})", state, parent.state, g, f);
			}
		}
		
		public Action<State> onTravelState;
		private IShortestPath<State> info;
		public PathFinder(IShortestPath<State> info){
			this.info = info;
		}
		
		public List<State> Travel (State fromState, State toState)
		{
			List<State> closedList = new List<State> ();
			Stack<Node<State>> openList = new Stack<Node<State>>();
			Node<State> startNode = new Node<State>(null,0,0,fromState);
			
			openList.Push (startNode);

			while (openList.Count > 0)
			{
				Node<State> node = openList.Pop ();
				if (node.state.Equals(toState)) return BuildShortestPath (node);

				closedList.Add (node.state);
				foreach (State neighbourState in info.Expand(node.state))
				{
					if (closedList.Contains (neighbourState)) continue;

					Node<State> searchNode = CreateNode(node, neighbourState, toState);
					openList.Push (searchNode);
					if (onTravelState != null) onTravelState (neighbourState);
				}
			}
			return null;
		}
		
		private Node<State> CreateNode(Node<State> node, State child, State toState)
		{
			State parent = (node.parent != null) ? node.parent.state : default(State);
			float cost = info.ActualCost(parent, node.state, child);
			
			float heuristic = info.Heuristic(child, toState);
			return new Node<State>(node, node.g+cost, node.g+cost+heuristic,child);
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