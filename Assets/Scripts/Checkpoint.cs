using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	private GameObject[] checkpoints;
	private bool activated = false;
	private float particleEmissionRate;

	[Tooltip("How long the checkpoint activation particles are active")]
	[SerializeField] private float particleEmissionDuration;
	[SerializeField] private Animator animator;
	[SerializeField] private ParticleSystem particles;

	private void Awake()
	{
		checkpoints = GameObject.FindGameObjectsWithTag("Checkpoint");
		particleEmissionRate = particles.emissionRate;
		particles.emissionRate = 0f;
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
			StartCoroutine(activateParticles());
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

	private IEnumerator activateParticles()
	{
		particles.emissionRate = particleEmissionRate;
		yield return new WaitForSeconds(particleEmissionDuration);
		particles.emissionRate = 0;
	}
}
