using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnDisplayRender : MonoBehaviour {

    /// <summary>
    /// The fade out delay.
    /// </summary>
    public float fadeOutDelay = 1f;

    /// <summary>
    /// The time.
    /// </summary>
    private float time;

    /// <summary>
    /// The reached delay.
    /// </summary>
    private bool reachedDelay;

	/// <summary>
    /// Ran at the start of the parent gameobject
    /// </summary>
	void Start () {
        GetComponent<FadeControllerUI>().BeginFadingIn();
        GetComponent<CanvasGroup>().alpha = 0;
    }
	
    /// <summary>
    /// Updates at a fixed rate, from base MonoBehaviour
    /// </summary>
    void FixedUpdate() {
        time += Time.fixedDeltaTime;

        if (time > fadeOutDelay && !reachedDelay) {
            reachedDelay = true;
            GetComponent<ActiveStateFade>().FadeAndDestory();
        }
    }
}
