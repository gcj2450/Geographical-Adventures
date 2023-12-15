using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

/// <summary>
/// https://stackoverflow.com/questions/47115451/creating-a-google-earth-like-navigation-for-a-sphere
/// </summary>

public class GoogleEarthCamera : MonoBehaviour
{
    /// <summary>
    /// ����뾶
    /// </summary>
    public int SphereRadius = 100;
    /// <summary>
    /// �������
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
    /// �����ٶ�
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
            //����״̬
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
            //������ת
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
    /// �Ƿ��Ѿ����ù�����ֵ
    /// </summary>
    bool setDampAngle = false;
    /// <summary>
    /// �����Ƕ�ֵ
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
            //����һ�λ���ֵ����ʹ��dotween�𽥼���
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

            //����᲻ͣ��updateֱ��dampAngleΪ0ֹͣ
            transform.RotateAround(transform.position, lastCross, dampAngle);
        }
    }

    /// <summary>
    /// �Ƿ���Ҫ����Ч��
    /// </summary>
    bool needDamp = false;
    /// <summary>
    /// ��һ�ε���ת��
    /// </summary>
    Vector3 lastCross = Vector3.zero;
    /// <summary>
    /// ��һ�εĽǶ�
    /// </summary>
    float lastAnle = 0;
    Vector3 lastEndPos = Vector3.zero;
    private void RotateCamera(Vector3 dragStartPosition, Vector3 dragEndPosition)
    {
        // in case the spehre model is not a perfect sphere..
        dragEndPosition = dragEndPosition.normalized * SphereRadius;
        dragStartPosition = dragStartPosition.normalized * SphereRadius;

        //��һ����ק��λ�ú͵�ǰλ�ò��̫��ͻ�����������뻺������������ͣ��һ�¼���
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
    /// ����ϳ�����
    /// </summary>
    bool moveOutSpere = false;
    /// <summary>
    /// �ϴ���ײ����λ�ã���ֹΪ�գ��뿪�����������һ����ײ�������λ��
    /// ʹ��5��ֵ����Ϊ���뿪�����ʱ�����׳��ַ�ת����
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