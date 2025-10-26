using System.Collections;
using UnityEngine;

public class MusicBehavior : MonoBehaviour
{
    private AudioSource music;
    private bool keepFadingIn = false;
    private bool keepFadingOut = false;
    
    [Header("Fade Time")]
    [Tooltip("Length of music fade in/out in seconds")]
    public float fadeTime = 1;

    [Header("Max Volume")]
    [Tooltip("Maximum Music Volume")]
    public float maxVolume = 1;

    void Awake()
    {
        music = GetComponent<AudioSource>();

        if (music == null)
            Debug.Log("Audio Source could not be found");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeIn(fadeTime, maxVolume));
    }

    // Update is called once per frame
    void Update()
    {

    }

    //TO-DO: add behavior for when pause menu is opened


    /**
    Code from: https://www.youtube.com/watch?v=qkhisBC1_zg
    **/
    IEnumerator FadeIn(float fadeTime, float maxVolume)
    {
        keepFadingIn = true;
        keepFadingOut = false;

        float speed = maxVolume / (10f * fadeTime);

        music.volume = 0f;
        music.Play();

        while (music.volume < maxVolume && keepFadingIn)
        {
            music.volume += speed;
            yield return new WaitForSeconds(.1f);
        }

    }

    IEnumerator FadeOut(float fadeTime)
    {
        keepFadingIn = true;
        keepFadingOut = false;

        float speed = maxVolume / (10f * fadeTime);

        while (music.volume >= speed && keepFadingIn)
        {
            music.volume -= speed;
            yield return new WaitForSeconds(.1f);
        }

        music.Stop();
    }
}
