using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class MoveSet {
    
    /// <summary>
    /// The move list.
    /// </summary>
    private List<Move> moveList;

    /// <summary>
    /// The destinations.
    /// </summary>
    private HashSet<TileNode> destinations;

    /// <summary>
    /// Gets the destinations.
    /// </summary>
    /// <value>The destinations.</value>
    public HashSet<TileNode> Destinations { get{ return destinations; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="MoveSet"/> class.
    /// </summary>
    public MoveSet(){
        moveList = new List<Move>();
        destinations = new HashSet<TileNode>();
    }
        
    /// <summary>
    /// Adds the move.
    /// </summary>
    /// <param name="origin">Origin.</param>
    /// <param name="hookInvolved">If set to <c>true</c> hook involved.</param>
    /// <param name="movement">Movement.</param>
    public void AddMove(TileNode origin, bool hookInvolved, params TileNode[] movement){
        Move newMove = new Move(origin, hookInvolved, movement);
        moveList.Add(newMove);
        destinations.Add(newMove.GetDest());
    }

    /// <summary>
    /// Gets the move object.
    /// </summary>
    /// <returns>The move object.</returns>
    /// <param name="tile">Tile.</param>
    public MoveSet.Move GetMoveObject(TileNode tile){
        foreach (Move move in moveList)
        {
            if (move.GetDest() == tile)
            {
                return move;
            }
        }
        return null;
    }

    /// <summary>
    /// A class represent that represents a possible move.
    /// Move is broken into two TileNode, one for normal and another if hook is involved
    /// </summary>
    public class Move{
        
        /// <summary>
        /// If a hook is involved.
        /// </summary>
        public readonly bool hookInvolved;

        /// <summary>
        /// The origin node.
        /// </summary>
        public readonly TileNode originNode;

        /// <summary>
        /// The movement nodes.
        /// </summary>
        public readonly TileNode[] movementNodes;

        public Move(TileNode originNode, bool hookInvolved, params TileNode[] movementNodes){
            this.originNode = originNode;
            this.hookInvolved = hookInvolved;
            this.movementNodes = movementNodes;
        }

        /// <summary>
        /// Gets the destination.
        /// </summary>
        /// <returns>The destination.</returns>
        public TileNode GetDest() {
            return movementNodes[movementNodes.Length - 1];
        }
    }

}