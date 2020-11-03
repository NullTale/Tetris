using Priority_Queue;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;

namespace AStar
{
	public interface iExplorer<T>
	{
		IEnumerable<T> GetNeighbours(T node);
		float GetPathCost(T start, T goal);
		float GetShortestPath(T start, T goal);
		bool Reachable(T start, T goal);
        bool Passable(T node);
	}

	public class PathNode<T> : FastPriorityQueueNode
    {
        // previous node
		public PathNode<T>          СameFrom;
        // estimated length to target
		public float                PathCostEstimated;
        // length from start, used Priority instead
		public float                PathCost;
        // link to owner
        public readonly T           Master;
        // priority of this node
        public float                Cost;

		//////////////////////////////////////////////////////////////////////////
		public PathNode(T master)
		{
			Master = master;
		}
    }

	public class FindPathProcess<T> : CustomYieldInstruction, IEnumerable<T>
	{ 
        const float                             c_ChecksMeasure = 1.0f / 15.0f;

        //////////////////////////////////////////////////////////////////////////
		public LinkedList<T>                    Path { get; private set; }
		public FastPriorityQueue<PathNode<T>>   OpenSet { get; private set; }
		public HashSet<T>					    ClosedSet { get; private set; }
        private LinkedList<PathNode<T>>         ClosedNodes;
        public T                                Start { get; private set; }
        public T[]                              Goal { get; private set; }
        public iExplorer<T>                     Explorer { get; private set; }
        public FindPathOptions                  Options { get; private set; }

        public FindPathResult                   Result { get; private set; }

        public bool                             IsPathFound => Result == FindPathResult.Found;

