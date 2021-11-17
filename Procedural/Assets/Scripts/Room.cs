using CreativeSpore.SuperTilemapEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorType
{
	NORTH,
	SOUTH,
	WEST,
	EAST,
	NORTH_SOUTH,
	WEST_EAST,
	NORTH_WEST,
	NORTH_EAST,
	SOUTH_WEST,
	SOUTH_EAST,
	NORTH_WEST_EAST,
	NORTH_WEST_SOUTH,
	NORTH_EAST_SOUTH,
	SOUTH_WEST_EAST,
	FULL,
}

public class Room : MonoBehaviour {

    public bool isStartRoom = false;
	public Vector2Int position = Vector2Int.zero;

	public Door NorthDoor;
	public Door SouthDoor;
	public Door WestDoor;
	public Door EastDoor;

	public DoorType doorType;

	private TilemapGroup _tilemapGroup;

	public static List<Room> allRooms = new List<Room>();

    void Awake()
    {
		_tilemapGroup = GetComponentInChildren<TilemapGroup>();
		allRooms.Add(this);
	}

	private void OnDestroy()
	{
		allRooms.Remove(this);
	}

	void Start () {
        if(isStartRoom)
        {
            OnEnterRoom();
        }
    }
	
	public void OnEnterRoom()
    {
        CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
        Bounds cameraBounds = GetWorldRoomBounds();
        cameraFollow.SetBounds(cameraBounds);
		Player.Instance.EnterRoom(this);
    }


	public Bounds GetLocalRoomBounds()
    {
		Bounds roomBounds = new Bounds(Vector3.zero, Vector3.zero);
		if (_tilemapGroup == null)
			return roomBounds;

		foreach (STETilemap tilemap in _tilemapGroup.Tilemaps)
		{
			Bounds bounds = tilemap.MapBounds;
			roomBounds.Encapsulate(bounds);
		}
		return roomBounds;
    }

    public Bounds GetWorldRoomBounds()
    {
        Bounds result = GetLocalRoomBounds();
        result.center += transform.position;
        return result;
    }

	public bool Contains(Vector3 position)
	{
		position.z = 0;
		return (GetWorldRoomBounds().Contains(position));
	}

	public void SetDoor(DoorType door)
    {
        switch (door)
        {
            case DoorType.NORTH:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.WALL);
                EastDoor.SetState(Door.STATE.WALL);
                WestDoor.SetState(Door.STATE.WALL);
                break;

            case DoorType.SOUTH:
                NorthDoor.SetState(Door.STATE.WALL);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.WALL);
                WestDoor.SetState(Door.STATE.WALL);
                break;

            case DoorType.WEST:
                NorthDoor.SetState(Door.STATE.WALL);
                SouthDoor.SetState(Door.STATE.WALL);
                EastDoor.SetState(Door.STATE.WALL);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            case DoorType.EAST:
                NorthDoor.SetState(Door.STATE.WALL);
                SouthDoor.SetState(Door.STATE.WALL);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.WALL);
                break;
            
            case DoorType.NORTH_SOUTH:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.WALL);
                WestDoor.SetState(Door.STATE.WALL);
                break;
            
            case DoorType.WEST_EAST:
                NorthDoor.SetState(Door.STATE.WALL);
                SouthDoor.SetState(Door.STATE.WALL);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            case DoorType.NORTH_WEST:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.WALL);
                EastDoor.SetState(Door.STATE.WALL);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            case DoorType.NORTH_EAST:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.WALL);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.WALL);
                break;
            
            case DoorType.SOUTH_WEST:
                NorthDoor.SetState(Door.STATE.WALL);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.WALL);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            case DoorType.SOUTH_EAST:
                NorthDoor.SetState(Door.STATE.WALL);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.WALL);
                break;
            
            case DoorType.NORTH_WEST_EAST:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.WALL);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            case DoorType.NORTH_WEST_SOUTH:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.WALL);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            case DoorType.NORTH_EAST_SOUTH:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.WALL);
                break;
            
            case DoorType.SOUTH_WEST_EAST:
                NorthDoor.SetState(Door.STATE.WALL);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            case DoorType.FULL:
                NorthDoor.SetState(Door.STATE.OPEN);
                SouthDoor.SetState(Door.STATE.OPEN);
                EastDoor.SetState(Door.STATE.OPEN);
                WestDoor.SetState(Door.STATE.OPEN);
                break;
            
            default:
                break;
        }
    }
}
