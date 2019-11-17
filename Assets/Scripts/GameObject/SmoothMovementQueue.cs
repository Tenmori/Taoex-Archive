using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SmoothMovement))]
public class SmoothMovementQueue : MonoBehaviour {

    /// <summary>
    /// Queue of positions to move to
    /// </summary>
    private Queue<Vector3> locations;

    /// <summary>
    /// Queues the move.
    /// </summary>
    /// <param name="position">Position.</param>
    public void QueueMove(Vector3 position) {
        locations.Enqueue(position);   
    }

    /// <summary>
    /// Immediately moves to the position and resumes queue afte
    /// </summary>
    /// <param name="position">Position.</param>
    public void ImmediateMove(Vector3 position) {
        GetComponent<SmoothMovement>().MoveTo(position);
    }

    /// <summary>
    /// Clears the queue.
    /// </summary>
    public void ClearQueue() {
        locations.Clear();
    }

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake() {
        locations = new Queue<Vector3>();
    }

	/// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () {
        if (locations.Count != 0 && GetComponent<SmoothMovement>().Done) {

            // ensure the queue was not cleared in the middle of the check
            Vector3 v = locations.Dequeue();
            if (v != null) {
                GetComponent<SmoothMovement>().MoveTo(v);
            }
        }
	}
}
