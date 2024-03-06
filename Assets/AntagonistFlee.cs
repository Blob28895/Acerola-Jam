using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntagonistFlee : MonoBehaviour
{

    [SerializeField] private ParticleSystem particles;
	[SerializeField] private SpriteRenderer spriteRenderer;

	private void Awake()
	{
		particles.Stop();
	}

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.gameObject.CompareTag("Player") && spriteRenderer.enabled)
		{
			StartCoroutine(flee());
		}
	}

	private IEnumerator flee()
	{
		particles.Play();
		spriteRenderer.enabled = false;
		yield return new WaitForSeconds(0.5f);
		particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}

}
