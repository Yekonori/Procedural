using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomConfig : MonoBehaviour
{
    [Header("Room Position")]

    public bool isStartRoom = false;
    public bool isEndRoom = false;
    public bool isSecretRoom = false;

    [Header("Door available")]

    public bool southDoorAvailable = true;
    public bool northDoorAvailable = true;
    public bool westDoorAvailable = true;
    public bool eastDoorAvailable = true;

    [Header("Difficulty")]

    public Difficulty difficulty = Difficulty.easy;
    
    [Header("Key")]

    public bool HasKey = false;

    [Header("Teleporters")]

    public bool HasTeleporterA = false;
    public bool HasTeleporterB = false;
    public bool HasTeleporterC = false;
    public bool HasTeleporterD = false;
    public bool HasTeleporterE = false;
    public bool HasTeleporterF = false;
}
