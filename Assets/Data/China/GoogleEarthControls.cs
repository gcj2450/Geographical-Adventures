using UnityEngine;

public class GoogleEarthControls : MonoBehaviour
{
    private const int SpehreRadius = 300;
    private Vector3? _mouseStartPos;
    private Vector3? _currentMousePos;

    void Start()
    {
        // init the camera to look at this object
        Vector3 cameraPos = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z - 209);

        Camera.main.transform.position = cameraPos;
        Camera.main.transform.LookAt(transform.position);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouseStartPos = GetMouseHit();
            Debug.Log(_mouseStartPos);
        }
        if (_mouseStartPos != null) HandleDrag();
        if (Input.GetMouseButtonUp(0)) HandleDrop();
    }

    private void HandleDrag()
    {
        _currentMousePos = GetMouseHit();
        RotateCamera((Vector3)_mouseStartPos, (Vector3)_currentMousePos);
    }

    private void HandleDrop()
    {
        _mouseStartPos = null;
        _currentMousePos = null;
    }

    private void RotateCamera(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        // in case the spehre model is not a perfect sphere..
        dragEndPosition = dragEndPosition.normalized * SpehreRadius;
        dragStartPosition = dragStartPosition.normalized * SpehreRadius;
        // calc a vertical vector to rotate around..
        var cross = Vector3.Cross(dragEndPosition, dragStartPosition);
        // calc the angle for the rotation..
        var angle = Vector3.SignedAngle(dragEndPosition, dragStartPosition, cross);
        // roatate around the vector..
        Camera.main.transform.RotateAround(transform.position, cross, angle);
    }

    /**
     * Projects the mouse position to the sphere and returns the intersection point. 
     */
    private static Vector3? GetMouseHit()
    {
        // make sure there is a shepre mesh with a colider centered at this game object
        // with a radius of SpehreRadius
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            return hit.point;
        }
        return null;
    }
}