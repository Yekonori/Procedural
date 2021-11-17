using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum DOORSTATE
{
    WALL = 0,
    OPEN = 1,
    CLOSED = 2,
    SECRET = 3,
}

public class Door : MonoBehaviour {
    public const string PLAYER_NAME = "Player";

    Utils.ORIENTATION _orientation = Utils.ORIENTATION.NONE;
	public Utils.ORIENTATION Orientation { get { return _orientation; } }

	DOORSTATE _state = DOORSTATE.OPEN;
	public DOORSTATE State { get { return _state; } }
	public GameObject closedGo = null;
    public GameObject openGo = null;
    public GameObject wallGo = null;
    public GameObject secretGo = null;

	private Room _room = null;

	public void Awake()
	{
		_room = GetComponentInParent<Room>();
	}

	public void Start()
    {
        Bounds roomBounds = _room.GetWorldRoomBounds();
        float ratio = roomBounds.size.x / roomBounds.size.y;
        Vector2 dir = transform.position - (_room.transform.position + roomBounds.center);
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y) * ratio)
        {
            _orientation = dir.x > 0 ? Utils.ORIENTATION.EAST : Utils.ORIENTATION.WEST;
        } else {
            _orientation = dir.y > 0 ? Utils.ORIENTATION.NORTH : Utils.ORIENTATION.SOUTH;
        }
        transform.rotation = Quaternion.Euler(0, 0, -Utils.OrientationToAngle(_orientation));
		if(closedGo.gameObject.activeSelf)
		{
			SetState(DOORSTATE.CLOSED);
		} else if (openGo.gameObject.activeSelf)
		{
			SetState(DOORSTATE.OPEN);
		} else if (wallGo.gameObject.activeSelf)
		{
			SetState(DOORSTATE.WALL);
		} else if (secretGo.gameObject.activeSelf)
		{
			SetState(DOORSTATE.SECRET);
		}
	}

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.parent != Player.Instance.gameObject.transform)
            return;

        switch (_state) {
            case DOORSTATE.CLOSED:
                if (Player.Instance.KeyCount > 0)
                {
                    Player.Instance.KeyCount--;
                    SetState(DOORSTATE.OPEN);
					Room nextRoom = GetNextRoom();
					if(nextRoom)
					{
						Door[] doors = nextRoom.GetComponentsInChildren<Door>(true);
						foreach(Door door in doors)
						{
							if (_orientation == Utils.OppositeOrientation(door.Orientation) && door._state == DOORSTATE.CLOSED)
							{
								door.SetState(DOORSTATE.OPEN);
							}
						}
					}
				}
                break;
        }
    }

	private Room GetNextRoom()
	{
		Vector2Int dir = Utils.OrientationToDir(_orientation);
		Room nextRoom = Room.allRooms.Find(x => x.position == _room.position + dir);
		return nextRoom;
	} 

    public void SetState(DOORSTATE state)
    {
        if (closedGo) { closedGo.SetActive(false); }
        if (openGo) { openGo.SetActive(false); }
        if (wallGo) { wallGo.SetActive(false); }
        if (secretGo) { secretGo.SetActive(false); }
        _state = state;
        switch(_state)
        {
            case DOORSTATE.CLOSED:
                if (closedGo) { closedGo.SetActive(true); }
                break;
            case DOORSTATE.OPEN:
                if (openGo) { openGo.SetActive(true); }
                break;
            case DOORSTATE.WALL:
                if (wallGo) { wallGo.SetActive(true); }
                break;
            case DOORSTATE.SECRET:
                if (secretGo) { secretGo.SetActive(true); }
                break;
        }
    }

}
