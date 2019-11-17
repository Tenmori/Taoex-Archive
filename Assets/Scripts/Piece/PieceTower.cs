using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/// <summary>
/// A tower of pieces
/// </summary>
public class PieceTower {
  
    private static readonly float baseY = 20f;
  
    /// <summary>
    /// The pieces.
    /// </summary>
    public List<PieceData> pieces;

    /// <summary>
    /// The hook piece.
    /// </summary>
    private PieceData hook;

    /// <summary>
    /// The player who owns this tower.
    /// </summary>
    public readonly Player owningPlayer;

    /// <summary>
    /// The owning colour.
    /// </summary>
    public readonly Player.PlayerColour owningColour;

    /// <summary>
    /// The tile node where the tower is on
    /// </summary>
    private TileNode node;

    /// <summary>
    /// A count of the colours
    /// </summary>
    private int[] colourCount = new int[7];

    /// <summary>
    /// Constructor for a new tower
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="startingPiece"></param>
    /// <param name="startNode"></param>
    public PieceTower(Player owner, PieceData startingPiece, TileNode startNode) {
        pieces = new List<PieceData>(6);
        owningColour = owner.colour;
        owningPlayer = owner;
        AddPiece(startingPiece);
        node = startNode;
        MoveTo(startNode);
        startingPiece.SetupTextures();
    }

    /// <summary>
    /// Returns a set of tile nodes that this tower can legally move to
    /// </summary>
    /// <returns></returns>
    public MoveSet GetMoves() {
        MoveSet moves = new MoveSet();

        foreach (PieceData curPiece in pieces) {

            // hook alone does not make a move
            if (curPiece.type == PieceData.Type.Hook) {
                continue;
            }

            // start from tower's position
            TileNode cur = node;

            #region moving off way check
            // must move to new way or off current way if on way (standard way)
            if (node.type == TileNode.Type.Way) {
                if (cur.adjacent[curPiece.direction] == null) {
                    continue;
                }

                // same line
                if (node.adjacent[curPiece.direction].wayLine == node.wayLine || node.adjacent[curPiece.direction].type == TileNode.Type.WayCross) {
                    continue;
                }
            }

            // must move to new way or off current way if on way (intersection)
            if (node.type == TileNode.Type.WayCross) {
                if (cur.adjacent[curPiece.direction] == null) {
                    continue;
                }

                // same line
                if (node.adjacent[curPiece.direction].type == TileNode.Type.Way) {
                    continue;
                }
            }
            #endregion

            #region normal move with range
            // extend out by the range and search for more.
            SearchNormalMoves(curPiece, moves);

            // reverse hook
            if (hook != null && pieces.Count == 6) {
                SearchReverseHookMoves(curPiece, moves);
            }
            #endregion
        }

        return moves;
    }

    /// <summary>
    /// Searchs the normal moves.
    /// </summary>
    /// <param name="piece">Piece.</param>
    /// <param name="moves">Moves.</param>
    private void SearchNormalMoves(PieceData piece, MoveSet moves) {
        TileNode cur = node;

        // extend out by the range and search for more.
        for (int r = 0; r < piece.range; r++) {
            // move to next tile
            cur = cur.adjacent[piece.direction];

            // check if current tile is one you can move to
            if (CheckCurrentTileMove(cur)) {
                moves.AddMove(node, false, cur);
            }

            // check if any more moves can be made after (not including range)
            if (!CheckFurtherMovement(cur)) {
                break;
            }

            // run checks to see if hooking is possible
            if (CheckHookMove(cur, piece)) {
                moves.AddMove(node, true, cur, cur.adjacent[hook.direction]);
            }
        }
    }

    /// <summary>
    /// Searchs the reverse hook moves.
    /// </summary>
    /// <param name="piece">Piece.</param>
    /// <param name="moves">Moves.</param>
    private void SearchReverseHookMoves(PieceData piece, MoveSet moves) {
        TileNode cur = node.adjacent[hook.direction];
        TileNode hooked = node.adjacent[hook.direction];

        #region reverse hook condition
        // can't revese hook your own pieces
        if (piece.colour == owningColour) {
            return;
        }

        // must have tower of 6 to hook
        if (pieces.Count != 6) {
            return;
        }

        // check if able to move on the given tile
        if (!CheckCurrentTileMove(cur)) {
            return;
        }

        // Can't jump or move along the way
        if (cur.type == TileNode.Type.Way && cur.adjacent[piece.direction] != node)
        {
            return;
        }

        #endregion

        // add hook'd
        //moves.AddMove(node, false, hooked);

        #region move search
        // extend out by the range and search for more.
        for (int r = 0; r < piece.range; r++) {
            // move to next tile
            cur = cur.adjacent[piece.direction];

            // check if current tile is one you can move to
            if (CheckCurrentTileMove(cur)) {
                moves.AddMove(node, true, hooked, cur);
            }

            // check if any more moves can be made after (not including range)
            if (!CheckFurtherMovement(cur)) {
                break;
            }
        }
        #endregion
    }

