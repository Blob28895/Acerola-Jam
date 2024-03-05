using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Checkpoint : MonoBehaviour
{
	private GameObject[] checkpoints;
	private bool activated = false;

	[SerializeField] private Animator animator;

	private void Start()
	{
		checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
	}
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{
			ActivateCheckpoint();

		}
	}
	private void ActivateCheckpoint()
	{
		if(!activated)
		{
			activated = true;
			animator.SetTrigger("Activate");
			foreach (GameObject go in checkpoints)
			{
				if (go != gameObject) {
					go.GetComponent<Checkpoint>().DeactivateCheckpoint();
				}
			}
		}
	}
	public void DeactivateCheckpoint()
	{
		if(activated)
		{
			animator.SetTrigger("Deactivate");
			activated = false;
		}
	}
	public bool isActivated()
	{
		return activated;
	} 
}
