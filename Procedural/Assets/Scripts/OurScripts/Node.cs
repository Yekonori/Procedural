using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public enum NodeType
    {
        standard,
        secret,
        start,
        end
    }

    public enum Difficulty
    {
        easy,
        medium,
        hard
    }

    public DOORSTATE doorLeftOpen = DOORSTATE.WALL;
    public DOORSTATE doorRightOpen = DOORSTATE.WALL;
    public DOORSTATE doorUpOpen = DOORSTATE.WALL;
    public DOORSTATE doorDownOpen = DOORSTATE.WALL;

    public Vector2Int position = Vector2Int.zero;
    public NodeType nodeType = NodeType.standard;
    public Difficulty difficulty = Difficulty.easy;
}
