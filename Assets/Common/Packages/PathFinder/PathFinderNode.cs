using UnityEngine;
using System.Collections.Generic;

namespace Common.Packages.PathFinder
{
    public class PathFinderNode : MonoBehaviour
    {
        public bool walkable { get; private set; }
        public Vector3Int cubeCoord { get; private set; }
        public List<PathFinderNode> neighbors { get; private set; } = new List<PathFinderNode>();

        public void SetWalkable(bool _walkable)
        {
            walkable = _walkable;
        }

        public void SetCubeCoord(Vector3Int coord)
        {
            cubeCoord = coord;
        }

        public void AddNeighbor(PathFinderNode neighbor)
        {
            if (neighbor != null && !neighbors.Contains(neighbor))
                neighbors.Add(neighbor);
        }
    }
}