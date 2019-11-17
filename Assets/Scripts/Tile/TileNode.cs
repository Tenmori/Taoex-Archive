using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[RequireComponent(typeof(SmoothMovement))]
public class TileNode : MonoBehaviour {

    #region variables
    /// <summary>
    /// Tile type.
    /// </summary>
    public enum Type { Normal, Edge, Block, Way, WayCross };

    #region radius variables
    /// <summary>
    /// The edge radius.
    /// </summary>
    private static float edgeRadius = 50f;

    /// <summary>
    /// The face radius.
    /// </summary>
    private static float faceRadius = edgeRadius * Mathf.Cos(30f * (Mathf.PI / 180f));
    #endregion

    #region highlight variables
    /// <summary>
    /// The highlight colour.
    /// </summary>
    private static Color highlightColour = Color.red;

    /// <summary>
    /// The original colour.
    /// </summary>
    private Color originalColour;

    /// <summary>
    /// The counter for the number of things that want this to be highlighted
    /// </summary>
    private int highlightCount;
    #endregion

    #region grid and position variables
    /// <summary>
    /// The adjacent tiles.
    /// </summary>
    public TileNode[] adjacent;

    /// <summary>
    /// x and z position on the grid
    /// </summary>
    private int gridX, gridZ;

    /// <summary>
    /// Gets the grid x position.
    /// </summary>
    /// <value>The grid x.</value>
    public int GridX { get{ return gridX; } }

    /// <summary>
    /// Gets the grix y position.
    /// </summary>
    /// <value>The grix y.</value>
    public int GridZ { get{ return gridZ; } }

    /// <summary>
    /// Gets the base position.
    /// </summary>
    private Vector3 basePosition;

    /// <summary>
    /// Gets the base position.
    /// </summary>
    /// <value>The base position as a new vector3.</value>
    public Vector3 BasePosition { get{ return new Vector3(basePosition.x, basePosition.y, basePosition.z); }}
    #endregion

    #region other variables
    /// <summary>
    /// The tile type.
    /// </summary>
    public Type type;

    /// <summary>
    /// The tower thats on the tile.
    /// </summary>
    public PieceTower tower;

    /// <summary>
    /// Used for identifying which way line
    /// </summary>
    public int wayLine = -1;

    /// <summary>
    /// Used for identifying which edge this belongs to.
    /// -999 is default for when not set
    /// </summary>
    public int edgeId = -999;

    /// <summary>
    /// Random object used for smooth move variation
    /// </summary>
    private static System.Random rand = new System.Random();
    #endregion
    #endregion

    /// <summary>
    /// Initializes a new instance of the <see cref="TileNode"/> class.
    /// </summary>
    /// <param name="gridX">Grid x.</param>
    /// <param name="gridZ">Grid z.</param>
    /// <param name="obj">Object.</param>
    /// <param name="type">Type.</param>
    public void Init(int gridX, int gridZ, Type type) {
        this.gridX = gridX;
        this.gridZ = gridZ;
        this.type = type;
        adjacent = new TileNode[6];
        float worldX = gridX * edgeRadius * 1.5f;
        float worldZ = gridZ * faceRadius * 2.0f;

        // even offset
        if (gridX % 2 != 0) {
            worldZ -= faceRadius;
        }

        basePosition = new Vector3(worldX, 0f, worldZ);

        gameObject.transform.SetParent(GameObject.Find("Board").transform);
        gameObject.transform.position = new Vector3(worldX, 1500f, worldZ);

        originalColour = gameObject.GetComponent<MeshRenderer>().material.color;
    
        Vector3 location = basePosition;

        GetComponent<SmoothMovement>().MoveToDelayed(location, (float)rand.Next(1, 50) / 100f );
        GetComponent<SmoothMovement>().rate = (float)rand.Next(2, 5) / 100f;
    }

    /// <summary>
    /// Highlights the tile.
    /// </summary>
    public void highlight() {
        if (highlightCount == 0) {
            ShowIndicator();
        }
        highlightCount++;
    }

    /// <summary>
    /// Unhighlight the tile.
    /// </summary>
    public void unhighlight() {
        highlightCount--;

        if (highlightCount < 0) {
            highlightCount = 0;
        }

        if (highlightCount <= 0) {
            HideIndicator();
        }
    }

    /// <summary>
    /// Shows the indicator.
    /// </summary>
    private void ShowIndicator() {
        gameObject.GetComponent<MeshRenderer>().material.color = highlightColour;

        Vector3 upPos = BasePosition;
        upPos.y += 15f;

        GetComponent<SmoothMovement>().rate = 0.2f;
        GetComponent<SmoothMovement>().MoveTo(upPos);

        if (tower != null) {
            tower.UpdatePosition(upPos.y - basePosition.y);
        }
    }

    /// <summary>
    /// Hides the indicator.
    /// </summary>
    private void HideIndicator() {
        gameObject.GetComponent<MeshRenderer>().material.color = originalColour;

        GetComponent<SmoothMovement>().rate = 0.2f;
        GetComponent<SmoothMovement>().MoveTo(BasePosition);

        if (tower != null) {
            tower.updatePosition();
        }
    }
}

