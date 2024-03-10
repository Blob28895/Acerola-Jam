using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerProgression : MonoBehaviour
{
	[Tooltip("Boolean for if the player is able to break through branches or not. If they are, the player should have horns")]
	[SerializeField] public static bool canBreakBranches;
	[SerializeField] public static bool canDoubleJump;

	[SerializeField] private PlayerMovement playerMovement;

	private Transform startPosition;

	private void Awake()
	{
		if(SceneManager.GetActiveScene().name == "Level 3")
		{
			canBreakBranches = true;
		}
		startPosition = GameObject.FindGameObjectWithTag("Spawn").transform;
		Debug.Log(startPosition);
	}

	public bool canBreak()
	{
		return (canBreakBranches && playerMovement.isDashing());
	}

	public Transform getStartPosition()
	{
		return startPosition;
	}
}
