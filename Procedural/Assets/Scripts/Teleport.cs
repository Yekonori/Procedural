using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	public Teleport binome;
	public bool canEnter = true;

	public int teleportIndex = 0;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (Player.Instance == null)
			return;
		if (collision.attachedRigidbody.gameObject != Player.Instance.gameObject)
			return;

		if (binome == null)
        {
			Debug.LogError("Binome not set !");
			return;
        }
		if (!canEnter) return;

		binome.canEnter = false;

		Debug.Log("Player enter in teleporter n°" + teleportIndex);

		if(TeleportManager.Get.UpdateQueue(teleportIndex))
        {
			//Teleport to RoomSecret
        }
		else
        {
			Player.Instance.TeleportTo(binome.transform.position, binome.transform.parent.parent.GetComponent<Room>());
		}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
		if (Player.Instance == null)
			return;
		if (collision.attachedRigidbody.gameObject != Player.Instance.gameObject)
			return;

		if (!canEnter) canEnter = true;
	}

	public void SetValues(Teleport teleport, int index)
    {
		binome = teleport;
		teleportIndex = index;
	}
}
