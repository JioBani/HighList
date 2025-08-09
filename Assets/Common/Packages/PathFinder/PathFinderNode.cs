using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;

namespace Common.Packages.PathFinder
{
    public class PathFinderNode : MonoBehaviour
    {
        public bool walkable = true;

        public Vector3Int cubeCoord;
        public Dictionary<Vector3Int, PathFinderNode> neighbors = new Dictionary<Vector3Int, PathFinderNode>();
        public Vector3 localPos;
        public List<PathFinderNode> showNeighbors = new List<PathFinderNode>(); // TODO 삭제
        public List<Vector3Int> showNeighborCoords = new List<Vector3Int>(); // TODO 삭제

        public void SetWalkable(bool _walkable)
        {
            walkable = _walkable;
        }

        public void SetCubeCoord(Vector3Int coord)
        {
            cubeCoord = coord;
        }

        public void AddNeighbor(PathFinderNode neighbor, Vector3Int coord)
        {
            if(neighbor != null && !neighbors.ContainsKey(coord))
            {
                neighbors.Add(coord, neighbor);
                showNeighbors.Add(neighbor);
                showNeighborCoords.Add(coord);
            }
        }

        public bool CirculateWalkable(Vector3Int coord)
        {
            if (coord == PathFinder.NeighborDirs[6])
            {
                return (neighbors.ContainsKey(PathFinder.NeighborDirs[1]) && neighbors[PathFinder.NeighborDirs[1]].walkable) ||
                       (neighbors.ContainsKey(PathFinder.NeighborDirs[2]) && neighbors[PathFinder.NeighborDirs[2]].walkable);
            }
            
            if (coord == PathFinder.NeighborDirs[7])
            {
                return (neighbors.ContainsKey(PathFinder.NeighborDirs[4]) && neighbors[PathFinder.NeighborDirs[4]].walkable) ||
                       (neighbors.ContainsKey(PathFinder.NeighborDirs[5]) && neighbors[PathFinder.NeighborDirs[5]].walkable);
            }
            
            return neighbors.TryGetValue(coord, out var neighbor) && neighbor.walkable;
        }
    }
}