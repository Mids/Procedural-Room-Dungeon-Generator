using UnityEngine;

public enum MazeDirection {
    North,
    East,
    South,
    West
}

public static class MazeDirections
{
    public const int Count = 4;

    public static MazeDirection RandomValue
    {
        get
        {
            return (MazeDirection)Random.Range(0, Count);
        }
    }
}