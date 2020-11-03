using System.Collections.Generic;

using UnityEngine;

namespace PathFinding
{

    public class Node {

        public Vector3 Position { get { return position; } }
        public List<Edge> Edges { get { return edges; } }
		public int Index { get { return index; } }

        protected Vector3 position;
        protected List<Edge> edges;
		protected int index;

        // for path finding
        public float distance;
        public Node prev = null;

        public Node(Vector3 p) {
            position = p;
            edges = new List<Edge>();
		}
		public Node(Vector3 p, int i)
		{
			position = p;
			index = i;
			edges = new List<Edge>();
		}

		public Edge Connect(Node node, float weight = 1f)
        {
            var e = new Edge(this, node, weight);
            edges.Add(e);
            node.edges.Add(e);
            return e;
        }

    }

}


