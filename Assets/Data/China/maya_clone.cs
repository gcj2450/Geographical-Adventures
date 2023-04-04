using UnityEngine;
using System.Collections;

public class maya_clone : MonoBehaviour
{
    public float zoomSpeed = 1.2f;
    public float rotateSpeed = 4.0f;
    public Vector3 startpos = new Vector3(0, 130, 60);

    private GameObject orbitVector;
    private Quaternion orbt_rot_original;

    private Vector3 orbt_xform_original;

    public float MinDist = 150;
    public float MaxDist = 250;

    // Use this for initialization
    void Start()
    {
        // Create a capsule (which will be the lookAt target and global orbit vector)
        orbitVector = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        orbitVector.transform.position = Vector3.zero;

        // Snap the camera to align with the grid in set starting position (otherwise everything gets a bit wonky)
        transform.position = startpos;
        transform.LookAt(orbitVector.transform.position, Vector3.up);
        orbitVector.GetComponent<Renderer>().enabled = false; //hide the capsule object 	

        ///
        orbt_xform_original = orbitVector.transform.position;
        orbt_rot_original = orbitVector.transform.rotation;

    }


    /******************/

    void reset_xforms()
    {
        transform.parent = orbitVector.transform;
        orbitVector.transform.position = orbt_xform_original;
        orbitVector.transform.rotation = orbt_rot_original;
        transform.parent = null;
        transform.position = startpos;
    }


    /******************/

    void LateUpdate()
    {

        var x = Input.GetAxis("Mouse X");
        var y = Input.GetAxis("Mouse Y");


        if (Input.GetKey(KeyCode.Home))
        {
            this.reset_xforms();
        }
        /***********************/

        var wheelie = Input.GetAxis("Mouse ScrollWheel");

        if (wheelie < 0) // back
        {
            //缩小,后退
            Debug.Log("wheelie < 0");
            var currentZoomSpeed = 100f;
            transform.Translate(Vector3.forward * (wheelie * currentZoomSpeed));

        }
        if (wheelie > 0) // back
        {
            //放大,前进
            Debug.Log("wheelie > 0");
            var currentZoomSpeed = 100f;
            transform.Translate(Vector3.forward * (wheelie * currentZoomSpeed));

        }

        /***********************/


        // Distance between camera and orbitVector. We'll need this in a few places
        var distanceToOrbit = Vector3.Distance(transform.position, orbitVector.transform.position);

        if (Input.GetMouseButton(0))
        {

            // Refine the rotateSpeed based on distance to orbitVector
            var currentRotateSpeed = Mathf.Clamp(rotateSpeed * (distanceToOrbit / 50), 1.0f, rotateSpeed);


            // Temporarily parent the camera to orbitVector and rotate orbitVector as desired
            transform.parent = orbitVector.transform;
            orbitVector.transform.Rotate(-Vector3.right * (y * currentRotateSpeed));
            orbitVector.transform.Rotate(Vector3.up * (x * currentRotateSpeed), Space.World);
            transform.parent = null;
        }



    }//lateupdate 

}
