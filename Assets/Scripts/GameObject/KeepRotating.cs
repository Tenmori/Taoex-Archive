using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotating : MonoBehaviour {

    /// <summary>
    /// The rotation.
    /// </summary>
    public Vector3 rotation;
	
	/// <summary>
    /// Fixed duration between updates.
    /// </summary>
	void FixedUpdate () {
        gameObject.transform.eulerAngles += rotation;
	}
}
