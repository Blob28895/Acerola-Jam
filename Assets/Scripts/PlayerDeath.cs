using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    [SerializeField] private Vector3 startPosition;
    // Start is called before the first frame update
    void Start()
    {
        startPosition = transform.position;
    }

	public void Die()
	{
		GameObject[] checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		foreach (GameObject checkpoint in checkpoints)
		{
			if (checkpoint.GetComponent<Checkpoint>().isActivated())
			{
				gameObject.transform.position = checkpoint.transform.position;
				return;
			}
		}
		transform.position = startPosition;
	}
}