    /// <summary>
    /// Checks the current tile if it's a possible Move
    /// </summary>
    /// <param name="cur">Current tile unhooked.</param>
    /// <returns><c>true</c>, if current tile was checked, <c>false</c> otherwise.</returns>
    private bool CheckCurrentTileMove(TileNode cur) {
        // can't move to non tile
        if (cur == null) {
            return false;
        }

        // can't take your own tower
        if (cur.tower != null && cur.tower.owningColour == owningColour) {
            return false;
        }

        // all checks passed
        return true;
    }

    /// <summary>
    /// Checks if the current tile provide further movement
    /// </summary>
    /// <param name="cur">Current tile unhooked.</param>
    /// <returns><c>true</c>, if further movement was checked, <c>false</c> otherwise.</returns>
    private bool CheckFurtherMovement(TileNode cur) {
        // can't move to non tile
        if (cur == null) {
            return false;
        }

        // can't jump over way
        if (cur.type == TileNode.Type.Way || cur.type == TileNode.Type.WayCross) {
            return false;
        }

        // can't jump over enemies
        if (cur.tower != null && cur.tower.owningColour != owningColour) {
            return false;
        }

        // passed all checks
        return true;
    }

    /// <summary>
    /// Checks if a hook move is possible from the current tilenode with the given piece
    /// </summary>
    /// <returns><c>true</c>, if hoo K move was checked, <c>false</c> otherwise.</returns>
    /// <param name="cur">Current tile unhooked.</param>
    /// <param name="curPiece">Current piece.</param>
    private bool CheckHookMove(TileNode cur, PieceData curPiece) {
        // don't have a hook
        if (hook == null) {
            return false;
        }

        TileNode hookedTile = cur.adjacent[hook.direction];

        // can only use a hook with own colour
        if (curPiece.colour != owningColour) {
            return false;
        }

        // can't hook to non tile
        if (hookedTile == null) {
            return false;
        }

        // attack related
        if (hookedTile.tower != null) {

            // can hook back to original position
            if (hookedTile.tower == this) {
                return true;
            }

            // can't move to own piece
            if (hookedTile.tower.owningColour == owningColour) {
                return false;
            }
        }

        // passed all checks
        return true;
    }

    /// <summary>
    /// Returns a set of moves based on random way roll.
    /// </summary>
    /// <param name="roll"></param>
    /// <returns></returns>
    public MoveSet GetWayMove(int roll, int wayLineID) {
        MoveSet moves = new MoveSet();
        bool c = false;
        if (roll == 9) {
            c = true;
            roll = 5;
            moves.AddMove(node, false, node); // on a c, player can stay
        } else if (roll > 4) {
            roll -= 4;
        }

        for (int d = 0; d < 6; d++) {

            TileNode cur = node;

            if (node.adjacent[d] == null) {
                continue;
            }

            // not along the way, look in another direction
            if (wayLineID != node.adjacent[d].wayLine && node.adjacent[d].type != TileNode.Type.WayCross) {
                continue;
            }

            // keep going along direction d
            for (int r = 0; r < roll; r++) {
                
                if (cur.adjacent[d] == null || cur.adjacent[d].type == TileNode.Type.Block) {
                    break;
                }
                
                cur = cur.adjacent[d];

                // Can't capture own pieces
                if (cur.tower != null && cur.tower.owningColour == owningColour)
                {
                    continue;
                }

                if (c || r == roll - 1) {
                    moves.AddMove(node, false, cur);
                }
            }
        }
        
        return moves;
    }

    /// <summary>
    /// Moves the tower to the destination
    /// </summary>
    /// <param name="dest">The tilenode that is the destination</param>
    private void MoveTo(TileNode dest) {
        node.tower = null;
        dest.tower = this;
        node = dest;
        Debug.Assert(pieces.Count() != 0, "Attempt to move a tower with no pieces");
        updatePosition();
    }

    /// <summary>
    /// Moves to.
    /// </summary>
    /// <param name="happyMove">Happy move.</param>
    public void MoveTo(MoveSet.Move happyMove){
        if (happyMove.hookInvolved)
        {
            DoubleMoveTo(happyMove);
        }
        else
        {
            MoveTo(happyMove.GetDest());
        }


    }

