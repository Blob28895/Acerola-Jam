using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseRun : MonoBehaviour
{
	[SerializeField] private GameObject level4BackwardsTransition;
	public void StartReverseRun()
	{
		level4BackwardsTransition.SetActive(true);
		GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerProgression>().activateWings();
	}
}
