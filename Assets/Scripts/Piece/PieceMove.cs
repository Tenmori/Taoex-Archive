using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

/// <summary>
/// A class used to store information about a player's desired move
/// </summary>
public class PieceMove {
    /// <summary>
    /// Move types.
    /// </summary>
    public enum MoveType { Move, Attack, Way, WayAttack, Unknown };

    /// <summary>
    /// Result types.
    /// </summary>
    public enum ResultType { Undetermined, Failed, Normal, ForcedWay, ForcedWayCross, Win}

    /// <summary>
    /// Win types.
    /// </summary>
    public enum WinType { Undetermined, CaptureSix, ContainFive, ElimAll}

    /// <summary>
    /// The destination move.
    /// </summary>
    private MoveSet.Move destMove;

    /// <summary>
    /// Type of move 
    /// </summary>
    private MoveType type;

    /// <summary>
    /// Type of move 
    /// </summary>
    public MoveType Type {
        get { 
            return type;
        }
    }

    /// <summary>
    /// Source tile node
    /// </summary>
    public TileNode src;

    /// <summary>
    /// destination tile node
    /// </summary>
    public TileNode dest;

    /// <summary>
    /// legal move list
    /// </summary>
    private HashSet<TileNode> moves;

    /// <summary>
    /// The result of the move.
    /// </summary>
    private ResultType result = ResultType.Undetermined;

    /// <summary>
    /// Gets the result of the move..
    /// </summary>
    /// <value>The result of the move.</value>
    public ResultType Result {
        get { 
            return result;
        }
    }
    /// <summary>
    /// The win case.
    /// </summary>
    public static WinType WIN = WinType.Undetermined;

    /// <summary>
    /// The first tower. (attacker)
    /// </summary>
    public PieceTower firstTower;

    /// <summary>
    /// The second tower. (victim)
    /// </summary>
    public PieceTower secondTower;

    /// <summary>
    /// the winner player;
    /// </summary>
    public static Player winner;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="src">from this tile</param>
    /// <param name="dest">to this tile</param>
    /// <param name="srcMoves">set of legal moves for source tower</param>
    public PieceMove(TileNode src, MoveSet.Move destMove, HashSet<TileNode> srcMoves) {
        Debug.Assert(src != null, "PieceMove src is null when creating one");
        this.src = src;
        if (destMove != null) {
            this.dest = destMove.GetDest();
            Debug.Assert(dest != null, "PieceMove dest is null when creating one");
            this.destMove = destMove;
            moves = srcMoves;
            firstTower = src.tower;
            secondTower = dest.tower;

        }
    }

