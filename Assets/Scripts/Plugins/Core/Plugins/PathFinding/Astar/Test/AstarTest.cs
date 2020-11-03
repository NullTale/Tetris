using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;
using CielaSpike;
using Debug = UnityEngine.Debug;
using Core;

namespace AStar
{
    public class AstarTest : MonoBehaviour
    {
        [Serializable]
        public class NodeExplorer : iExplorer<ANode>
        {
            public float        m_HeuristicWeight = 1.0f;
            public float        m_PathWeight = 1.0f;

            //////////////////////////////////////////////////////////////////////////
            public IEnumerable<ANode> GetNeighbours(ANode node)
            {
                yield return node.Left;
                yield return node.Right;
                yield return node.Top;
                yield return node.Bottom;
            }

            public float GetPathCost(ANode start, ANode goal)
            {
                return (start.transform.position - goal.transform.position).magnitude * m_PathWeight;
            }

            public float GetShortestPath(ANode start, ANode goal)
            {
                return (start.transform.position - goal.transform.position).magnitude * m_HeuristicWeight;
            }

            public bool Reachable(ANode start, ANode goal)
            {
                return true;
            }

            public bool Passable(ANode node)
            {
                return node.Passable;
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public NodeExplorer             Explorer;
        public FindPathOptions          Options;

        public Vector2Int               GridSize;
        [NonSerialized]
        public MatrixWrapper<ANode>     Grid;

        public FindPathProcess<ANode>   FindPathProcess;

        //////////////////////////////////////////////////////////////////////////
        [Button]
        public void Init()
        {
            // clear
            Clear();

            // allocate
            Grid = new MatrixWrapper<ANode>();
            Grid.Allocate(GridSize.x, GridSize.y, (x, y) => 
            { 
                var go = new GameObject($"Node: {x} {y}", typeof(ANode));
                var node = go.GetComponent<ANode>();

                // set position
                go.transform.SetParent(transform);
                go.transform.localPosition = new Vector3(x, y, 0);

                return node;
            });

            // make connection
            foreach (var valuePair in Grid)
            {
                if (Grid.TryGetValue(valuePair.Key.x - 1, valuePair.Key.y, out var left))
                    valuePair.Value.Left = left;
                if (Grid.TryGetValue(valuePair.Key.x + 1, valuePair.Key.y, out var right))
                    valuePair.Value.Right = right;
                if (Grid.TryGetValue(valuePair.Key.x, valuePair.Key.y + 1, out var top))
                    valuePair.Value.Top = top;
                if (Grid.TryGetValue(valuePair.Key.x, valuePair.Key.y - 1, out var bottom))
                    valuePair.Value.Bottom = bottom;
            }
        }

        private void OnValidate()
        {
            // format search coords
            if (From.x < 0)                 From.x = 0;
            if (From.x >= GridSize.x)       From.x = GridSize.x - 1;
            if (From.y < 0)                 From.y = 0;
            if (From.y >= GridSize.x)       From.y = GridSize.y - 1;
            if (To.x < 0)                   To.x = 0;
            if (To.x >= GridSize.x)         To.x = GridSize.x - 1;
            if (To.y < 0)                   To.y = 0;
            if (To.y >= GridSize.x)         To.y = GridSize.y - 1;

            // reconnect nodes
            if ((Grid?.Count ?? 0) == 0)
            {
                // organize node list
                var nodeList = new List<Tuple<Vector2Int, ANode>>();

                foreach (var child in gameObject.GetChildren())
                {
                    var node = child.GetComponent<ANode>();
                    var coords = Regex.Matches(child.name, @" \d+").OfType<Match>().Select(n => int.Parse(n.Value))
                        .ToList();
                    nodeList.Add(new Tuple<Vector2Int, ANode>(new Vector2Int(coords[0], coords[1]), node));
                }

                // initialize grid
                var w = nodeList.Max(n => n.Item1.x) + 1;
                var h = nodeList.Max(n => n.Item1.y) + 1;

                Grid = new MatrixWrapper<ANode>();
                Grid.Allocate(w, h);

                // fill grid
                foreach (var (pos, node) in nodeList)
                    Grid[pos] = node;
            }
        }

        [Button]
        public void Clear()
        {
            gameObject.DestroyChildren();
            FindPathProcess = null;
            Grid = null;
        }
        
        [Header("Pathfinding")]
        public Vector2Int       From;
        public Vector2Int       To;
        

        [Header("Performance test")]
        public int              Calls;

        [Button]
        public void SpeedTest()
        {
            var sw = new Stopwatch();

            sw.Start();
            for (var n = 0; n < Calls; n++)
                Explorer.FindPath(Options, Grid[From], Grid[To]).Implement();
            sw.Stop();

            Debug.Log($"Calls: {Calls}, Duration: {sw.ElapsedMilliseconds}");
        }

        [Button]
        public void FindPath()
        {
            var sw = new Stopwatch();

            sw.Start();
            FindPathProcess = Explorer.FindPath(Options, Grid[From], Grid[To]).Implement();
            sw.Stop();
            var duration = sw.ElapsedMilliseconds;

            // if not found get closest point path
            if (FindPathProcess.IsPathFound == false)
                FindPathProcess.BuildClosestPath();

            _LogFindPathResult($"duration: {duration}");
        }

        [Button]
        public void FindPathCoroutine()
        {
            StartCoroutine(_FindPath());

            _LogFindPathResult();
        }
        
        [Button]
        public void FindPathCoroutineAsync()
        {
            this.StartCoroutineAsync(_FindPath());
        }

        private IEnumerator _FindPath()
        {
            yield return null;

            yield return Explorer.FindPath(out FindPathProcess, Options, Grid[From], Grid[To]);;

        }

        private void _LogFindPathResult(params string[] append)
        {
            if (FindPathProcess == null)
                return;

            // log result
            Debug.Log("" + 
                      $"closedSet: {FindPathProcess.ClosedSet?.Count}; " +
                      $"openSet: {FindPathProcess.OpenSet?.Count}; " +
                      $"buffer size: {FindPathProcess.OpenSet?.MaxSize}; " +
                      $"result: {FindPathProcess.Result}; " +
                      append.Aggregate("", (s, str) => s + " " + str) + ";");
        }

        private void OnDrawGizmos()
        {
            const float nodeSize = 0.2f;
            const float nodeRadius = nodeSize * 0.4f;

            // draw grid state
            if (Grid != null)
            {
                foreach (var pair in Grid)
                {
                    Gizmos.color = pair.Value.Passable ? Color.blue : Color.gray;
                    Gizmos.DrawCube(pair.Value.transform.position, new Vector3(nodeSize, nodeSize, nodeSize));
                }
            }

            // draw closed set
            if (FindPathProcess?.ClosedSet != null)
            {
                Gizmos.color = Color.white;
                foreach (var node in FindPathProcess.ClosedSet)
                    Gizmos.DrawSphere(node.transform.position, nodeRadius);
            }
            
            // draw open set
            if (FindPathProcess?.OpenSet != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var node in FindPathProcess.OpenSet)
                    Gizmos.DrawSphere(node.Master.transform.position, nodeRadius);
            }

            // draw path
            if (FindPathProcess?.Path != null)
            {
                Gizmos.color = Color.green;
                for (var node = FindPathProcess.Path.First; node != null; node = node.Next)
                {
                    if (node.Next != null)
                        Gizmos.DrawLine(node.Value.transform.position, node.Next.Value.transform.position);
                    
                }
            }
        }
    }
}