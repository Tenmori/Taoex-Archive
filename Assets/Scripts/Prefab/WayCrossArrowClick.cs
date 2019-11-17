using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayCrossArrowClick : MonoBehaviour {

	public TileNode tile;
	public WayCrossPicker picker;

    private Color colour;
    private static Color hoverColour = new Color32(255, 145, 0, 255);

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
        colour = GetComponent<MeshRenderer>().material.color;
    }

	/// <summary>
	/// Click listener with the mesh collider
	/// </summary>
    public void OnMouseUpAsButton() {
        Debug.Log("Arrow was clicked on!");
		picker.pickLine(tile.wayLine);
	}

    /// <summary>
    /// handles the mouse hover over event
    /// </summary>
    void OnMouseOver() {
        picker.HoverOnArrow(tile.wayLine);
    }

    /// <summary>
    /// handles the mouse hover exit event
    /// </summary>
    void OnMouseExit() {
        picker.HoverOnArrow(-999);
    }

    /// <summary>
    /// Highlight the specified arrow.
    /// </summary>
    /// <param name="highlight">If set to <c>true</c> highlight.</param>
    public void Highlight(bool highlight) {
        if (highlight) {
            GetComponent<MeshRenderer>().material.color = hoverColour;
        } else {
            GetComponent<MeshRenderer>().material.color = colour;
        }
    }
}
