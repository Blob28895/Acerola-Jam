using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProgression : MonoBehaviour
{
	private Transform startPosition;
	public static bool canBreakBranches;
	public static bool canDoubleJump;

	[Header("References")]
	[SerializeField] private PlayerMovement playerMovement;
	[SerializeField] private GameObject horns;
	[SerializeField] private GameObject wings;


	private void Awake()
	{
		if(SceneManager.GetActiveScene().name == "Level 3")
		{
			canBreakBranches = true; //This variable starts as false and will just become true when you start level 3
		}
		startPosition = GameObject.FindGameObjectWithTag("Spawn").transform;
		Debug.Log(startPosition);

		//If we have unlocked either of these things display that
		if(canBreakBranches) {	horns.SetActive(true);}
		if(canDoubleJump) {	wings.SetActive(true);}
	}

	public bool canBreak()
	{
		return (canBreakBranches && playerMovement.isDashing());
	}

	public Transform getStartPosition()
	{
		return startPosition;
	}

	public void activateWings()
	{
		canDoubleJump = true;
		GetComponent<PlayerMovement>().growWings();
		wings.SetActive(true);
	}
}
