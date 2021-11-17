using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConfig : MonoBehaviour
{
    public bool southDoorAvailable = true;
    public bool northDoorAvailable = true;
    public bool westDoorAvailable = true;
    public bool eastDoorAvailable = true;

    public Difficulty difficulty = Difficulty.easy;
    public bool HasKey = false;
}
