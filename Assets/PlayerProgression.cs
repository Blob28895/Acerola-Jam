using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgression : MonoBehaviour
{
	[Tooltip("Boolean for if the player is able to break through branches or not. If they are, the player should have horns")]
	[SerializeField] private bool canBreakBranches;

	[SerializeField] private PlayerMovement playerMovement;

	public bool canBreak()
	{
		return (canBreakBranches && playerMovement.isDashing());
	}
}
