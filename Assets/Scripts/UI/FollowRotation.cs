using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class FollowRotation : MonoBehaviour {

    /// <summary>
    /// The follow x rotation.
    /// </summary>
    public bool followX;

    /// <summary>
    /// The follow y rotation.
    /// </summary>
    public bool followY;

    /// <summary>
    /// The follow z rotation.
    /// </summary>
    public bool followZ = true;

    /// <summary>
    /// The name of a non UI object
    /// </summary>
    public string objectName;

    /// <summary>
    /// The object trans.
    /// </summary>
    private Transform objTrans;

    /// <summary>
    /// The rect trans.
    /// </summary>
    private RectTransform rectTrans;

	/// <summary>
    /// Start this instance.
    /// </summary>
	void Start () {
        objTrans = GameObject.Find(objectName).transform;
        rectTrans = GetComponent<RectTransform>();
	}
	
    /// <summary>
    /// Fixed duration between updates
    /// </summary>
    void FixedUpdate() {

        float rotX = rectTrans.localEulerAngles.x;
        float rotY = rectTrans.localEulerAngles.y;
        float rotZ = rectTrans.localEulerAngles.z;

        if (followX) {
            rotX = objTrans.eulerAngles.y;
        }

        if (followY) {
            rotY = objTrans.eulerAngles.y;
        }

        if (followZ) {
            rotZ = objTrans.eulerAngles.y;
        }

        //rectTrans.rotation = new Quaternion(rotX, rotY, rotZ, 1f);
        rectTrans.localEulerAngles = new Vector3(rotX, rotY, rotZ);
    }
}
