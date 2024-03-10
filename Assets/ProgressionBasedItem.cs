using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressionBasedItem : MonoBehaviour
{
    [Tooltip("If this boolean is checked the item will appear before the player unlocks double jump")]
    [SerializeField] private bool ActiveOnFirstRun;
    [Tooltip("If this boolean is checked the item will appear after the player unlocks double jump")]
    [SerializeField] private bool ActiveOnSecondRun;

	private void Awake()
	{
        bool secondRun = PlayerProgression.canDoubleJump;
        if(secondRun && ActiveOnSecondRun)
        {
            gameObject.SetActive(true);
        }
        else if(secondRun && !ActiveOnSecondRun)
        {
            gameObject.SetActive(false);
        }
        else if(!secondRun && !ActiveOnFirstRun)
        {
            gameObject.SetActive(false);
        }

	}

	// Update is called once per frame
	void Update()
    {
        
    }
}
