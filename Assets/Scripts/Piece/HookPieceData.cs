using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class HookPieceData : PieceData{

    /// <summary>
    /// Constructor of a PieceData for a hook piece. The alt direction is the same as the given dir and normal/alt range set to 1
    /// </summary>
    /// <param name="dir">the direction for the hook piece</param>
    /// <param name="obj">Game object associated with this pieceData</param>
    public HookPieceData(int dir, GameObject obj) : base(Player.PlayerColour.Hook, dir, 1, dir, 1, obj) {
        Debug.Assert(dir < 6 && dir >= 0);
        value = 5;
        type = Type.Hook;
    }

}
