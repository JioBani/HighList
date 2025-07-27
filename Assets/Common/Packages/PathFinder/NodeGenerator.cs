using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Common.Packages.PathFinder;

public class NodeGenerator : MonoBehaviour
{
    public Tilemap tilemap;        // 노드를 배치할 타일맵
    public GameObject nodePrefab;  // 생성할 노드 프리팹
    public Transform nodeParent;   // 노드들을 담을 부모 (선택 사항)

    [HideInInspector]
    public List<GameObject> generatedNodes = new List<GameObject>();
    private Dictionary<Vector3Int, PathFinderNode> nodeMap = new Dictionary<Vector3Int, PathFinderNode>();

    // 6방향 (Cube 좌표 기준)
    private static readonly Vector3Int[] directions =
    {
        new Vector3Int(+1, -1, 0), // 동
        new Vector3Int(+1, 0, -1), // 북동
        new Vector3Int(0, +1, -1), // 북서
        new Vector3Int(-1, +1, 0), // 서
        new Vector3Int(-1, 0, +1), // 남서
        new Vector3Int(0, -1, +1)  // 남동
    };

    public void GenerateNodes()
    {
        ClearNodes();
        nodeMap.Clear();

        BoundsInt bounds = tilemap.cellBounds;
        Vector3Int offset = new Vector3Int(bounds.xMin, bounds.yMin, 0);

        // 1단계: 노드 생성 + 좌표 세팅 + Dictionary 등록
        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile(pos))
            {
                Vector3Int localPos = pos - offset;
                Vector3 worldPos = tilemap.GetCellCenterWorld(pos);

                var nodeObj = Instantiate(nodePrefab, worldPos, Quaternion.identity,
                    nodeParent != null ? nodeParent : transform);

                nodeObj.name = $"Node_{localPos.x}_{localPos.y}";
                generatedNodes.Add(nodeObj);

                var node = nodeObj.GetComponent<PathFinderNode>();
                if (node != null)
                {
                    Vector3Int cubeCoord = OffsetToCube(localPos);
                    node.SetCubeCoord(cubeCoord);
                    node.SetWalkable(true);

                    nodeMap[cubeCoord] = node; // Dictionary 등록
                }
            }
        }

        // 2단계: 인접 노드 연결
        foreach (var kvp in nodeMap)
        {
            PathFinderNode node = kvp.Value;
            foreach (var dir in directions)
            {
                Vector3Int neighborCoord = node.cubeCoord + dir;
                if (nodeMap.TryGetValue(neighborCoord, out PathFinderNode neighbor))
                {
                    node.AddNeighbor(neighbor);
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
        int x = offsetPos.x - (offsetPos.y - (offsetPos.y & 1)) / 2;
        int z = offsetPos.y;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }
}
