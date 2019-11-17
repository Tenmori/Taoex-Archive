using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WeightedAIPlayer : AIPlayer
{
    /// <summary>
    /// Weighting values.
    /// Larger spread will make the AI more difficult in general.
    /// </summary>
    private int attackWeight;
    private int safeWeight;
    private int wayWeight;
    private int neutralWeight;
    private int badWeight;

    public enum Difficulty { Easy, Normal, Hard }
    public Difficulty difficulty;

    /// <summary>
    /// The bad moves.
    /// </summary>
    public List<WeightedMove> BadMoves;

    /// <summary>
    /// The neutral moves.
    /// </summary>
    public List<WeightedMove> NeutralMoves;

    /// <summary>
    /// The way moves.
    /// </summary>
    public List<WeightedMove> WayMoves;

    /// <summary>
    /// The safe moves.
    /// </summary>
    public List<WeightedMove> SafeMoves;

    /// <summary>
    /// The attack moves.
    /// </summary>
    public List<WeightedMove> AttackMoves;

    /// <summary>
    /// Default constructor.
    /// </summary>
    public WeightedAIPlayer()
    {
        BadMoves = new List<WeightedMove>();
        NeutralMoves = new List<WeightedMove>();
        WayMoves = new List<WeightedMove>();
        SafeMoves = new List<WeightedMove>();
        AttackMoves = new List<WeightedMove>();
        moves = new Dictionary<PieceTower, HashSet<TileNode>>();
    }

    /// <summary>
    /// Constructor that changes the weights based on difficulty.
    /// </summary>
    /// <param name="difficulty">The difficulty of the AI</param>
    public WeightedAIPlayer(Difficulty difficulty)
    {
        BadMoves = new List<WeightedMove>();
        NeutralMoves = new List<WeightedMove>();
        WayMoves = new List<WeightedMove>();
        SafeMoves = new List<WeightedMove>();
        AttackMoves = new List<WeightedMove>();
        moves = new Dictionary<PieceTower, HashSet<TileNode>>();
        this.difficulty = difficulty;

        WeightDifficulty(difficulty);
    }

    /// <summary>
    /// Sets the weights based on the difficulty of the AI.
    /// </summary>
    /// <param name="difficulty">The difficulty of the AI</param>
    public void WeightDifficulty(Difficulty difficulty)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                attackWeight = 250;
                safeWeight = 25;
                wayWeight = 75;
                neutralWeight = 150;
                badWeight = 500;
                break;

            case Difficulty.Normal:
                attackWeight = 400;
                safeWeight = 100;
                wayWeight = 100;
                neutralWeight = 250;
                badWeight = 150;
                break;

            case Difficulty.Hard:
                attackWeight = 800;
                safeWeight = 125;
                wayWeight = 60;
                neutralWeight = 10;
                badWeight = 5;
                break;
        }
    }

    /// <summary>
    /// Weights the moves.
    /// Takes all available moves and
    /// puts them in the lists / categories
    /// intialized in the contructor
    /// </summary>
    /// <param name="moves">Moves.</param>
    public void WeightMoves(Dictionary<PieceTower, HashSet<TileNode>> moves)
    {
        List<PieceTower> towers = new List<PieceTower>(moves.Keys);

        foreach (PieceTower pieceTower in towers)
        {
            // Indicates if this tower can be attacked by another tower
            bool srcThreatened = CheckThreat(pieceTower.GetNode());

            foreach (TileNode tile in moves[pieceTower])
            {
                // Indicates if this tile can be reached by another tower
                bool destThreatened = CheckThreat(tile);

                // Check for attacking
                if (tile.tower != null && tile.tower.owningColour != colour)
                {
                    int attackValue = WeightAttackValue(tile.tower);
                    AttackMoves.Add(new WeightedMove(pieceTower, tile, attackWeight, attackValue));
                }

                // Check if this tile can be reached by an opponent, and the target tile cannot
                else if (srcThreatened && !destThreatened)
                {
                    SafeMoves.Add(new WeightedMove(pieceTower, tile, safeWeight));
                }

                // Getting off the way
                else if ((pieceTower.GetNode().type == TileNode.Type.Way || pieceTower.GetNode().type == TileNode.Type.WayCross)
                    && (tile.type != TileNode.Type.Way && tile.type != TileNode.Type.WayCross))
                {
                    SafeMoves.Add(new WeightedMove(pieceTower, tile, safeWeight));
                }

                //If it's a way move
                else if (tile.type == TileNode.Type.Way || tile.type == TileNode.Type.WayCross)
                {
                    WayMoves.Add(new WeightedMove(pieceTower, tile, wayWeight));
                }

                // Check if it's a move that gets onto the way
                else if ((tile.type == TileNode.Type.Way || tile.type == TileNode.Type.WayCross) && tile.tower == null)
                {
                    WayMoves.Add(new WeightedMove(pieceTower, tile, wayWeight));
                }

                //If tile can be attacked by another player
                else if (destThreatened)
                {
                    BadMoves.Add(new WeightedMove(pieceTower, tile, badWeight));
                }

                // All other moves fall into this category
                else
                {
                    NeutralMoves.Add(new WeightedMove(pieceTower, tile, neutralWeight));
                }
            }
        }
    }
    
    /// <summary>
    /// Checks the value of each move by
    /// determining each tower's size
    /// and if they have a hook tile or not
    /// </summary>
    /// <returns> attackValue, the nurmerical value of the attac. </returns>
    /// <param name="piece">Piece of who's value we are determining. </param>
    public int WeightAttackValue (PieceTower piece)
    {
        int attackValue = 0;

        foreach (PieceData pieceValue in piece.pieces)
        {
            if (pieceValue.isHook())
            {
                attackValue += 2;
            }
            else
            {
                attackValue += 1;
            }
        }
        
        return attackValue;
    }

    /// <summary>
    /// Checks all enemy moves and if one of those
    /// moves is the input tile threat returns true
    /// otherwise there is not threat to said tile
    /// </summary>
    /// <returns><c>true</c>, if threat was checked on tile input, <c>false</c> otherwise.</returns>
    /// <param name="tile">Tile, the current tile of a tower</param>
    public Boolean CheckThreat(TileNode tile)
    {
        foreach (Player enemy in TurnHandler.players)
        {
            if (!enemy.colour.Equals(colour))
            {
                foreach (PieceTower enemyTower in Towers)
                {
                    HashSet<TileNode> moves = enemyTower.GetMoves().Destinations;

                    foreach (TileNode move in moves)
                    {
                        if (move.Equals(tile))
                        {
                            return true;
                        }
                    }
                }
            }
        }
        // No threat
        return false;
    }

    /// <summary>
    /// Returns the piece to be moved and the destination
    /// </summary>
    /// <param name="piece">Piece.</param>
    /// <param name="dest">Destination.</param>
    public WeightedMove Move(PieceTower piece, TileNode dest)
    {
        return new WeightedMove(piece, dest, 1);
    }

    public override WeightedMove ForcedWay(PieceTower tower, HashSet<TileNode> moves)
    {
        // Check if anything is attackable
        foreach (TileNode tile in moves)
        {
            if (tile.tower != null && tile.tower.owningColour != colour)
            {
                return new WeightedMove(tower, tile, 1);
            }
        }

        // Random move
        return base.ForcedWay(tower, moves);
    }

    /// <summary>
    /// Picks attack moves from the pool of attack moves based on difficulty.
    /// Easy and Normal not yet implimented.
    /// Hard picks the strongest moves available and discards all others.
    /// </summary>
    /// <param name="AttackMoves">The pool of attack moves to choose from</param>
    public void PickAttackMoves(List<WeightedMove> AttackMoves)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:

                break;

            case Difficulty.Normal:

                break;

            case Difficulty.Hard:
                int curTowerSize = 6;
                
                List<WeightedMove> newAttackMoves = new List<WeightedMove>();
                
                AttackMoves.Sort((x, y) => x.attackValue.CompareTo(y.attackValue));

                while (curTowerSize > 0)
                {
                    foreach (WeightedMove move in AttackMoves)
                    {
                        if (move.attackValue >= curTowerSize)
                        {
                            newAttackMoves.Add(move);
                        }
                    }

                    if (newAttackMoves.Count > 0)
                    {
                        AttackMoves = newAttackMoves;
                        newAttackMoves.Clear();
                        curTowerSize = -1;
                    }
                    else
                    {
                        newAttackMoves.Clear();
                        curTowerSize--;
                    }
                }

                break;
        }
    }

    /// <summary>
    /// Ties the WeightedAI together, gets the
    /// moves, picks one and executes
    /// </summary>
    /// <returns>The turn.</returns>
    public override WeightedMove DoTurn()
    {
        moves = GetAllAvailableMoves();

        WeightMoves(moves);
        WeightedMove pickedMove = null;

        //Attempts before defaulting to random move
        int Attempts = 10;

        while (Attempts > 0)
        {
            int maxRoll = 999;
            int roll = new System.Random().Next(maxRoll + 1);

            if (roll > maxRoll - attackWeight && AttackMoves.Count > 0)
            {
                PickAttackMoves(AttackMoves);
                pickedMove = AttackMoves[new System.Random().Next(0, AttackMoves.Count)];
                UnityEngine.Debug.Log(pickedMove.attackValue);
                UnityEngine.Debug.Log("Attack move picked");
            }
            else if (roll > maxRoll - attackWeight - safeWeight && SafeMoves.Count > 0)
            {
                pickedMove = SafeMoves[new System.Random().Next(0, SafeMoves.Count)];
                UnityEngine.Debug.Log("Safe move picked");
            }
            else if (roll > maxRoll - attackWeight - safeWeight - wayWeight && WayMoves.Count > 0)
            {
                pickedMove = WayMoves[new System.Random().Next(0, WayMoves.Count)];
                UnityEngine.Debug.Log("Way move picked");
            }
            else if (roll > maxRoll - attackWeight - safeWeight - wayWeight - neutralWeight && NeutralMoves.Count > 0)
            {
                pickedMove = NeutralMoves[new System.Random().Next(0, NeutralMoves.Count)];
                UnityEngine.Debug.Log("Way move picked");
            }
            else if (BadMoves.Count > 0)
            {
                pickedMove = BadMoves[new System.Random().Next(0, BadMoves.Count)];
                UnityEngine.Debug.Log("Bad move picked");
            }

            if (pickedMove != null)
            {
                break;
            }
            Attempts--;
        }

        //Empty out all saved moves
        BadMoves.Clear();
        WayMoves.Clear();
        SafeMoves.Clear();
        AttackMoves.Clear();

        //No valid moves, or failed to choose
        if (Attempts == 0)
        {
            UnityEngine.Debug.Log("WeightedAI could not decide. Random Move.");
            return RandomMove();
        }
        else
        {
            UnityEngine.Debug.Log("WeightedAI moved.");
            return pickedMove;
        }
    }

    /// <summary>
    /// called when this player's turn starts
    /// </summary>
    public override void TurnStart()
    {
        DisplayTurnDisplay();
    }

    /// <summary>
    /// Called when this player's turn starts
    /// </summary>
    public override void TurnEnd()
    {

    }

    /// <summary>
    /// called when this player's turn was skipped
    /// </summary>
    public override void TurnSkipped()
    {

    }
}
