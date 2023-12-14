using UnityEngine;
using DG.Tweening;

public class GoogleEarthCamera : MonoBehaviour
{
    /// <summary>
    /// 球体半径
    /// </summary>
    public int SphereRadius = 100;
    public float CameraDist = 200;
    private Vector3? _mouseStartPos;
    private Vector3? _currentMousePos;

    public GameObject CameraObj;
    public Transform XRot;
    public Transform YRot;

    void Start()
    {
        // init the camera to look at this object
        Vector3 cameraPos = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z - CameraDist);

        CameraObj.transform.position = cameraPos;
        CameraObj.transform.LookAt(transform.position);
    }

    public float zoomSpeed = 5f;
    public float minZoomDistance = 10f;
    public float maxZoomDistance = 100f;

    private float zoomDistance = 50f;
    private Vector3 previousMousePosition;
    float zoomInput = 0;
    Vector3 zoomVector = Vector3.zero;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) _mouseStartPos = GetMouseHit();
        if (_mouseStartPos != null) HandleDrag();
        if (Input.GetMouseButtonUp(0)) HandleDrop();

        // Zoom camera
        zoomInput = Input.GetAxis("Mouse ScrollWheel");
        zoomDistance -= zoomInput * zoomSpeed;
        zoomDistance = Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);

        zoomVector = transform.forward * -zoomInput * zoomSpeed;
        CameraObj.transform.position += zoomVector;

        previousMousePosition = Input.mousePosition;

    }

    private void HandleDrag()
    {
        _currentMousePos = GetMouseHit();
        if (_currentMousePos != null)
            RotateCamera((Vector3)_mouseStartPos, (Vector3)_currentMousePos);
        else
            HandleDrop();
    }

    private void HandleDrop()
    {
        _mouseStartPos = null;
        _currentMousePos = null;
    }

    private void RotateCamera(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        if (dragEndPosition==null)
        {
            Debug.Log("dragEndPosition==null");
            return;
        }
        // in case the spehre model is not a perfect sphere..
        dragEndPosition = dragEndPosition.normalized * SphereRadius;
        dragStartPosition = dragStartPosition.normalized * SphereRadius;
        // calc a vertical vector to rotate around..
        var cross = Vector3.Cross(dragEndPosition, dragStartPosition);

        // calc the angle for the rotation..
        var angle = Vector3.SignedAngle(dragEndPosition, dragStartPosition, cross);
        // roatate around the vector..
        transform.RotateAround(transform.position, cross, angle);

        //// calc the angle for the rotation..
        //var angleX = Vector3.SignedAngle(dragEndPosition, dragStartPosition, Vector3.left);
        //var angleY = Vector3.SignedAngle(dragEndPosition, dragStartPosition, Vector3.up);
        //// roatate around the vector..
        //XRot.transform.RotateAround(transform.position, Vector3.left, angleX);
        //YRot.transform.RotateAround(transform.position, Vector3.up, angleY);
    }

    /// <summary>
    /// 上次碰撞到的位置，防止为空，离开球体后就是最后一次碰撞到球体的位置
    /// 不松手会一直转动
    /// </summary>
    static Vector3 lastPoint = Vector3.zero;
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
            Debug.Log(hit.collider.gameObject.name);
            lastPoint = hit.point;
            return hit.point;
        }
        //return lastPoint;
        return null;
    }
}