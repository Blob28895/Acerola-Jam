using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bramble : MonoBehaviour
{
	//private void
	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{
			collision.gameObject.GetComponent<PlayerDeath>().Die();
		}
	}
}
