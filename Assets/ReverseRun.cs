using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReverseRun : MonoBehaviour
{

	public void StartReverseRun()
	{
		GameObject.FindGameObjectWithTag("Transition").SetActive(true);
	}
}
