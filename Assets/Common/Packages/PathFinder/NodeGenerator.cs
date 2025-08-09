using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Common.Packages.PathFinder;

namespace Common.Packages.PathFinder
{
    public class NodeGenerator : MonoBehaviour
    {
        public Tilemap tilemap;        // 노드를 배치할 타일맵
        public GameObject nodePrefab;  // 생성할 노드 프리팹
        public Transform nodeParent;   // 노드들을 담을 부모 (선택 사항)

        [HideInInspector]
        public List<GameObject> generatedNodes = new List<GameObject>();
        private Dictionary<Vector3Int, PathFinderNode> nodeMap = new Dictionary<Vector3Int, PathFinderNode>();
        
        public PathFinderNode startNode;
        public PathFinderNode endNode;
        public PathFinder pathFinder;

        public void GenerateNodes()
        {
            ClearNodes();
            nodeMap.Clear();

            BoundsInt bounds = tilemap.cellBounds;

            string log = "";

            int x = 0;
            int y = 0;
            int initY = 0;
            Vector3Int vector;

            // 1단계: 노드 생성 + 좌표 세팅 + Dictionary 등록
            foreach (var pos in bounds.allPositionsWithin)
            {
                if (tilemap.HasTile(pos))
                {
                    vector = new Vector3Int(x, -y, -(x - y));
                    Vector3 worldPos = tilemap.GetCellCenterWorld(pos);
                    
                    var nodeObj = Instantiate(nodePrefab, worldPos, Quaternion.identity,
                        nodeParent != null ? nodeParent : transform);

                    generatedNodes.Add(nodeObj);

                    var node = nodeObj.GetComponent<PathFinderNode>();
                    if (node != null)
                    {
                        node.SetCubeCoord(vector);
                        node.SetWalkable(true);

                        nodeMap[vector] = node; // Dictionary 등록
                        
                        nodeObj.name = $"Node_{vector.x}_{vector.y}_{vector.z}";
                        node.localPos = vector;
                    }
                    
                    y++;    
                    
                    if (x % 2 == 0)
                    {
                        if (y - initY == 6)
                        {
                            x++;
                            y = ((x + 1) / 2);
                            initY = y;
                        }
                    }
                    else
                    {
                        if (y - initY == 5)
                        {
                            x++;
                            y = ((x + 1) / 2);
                            initY = y;
                        }
                    }
                }
                
            }

            // 2단계: 인접 노드 연결
            foreach (var kvp in nodeMap)
            {
                PathFinderNode node = kvp.Value;
            
                foreach (var dir in PathFinder.NeighborDirs)
                {
                    Vector3Int neighborOffset = Vector3Int.FloorToInt(node.localPos) + dir;
                    Vector3Int neighborCube = OffsetToCube(neighborOffset);
            
                    if (nodeMap.TryGetValue(node.cubeCoord + dir, out PathFinderNode neighbor))
                    {
                        node.AddNeighbor(neighbor,dir);
                    }
                }
            }
        }

        public void ClearNodes()
        {
            foreach (var node in generatedNodes)
            {
                if (node != null) DestroyImmediate(node);
            }
            generatedNodes.Clear();
        }

        private Vector3Int OffsetToCube(Vector3Int offsetPos)
        {
            int x = offsetPos.x;
            int z = offsetPos.y - (offsetPos.x - (offsetPos.x & 1)) / 2;
            int y = -x - z;
            return new Vector3Int(x, y, z);
        }

        public void FindRoute()
        {
            List<PathFinderNode> route = pathFinder.Find(startNode:startNode,endNode:endNode);
            
            Debug.Log("route.count : " + route.Count);

            foreach (var node in route) 
            {
                node.GetComponent<SpriteRenderer>().color = Color.skyBlue;
            }

            startNode.GetComponent<SpriteRenderer>().color = Color.red;
            endNode.GetComponent<SpriteRenderer>().color = Color.green;
        }
    }

}
