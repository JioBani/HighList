using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Common.Packages.PathFinder.Tests
{
    public class PathFinderUTest
    {
        [UnityTest]
        public IEnumerator Find_경로탐색_정상동작()
        {
            // 1. PathFinder 오브젝트 및 컴포넌트 생성
            var pfObj = new GameObject("PathFinder");
            var pathFinder = pfObj.AddComponent<PathFinder>();

            // 2. 노드 3개 생성 (A-B-C 직선)
            var nodeAObj = new GameObject("NodeA");
            var nodeA = nodeAObj.AddComponent<PathFinderNode>();
            nodeAObj.AddComponent<SpriteRenderer>();
            nodeA.SetCubeCoord(new Vector3Int(0, 0, 0));
            nodeA.SetWalkable(true);

            var nodeBObj = new GameObject("NodeB");
            var nodeB = nodeBObj.AddComponent<PathFinderNode>();
            nodeBObj.AddComponent<SpriteRenderer>();
            nodeB.SetCubeCoord(new Vector3Int(1, -1, 0));
            nodeB.SetWalkable(true);

            var nodeCObj = new GameObject("NodeC");
            var nodeC = nodeCObj.AddComponent<PathFinderNode>();
            nodeCObj.AddComponent<SpriteRenderer>();
            nodeC.SetCubeCoord(new Vector3Int(2, -2, 0));
            nodeC.SetWalkable(true);

            // 3. 이웃 연결 (A<->B, B<->C)
            nodeA.AddNeighbor(nodeB, PathFinder.NeighborDirs[1]);
            nodeB.AddNeighbor(nodeA, PathFinder.NeighborDirs[4]);
            nodeB.AddNeighbor(nodeC, PathFinder.NeighborDirs[1]);
            nodeC.AddNeighbor(nodeB, PathFinder.NeighborDirs[4]);

            // 4. Find 호출 (A->C)
            var result = pathFinder.Find(nodeA, nodeC);

            // 5. 경로 검증 (A->B->C)
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(nodeA, result[0]);
            Assert.AreEqual(nodeB, result[1]);
            Assert.AreEqual(nodeC, result[2]);

            // 6. 오브젝트 정리
            Object.DestroyImmediate(pfObj);
            Object.DestroyImmediate(nodeAObj);
            Object.DestroyImmediate(nodeBObj);
            Object.DestroyImmediate(nodeCObj);

            yield return null;
        }
    }
}


