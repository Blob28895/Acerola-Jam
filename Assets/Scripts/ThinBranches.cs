using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ThinBranches : MonoBehaviour
{
	[SerializeField] private ParticleSystem particles;
	[SerializeField] private SpriteRenderer spriteRenderer;
	[SerializeField] private BoxCollider2D boxCollider;

	private void Awake()
	{
		particles.Stop();
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if(collision.gameObject.CompareTag("Player"))
		{
			if (collision.gameObject.GetComponent<PlayerProgression>().canBreak())
			{
				boxCollider.enabled = false;
				StartCoroutine(demolish());

			}
		}
	}

	private IEnumerator demolish()
	{
		GetComponent<AudioSource>().Play();
		spriteRenderer.enabled = false;
		particles.Play();
		spriteRenderer.enabled = false;
		yield return new WaitForSeconds(0.5f);
		particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}
}
