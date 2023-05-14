using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 6f;

    private Rigidbody rb;
    private Camera viewCamera;
    private Vector3 velocity;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        viewCamera = Camera.main;
    }

    void Update()
    {
        // get the direction from player's input
        Vector3 input = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        Quaternion cameraRotation = Quaternion.Euler(0, viewCamera.transform.eulerAngles.y, 0);
        Vector3 rotatedInput = cameraRotation * input;

        velocity = rotatedInput.normalized * moveSpeed;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }
}