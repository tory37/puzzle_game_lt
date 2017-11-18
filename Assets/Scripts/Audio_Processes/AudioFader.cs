using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioFader : MonoBehaviour
{
    //Each level/room gets an audiosource with that level/rooms music
    //attach this to each level/room to fade in and out between each room
    //works when player enters and exits each level/room

	private AudioSource audiosource;

	void Start()
	{
		audiosource = GetComponent<AudioSource>();

        //makes sure AudioSource is off to begin
		audiosource.enabled = false;

	}

	IEnumerator OnTriggerEnter(Collider other)
	{
        //Turn on AudioSource
		audiosource.enabled = true;

        //Interupts OnTriggerEnter2D to fade in
		yield return StartCoroutine(FadeIn());
	}

	IEnumerator OnTriggerExit(Collider other)
	{
        //Fade out first before AudioSource is turned off
		yield return StartCoroutine(FadeOut());

        //Turn off AudioSource
		audiosource.enabled = false;

	}

	IEnumerator FadeIn()
	{
		while (audiosource.volume < 1)
		{
			audiosource.volume += 1 * Time.deltaTime * .5f;

			yield return null;
		}
	}

	IEnumerator FadeOut()
	{
		while (audiosource.volume > 0)
		{
			audiosource.volume -= 1 * Time.deltaTime / 2;

			yield return null;
		}
	}
}