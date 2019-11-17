using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementPiece : MonoBehaviour
{

    /// <summary>
    /// Reference to the turn handler.
    /// </summary>
    private TurnHandler turnHandler;

    /// <summary>
    /// Reference to the piece.
    /// </summary>
    private PieceData piece;

    /// <summary>
    /// Represents the placement state.
    /// </summary>
    private bool placed;

    /// <summary>
    /// The placement position of this piece in world coordinates.
    /// </summary>
    private Vector3 placementPosition;

    /// <summary>
    /// The placement rotation of this piece.
    /// </summary>
    private Vector3 placementRotation;

    /// <summary>
    /// Accessor for placementPosition.
    /// </summary>
    public Vector3 PlacementPosition { get { return placementPosition; } set { placementPosition = value; } }

    /// <summary>
    /// Accessor for placementRotation.
    /// </summary>
    public Vector3 PlacementRotation { get { return placementRotation; } set { placementRotation = value; } }

    /// <summary>
    /// Accessor for placed.
    /// </summary>
    public bool Placed { get { return placed; } set { placed = value; } }

    /// <summary>
    /// Accessor for piece.
    /// </summary>
    public PieceData Piece { get { return piece; } set { piece = value; } }

    private void Awake()
    {
        // Initialize references
        turnHandler = GameObject.Find("Taoex").GetComponent<TurnHandler>();

        // For click detection
        gameObject.AddComponent<MeshCollider>().convex = true;
    }

    /// <summary>
    /// Handles click events on this placement piece.
    /// </summary>
    public void OnMouseUpAsButton()
    {
        if (!placed)
        {
            // Piece shouldn't be null
            Debug.Assert(piece != null);

            // Selected this piece for placement
            if (turnHandler.GetCurrentPlayer().colour == piece.colour && turnHandler.state == TurnHandler.State.Placement)
            {
                turnHandler.PlacementPiece = this;
                turnHandler.UnhighlightPlacementTiles();

                RemovePlacementArrows();

                // Highlight the available placement tiles
                foreach (TileNode placementTile in turnHandler.PlacementTiles)
                {
                    if ((placementTile.edgeId + 3) % 6 == piece.direction && placementTile.tower == null)
                    {
                        placementTile.highlight();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Gets all the tiles that this piece can be placed onto.
    /// </summary>
    /// <returns>List of all the tiles that this piece can be placed onto</returns>
    public List<TileNode> GetAvailableTiles()
    {
        List<TileNode> result = new List<TileNode>();

        foreach (TileNode placementTile in turnHandler.PlacementTiles)
        {
            if ((placementTile.edgeId + 3) % 6 == piece.direction && placementTile.tower == null)
            {
                result.Add(placementTile);
            }
        }

        return result;
    }

    /// <summary>
    /// Liberates this piece
    /// </summary>
    public void Liberate()
    {
        Player owningPlayer = turnHandler.getPlayerByColour(piece.colour);

        // Find any open spots for this piece to return to
        List<TileNode> availableTiles = new List<TileNode>();
        foreach (TileNode placementTile in turnHandler.PlacementTiles)
        {
            if ((placementTile.edgeId + 3) % 6 == piece.NormalDir)
            {
                availableTiles.Add(placementTile);
            }
        }

        if (availableTiles.Count > 0)
        {
            // Randomly put this piece back into its available edge tiles
            TileNode randomTile = availableTiles[new System.Random().Next(0, availableTiles.Count)];
            owningPlayer.AddTower(new PieceTower(turnHandler.getPlayerByColour(piece.colour), piece, randomTile));
        }
        else
        {
            // Restore the original placement transform
            gameObject.GetComponent<SmoothMovement>().MoveTo(placementPosition);
            gameObject.transform.eulerAngles = PlacementRotation;

            // Add this back to the placement list for the owning player
            placed = false;
            owningPlayer.PlacementPieces.Add(this);
        }


    }

    /// <summary>
    /// Removes the placement arrows.
    /// </summary>
    private void RemovePlacementArrows() {
        GameObject[] arrows = GameObject.FindGameObjectsWithTag("Indicators");

        foreach (GameObject obj in arrows) {
            if (obj.name.Equals("FloatingArrowPointer(Clone)")) {
                Destroy(obj);
            }
        }
    }
}
