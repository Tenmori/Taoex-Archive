using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WayCrossPicker {

	private TileNode crossTile;
	public bool decided;
	public int resultLine = -1;

	private GameObject[] arrows;

    /// <summary>
    /// Accessor for arrows.
    /// </summary>
    public GameObject[] Arrows { get { return arrows; } }

	/// <summary>
	/// Constructor for a waycross picker
	/// </summary>
	public WayCrossPicker(TileNode crossTile) {
		Debug.Assert(crossTile.type == TileNode.Type.WayCross);
		this.crossTile = crossTile;
        decided = false;
		arrows = new GameObject[4];

		createArrows();
	}

	/// <summary>
	/// Picks a line and sets status of decided
	/// </summary>
	/// <param name="line">Line.</param>
	public void pickLine(int line) {
		resultLine = line;
        decided = true;
        Debug.Log("Attempting to destory arrows");
        destoryArrows();
	}

    /// <summary>
    /// Hover event for Arrow.
    /// </summary>
    /// <param name="line">Line.</param>
    public void HoverOnArrow(int line) {
        foreach (GameObject obj in arrows) {
            WayCrossArrowClick wcac = obj.GetComponent<WayCrossArrowClick>();
            wcac.Highlight(wcac.tile.wayLine == line);
        }
    }

	/// <summary>
	/// Creates the arrows.
	/// </summary>
	private void createArrows() {
        // arrow index 
		int index = 0;

        // angle for the arrow
		float angle = 0;

        // find all adjacent tile, if way, place an arrow
        for (int i = 0; i < crossTile.adjacent.Length; i++) {
            TileNode tn = crossTile.adjacent[i];

			if (tn.type == TileNode.Type.Way) {
                // contiune down the line
                TileNode tnCountiune = tn.adjacent[i];

                // create an arrow object
				arrows[index] = Object.Instantiate(Resources.Load("Prefabs/GameObject/WayCrossArrow")) as GameObject;

				// transform
                arrows[index].transform.position = new Vector3(tnCountiune.gameObject.transform.position.x, 50f, tnCountiune.gameObject.transform.position.z);
                arrows[index].transform.RotateAround(arrows[index].transform.position, Vector3.up, angle);

				// script varibles
				WayCrossArrowClick wcac = arrows[index].GetComponent<WayCrossArrowClick>();
				wcac.picker = this;
                wcac.tile = tnCountiune;

				index++;
			}

			angle += 60f;
		}

        // index should always be 4 on a proper cross section
		Debug.Assert(index == 4);
	}

	/// <summary>
	/// Destories the arrow objects.
	/// </summary>
	private void destoryArrows() {
		foreach (GameObject obj in arrows) {
			Object.DestroyObject(obj);
		}
	}
}
