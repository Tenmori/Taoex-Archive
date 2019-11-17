using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementMove {

    /// <summary>
    /// The placement piece.
    /// </summary>
    private PlacementPiece placementPiece;

    /// <summary>
    /// Destination tile for the placement piece.
    /// </summary>
    private TileNode destination;

    /// <summary>
    /// Accessor for placementPiece.
    /// </summary>
    public PlacementPiece PlacementPiece { get { return placementPiece; } }

    /// <summary>
    /// Accessor for destination.
    /// </summary>
    public TileNode Destination { get { return destination; } }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="placementPiece">The placement piece</param>
    /// <param name="destination">The destination tile node</param>
    public PlacementMove(PlacementPiece placementPiece, TileNode destination)
    {
        this.placementPiece = placementPiece;
        this.destination = destination;
    }

}
