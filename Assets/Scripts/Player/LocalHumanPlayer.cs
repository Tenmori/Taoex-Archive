using UnityEngine;
using UnityEngine.UI;

public class LocalHumanPlayer : Player{

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalHumanPlayer"/> class.
    /// </summary>
    public LocalHumanPlayer() {
        playerName = "Human";
    }

    /// <summary>
    /// called when this player's turn starts
    /// </summary>
    public override void TurnStart() {
        DisplayTurnDisplay();
    }

    /// <summary>
    /// Called when this player's turn starts
    /// </summary>
    public override void TurnEnd() {

    }

    /// <summary>
    /// called when this player's turn was skipped
    /// </summary>
    public override void TurnSkipped() {

    }
}