    /// <summary>
    /// Doubles the move to.
    /// </summary>
    /// <param name="happyMove">Happy move.</param>
    private void DoubleMoveTo(MoveSet.Move happyMove) {
        // for each piece
        for (int i = 0; i < pieces.Count(); i++)
        {
            Vector3[] locations = new Vector3[happyMove.movementNodes.Length];

            for (int m = 0; m < happyMove.movementNodes.Length; m++) {
                if (pieces[i].flipped) {
                    locations[m] = happyMove.movementNodes[m].BasePosition;
                    locations[m].y += (25f * (i + 1f)) + baseY;
                }
                else {
                    locations[m] = happyMove.movementNodes[m].BasePosition;
                    locations[m].y += (25f * i) + baseY;
                }    
            }

            pieces[i].MultiMoveTo(locations);
        }

        node.tower = null;
        happyMove.GetDest().tower = this;
        node = happyMove.GetDest();
        Debug.Assert(pieces.Count() != 0, "Attempt to move a tower with no pieces");
    }

    /// <summary>
    /// Updates the positions of the pieces in the tower
    /// </summary>
    public void updatePosition() {
        for (int i = 0; i < pieces.Count(); i++) {
            if (pieces[i].flipped) {
                Vector3 dest = node.BasePosition;
                dest.y += (25f * (i - 1f)) + baseY;
                pieces[i].MoveTo(dest);
            } else {
                Vector3 dest = node.BasePosition;
                dest.y += (25f * i) + baseY;
                pieces[i].MoveTo(dest);
            }
        }
    }

    /// <summary>
    /// Updates the positions of the pieces in the tower
    /// </summary>
    /// <param name="yOffset">Y offset.</param>
    public void UpdatePosition(float yOffset) {
        SortTower();

        for (int i = 0; i < pieces.Count(); i++) {
            if (pieces[i].flipped) {
                Vector3 dest = node.BasePosition;
                dest.y += (25f * (i - 1f)) + baseY + yOffset;
                pieces[i].MoveTo(dest);
            } else {
                Vector3 dest = node.BasePosition;
                dest.y += (25f * i) + baseY + yOffset;
                pieces[i].MoveTo(dest);
            }
        }
    }

    /// <summary>
    /// Sorts the tower
    /// </summary>
    public void SortTower() {
        for (int i = 0; i < 1; i++) {
            pieces.Sort();
        }
    }

    /// <summary>
    /// Adds a piece to this tower.
    /// If the colour doesn't match the tower's colour, the piece will be captured or flipped
    /// </summary>
    /// <param name="piece">Piece to be moved into this tower</param>
    public void AddPiece(PieceData piece) { 
        pieces.Add(piece);
        piece.SetFlipped(piece.colour != owningColour);

        if (piece.colour != Player.PlayerColour.Hook) {
              if (piece.colour == owningColour) {
                piece.value = 0;

                // starting value
                piece.sortingValue = 0;

                // seperate by color
                piece.sortingValue -= ((int)piece.colour + 1) * 100;

                // seperate by pieces count
                piece.sortingValue -= pieces.Count;
            } else {
                piece.value = 1;

                // starting value
                piece.sortingValue = -1000;

                // seperate by colour
                piece.sortingValue -= ((int)piece.colour + 1) * 100;

                // seperate by pieces count
                piece.sortingValue -= pieces.Count;
            }
        } else {
            piece.sortingValue = -10000000;
        }

        if (piece.type == PieceData.Type.Hook) {
            Debug.Assert(hook == null);
            hook = piece;
        } else {
            Debug.Assert(colourCount != null);
            colourCount[(int)piece.colour]++;
        }

        SortTower();
    }

    //Used for overstacking
    public void RemovePieces(List<PieceData> piecesToRemove)
    {
        foreach (PieceData p in piecesToRemove)
        {
            if (pieces.Contains(p))
            {
                pieces.Remove(p);
                colourCount[(int)p.colour]--;
            }
            if (p == hook) {
                hook = null;
            }
        }
    }

    /// <summary>
    /// Kills the tower, removing all of the tower's pieces and removing it's position
    /// </summary>
    public void Die() {
        pieces.Clear();
        owningPlayer.RemoveTower(this);

        // Possible for the node's tower reference to have changed due to hook returning
        if (node.tower == this)
        {
            node.tower = null;
        }
        node = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="colour"></param>
    /// <returns>The number of pieces with the parameter colour in the tower</returns>
    public int GetColourCount(Player.PlayerColour colour) {
        return colourCount[(int)colour];
    }

    /// <summary>
    /// Gets the node.
    /// </summary>
    /// <returns>The node.</returns>
    public TileNode GetNode()
    {
        return node;
    }

    /// <summary>
    /// Gets the hook.
    /// </summary>
    /// <returns>The hook.</returns>
    public PieceData GetHook() {
        return hook;
    }
}

