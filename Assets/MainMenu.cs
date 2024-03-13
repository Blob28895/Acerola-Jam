using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

	[SerializeField] private GameObject fadeObject;
	public void StartGame()
	{
		SceneManager.LoadScene("Level 1");
	}

	public void fadeBackToMainMenu()
	{
		StartCoroutine(fade());
	}

	public IEnumerator fade()
	{
		fadeObject.GetComponent<Animator>().SetTrigger("Fade");
		yield return new WaitForSeconds(4f);
		SceneManager.LoadScene("Main Menu");
	}


}
