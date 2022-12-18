using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Movement : MonoBehaviour
{

    [SerializeField] private bool canMove;

    [SerializeField] private CharacterController characterBody;
    [SerializeField] private Transform characterPos;

    public float playerSpeed = 5f;
    [SerializeField] private float runSpeed;
    private float originalPlayerSpeed;

    bool startsRunning;
    bool running;
    [SerializeField] private int maxStamina;
    [SerializeField] private int currentStamina;
    [SerializeField] private int staminaLossRate;
    [SerializeField] private int staminaGainRate;

    bool isMoving;

    bool playingWalkingSound;
    bool playingLowStaminaNoise;
    bool playingScaredSound;
    bool playingOutOfStaminaNoise;

    [SerializeField] private AudioSource movementSounds;
    [SerializeField] private AudioSource lowStaminaSound;
    [SerializeField] private AudioSource scaredSound;
    [SerializeField] private AudioSource outOfStaminaSound;
    
    [SerializeField] private Volume staminaExhaustionFilter;

    // Start is called before the first frame update
    private void Start()
    {
        originalPlayerSpeed = playerSpeed;
        canMove = true;
        currentStamina = maxStamina;
        startsRunning = false;
        running = false;
        isMoving = false;
        playingWalkingSound = false;

        playingLowStaminaNoise = false;
        playingScaredSound = false;
        playingOutOfStaminaNoise = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if(canMove)
        {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;
            if(move != Vector3.zero) isMoving = true;
            else isMoving = false; 

            characterBody.Move(move * playerSpeed * Time.deltaTime);
        }

        if(running && !playingLowStaminaNoise && currentStamina < maxStamina/2)
        {
            StartCoroutine(PlayRunNoises());
        }
    }

    private void FixedUpdate()
    {
        if(startsRunning && !running)
        {
            running = true;
            startsRunning = false;
            StartCoroutine(CharacterRun());
        }
    }

    private void OnGUI()
    {
        if(Input.GetKeyDown(KeyCode.Space) && Input.GetKey(KeyCode.W) && !running && currentStamina > maxStamina/4)
        { 
            startsRunning = true;
        }

        if(Input.GetKeyUp(KeyCode.Space) && running)
        {
            running = false;
        }

        if((Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)) && !playingWalkingSound && isMoving) 
        {
            StartCoroutine(PlayMovementSounds());
        }
    }

    public void MoveCharacter(Vector3 position)
    {
        characterPos.position = position;
    }

    public IEnumerator CharacterRun()
    {
        playerSpeed = runSpeed;
        StartCoroutine(ReduceStamina());
        while(Input.GetKey(KeyCode.Space) && Input.GetKey(KeyCode.W) && currentStamina > 0)
        {
            yield return null;
        }

        running = false;

        StartCoroutine(ReplenishStamina());
        StopCoroutine(CharacterRun());
    }

    public IEnumerator ReduceStamina()
    {
        while(Input.GetKey(KeyCode.Space) && currentStamina > 0)
        {
            currentStamina -= staminaLossRate;
            yield return new WaitForSeconds(1f);
        }
        StopCoroutine(ReduceStamina());
    }

    public IEnumerator ReplenishStamina()
    {
        playerSpeed = originalPlayerSpeed;
        while(Input.GetKey(KeyCode.Space)) yield return null;
        while(!startsRunning && !running && currentStamina < maxStamina)
        {
            currentStamina += staminaGainRate;
            yield return new WaitForSeconds(1f);
        }
        StopCoroutine(ReplenishStamina());
    }

    public IEnumerator PlayMovementSounds()
    {
        playingWalkingSound = true;
        //movementSounds.Play();
        if(!running) yield return new WaitForSeconds(.7f);
        else
        { 
            yield return new WaitForSeconds(.3f);
        }
        playingWalkingSound = false;
        StopCoroutine(PlayMovementSounds());
    }

    public IEnumerator PlayRunNoises()
    {
        //Look for a better way to code this
        outOfStaminaSound.Stop();
        lowStaminaSound.Stop();
        playingLowStaminaNoise = true;
        lowStaminaSound.Play();
        while(running) yield return null;
        if(currentStamina < 2) 
        {
            lowStaminaSound.Stop();
            outOfStaminaSound.Play();
            // if (staminaExhaustionFilter.profile.TryGet<Vignette>(out var vignette))
            //     {
            //         vignette.intensity.overrideState = true;
            //         while(vignette.intensity.value < .3f) 
            //         {
            //             vignette.intensity.value += .1f;
            //             yield return new WaitForSeconds(.1f);
            //         }
            //     }
            playingOutOfStaminaNoise = true;
            playingLowStaminaNoise = false;
        }
        while(!startsRunning && currentStamina < maxStamina/4)
        {
            yield return null;
        }
        playingLowStaminaNoise = false;
        playingOutOfStaminaNoise = false;
        //lowStaminaSound.Stop();
        //outOfStaminaSound.Stop();
        // if (staminaExhaustionFilter.profile.TryGet<Vignette>(out var vignette2))
        // {
        //     vignette2.intensity.overrideState = true;
        //         while(vignette2.intensity.value > 0) 
        //         {
        //             vignette2.intensity.value -= .1f;
        //             yield return new WaitForSeconds(.1f);
        //         }
        // }
        StopCoroutine(PlayRunNoises());
    }
}
