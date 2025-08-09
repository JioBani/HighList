using System.Collections.Generic;
using Common.Packages.Heap;
using UnityEngine;

namespace Common.Packages.PathFinder
{
    public class PathFinder : MonoBehaviour
    {
        public static readonly Vector3Int[] NeighborDirs = new Vector3Int[]
        {
            new Vector3Int(0, -1, 1), // 12
            new Vector3Int(1, -1, 0), // 2
            new Vector3Int(1, 0, -1), // 4
            new Vector3Int(0, 1, -1), // 6
            new Vector3Int(-1, 1, 0), // 8
            new Vector3Int(-1, 0, 1), // 10
            new Vector3Int(2, -1, -1), // 오른쪽 
            new Vector3Int(-2, 1, 1), // 왼쪽
        };
        
        public static readonly Dictionary<Vector3Int, int> NeighborDirsWithCost = new Dictionary<Vector3Int, int>
        {
            { new Vector3Int(0, -1, 1), 1 },  // 아래
            { new Vector3Int(1, -1, 0), 1 },  // 오른쪽 아래
            { new Vector3Int(1, 0, -1), 1 },  // 오른쪽
            { new Vector3Int(0, 1, -1), 1 },  // 위
            { new Vector3Int(-1, 1, 0), 1 },  // 왼쪽 위
            { new Vector3Int(-1, 0, 1), 1 },  // 왼쪽 
            { new Vector3Int(2, -1, -1), 2 }, // 오른쪽 직선 2칸 (비용 2)
            { new Vector3Int(-2, 1, 1), 2 }   // 왼쪽 직선 2칸 (비용 2)
        };
        
        private int HeuristicCost(PathFinderNode a, PathFinderNode b)
        {
            // Cube 거리 계산
            return Mathf.Max(
                Mathf.Abs(a.cubeCoord.x - b.cubeCoord.x),
                Mathf.Abs(a.cubeCoord.y - b.cubeCoord.y),
                Mathf.Abs(a.cubeCoord.z - b.cubeCoord.z));
        }
        
        private List<PathFinderNode> ReconstructPath(Dictionary<PathFinderNode, PathFinderNode> cameFrom, PathFinderNode current)
        {
            var totalPath = new List<PathFinderNode> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            return totalPath;
        }

        // 현재 노드에서 특정 노드까지의 이동 Node List 반환
        public List<PathFinderNode> Find(PathFinderNode startNode, PathFinderNode endNode)
        {
            var openSet = new MinHeap<PathFinderNode>();
            var cameFrom = new Dictionary<PathFinderNode, PathFinderNode>();
            var gScore = new Dictionary<PathFinderNode, int>();
            var fScore = new Dictionary<PathFinderNode, int>();
            var closedSet = new HashSet<PathFinderNode>();

            gScore[startNode] = 0;
            fScore[startNode] = HeuristicCost(startNode, endNode);
            openSet.Insert(startNode, fScore[startNode]);

            while (openSet.Count > 0)
            {
                var current = openSet.ExtractMin();

                if (current == endNode)
                    return ReconstructPath(cameFrom, current);

                closedSet.Add(current);
                
                string neighborText = "";
                foreach (var neighbor in current.neighbors)
                {
                    neighborText += $"{neighbor.Key} : {neighbor.Value.cubeCoord} \n";
                }

                foreach (var neighborPair in current.neighbors)
                {
                    Vector3Int coord = neighborPair.Key;
                    PathFinderNode node = neighborPair.Value;
                    
                    if (!current.CirculateWalkable(coord) || closedSet.Contains(node))
                        continue;

                    ProcessNeighbor(current, node, NeighborDirsWithCost[coord],
                        cameFrom, gScore, fScore, openSet, endNode);
                }
            }

            return new List<PathFinderNode>();
        }
        
        private void ProcessNeighbor(
            PathFinderNode current,
            PathFinderNode neighbor,
            int cost,
            Dictionary<PathFinderNode, PathFinderNode> cameFrom,
            Dictionary<PathFinderNode, int> gScore,
            Dictionary<PathFinderNode, int> fScore,
            MinHeap<PathFinderNode> openSet,
            PathFinderNode endNode)
        {
            int tentativeG = gScore[current] + cost;

            if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
            {
                cameFrom[neighbor] = current;
                gScore[neighbor] = tentativeG;
                int fCost = tentativeG + HeuristicCost(neighbor, endNode);
                fScore[neighbor] = fCost;
                openSet.InsertOrUpdate(neighbor, fCost);
            }
        }
    }
}

