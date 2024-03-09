using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DialogueController : MonoBehaviour
{

	private bool isTalking = false;
    private PlayerMovement player;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.CompareTag("Player"))
		{
			player = collision.gameObject.GetComponent<PlayerMovement>();
			player.setCanMove(false);
			isTalking = true;
		}
	}

	private void Update()
	{
		if (!isTalking) { return; }

		if(Input.GetKeyDown(KeyCode.Space))
		{

		}
	}
}
