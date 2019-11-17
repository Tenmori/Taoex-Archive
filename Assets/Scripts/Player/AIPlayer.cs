using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;

public class AIPlayer : Player
{
    public Dictionary<PieceTower, HashSet<TileNode>> moves;

    /// <summary>
    /// How long the AI delays its decisions by.
    /// </summary>
    public static readonly float AIDelay = 0.5f;

    public AIPlayer ()
    {
        moves = new Dictionary<PieceTower, HashSet<TileNode>>();
        isAI = true;
        UnityEngine.Debug.Log("AI Created");
        playerName = "AI";
    }

    //Returns all available moves for all pieces the AI owns
    public Dictionary<PieceTower, HashSet<TileNode>> GetAllAvailableMoves()
    {
        Dictionary<PieceTower, HashSet<TileNode>> allMoves = new Dictionary<PieceTower, HashSet<TileNode>>();

        List<PieceTower> towers = Towers;

        //Counts all moves for each piece
        foreach (PieceTower tower in Towers)
        {
            HashSet<TileNode> pieceMoves = tower.GetMoves().Destinations;

            // Only add if this tower can move
            if (pieceMoves.Count > 0)
            {
                allMoves.Add(tower, pieceMoves);
            }
        }

        return allMoves;
    }

    /// <summary>
    /// AI makes a random placement move.
    /// </summary>
    /// <returns>The placement move</returns>
    public PlacementMove PlacementMove()
    {
        // Map of piece placement options
        Dictionary<PlacementPiece, List<TileNode>> availablePlacements = new Dictionary<PlacementPiece, List<TileNode>>();

        // Populate the map
        foreach (PlacementPiece placementPiece in PlacementPieces)
        {
            List<TileNode> availableTiles = placementPiece.GetAvailableTiles();
            if (availableTiles.Count > 0)
            {
                availablePlacements.Add(placementPiece, availableTiles);
            }
        }

        // Get a random piece
        List<PlacementPiece> availablePieces = new List<PlacementPiece>(availablePlacements.Keys);
        PlacementPiece randomPiece = availablePieces[new System.Random().Next(0, availablePieces.Count)];

        // Get a random tile
        List<TileNode> tiles = availablePlacements[randomPiece];
        TileNode randomTile = tiles[new System.Random().Next(0, tiles.Count)];

        return new PlacementMove(randomPiece, randomTile);
    }

    //AI makes a random move
    public WeightedMove RandomMove()
    {
        // Get a random piece
        List<PieceTower> availablePieces = new List<PieceTower>(moves.Keys);
        PieceTower randomPiece = availablePieces[new System.Random().Next(0, availablePieces.Count)];

        // Get a random move from that random piece
        HashSet<TileNode> randomPieceMoves = new HashSet<TileNode>(randomPiece.GetMoves().Destinations);
        List<TileNode> randomPieceMovesList = randomPieceMoves.ToList();

        // The node to move onto
        TileNode randomTile = randomPieceMovesList[new System.Random().Next(0, randomPieceMoves.Count)];

		return (new WeightedMove(randomPiece, randomTile, 1));
    }

    //AI makes turn
	public override WeightedMove DoTurn()
    {
        moves = GetAllAvailableMoves();

        //Only move if there are valid moves
        if (moves.Count > 0)
        {
			UnityEngine.Debug.Log("AI Moved");
            return RandomMove();
        }
        else
        {
			return null;
        }
    }

    /// <summary>
    /// Handles forced way movements for the AI.
    /// </summary>
    /// <param name="tower">The piece tower selected</param>
    /// <param name="moves">The list of way moves available</param>
    /// <returns></returns>
    public virtual WeightedMove ForcedWay(PieceTower tower, HashSet<TileNode> moves)
    {
        // Random way move
        int randomIndex = new System.Random().Next(0, moves.Count);

        return new WeightedMove(tower, moves.ToArray()[randomIndex], 1);
    }

    /// <summary>
    /// Handles AI choices for overstacking.
    /// </summary>
    /// <returns></returns>
    public IEnumerator AIOverstack()
    {
        // Get all the pieces by searching in the Overstack object
        GameObject ga = GameObject.Find("UICanvas").transform.Find("Overstack").gameObject;

        // List of possible indexes to choose
        List<int> indices = new List<int>();
        for (int i = 0; i < ga.transform.childCount; i++)
        {
            indices.Add(i);
        }

        // Select one of own pieces first
        for (int i = 0; i < ga.transform.childCount; i++)
        {
            // Information about the overstack piece
            OverstackPieceUI overstackPiece = ga.transform.GetChild(i).GetComponent<OverstackPieceUI>();
            if (overstackPiece.Piece.colour == colour)
            {
                yield return new WaitForSeconds(AIDelay);

                // Select the piece
                overstackPiece.OnMouseUpAsButton();

                indices.Remove(i);
                break;
            }
        }

        // Attempt to select a hook
        for (int i = 0; i < ga.transform.childCount; i++)
        {
            // Information about the overstack piece
            OverstackPieceUI overstackPiece = ga.transform.GetChild(i).GetComponent<OverstackPieceUI>();
            if (overstackPiece.Piece.type == PieceData.Type.Hook)
            {
                yield return new WaitForSeconds(AIDelay);

                // Select the piece
                overstackPiece.OnMouseUpAsButton();

                indices.Remove(i);
                break;
            }
        }

        // Keep selecting random pieces
        while (indices.Count > 0)
        {
            yield return new WaitForSeconds(AIDelay);
            int randomIndex = new System.Random().Next(0, indices.Count);
            OverstackPieceUI overstackPiece = ga.transform.GetChild(indices[randomIndex]).GetComponent<OverstackPieceUI>();
            overstackPiece.OnMouseUpAsButton();

            indices.RemoveAt(randomIndex);
        }

        // Done selecting
        GameObject.Find("UICanvas").transform.Find("DoneOverstackBtn").GetComponent<OverstackBtnClick>().Finished();
    }

    /// <summary>
    /// called when this player's turn starts
    /// </summary>
    public override void TurnStart() {
        DisplayTurnDisplay();
    }

    /// <summary>
    /// Called when this player's turn ends
    /// </summary>
    public override void TurnEnd() {

    }

    /// <summary>
    /// called when this player's turn was skipped
    /// </summary>
    public override void TurnSkipped() {

    }
}