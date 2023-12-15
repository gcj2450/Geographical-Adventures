using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

/// <summary>
/// https://stackoverflow.com/questions/47115451/creating-a-google-earth-like-navigation-for-a-sphere
/// </summary>

public class GoogleEarthCamera : MonoBehaviour
{
    /// <summary>
    /// 球体半径
    /// </summary>
    public int SphereRadius = 100;
    /// <summary>
    /// 相机距离
    /// </summary>
    public float CameraDist = 200;
    private Vector3 _mouseStartPos;
    private Vector3 _currentMousePos;

    public GameObject CameraObj;
    public Transform XRot;
    public Transform YRot;

    Quaternion initRot;
    void Start()
    {
        initRot = transform.localRotation;
        // init the camera to look at this object
        Vector3 cameraPos = new Vector3(
            transform.position.x,
            transform.position.y,
            transform.position.z - CameraDist);

        CameraObj.transform.position = cameraPos;
        CameraObj.transform.LookAt(transform.position);
    }

    /// <summary>
    /// 缩放速度
    /// </summary>
    public float zoomSpeed = 15f;
    public float minZoomDistance = 10f;
    public float maxZoomDistance = 100f;

    private float zoomDistance = 50f;

    float zoomInput = 0;
    Vector3 zoomVector = Vector3.zero;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //重置状态
            curTween.Kill();
            _mouseStartPos = GetMouseHit();
            setDampAngle = false;
            dampAngle = 0;
            lastCross = Vector3.zero;
            lastAnle = 0;
            needDamp = false;
        }
        if (_mouseStartPos != Vector3.zero) HandleDrag();

        if (Input.GetMouseButtonUp(0))
        {
            HandleDrop();
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            // Zoom camera
            zoomInput = Input.GetAxis("Mouse ScrollWheel");
            zoomDistance -= zoomInput * zoomSpeed;
            zoomDistance = Mathf.Clamp(zoomDistance, minZoomDistance, maxZoomDistance);

            zoomVector = transform.forward * -zoomInput * zoomSpeed;
            CameraObj.transform.position += zoomVector;
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            //重置旋转
            curTween = transform.DORotateQuaternion(initRot, 0.5f)
                .SetEase(Ease.OutCubic);
        }
    }

    /// <summary>
    /// MouseButtonUp
    /// </summary>
    private void HandleDrop()
    {
        moveOutSpere = true;
        //_mouseStartPos = Vector3.zero;
        //_currentMousePos = Vector3.zero;
    }

    /// <summary>
    /// 是否已经设置过缓动值
    /// </summary>
    bool setDampAngle = false;
    /// <summary>
    /// 缓动角度值
    /// </summary>
    float dampAngle = 0;
    Tween curTween;
    private void HandleDrag()
    {
        if (!moveOutSpere)
        {
            _currentMousePos = GetMouseHit();
            RotateCamera(_mouseStartPos, _currentMousePos);
        }
        else
        {
            if (!needDamp)
                return;
            //设置一次缓动值，并使用dotween逐渐减少
            if (!setDampAngle)
            {
                setDampAngle = true;

                dampAngle = 0.5f;
                curTween=DOTween.To(() => dampAngle, x => dampAngle = x, 0, 1f).SetEase(Ease.OutCubic).OnComplete(() =>
                {
                    _mouseStartPos = Vector3.zero;
                    _currentMousePos = Vector3.zero;
                });
                //Debug.Log("continue rotate: " + dampAngle + "__" + lastCross);
            }

            //这里会不停的update直到dampAngle为0停止
            transform.RotateAround(transform.position, lastCross, dampAngle);
        }
    }

    /// <summary>
    /// 是否需要缓动效果
    /// </summary>
    bool needDamp = false;
    /// <summary>
    /// 上一次的旋转轴
    /// </summary>
    Vector3 lastCross = Vector3.zero;
    /// <summary>
    /// 上一次的角度
    /// </summary>
    float lastAnle = 0;
    Vector3 lastEndPos = Vector3.zero;
    private void RotateCamera(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        // in case the spehre model is not a perfect sphere..
        dragEndPosition = dragEndPosition.normalized * SphereRadius;
        dragStartPosition = dragStartPosition.normalized * SphereRadius;

        //上一次拖拽的位置和当前位置差距太大就缓动，如果不想缓动，手在球上停留一下即可
        if ((dragEndPosition - lastEndPos).sqrMagnitude >= 1)
        {
            needDamp = true;
        }
        else
            needDamp = false;
        lastEndPos = dragEndPosition;

        // calc a vertical vector to rotate around..
        lastCross = Vector3.Cross(dragEndPosition, dragStartPosition);

        // calc the angle for the rotation..
        lastAnle =Vector3.SignedAngle(dragEndPosition, dragStartPosition, lastCross);
        // roatate around the vector..
        transform.RotateAround(transform.position, lastCross, lastAnle);
    }

    /// <summary>
    /// 鼠标拖出地球
    /// </summary>
    bool moveOutSpere = false;
    /// <summary>
    /// 上次碰撞到的位置，防止为空，离开球体后就是最后一次碰撞到球体的位置
    /// 使用5个值是因为在离开球体的时候容易出现反转问题
    /// </summary>
    Vector3[] lastPoints =new Vector3[5] { Vector3.zero, Vector3.zero , Vector3.zero , Vector3.zero , Vector3.zero };
    int id = 0;
    /// <summary>
    ///  Projects the mouse position to the sphere and returns the intersection point. 
    /// </summary>
    /// <returns></returns>
    private Vector3 GetMouseHit()
    {
        // make sure there is a shepre mesh with a colider centered at this game object
        // with a radius of SpehreRadius
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            lastPoints[id] = hit.point;
            if (id == 4)
                id = 0;
            else
                id++;
            moveOutSpere = false;
            return hit.point;
        }
        else
        {
            if (!moveOutSpere)
            {
                needDamp = true;
                moveOutSpere = true;
            }
        }
        if (id == 0)
            return lastPoints[2];
        else if(id==1)
            return lastPoints[3];
        else
            return lastPoints[id-2];
    }

}