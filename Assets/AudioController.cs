using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
	[SerializeField] private AudioSource source;
	[SerializeField] GameObject soundEffectObject;

	public void playSoundScript(AudioClip ac, float volume)
	{
		GameObject obj = Instantiate(soundEffectObject);
		obj.GetComponent<AudioSource>().clip = ac;
		obj.GetComponent<AudioSource>().volume = volume;
		obj.GetComponent<AudioSource>().Play();
		Destroy(obj, 3f);
	}
	public void playSoundAE(AnimationEvent av)
	{
		GameObject obj = Instantiate(soundEffectObject);
		obj.GetComponent<AudioSource>().clip = (AudioClip) av.objectReferenceParameter;
		obj.GetComponent<AudioSource>().volume = av.floatParameter;
		obj.GetComponent<AudioSource>().Play();
		Destroy(obj, 3f);
	}

	
}