    /// <summary>
    /// Attempts to carry out the desired move.
    /// </summary>
    /// <returns></returns>
    public bool Execute() {
        
        // check if valid move
        if (!CheckValid()) {
            result = ResultType.Failed;
            return false;
        }

        bool forcedWayMove;
        bool srcIsWay = src.type == TileNode.Type.Way || src.type == TileNode.Type.WayCross;
        bool destIsWay = dest.type == TileNode.Type.Way || dest.type == TileNode.Type.WayCross;

        // determine inital type
        #region determine move type
        // same position
        if (src == dest) {
            type = MoveType.Move;

        // move to empty tile
        } else if (dest.tower == null) {
            type = MoveType.Move;
            src.tower.MoveTo(destMove);
           
        // attacking move on existing tower
        } else {
            PieceTower attacker = src.tower;
            PieceTower victim = dest.tower;
            attacker.owningPlayer.capture += victim.pieces.Count;

            // check attack type
            if ((dest.type == TileNode.Type.Way || dest.type == TileNode.Type.WayCross) && 
                (src.type == TileNode.Type.Way || src.type == TileNode.Type.WayCross)) {
                type = MoveType.WayAttack;
            } else {
                type = MoveType.Attack;
            }

            // Check if this move results in a win
            if (CheckWin()) {
                result = ResultType.Win;
                for (int i = 0; i < victim.pieces.Count; i++)
                {
                    attacker.AddPiece(victim.pieces[i]);
                }
                victim.Die();
                attacker.MoveTo(destMove);
                attacker.owningPlayer.updateScore();
                victim.owningPlayer.updateScore();
                return true;
            }

            // trigger overstack UI if too many pieces
            if (victim.pieces.Count + attacker.pieces.Count > 6 || (victim.GetHook() != null && attacker.GetHook() != null))
            {
                // Start the overstack UI and return immediately
                GameObject.Find("UICanvas").transform.Find("Overstack").GetComponent<OverstackUI>().Construct(attacker, victim);
                attacker.owningPlayer.HighestTower = 6;
            }
            else
            {
                // transfer all pieces to attacker
                for (int i = 0; i < victim.pieces.Count; i++)
                {
                    attacker.AddPiece(victim.pieces[i]);
                }
                victim.Die();
                attacker.MoveTo(destMove);
            }
            //update tower info
            attacker.owningPlayer.HighestTower = attacker.pieces.Count;

            // update scores
            attacker.owningPlayer.updateScore();
            victim.owningPlayer.updateScore();
        }
        #endregion

        #region checking waymove
        forcedWayMove = true;

        // if not moving to way
        if (!destIsWay) {
            forcedWayMove = false;
        }

        // endturn if moving along way (after way move)
        if ((srcIsWay && destIsWay) && (src.wayLine == dest.wayLine) && !destMove.hookInvolved) {
            forcedWayMove = false;
        }

        // check for proper forced way
        if (forcedWayMove) {
            Debug.Assert(forcedWayMove && destIsWay, "Way conditions did not match");
        }
        #endregion

        if (result == ResultType.Undetermined) {
            if (forcedWayMove) {
				if (dest.type == TileNode.Type.Way) {
					result = ResultType.ForcedWay;
				} else {
					result = ResultType.ForcedWayCross;
				}
            } else {
                result = ResultType.Normal;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if the move is valid
    /// </summary>
    /// <returns>True if this move is legal or valid</returns>
    private bool CheckValid() {
        if (destMove == null) {
            return false;
        }

        // check if possible to reach
        if (!moves.Contains(dest)) {
            return false;
        }

        // non stay action
        if (dest != src) {
            // check if self capture
            if (dest.tower != null && src.tower.owningColour == dest.tower.owningColour) {
                return false;
            }
        }

        // valid!
        return true;
    }

    /// <summary>
    /// Checks if this move has achieved a win
    /// </summary>
    /// <returns></returns>
    private bool CheckWin() {

        int SumTower = 0;

        // can't win if less than 6 pieces
        if ((dest.tower.pieces.Count + src.tower.pieces.Count) < 6) {
            return false;
        }

        // if attacked tower has 5 of attacker's tower (liberation condition)
        if (dest.tower.GetColourCount(src.tower.owningColour) == 5) {
            winner = src.tower.owningPlayer;
            WIN = WinType.ContainFive;
            return true;
        }

        // check if containing 6 of any colour (declaration condition)
        foreach (Player.PlayerColour c in Enum.GetValues(typeof(Player.PlayerColour))) {
            if ((dest.tower.GetColourCount(c) + src.tower.GetColourCount(c)) > 5) {
                winner = src.tower.owningPlayer;
                WIN = WinType.CaptureSix;
                return true;
            }
        }

        // check win Condition 3 -> Eliminate all opponent colors from the board for a win by attrition.
        foreach (Player p in TurnHandler.players)
        {
            if (p != src.tower.owningPlayer)
            {
                SumTower += p.Towers.Count;
            }
        }

        if (SumTower == 0)
        {
            winner = src.tower.owningPlayer;
            WIN = WinType.ElimAll;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Gets the result.
    /// </summary>
    /// <returns>The result.</returns>
    public ResultType GetResult() {
        return result;
    }
}

