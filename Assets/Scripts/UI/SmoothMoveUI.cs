using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SmoothMoveUI : MonoBehaviour {

    private static float epsilon = 0.001f;

    /// <summary>
    /// The target location.
    /// </summary>
    public Vector3 target;

    /// <summary>
    /// The move rate.
    /// </summary>
    [Range(0f, 1.0f)]
    public float rate = 0.05f;

    /// <summary>
    /// The user interface trans.
    /// </summary>
    private Transform uiTrans;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
        uiTrans = GetComponent<RectTransform>().transform;
    }

    /// <summary>
    /// Fixed duration between updates
    /// </summary>
    void FixedUpdate() {

        // check if there
        if (equals(transform.localPosition, target)) {
            return;
        }

        Vector3 diff = target - transform.localPosition;
        diff.Scale(new Vector3(rate, rate, rate));

        transform.localPosition += diff;
    }

    /// <summary>
    /// checks if two floats are equal or very close to equal
    /// </summary>
    /// <returns><c>true</c>, if equals or close, <c>false</c> otherwise.</returns>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    private bool equals(float a, float b) {
        return Mathf.Abs(a - b) < epsilon;
    }

    /// <summary>
    /// Equals the specified a and b.
    /// </summary>
    /// <param name="a">The alpha component.</param>
    /// <param name="b">The blue component.</param>
    private bool equals(Vector3 a, Vector3 b) {
        return equals(a.x, b.x) && equals(a.y, b.y) && equals(a.z, b.z);
    }

}
