using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class OverstackUI : MonoBehaviour
{
    /// <summary>
    /// How much to offset a piece's y position when it is flipped.
    /// </summary>
    private static float consideredFlippedOffset = 24f;

    /// <summary>
    /// Flag to indicate that the overstack is done.
    /// </summary>
    public static bool done = true;

    /// <summary>
    /// The attacking tower.
    /// </summary>
    private PieceTower attacker;

    /// <summary>
    /// The victim tower.
    /// </summary>
    private PieceTower victim;

    /// <summary>
    /// The attacker's destination.
    /// </summary>
    private TileNode destination;

    /// <summary>
    /// The new tower to be made for the attacker.
    /// </summary>
    private PieceTower newTower;

    /// <summary>
    /// Pieces that have been chosen to keep.
    /// </summary>
    private LinkedList<OverstackPieceUI> consideredPieces;

    /// <summary>
    /// Offsets for the considered tower pieces.
    /// </summary>
    private int[] offsets;

    /// <summary>
    /// Whether a hook has been chosen.
    /// </summary>
    private bool hookConsidered;

    /// <summary>
    /// Colour of the attacking player.
    /// </summary>
    private Player.PlayerColour attackerColour;

    /// <summary>
    /// Accessor for the attacker's colour.
    /// </summary>
    public Player.PlayerColour AttackerColour { get { return attackerColour; } }

    /// <summary>
    /// Accessor for newTower.
    /// </summary>
    public PieceTower NewTower { get { return newTower; } set { newTower = value; } }

    /// <summary>
    /// Constructor for overstacking.
    /// Sets up and displays the overstack interface.
    /// </summary>
    /// <param name="attacker">The attacking piece</param>
    /// <param name="victim">The victim piece</param>
    public void Construct(PieceTower attacker, PieceTower victim)
    {
        this.attacker = attacker;
        this.victim = victim;
        destination = victim.GetNode();
        attackerColour = attacker.owningColour;

        done = false;

        // Offset the piece locations
        float offset = 0f;

        for (int i = attacker.pieces.Count - 1; i >= 0; i--)
        {
            PieceData p = attacker.pieces[i];
            // Duplicate the piece onto the canvas
            GameObject dup = GameObject.Instantiate(p.getObj());
            dup.AddComponent<OverstackPieceUI>();
            dup.GetComponent<OverstackPieceUI>().Piece = p;
            dup.transform.SetParent(transform, false);

            // Position on the screen
            Vector3 newPos = new Vector3(-250f, 150f - offset, 0f);
            Vector3 newRot = new Vector3(270f, 120f, 0f);

            // Flip pieces that are not the same colour as the attacker
            if (p.colour != attacker.owningColour && p.type != PieceData.Type.Hook)
            {
                newPos.y += 25f;
                newRot = new Vector3(-270f, 60f, 0f);
                dup.GetComponent<OverstackPieceUI>().Flipped = true;
            }

            // Set the position and angles
            dup.transform.localPosition = newPos;
            dup.transform.localEulerAngles = newRot;

            offset += 30f;
        }

        // Offset between towers
        offset += 30f;

        for (int i = victim.pieces.Count - 1; i >= 0; i--)
        {
            PieceData p = victim.pieces[i];
            // Duplicate the piece onto the canvas
            GameObject dup = GameObject.Instantiate(p.getObj());
            dup.AddComponent<OverstackPieceUI>();
            dup.GetComponent<OverstackPieceUI>().Piece = p;
            dup.GetComponent<OverstackPieceUI>().Flipped = true;
            dup.transform.SetParent(transform, false);

            // Position on the screen
            Vector3 newPos = new Vector3(-250f, 175f - offset, 0f);
            Vector3 newRot = new Vector3(-270f, 60f, 0f);

            // Don't flip hook pieces or pieces that are the same colour as the attacker
            if (p.type == PieceData.Type.Hook || p.colour == attacker.owningColour)
            {
                newPos.y -= 25f;
                newRot = new Vector3(270f, 120f, 0f);
                dup.GetComponent<OverstackPieceUI>().Flipped = false;
            }

            // Set the location and angles
            dup.transform.localPosition = newPos;
            dup.transform.localEulerAngles = newRot;

            offset += 30;
        }

        offsets = new int[] { 0, 25, 50, 75, 100, 125 };
        consideredPieces = new LinkedList<OverstackPieceUI>();

        // Enable the backdrop and overstack button
        GameObject.Find("UICanvas").GetComponent<RawImage>().enabled = true;
        GameObject.Find("UICanvas").transform.Find("DoneOverstackBtn").gameObject.SetActive(true);

        // Overstacking for AI
        if (attacker.owningPlayer.IsAI)
        {
            StartCoroutine(((AIPlayer)attacker.owningPlayer).AIOverstack());
        }
    }

    /// <summary>
    /// Called when there is a valid finish request from the overstack button.
    /// Handles the creation of the new tower, and removes all old pieces from the game.
    /// </summary>
    /// <param name="pieces">The considered pieces that have been validated.</param>
    public void Finished(List<PieceData> pieces)
    {
        // Reference to the attacking player
        Player player = GameObject.Find("Taoex").GetComponent<TurnHandler>().getPlayerByColour(attacker.owningColour);
        PieceData startingPiece = null;

        HashSet<TileNode> attackerMoves = attacker.GetMoves().Destinations;

        // Remove references to the selected pieces
        attacker.RemovePieces(pieces);
        victim.RemovePieces(pieces);

        // Pick the first selected attacker piece as the starting piece
        foreach (PieceData p in pieces)
        {
            if (p.colour == player.colour)
            {
                startingPiece = p;
                break;
            }
        }

        Debug.Assert(startingPiece != null, "Starting piece was null");

        // Create the new tower
        PieceTower newTower = new PieceTower(player, startingPiece, destination);
        newTower.owningPlayer.AddTower(newTower);

        // Add rest of the pieces
        foreach (PieceData p in pieces)
        {
            if (p != startingPiece)
            {
                newTower.AddPiece(p);
            }
        }

        // Clear the UI
        foreach (Transform ga in transform)
        {
            Destroy(ga.gameObject);
        }

        // Kill off the extra pieces
        foreach (PieceData p in attacker.pieces)
        {
            // Take extra pieces for scoring
            if (!pieces.Contains(p))
            {
                player.TakePiece(p);
            }

            // Liberate the piece if it's the attacker's colour
            if (p.colour == attackerColour)
            {
                if (p.getObj().GetComponent<PlacementPiece>() != null)
                {
                    p.getObj().GetComponent<PlacementPiece>().Liberate();
                }
                else
                {
                    Debug.Log("PlacementPiece script was null, probably testing with sandbox mode");
                    Destroy(p.getObj());
                }
            }
            else if (p.type == PieceData.Type.Hook)
            {
                // Return hook
                ReturnHook(p);
            }
            else
            {
                Destroy(p.getObj());
            }
        }

        foreach (PieceData p in victim.pieces)
        {
            // Take extra pieces for scoring
            if (!pieces.Contains(p))
            {
                player.TakePiece(p);
            }

            // Liberate the piece if it's the attacker's colour
            if (p.colour == attackerColour)
            {
                if (p.getObj().GetComponent<PlacementPiece>() != null)
                {
                    p.getObj().GetComponent<PlacementPiece>().Liberate();
                }
                else
                {
                    Debug.Log("PlacementPiece script was null, probably testing with sandbox mode");
                    Destroy(p.getObj());
                }
            }
            else if (p.type == PieceData.Type.Hook)
            {
                // Return hook
                ReturnHook(p);
            }
            else
            {
                Destroy(p.getObj());
            }
        }

        attacker.Die();
        victim.Die();

        // Update player status
        newTower.GetNode().tower = newTower;
        newTower.updatePosition();
        attacker.owningPlayer.updateScore();
        victim.owningPlayer.updateScore();

        // Reset variables
        hookConsidered = false;
        consideredPieces.Clear();

        this.newTower = newTower;

        // Disable the dim effect
        GameObject.Find("UICanvas").GetComponent<RawImage>().enabled = false;

        done = true;
    }

    /// <summary>
    /// Considers a piece to be put into the considered tower.
    /// </summary>
    /// <param name="consideredPiece">The piece to consider</param>
    public void ConsiderPiece(OverstackPieceUI consideredPiece)
    {
        // Check if there are already six pieces
        if (consideredPieces.Count == 6)
        {
            return;
        }

        // Hook piece validation
        if (consideredPiece.Piece.type == PieceData.Type.Hook)
        {
            if (!hookConsidered)
            {
                hookConsidered = true;
            }
            else
            {
                if (!GameObject.Find("Taoex").GetComponent<TurnHandler>().GetCurrentPlayer().IsAI)
                {
                    GameObject.Find("OverstackError").GetComponent<OverstackError>().ShowError("Can't select more than one hook.");
                }
                return;
            }

            // Pushing the hook piece to the front of the list
            consideredPieces.AddFirst(consideredPiece);
        }
        else
        {
            // Add it to the considered list
            consideredPieces.AddLast(consideredPiece);
        }
        consideredPiece.Considered = true;

        // Update the considered tower
        BuildConsideredTower();
    }

    /// <summary>
    /// Removes a piece from the considered tower.
    /// </summary>
    /// <param name="piece">The piece to remove</param>
    public void DeconsiderPiece(OverstackPieceUI piece)
    {
        // Remove from the considered list
        consideredPieces.Remove(piece);
        piece.Considered = false;

        // Deselected a hook piece
        if (piece.Piece.type == PieceData.Type.Hook)
        {
            hookConsidered = false;
        }

        // Update the considered tower
        BuildConsideredTower();
    }

    /// <summary>
    /// Sorts the considered tower.
    /// </summary>
    public void SortConsideredTower()
    {
        // Get the piece sorted into a list
        List<OverstackPieceUI> sortedList = consideredPieces.OrderBy(o => -o.Piece.sortingValue).ToList();

        // Clear the considered pieces
        consideredPieces.Clear();

        // Add the sorted elements back into the considered list
        foreach (OverstackPieceUI piece in sortedList)
        {
            consideredPieces.AddFirst(piece);
        }
    }

    /// <summary>
    /// Builds the considered tower.
    /// alls the sorting function.
    /// </summary>
    public void BuildConsideredTower()
    {
        // Sort the tower
        SortConsideredTower();

        int i = 0;
        LinkedListNode<OverstackPieceUI> cur = consideredPieces.First;

        while (cur != null)
        {
            // Starting position of the piece
            Vector3 startPos = cur.Value.StartPos;

            // New position of the piece
            Vector3 newPos = new Vector3(startPos.x + 300, -100 +
            offsets[i], startPos.z);

            // Move flipped pieces up slightly
            if (cur.Value.Flipped)
            {
                newPos.y += consideredFlippedOffset;
            }

            // Update the position
            cur.Value.transform.localPosition = newPos;

            // Next piece
            cur = cur.Next;
            i++;
        }
    }

    public void ReturnHook(PieceData piece)
    {
        // Refernece to the board
        TaoexBoard tb = GameObject.Find("Taoex").GetComponent<TaoexBoard>();
        TileNode[] wayCrossTiles = tb.WayCrossTiles;

        // Closest tile information
        float closestDistance = float.MaxValue;
        TileNode closestTile = null;

        for (int i = 0; i < 6; i++)
        {
            // Find empty way crosses
            if (wayCrossTiles[i].tower != null)
            {
                continue;
            }

            // Get the closest way cross
            float distance = Vector3.Distance(piece.getObj().transform.position, wayCrossTiles[i].transform.position);
            if (distance < closestDistance)
            {
                Debug.Log(i + " : " + distance);
                closestDistance = distance;
                closestTile = wayCrossTiles[i];
            }
        }

        // If there is no available way cross, try the center
        if (closestTile == null && (tb.GetTiles()[13, 11].tower == null || tb.GetTiles()[13, 11].tower == attacker))
        {
            closestTile = tb.GetTiles()[13, 11];
        }

        if (closestTile != null)
        {
            // Return the hook
            closestTile.tower = new PieceTower(tb.HookPlayer, piece, closestTile);
            //piece.MoveTo(closestTile.transform.position);
        }
        else
        {
            Destroy(piece.getObj());
            Debug.Log("Couldn't find a tile to put the hook back onto");
        }

    }
}
