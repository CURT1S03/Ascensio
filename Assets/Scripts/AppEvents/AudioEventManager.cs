using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class AudioEventManager : MonoBehaviour
{

    public EventSound3D eventSound3DPrefab;
    public AudioClip playerLandsAudio;
    public AudioClip[] grassStepAudio;
    public AudioClip[] hardStepAudio;
    public AudioClip hitAudio;
    public AudioClip hissAudio;
    private EventSound3D hissEventSound;
    public AudioClip[] growlAudio;
    public AudioClip roarAudio;
    private UnityAction<Vector3, float> hitGroundEventListener;
    private UnityAction<Vector3, float, GameObject> footstepEventListener;
    private UnityAction<Vector3, float> enemyCollisionEventListener;
    private UnityAction<Vector3> hissEventListener;
    private UnityAction<Vector3> tigerGrowlEventListener;
    private EventSound3D growlEventSound;
    private UnityAction<Vector3> tigerRoarEventListener;

    void Awake()
    {
        hitGroundEventListener = new UnityAction<Vector3, float>(HitGroundEventHandler);

        footstepEventListener = new UnityAction<Vector3, float, GameObject>(FootstepEventHandler);

        enemyCollisionEventListener = new UnityAction<Vector3, float>(EnemyCollisionEventHandler);

        hissEventListener = new UnityAction<Vector3>(HissEventHandler);

        tigerGrowlEventListener = new UnityAction<Vector3>(TigerGrowlEventHandler);

        tigerRoarEventListener = new UnityAction<Vector3>(TigerRoarEventHandler);

    }


    // Use this for initialization
    void Start()
    {



    }


    void OnEnable()
    {
        EventManager.StartListening<HitGroundEvent, Vector3, float>(hitGroundEventListener);
        EventManager.StartListening<FootstepEvent, Vector3, float, GameObject>(footstepEventListener);
        EventManager.StartListening<EnemyCollisionEvent, Vector3, float>(enemyCollisionEventListener);
        EventManager.StartListening<HissEvent, Vector3>(hissEventListener);
        EventManager.StartListening<TigerGrowlEvent, Vector3>(tigerGrowlEventListener);
        EventManager.StartListening<TigerRoarEvent, Vector3>(tigerRoarEventListener);
    }

    void OnDisable()
    {
        EventManager.StopListening<HitGroundEvent, Vector3, float>(hitGroundEventListener);
        EventManager.StopListening<FootstepEvent, Vector3, float, GameObject>(footstepEventListener);
        EventManager.StopListening<EnemyCollisionEvent, Vector3, float>(enemyCollisionEventListener);
        EventManager.StopListening<HissEvent, Vector3>(hissEventListener);
        EventManager.StopListening<TigerGrowlEvent, Vector3>(tigerGrowlEventListener);
        EventManager.StopListening<TigerRoarEvent, Vector3>(tigerRoarEventListener);
    }

    void HitGroundEventHandler(Vector3 worldPos, float collisionMagnitude)
    {
        //AudioSource.PlayClipAtPoint(this.explosionAudio, worldPos, 1f);

        if (eventSound3DPrefab)
        {
            //minimum force required to play land sound effect volume
            float minForce = 6f;
            //force required for maximum land sound effect volume
            float maxForce = 60f;
            float maxVolume = 1f;
            if (collisionMagnitude > minForce)
            {
                //TO-DO: add terrain checking (see footstepEventHandler)
                EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);

                //Use a linear scale to modify volume based on collision magnitude
                snd.audioSrc.volume = (Mathf.Min(collisionMagnitude, maxForce) - minForce) / (maxForce - minForce) * maxVolume;
                snd.audioSrc.clip = this.playerLandsAudio;

                //Debug.Log(Time.time + " land magnitude: " + collisionMagnitude);

                snd.audioSrc.minDistance = 5f;
                snd.audioSrc.maxDistance = 100f;

                snd.audioSrc.Play();
            }
        }
    }

    void FootstepEventHandler(Vector3 pos, float footstepWeight, GameObject ground)
    {

        //Get terrain
        //checkTerrainTexture.GetTerrainTexture();
        // When we implement terrains later, can access terrain with checkTerrainTexture.t
        //      and change sound accordingly
        // TO-DO: figure out how to get call GetTerrainTexture from the correct GameObject

        if (eventSound3DPrefab)
        {
            EventSound3D snd = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);

            // Avoid divide by 0 error
            if (footstepWeight == 0)
                return;
            snd.audioSrc.pitch = UnityEngine.Random.Range(.9f, 1.1f) / footstepWeight * 1.2f;

            snd.audioSrc.volume = Mathf.Clamp(footstepWeight, 0, 2);

            //set footstep array based on tag of ground character is on
            if(ground.CompareTag("ground") || ground.CompareTag("Plane"))
            snd.audioSrc.clip = this.grassStepAudio[UnityEngine.Random.Range(0, grassStepAudio.Length)];
            /**else if (ground.CompareTag("wood"))
            snd.audioSrc.clip = this.grassStepAudio[UnityEngine.Random.Range(0, grassStepAudio.Length)];
            **/else
            snd.audioSrc.clip = this.grassStepAudio[UnityEngine.Random.Range(0, grassStepAudio.Length)];

            snd.audioSrc.minDistance = 5f;
            snd.audioSrc.maxDistance = 100f;

            snd.audioSrc.Play();
        }

    }

    void EnemyCollisionEventHandler(Vector3 worldPos, float collisionMagnitude)
    {
        if (eventSound3DPrefab)
        {
            EventSound3D snd = Instantiate(eventSound3DPrefab, worldPos, Quaternion.identity, null);
            snd.audioSrc.clip = this.hitAudio;
            
            float maxForce = .4f;
            float minForce = .025f;
            float maxVolume = 1f;

            // Scale volume based on collsion force
            snd.audioSrc.volume = (Mathf.Min(collisionMagnitude, maxForce) - minForce) / (maxForce - minForce) * maxVolume;

            snd.audioSrc.minDistance = 5f;
            snd.audioSrc.maxDistance = 100f;

            snd.audioSrc.Play();
        }
    }

    void HissEventHandler(Vector3 pos)
    {
        if (hissAudio)
        {
            if (hissEventSound)
                if (hissEventSound.audioSrc.isPlaying)
                    return;
            hissEventSound = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);

            hissEventSound.audioSrc.minDistance = 5f;
            hissEventSound.audioSrc.maxDistance = 100f;

            hissEventSound.audioSrc.clip = this.hissAudio;
            hissEventSound.audioSrc.Play();
        }
    }

    void TigerGrowlEventHandler(Vector3 pos)
    {
        if (eventSound3DPrefab)
        {
            //Don't play a sound effect if one is already playing
            if (growlEventSound)
                if (growlEventSound.audioSrc.isPlaying)
                    return;
            growlEventSound = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);
            growlEventSound.audioSrc.clip = this.growlAudio[UnityEngine.Random.Range(0, growlAudio.Length)];

            growlEventSound.audioSrc.minDistance = 5f;
            growlEventSound.audioSrc.maxDistance = 100f;

            //Debug.Log("Playing growl");
            growlEventSound.audioSrc.Play();
        }
    }

    void TigerRoarEventHandler(Vector3 pos)
    {
        if (roarAudio && eventSound3DPrefab)
        {
            //if it's making noise, fade that out first
            if (growlEventSound)
            {
                StartCoroutine(FadeOut(growlEventSound.audioSrc, .5f));
            }

            growlEventSound = Instantiate(eventSound3DPrefab, pos, Quaternion.identity, null);

            growlEventSound.audioSrc.minDistance = 5f;
            growlEventSound.audioSrc.maxDistance = 100f;

            //Debug.Log("Playing roar");
            growlEventSound.audioSrc.clip = this.roarAudio;
            growlEventSound.audioSrc.Play();
        }
    }

    IEnumerator FadeOut(AudioSource src, float fadeTime)
    {
        if (src.isPlaying)
        {
            Debug.Log("Growl playing! Fading out.");
            float speed = src.volume / (10f * fadeTime);
            while (src.volume > 0)
            {
                src.volume -= speed * .1f;
                yield return new WaitForSeconds(.1f);
                //if the AudioSource no longer exists after waiting, exit the loop
                if (!src)
                    break;
            }
            
            if (src)
                src.Stop();
        }
    }
}
