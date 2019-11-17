using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveHistoryHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    /// <summary>
    /// The source tile
    /// </summary>
    public TileNode src;

    /// <summary>
    /// The destination tile
    /// </summary>
    public TileNode dest;

    /// <summary>
    /// Raises the pointer enter event.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void OnPointerEnter(PointerEventData eventData) {
        src.highlight();
        dest.highlight();
    }

    /// <summary>
    /// Raises the pointer exit event.
    /// </summary>
    /// <param name="eventData">Event data.</param>
    public void OnPointerExit(PointerEventData eventData) {
        src.unhighlight();
        dest.unhighlight();
    }
}
