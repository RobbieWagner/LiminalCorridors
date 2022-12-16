using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadBobController : MonoBehaviour
{
    [SerializeField] private bool enabled = true;

    [SerializeField, Range(0, 0.1f)] private float amplitude = 0.015f;
    [SerializeField, Range(0, 30f)] private float frequency = 10f;

    [SerializeField] private Transform camera;
    [SerializeField] private Transform cameraHolder;

    private float toggleSpeed = 10f;
    private Vector3 startPos;
    private CharacterController playerCharacterController;
    private Movement playerMovement;
    // Start is called before the first frame update
    private void Start() {
        playerCharacterController = gameObject.GetComponent<CharacterController>();
        playerMovement = gameObject.GetComponent<Movement>();
        startPos = camera.localPosition;
    }

    private void Update() {
        if(!enabled) return;

        CheckSpeed();
        ResetPosition();
        camera.LookAt(FocusTarget());
    }

    private void PlayMotion(Vector3 motion) {
        camera.localPosition += motion;
    }

    private void CheckSpeed() {
        if(playerMovement.playerSpeed >= toggleSpeed)

        PlayMotion(FootstepMotion());
    }

    private Vector3 FootstepMotion() {
        Vector3 pos = Vector3.zero;
        pos.y += Mathf.Sin(Time.time * frequency) * amplitude;
        pos.x += Mathf.Cos(Time.time * frequency / 2) * amplitude * 2;
        return pos; 
    }

    private void ResetPosition() {
        if(camera.localPosition == startPos) return;
        camera.localPosition = Vector3.Lerp(camera.localPosition, startPos, 1*Time.deltaTime);
    }

    private Vector3 FocusTarget() {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y - cameraHolder.localPosition.y, transform.position.z);
        pos += cameraHolder.forward * 15f;
        return pos;
    }
}
