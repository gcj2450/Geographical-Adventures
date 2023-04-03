using UnityEngine;
//using DG.Tweening;
public class RtsFollowHelper : MonoBehaviour
{
    public GameObject FollowTarget;
    public bool Snap = true;
    
    [SerializeField]
    private float rotationDamping = 1;

    private RtsCamera _rtsCamera;
    private GameObject _prevFollowTarget;

    void Reset()
    {
        FollowTarget = null;
        Snap = true;
    }

    void Start()
    {
        _rtsCamera = Camera.main.GetComponent<RtsCamera>();
        //SetTarget();
    }
    public bool EnableFollowPath = false;
    void Update()
    {
        if (EnableFollowPath)
        {
            if (FollowTarget != _prevFollowTarget)
            {
                SetTarget();
            }
            //在跟随的同时旋转摄像机
            if (_rtsCamera.IsFollowing)
                _rtsCamera.Rotation = Mathf.LerpAngle(
                    _rtsCamera.Rotation, FollowTarget.transform.eulerAngles.y,
                    rotationDamping * Time.deltaTime);

            if(Input.GetKeyUp(KeyCode.E))
            {
                _rtsCamera.EndFollow();
            }
        }

        //if (Input.GetKeyUp(KeyCode.Space))
        //{
        //    DoGoToPos(_rtsCamera);
        //    //DoGoToPos(_rtsCamera, -32, -75, 100, 90, 35);
        //}
        //if (Input.GetKeyUp(KeyCode.KeypadEnter))
        //{
        //    //DoGoToPos(_rtsCamera);
        //    DoGoToPos(_rtsCamera, -32, -75, 100, 90, 35);
        //}
    }

    private void SetTarget()
    {
        _rtsCamera.Follow(FollowTarget, Snap);
        _prevFollowTarget = FollowTarget;
    }

    ////移动RtsCamera到指定位置和角度
    //public void DoGoToPos(RtsCamera _rts, float _lookX = 50, float _lookZ = -50,
    //                                    float _dist = 200, float _rot = 140, float _tilt = 35)
    //{
    //    DOTween.To(() => _rts.LookAt, x => _rts.LookAt = x, new Vector3(_lookX, 0, _lookZ), 2);
    //    DOTween.To(() => _rts.Distance, x => _rts.Distance = x, _dist, 2);
    //    DOTween.To(() => _rts.Rotation, x => _rts.Rotation = x, _rot, 2);
    //    DOTween.To(() => _rts.Tilt, x => _rts.Tilt = x, _tilt, 2);
    //}
}
