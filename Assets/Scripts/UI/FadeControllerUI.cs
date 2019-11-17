using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class FadeControllerUI : MonoBehaviour {

    #region Variables and class members
    /// <summary>
    /// The max alpha.
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float maxAlpha = 1.0f;

    /// <summary>
    /// The minimum alpha.
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float minAlpha = 0.0f;

    /// <summary>
    /// The alpha steps
    /// </summary>
    [Range(0.0f, 1.0f)]
    public float alphaStep = 0.05f;

    /// <summary>
    /// The canvas group used for alpha control
    /// </summary>
    private CanvasGroup cGroup;

    /// <summary>
    /// boolean flags to have this script fade the game object in or out fully.
    /// </summary>
    private bool fullFadeIn, fullFadeOut;
    #endregion

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Awake() {
        cGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// Fixed duration between updates
    /// </summary>
    void FixedUpdate() {
        if (fullFadeIn) {
            FadeInStep();

            // stop fading in when max alpha is reached
            if (cGroup.alpha == maxAlpha) {
                fullFadeIn = false;
            }
        }

        else if (fullFadeOut) {
            FadeOutStep();

            // stop fading out when min alpha is reached
            if (cGroup.alpha == minAlpha) {
                fullFadeOut = false;
            }
        }
    }

    #region fade functions
    /// <summary>
    /// Begins the fading in.
    /// </summary>
    public void BeginFadingIn() {
        fullFadeIn = true;
        fullFadeOut = false;
    }

    /// <summary>
    /// Begins the fading out.
    /// </summary>
    public void BeginFadingOut() {
        fullFadeIn = false;
        fullFadeOut = true;
    }

    /// <summary>
    /// Maxes the alpha based on the maxAlpha set
    /// </summary>
    public void AlphaToMax() {
        cGroup.alpha = maxAlpha;
    }

    /// <summary>
    /// Maxes the alpha based on the minAlpha set
    /// </summary>
    public void AlphaToMin() {
        cGroup.alpha = minAlpha;
    }

    /// <summary>
    /// Fades in by 1 step
    /// </summary>
    public void FadeInStep() {
        cGroup.alpha = Clamp(cGroup.alpha + alphaStep, minAlpha, maxAlpha);
    }

    /// <summary>
    /// Fades out by 1 step
    /// </summary>
    public void FadeOutStep() {
        cGroup.alpha = Clamp(cGroup.alpha - alphaStep, minAlpha, maxAlpha);
    }
    #endregion

    /// <summary>
    /// Checks if the game object is faded in to the max amount given
    /// </summary>
    /// <returns><c>true</c>, if fading in was doned, <c>false</c> otherwise.</returns>
    public bool DoneFadingIn() {
        return cGroup.alpha == maxAlpha;
    }

    /// <summary>
    /// Checks if the game object is faded out to the min amount given
    /// </summary>
    /// <returns><c>true</c>, if fading out was doned, <c>false</c> otherwise.</returns>
    public bool DoneFadingOut() {
        return cGroup.alpha == minAlpha;
    }

    /// <summary>
    /// Clamp the specified value, min and max.
    /// </summary>
    /// <param name="value">Value.</param>
    /// <param name="min">Minimum.</param>
    /// <param name="max">Max.</param>
    /// <returns></returns>
    private float Clamp(float value, float min, float max) {
        if (value > max) {
            return max;
        }

        if (value < min) {
            return min;
        }

        return value;
    }
}
