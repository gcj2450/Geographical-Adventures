using UnityEngine;

public class MouseVelocityDetector : MonoBehaviour
{
    private Vector3 previousMousePosition;
    private float previousTime;

    private Vector3 currentMouseVelocity;

    private void Start()
    {
        previousMousePosition = Input.mousePosition;
        previousTime = Time.time;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
        {

            Vector3 currentMousePosition = Input.mousePosition;
            float currentTime = Time.time;

            // Calculate distance moved since the previous frame
            Vector3 distanceMoved = currentMousePosition - previousMousePosition;

            // Calculate time elapsed since the previous frame
            float deltaTime = currentTime - previousTime;

            // Calculate velocity (distance / time)
            currentMouseVelocity = distanceMoved / deltaTime;

            // Store current values for the next frame
            previousMousePosition = currentMousePosition;
            previousTime = currentTime;

            // Output velocity for testing
            Debug.Log("Mouse Velocity: " + currentMouseVelocity);
        }
    }
}