using System.Collections.Generic;
using UnityEngine;

public class OverstackBtnClick : MonoBehaviour
{

    void Start()
    {
        // Ensure that this button is not visible until an overstack event occurs
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Called by the overstack interface button when clicked.
    /// </summary>
    public void Finished()
    {
        // Get all the pieces by searching in the Overstack object
        GameObject ga = GameObject.Find("UICanvas").transform.Find("Overstack").gameObject;

        // List of all the considered pieces
        List<PieceData> pieces = new List<PieceData>(6);
        int numHooks = 0;

        // Populate the list of considered pieces
        foreach (Transform child in ga.transform)
        {
            if (child.GetComponent<OverstackPieceUI>().Considered)
            {
                pieces.Add(child.GetComponent<OverstackPieceUI>().Piece);
            }

            if (child.GetComponent<OverstackPieceUI>().Piece.type == PieceData.Type.Hook)
            {
                numHooks++;
            }
        }

        // Must select a valid number of pieces
        if (pieces.Count < ga.transform.childCount - numHooks && pieces.Count != 6)
        {
            if (!GameObject.Find("Taoex").GetComponent<TurnHandler>().GetCurrentPlayer().IsAI) {
                GameObject.Find("OverstackError").GetComponent<OverstackError>().ShowError("Must select more pieces");
            }
            return;
        }

        // For validation purposes
        Player.PlayerColour attackerColour = ga.GetComponent<OverstackUI>().AttackerColour;
        bool ownColourSelected = false;

        // Validate the pieces
        foreach (PieceData piece in pieces)
        {
            if (piece.colour == attackerColour)
            {
                ownColourSelected = true;
            }
        }

        // Didn't select any of own pieces
        if (!ownColourSelected)
        {
            if (!GameObject.Find("Taoex").GetComponent<TurnHandler>().GetCurrentPlayer().IsAI)
            {
                GameObject.Find("OverstackError").GetComponent<OverstackError>().ShowError("Must select at least one of your own pieces");
            }
            return;
        }

        // Send validated tower to the overstack user interface for completion
        ga.GetComponent<OverstackUI>().Finished(pieces);

        // Disable this button
        gameObject.SetActive(false);
    }
}
