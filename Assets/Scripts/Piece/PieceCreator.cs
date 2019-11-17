using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class for creating pieces.
/// </summary>
public class PieceCreator : MonoBehaviour {

    private readonly int[] dirWheel = { 0, 0, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1 };
    private readonly int[,] dirWheelOffset = { { 1, 9 }, { 11, 7 }, { 9, 1 }, { 7, 3 }, { 5, 1 }, { 3, 11 } };

    /// <summary>
    /// Creates a piece.
    /// </summary>
    /// <returns>The piece.</returns>
    /// <param name="colour">Colour.</param>
    /// <param name="dir">Dir.</param>
    /// <param name="range">Range.</param>
    public PieceData CreatePiece(Player.PlayerColour colour, int dir, int range)
    {
        // only 6 directions
        Debug.Assert(0 <= dir && dir <= 5);

        // max range of 3
        Debug.Assert(1 <= range && range <= 3);

        // only hooks have range of 1
        if (colour == Player.PlayerColour.Hook)
        {
            Debug.Assert(range == 1);
        }
        else
        {
            Debug.Assert(range != 1);
        }

        // normal and flipped/alternate
        int normRan = range;
        int normDir = dir;
        int altRan = normRan == 2 ? 3 : 2;
        int wheelIndex = normDir * 2;

        if (normRan == 3)
        {
            wheelIndex++;
        }

        // array "wheel" dir offset
        wheelIndex += dirWheelOffset[(int)colour, normRan == 2 ? 0 : 1];
        wheelIndex = wheelIndex % 12;

        int altDir = dirWheel[wheelIndex];

        if (altDir == 1 || altDir == 4)
        {
            altDir = (altDir + 4) % 6;
        }
        else if (altDir == 5 || altDir == 2)
        {
            altDir = (altDir + 2) % 6;
        }

        // create piece
        PieceData piece = new PieceData(colour, normDir, normRan, altDir, altRan, Instantiate<GameObject>(Resources.Load<GameObject>("Prefabs/GameObject/HexagonPiece")));

        return piece;
    }
}
