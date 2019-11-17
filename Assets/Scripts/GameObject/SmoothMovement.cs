using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothMovement : MonoBehaviour {

    /// <summary>
    /// The float leyway for comparason
    /// </summary>
    private static readonly float epsilon = 0.01f;

    /// <summary>
    /// Rate of the movement
    /// </summary>
    [Range(0.0001f, 1f)]
    public float rate = 0.05f;

    /// <summary>
    /// The start position.
    /// </summary>
    private Vector3 startVec;

    /// <summary>
    /// The destination position
    /// </summary>
    private Vector3 destVec;

    /// <summary>
    /// The difference between start and dest.
    /// </summary>
    private Vector3 difference;

    /// <summary>
    /// The done flag
    /// </summary>
    private bool done;

    /// <summary>
    /// The done flag
    /// </summary>
    public bool Done { get { return done; } }

    /// <summary>
    /// The delay in seconds.
    /// </summary>
    private float delay;

    /// <summary>
    /// The t value for parametric movement
    /// </summary>
    private float t;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Awake() {
        startVec = gameObject.transform.position;
        destVec = startVec;

        // Starting state of done is true
        done = true;
    }

    /// <summary>
    /// begins to move to the given destination
    /// </summary>
    /// <param name="dest">Destination.</param>
    public void MoveTo(Vector3 dest) {
        // set new starting position
        startVec = gameObject.transform.position;

        // set new destination
        destVec = dest;

        // difference bewteen positions
        difference = destVec - startVec;

        delay = 0f;
        t = 0f;

        done = false;
    }

    /// <summary>
    /// begins to move to the given destination after the delay.
    /// </summary>
    /// <param name="dest">Destination.</param>
    /// <param name="seconds">Seconds.</param>
    public void MoveToDelayed(Vector3 dest, float seconds) {
        // set new starting position
        startVec = gameObject.transform.position;

        // set new destination
        destVec = dest;

        // difference bewteen positions
        difference = destVec - startVec;

        delay = seconds;
        t = 0f;

        done = false;
    }
	
    /// <summary>
    /// Fixed duration between updates
    /// </summary>
	void FixedUpdate () {
        if (!done && delay <= 0f) {
            // move
            t += rate;

            if (t < 1f) {
                Vector3 pos = new Vector3(startVec.x, startVec.y, startVec.z);
                Vector3 diff = new Vector3(difference.x, difference.y, difference.z);

                diff.Scale(new Vector3(t, t, t));
                pos += diff;
                gameObject.transform.position = pos;
            }
            else {
                gameObject.transform.position = destVec;
                startVec = destVec;
                done = true;
            }
        }

        if (delay > 0f) {
            delay -= Time.fixedDeltaTime;
        }
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
