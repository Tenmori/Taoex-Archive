using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TileNode))]
public class TileClick : MonoBehaviour, IPointerClickHandler {

    public static bool clickable;

    /// <summary>
    /// Reference to the tilenode data
    /// </summary>
    private TileNode node;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
        node = GetComponent<TileNode>();
    }

    /// <summary>
    /// Raises the mouse enter event.
    /// </summary>
    void OnMouseEnter() {
//        if (node.tower != null) {
//            foreach (TileNode tn in node.tower.GetMoves().Destinations) {
//                tn.highlight();
//                tn.gameObject.GetComponent<MeshCollider>().enabled = false;
//            }
//        }
    }

    /// <summary>
    /// Raises the mouse exit event.
    /// </summary>
    void OnMouseExit() {
//        if (node.tower != null) {
//            foreach (TileNode tn in node.tower.GetMoves().Destinations) {
//                tn.unhighlight();
//                tn.gameObject.GetComponent<MeshCollider>().enabled = true;
//            }
//        }
    }

    /// <summary>
    /// Raises the mouse up as button event.
    /// </summary>
    public void OnPointerClick(PointerEventData data) {
        if (clickable && !GameObject.Find("Taoex").GetComponent<TurnHandler>().GetCurrentPlayer().IsAI) {
            Debug.Log("Node= x:" + node.GridX + ", z:" + node.GridZ + ", type:" + node.type);

            SandboxHandler sbh = GameObject.Find("Taoex").GetComponent<SandboxHandler>();

            if (sbh.sandBoxEnabled) {
                sbh.SelectTile(node);
            } else {
                GameObject.Find("Taoex").GetComponent<TurnHandler>().SelectedTile(node);
            }
        }
    }

   
}
