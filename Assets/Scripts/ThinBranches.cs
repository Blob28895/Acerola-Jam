using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThinBranches : MonoBehaviour
{

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{
			if (collision.gameObject.GetComponent<PlayerProgression>().canBreak()) { 
			gameObject.SetActive(false);

			}
		}
	}
}
