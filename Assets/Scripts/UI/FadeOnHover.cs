using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(FadeControllerUI))]
public class FadeOnHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    #region Variables and class members
    /// <summary>
    /// Toggle for fade in or out effect
    /// </summary>
    public bool fadeIn;

    /// <summary>
    /// flag for if mouse is hovered over
    /// </summary>
    private bool hover;

    /// <summary>
    /// The canvas group used for alpha control
    /// </summary>
    private FadeControllerUI fader;
    #endregion

	/// <summary>
    /// Start this instance.
    /// </summary>
	void Start () {
        fader = GetComponent<FadeControllerUI>();

        if (fadeIn) {
            fader.AlphaToMin();
        }
        else {
            fader.AlphaToMax();
        }
	}
	
    /// <summary>
    /// Fixed duration between updates
    /// </summary>
    void FixedUpdate() {
        if (fadeIn) {
            // fade in on hover
            if (hover) {
                fader.FadeInStep();
            }
            // fade out on exit
            else {
                fader.FadeOutStep();
            }
        }
        else {
            // fade out on hover
            if (hover) {
                fader.FadeOutStep();
            }
            // fade in on exit
            else {
                fader.FadeInStep();
            }
        }
    }

    #region mouse detection
    /// <summary>
    /// Raises the pointer enter event.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void OnPointerEnter(PointerEventData eventData) {
        hover = true;
    }

    /// <summary>
    /// Raises the pointer exit event.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void OnPointerExit(PointerEventData eventData) {
        hover = false;
    }
    #endregion
}
