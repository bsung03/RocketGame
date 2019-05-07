using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class Rocket : MonoBehaviour
{

    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 1000f;
    [SerializeField] float levelLoadDelay;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip WinSound;
    [SerializeField] AudioClip DeathSound;

    [SerializeField] ParticleSystem EnginePart;
    [SerializeField] ParticleSystem WinPart;
    [SerializeField] ParticleSystem DeathPart;
    Rigidbody rigidBody;
    AudioSource audioSource;

    enum State {Alive, Dead, Trancending}
    State state = State.Alive;
    
    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(state == State.Alive){
            Thrust();
            Rotation();
        }
    }

    void OnCollisionEnter(Collision collision){

        if(state != State.Alive){return;}

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;

            case "Enemy":
                state = State.Dead;
                audioSource.Stop();
                audioSource.PlayOneShot(DeathSound);
                EnginePart.Stop();
                DeathPart.Play();
                Invoke("LoadFirstLevel", levelLoadDelay);
                break;

            case "Win" :
                state = State.Trancending;
                audioSource.Stop();
                audioSource.PlayOneShot(WinSound);
                EnginePart.Stop();
                WinPart.Play();
                Invoke("LoadNextLevel", levelLoadDelay);
                break;
        }
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1);
    }

    private void Rotation()
    {
        rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.A))
        {
            float rotationThisFrame = rcsThrust * Time.deltaTime;
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            float rotationThisFrame = rcsThrust * Time.deltaTime;
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false;
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            float thrustThisFrame = mainThrust * Time.deltaTime;
            rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
            if (audioSource.isPlaying == false)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            EnginePart.Play();
        }
        else
        {
            audioSource.Stop();
            EnginePart.Stop();
        }
    }
}