        public override bool keepWaiting
        {
            get
            {
                // implement pathfinding
                if (Result == FindPathResult.Running)
                {
                    _ImplementPathfinding((int)(Pathfinder.c_AdaptiveBufferExperiance.Average * c_ChecksMeasure));
                }

                // waiting if running
                return Result == FindPathResult.Running;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public void Init(FindPathOptions options, iExplorer<T> exlorer, T start, T[] goal)
        {
            if (Result == FindPathResult.None)
            {
                // save data
                Options = options;
                Explorer = exlorer;
                Start = start;
                Goal = goal;

                // set running state
                Result = FindPathResult.Running;


                // allocate collections
                Path = new LinkedList<T>();
                OpenSet = new FastPriorityQueue<PathNode<T>>(Pathfinder.c_AdaptiveBufferExperiance.Average);
                ClosedSet = new HashSet<T>();
                ClosedNodes = new LinkedList<PathNode<T>>();

                // create start node, add to open set
                var startNode = new PathNode<T>(start)
                {
                    СameFrom = null,
                    PathCost = 0.0f,
                    PathCostEstimated = 0.0f,
                    Cost = 0.0f
                };

                OpenSet.Enqueue(startNode, startNode.Cost);
            }
        }

        public void Complete(FindPathResult result)
        {
            // set state
            Result = result;

            // impact to adaptive buffer size
            if (OpenSet != null)
            {
                switch (result)
                {
                    case FindPathResult.Found:
                    {
                        Pathfinder.c_AdaptiveBufferExperiance.AddExperiance((int)((OpenSet.Count + ClosedSet.Count) * 1.2f));
                    }   break;
                    case FindPathResult.Interrupted:
                        break;
                    case FindPathResult.BadArguments:
                    case FindPathResult.NotReachable:
                    case FindPathResult.Overwhelm:
                        break;
                    case FindPathResult.None:
                    case FindPathResult.Running:
                    default:
                        throw new ArgumentOutOfRangeException(nameof(result), result, null);
                }
            }
        }

        public FindPathProcess<T> Implement()
        {
            // do while has variants
            _ImplementPathfinding(int.MaxValue);

            return this;
        }

        public bool Validate(bool connectionCheck, bool passableCheck)
        {
            // path found
            if (Path == null)
                return false;

            // all node passable
            if (passableCheck)
                if (Path.Any(node => Explorer.Passable(node) == false))
                    return false;

            // connection exists
            if (connectionCheck)
                for (var node = Path.First; node.Next != null; node = node.Next)
                {
                    if (Explorer.GetNeighbours(node.Value).Contains(node.Next.Value) == false)
                        return false;
                }

            return true;
        }

        public LinkedList<T> BuildClosestPath()
        {
            if (Path.Count > 0)
                return Path;
                
            // get closest node from closed set
            var closestNode = ClosedNodes.MinBy(node => Goal.Min(goal => Explorer.GetShortestPath(node.Master, goal)));
            
            for (var currentNode = closestNode; currentNode != null; currentNode = currentNode.СameFrom)
                Path.AddFirst(currentNode.Master);

            return Path;
        }

        public IEnumerator<T> GetEnumerator()
		{
			return Path.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return Path.GetEnumerator();
		}

        //////////////////////////////////////////////////////////////////////////
        private void _ImplementPathfinding(int checks)
        {
            // do while has variants
            while (checks-- > 0)
            {
                // not found
                if (OpenSet.Count == 0)
                {
                    Complete(FindPathResult.NotReachable);
                    return;
                }

                // limit achieved
                if (ClosedSet.Count >= Options.CheckLimit)
                {
                    Complete(FindPathResult.Overwhelm);
                    return;
                }

                // get next node
                var currentNode = OpenSet.First();

                // close current, move from open to close set
                OpenSet.Remove(currentNode);
                ClosedSet.Add(currentNode.Master);
                ClosedNodes.AddLast(currentNode);

                // goal check
                if (Goal.Any(n => n.Equals(currentNode.Master)))
                {
                    getPath(currentNode, Path);
                    Complete(FindPathResult.Found);
                    return;
                }

                // proceed connections
                foreach (var neighborNode in Explorer.GetNeighbours(currentNode.Master))
                {
                    // skip if not passable
                    if (neighborNode == null || Explorer.Passable(neighborNode) == false)
                        continue;

                    // IsClosed, skip if already checked
                    if (ClosedSet.Contains(neighborNode))
                        continue;
					
                    var pathCost = currentNode.PathCost + Explorer.GetPathCost(currentNode.Master, neighborNode);

                    var openNode = OpenSet.FirstOrDefault(pathNode => pathNode.Master.Equals(neighborNode));
                    if (openNode != null)
                    {	
                        // if presented and part is shorter, then reset his parent and cost
                        if (openNode.PathCost > pathCost)
                        {
                            openNode.СameFrom = currentNode;
                            openNode.PathCost = pathCost;
                            // update priority
                            openNode.Cost = openNode.PathCostEstimated + openNode.PathCost;
                            OpenSet.UpdatePriority(openNode, openNode.Cost);
                        }
                    }
                    else
                    {	
                        // if not presented, add as variant
                        var pathNode = new PathNode<T>(neighborNode);
                        pathNode.СameFrom = currentNode;
                        pathNode.PathCost = pathCost;
                        if (Options.Heuristic)
                        {
                            pathNode.PathCostEstimated = getShortestPath(Explorer, pathNode.Master, Goal);
                            pathNode.Cost = pathNode.PathCostEstimated + pathNode.PathCost;
                        }
                        else
                            pathNode.Cost = pathNode.PathCost;
                        OpenSet.Enqueue(pathNode, pathNode.Cost);
                    }
                }
            }
            
            /////////////////////////////////////
            float getShortestPath(iExplorer<T> explorer, T start, IEnumerable<T> goal)
            {
                var shortestPath = float.MaxValue;

                // ReSharper disable once ForeachCanBeConvertedToQueryUsingAnotherGetEnumerator
                foreach (var n in goal)
                {
                    var currentShortestPath = explorer.GetShortestPath(start, n);
                    if (shortestPath > currentShortestPath)
                        shortestPath = currentShortestPath;
                }

                return shortestPath;
            }
            
            void getPath(PathNode<T> pathNode, LinkedList<T> path)
            {
                for (var currentNode = pathNode; currentNode != null; currentNode = currentNode.СameFrom)
                    path.AddFirst(currentNode.Master);
            }
        }

    }

    [Serializable]
    public enum FindPathResult
    {
        None,
        Running,

        Found,

        Interrupted,
        BadArguments,
        NotReachable,
        Overwhelm,
    }

    [Serializable]
    public class FindPathOptions
    {
        public static readonly FindPathOptions      c_Default = new FindPathOptions();

        public int      CheckLimit = 256;
        public bool     Heuristic = true;
    }

    //////////////////////////////////////////////////////////////////////////
	public static class Pathfinder
	{
        public static HistoryBufferInt         c_AdaptiveBufferExperiance = new HistoryBufferInt(32, 256);

        //////////////////////////////////////////////////////////////////////////
        public static FindPathProcess<T> FindPath<T>(this iExplorer<T> explorer, T start, params T[] goal)
        {
            return explorer.FindPath(FindPathOptions.c_Default, start, goal);
        }

        public static FindPathProcess<T> FindPath<T>(this iExplorer<T> explorer, out FindPathProcess<T> findPathProcess, T start, params T[] goal)
        {
            findPathProcess = explorer.FindPath(FindPathOptions.c_Default, start, goal);
            return findPathProcess;
        }

        public static FindPathProcess<T> FindPath<T>(this iExplorer<T> explorer, out FindPathProcess<T> findPathProcess, FindPathOptions options, T start, params T[] goal)
        {
            findPathProcess = explorer.FindPath(options, start, goal);
            return findPathProcess;
        }

		public static FindPathProcess<T> FindPath<T>(this iExplorer<T> explorer, FindPathOptions options, T start, params T[] goal)
		{
            // set default options
            if (options == null)
                options = FindPathOptions.c_Default;

            // create result
            var result = new FindPathProcess<T>();

            // common sense check
			if (start == null || goal.Length == 0 || explorer == null)
            {
                result.Complete(FindPathResult.BadArguments);
                return result;
            }

            // find only reachable goals
            goal = goal
                .Where(n => explorer.Reachable(start, n))
                .ToArray();

            // is reachable
            if (goal.Length == 0)
            {
                result.Complete(FindPathResult.NotReachable);
                return result;
            }

            // init result inner data
            result.Init(options, explorer, start, goal);
            
            return result;
        }
    }
}