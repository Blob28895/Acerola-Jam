using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class ReverseRun : MonoBehaviour
{
	[SerializeField] private GameObject level4BackwardsTransition;
	[SerializeField] private GameObject level4Rocks;
	public void StartReverseRun()
	{
		level4BackwardsTransition.SetActive(true);
		StartCoroutine(rockFall());
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerProgression>().activateWings();
		
	}

	private IEnumerator rockFall()
	{
		ParticleSystem particles = level4Rocks.GetComponent<ParticleSystem>();
		level4Rocks.SetActive(true);
		particles.Play();
		yield return new WaitForSeconds(0.25f);
		particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
	}
}
