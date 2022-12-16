using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //This camera controller is for the use of a first person 3d game
    [SerializeField] private float mouseSensitivity = 100f;
    public Transform playerBody;

    [SerializeField] private Camera camera;

    float xRotation = 0f;

    [SerializeField] private bool lockCamera = true;

    [SerializeField] private Player player;

    [SerializeField] private LayerMask interactables;
    [SerializeField] private GameObject cameraDot;

    public bool canLookAround;

    // Start is called before the first frame update
    private void Start()
    {
        if(lockCamera) Cursor.lockState = CursorLockMode.Locked;
        else Cursor.lockState = CursorLockMode.None;
        gameObject.transform.rotation = Quaternion.Euler(0,0,0);
        playerBody.rotation = Quaternion.Euler(0,0,0);

        canLookAround = true;
    }

    // Update is called once per frame
    private void Update()
    {
        if(canLookAround)
        {
            float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            xRotation -= mouseY;
            xRotation = Mathf.Clamp(xRotation, -90f, 90f);

            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            playerBody.Rotate(Vector3.up * mouseX);
        }

        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits;
        // Casts the ray and get the first game object hit
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.red);
        bool didHit = Physics.Raycast(ray, out RaycastHit hit, 5, interactables);
        //Issue: bool didHit is always false, even if the object is an interactable.
    }
}
