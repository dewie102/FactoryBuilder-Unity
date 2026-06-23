using System.Collections.Generic;
using UnityEngine;

public static class DirectionUtils
{
    private static readonly Dictionary<Direction, Vector3Int> _toVector = new()
    {
        {Direction.UP, new(0,1, 0)},
        {Direction.DOWN, new(0, -1, 0)},
        {Direction.LEFT, new(-1, 0, 0)},
        {Direction.RIGHT, new(1, 0, 0)}
    };

    private static readonly Dictionary<Vector3Int, Direction> _fromVector = new()
    {
        {new(0, 1, 0), Direction.UP},
        {new(0, -1, 0), Direction.DOWN},
        {new(-1, 0, 0), Direction.LEFT},
        {new(1, 0, 0), Direction.RIGHT},
    };

    public static Vector3Int ToVector3Int(Direction dir)
    {
        return _toVector[dir];
    }

    public static bool TryGetDirection(Vector3Int vec, out Direction dir)
    {
        return _fromVector.TryGetValue(vec, out dir);
    }

    public static Direction Reverse(Direction direction)
    {
        return direction switch
        {
            Direction.UP => Direction.DOWN,
            Direction.DOWN => Direction.UP,
            Direction.LEFT => Direction.RIGHT,
            Direction.RIGHT => Direction.LEFT,
            _ => direction,
        };
    }
}