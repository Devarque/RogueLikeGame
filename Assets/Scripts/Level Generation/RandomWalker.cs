using System.Collections.Generic;
using UnityEngine;

public class RandomWalker : MonoBehaviour{
    [SerializeField] protected TileMapManager tileMapVisualizer = null;
    [SerializeField] protected Vector2Int startPosition = Vector2Int.zero;
    [SerializeField] protected ScriptableObjectForRandomWalker randomWalkParemeters;

    //RandomWalker temel method
    //Ek bilgi: https://en.wikipedia.org/wiki/Random_walk
    protected HashSet<Vector2Int> RunRandomWalk(ScriptableObjectForRandomWalker parameters, Vector2Int position) {
        Vector2Int currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        var iteration = Random.Range(parameters.minIterations, parameters.maxIterations);
        for (int i = 0; i < iteration; i++) {
            var path = ProceduralGenerationAlgorithms.RandomWalkerFloor(currentPosition, parameters); 
            floorPositions.UnionWith(path);                                 
        }
        return floorPositions;      
    }
}
