using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FadeControllerUI))]
public class ActiveStateFade : MonoBehaviour {

    /// <summary>
    /// Boolean flags used for fading in and out and changing the active state
    /// </summary>
    private bool fadeDisable, fadeEnable;

    /// <summary>
    /// Flag to destory game object on conmpletingg fade out
    /// </summary>
    private bool destoryOnFinish;

    /// <summary>
    /// Fade controller componenet
    /// </summary>
    private FadeControllerUI fader;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake() {
        fader = GetComponent<FadeControllerUI>();
    }

    /// <summary>
    /// Fixed duration between updates
    /// </summary>
    void FixedUpdate() {
        if (fadeEnable) {
            fader.FadeInStep();

            // stop fading in when max alpha is reached
            if (fader.DoneFadingIn()) {
                fadeEnable = false;
            }
        }

        else if (fadeDisable) {
            fader.FadeOutStep();

            // stop fading out when min alpha is reached
            if (fader.DoneFadingOut()) {
                fadeDisable = false;
                gameObject.SetActive(false);

                if (destoryOnFinish) {
                    Destroy(gameObject);
                }
            }
        }
    }

    /// <summary>
    /// Fades the and disables the game object after the fade
    /// </summary>
    public void FadeAndDisable() {
        fadeDisable = true;
        fadeEnable = false;
    }

    /// <summary>
    /// Enables the gameobject and fades it in
    /// </summary>
    public void FadeAndEnable() {
        gameObject.SetActive(true);
        fadeDisable = false;
        fadeEnable = true;
    }

    /// <summary>
    /// Fades out and destorys the gameobject when fade out is complete
    /// </summary>
    public void FadeAndDestory() {
        fadeDisable = true;
        destoryOnFinish = true;
    }
}
