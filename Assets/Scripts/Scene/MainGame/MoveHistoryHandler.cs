using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveHistoryHandler : MonoBehaviour {

    /// <summary>
    /// Max history list length.
    /// </summary>
    public int maxSize;

    /// <summary>
    /// Move list.
    /// </summary>
    private Queue<GameObject> moves;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
        moves = new Queue<GameObject>();
    }

    /// <summary>
    /// Adds a move to the list of moves and removes the ends if it exceeds the max size
    /// </summary>
    /// <param name="move">Move.</param>
    public void addMove(PieceMove move) {
        #region shifting list to accomidate new move
        // remove end
        if (moves.Count >= maxSize) {
            GameObject last = moves.Dequeue();
            Destroy(last);
        }

        // move everything else down
        foreach (GameObject o in moves) {
            SmoothMoveUI smuiCur = o.GetComponent<SmoothMoveUI>();
            smuiCur.target.y -= 50f;
        }
        #endregion

        #region adding new move
        // add new one
        GameObject display = Instantiate(Resources.Load("Prefabs/UI/PastMoveDisplay")) as GameObject;
        display.transform.SetParent(transform, false);

        SmoothMoveUI smui = display.GetComponent<SmoothMoveUI>();
        smui.target += display.transform.localPosition;
        smui.target.x -= 50f;

        AdjustDisplay(move, display);

        MoveHistoryHover mhh = display.GetComponent<MoveHistoryHover>();
        mhh.src = move.src;
        mhh.dest = move.dest;

        moves.Enqueue(display);
        #endregion
    }

    /// <summary>
    /// Adjusts the display icons and colours
    /// </summary>
    /// <param name="move">Move.</param>
    private void AdjustDisplay(PieceMove move, GameObject display) {

        // Rolled a zero on the way
        if (move.firstTower == null)
        {
            return;
        }

        // find both hexagon images
        Image firstHex = display.transform.Find("FirstHex").GetComponent<Image>();
        Image secHex = display.transform.Find("SecHex").GetComponent<Image>();
        MultiSpriteChanger msc = display.transform.Find("ActionArrow").GetComponent<MultiSpriteChanger>();

        // action maker colour (left)
        firstHex.color = move.firstTower.owningPlayer.getRealColor();

        switch (move.Type) {
            case PieceMove.MoveType.Move:
                secHex.enabled = false;
                msc.ChangeSprite(0);
                break;

            case PieceMove.MoveType.Attack:
                secHex.color = move.secondTower.owningPlayer.getRealColor();
                msc.ChangeSprite(1);
                break;

            case PieceMove.MoveType.Way:
                secHex.enabled = false;
                msc.ChangeSprite(2);
                break;

            case PieceMove.MoveType.WayAttack:
                secHex.color = move.secondTower.owningPlayer.getRealColor();
                msc.ChangeSprite(3);
                break;

        }
    }
}
