using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : MonoBehaviour
{
	public Teleport binome;

	private void OnTriggerStay2D(Collider2D collision)
	{
		if (Player.Instance == null)
			return;
		if (collision.attachedRigidbody.gameObject != Player.Instance.gameObject)
			return;

		if(binome == null)
        {
			Debug.LogError("Binome not set !");
			return;
        }

		Debug.Log("Player enter in teleporter");
		Player.Instance.TeleportTo(binome.transform.position);
    }
}
