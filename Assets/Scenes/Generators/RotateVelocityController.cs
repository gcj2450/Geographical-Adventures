using UnityEngine;

public class RotateVelocityController : MonoBehaviour
{
    private Vector3 previousRotation;
    private float previousTime;

    private Vector3 currentRotationVelocity;

    private void Start()
    {
        previousRotation = transform.eulerAngles;
        previousTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // Store the initial rotation and time when mouse button is pressed
            previousRotation = transform.eulerAngles;
            previousTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            // Calculate rotation velocity when the mouse button is released
            Vector3 currentRotation = transform.eulerAngles;
            float currentTime = Time.time;

            // Calculate rotation change since the button was pressed
            Vector3 rotationDelta = currentRotation - previousRotation;

            // Calculate time elapsed since the button was pressed
            float deltaTime = currentTime - previousTime;

            // Calculate rotation velocity (rotation change / time)
            currentRotationVelocity = rotationDelta / deltaTime;
        }
    }

    private void FixedUpdate()
    {
        if (!Input.GetMouseButton(0))
        {
            // Gradually slow down the rotation velocity
            currentRotationVelocity *= 0.95f;
        }
    }
}