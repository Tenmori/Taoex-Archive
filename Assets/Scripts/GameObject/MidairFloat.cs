using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MidairFloat : MonoBehaviour {

    /// <summary>
    /// The float amount in the y axis.
    /// </summary>
    public float floatAmount;

    /// <summary>
    /// The duration.
    /// </summary>
    [Range(0f, 10.0f)]
    public float duration;

    /// <summary>
    /// represents time
    /// </summary>
    private float t;
	
    /// <summary>
    /// The float up.
    /// </summary>
    private bool floatUp = true;

	/// <summary>
    /// Fixeds duration between update.
    /// </summary>
	void FixedUpdate () {
        float yAmt = floatAmount / duration;
        yAmt *= Time.fixedDeltaTime;
        Vector3 pos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);

        if (floatUp) {
            pos.y += yAmt;
        }

        else {
            pos.y -= yAmt;
        }

        gameObject.transform.position = pos;

        t += Time.fixedDeltaTime;

        if (t > duration) {
            floatUp = !floatUp;
            t = 0;
        }
	}
}
