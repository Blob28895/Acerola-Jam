using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
		startPosition = gameObject.GetComponent<PlayerProgression>().getStartPosition().position;
		gameObject.transform.position = startPosition;
    }

	public void Die()
	{
		GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		Vector3 respawnPosition = startPosition;
		foreach (GameObject checkpoint in checkpoints)
		{
			if (checkpoint.GetComponent<Checkpoint>().isActivated())
			{
				gameObject.transform.position = checkpoint.transform.position;
				return;
			}
		}

		StartCoroutine(respawn(respawnPosition));
	}

	//The reason that im using this function instead of animator transition times is because I want to delay the player's respawn too.
	private IEnumerator respawn(Vector3 respawnPos)
	{
		Rigidbody2D rb = GetComponent<Rigidbody2D>();
		RigidbodyConstraints2D constraints = rb.constraints;
		PlayerMovement movement = gameObject.GetComponent<PlayerMovement>();

		movement.animator.SetTrigger("Death");
		movement.setCanMove(false);
		rb.constraints = RigidbodyConstraints2D.FreezePosition;
		
		yield return new WaitForSeconds(0.25f);

		gameObject.transform.position = respawnPos;
		rb.constraints = constraints;
		movement.setCanMove(true);
		movement.animator.SetTrigger("EndDeath");
	}
}
