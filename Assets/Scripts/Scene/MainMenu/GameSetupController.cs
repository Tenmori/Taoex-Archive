using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetupController : MonoBehaviour {

    private static float gridCol = 3f;

    /// <summary>
    /// The starting number of panels.
    /// </summary>
    public int startingNumberOfPanels;

    /// <summary>
    /// The minimum number of players.
    /// </summary>
    public int minPlayers = 2;

    /// <summary>
    /// The max number of players.
    /// </summary>
    public int maxPlayers = 6;

    /// <summary>
    /// The player panels.
    /// </summary>
    private List<GameObject> playerPanels;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake() {
        playerPanels = new List<GameObject>();

        // starting amount
        for (int i = 0; i < startingNumberOfPanels; i++) {
            AddPlayerPanel();
        }
    }

    /// <summary>
    /// Changes the player count. Create or removes to match the value
    /// </summary>
    /// <param name="numPlayers">Number players.</param>
    public void ChangePlayerCount(float numPlayers) {
        // despawn if more players
        if (playerPanels.Count > (int)numPlayers) {
            while (playerPanels.Count > (int)numPlayers) {
                RemovePlayerPanel();
            }
        }

        // create if not enough
        if (playerPanels.Count < (int)numPlayers) {
            while (playerPanels.Count < (int)numPlayers) {
                AddPlayerPanel();
            }
        }
    }

    /// <summary>
    /// Increments the player count.
    /// </summary>
    public void IncrementPlayerCount() {
        if (playerPanels.Count < 6) {
            AddPlayerPanel();    
        }
    }

    /// <summary>
    /// Decrements the player count.
    /// </summary>
    public void DecrementPlayerCount() {
        if (playerPanels.Count > minPlayers) {
            RemovePlayerPanel();
        }
    }

    /// <summary>
    /// Adds a player panel.
    /// </summary>
    private void AddPlayerPanel() {
        GameObject panel = Instantiate(Resources.Load("Prefabs/UI/PlayerSetupPanel")) as GameObject;

        #region positioning
        RectTransform rect = panel.GetComponent<RectTransform>();

        // copy current position
        Vector3 pos = new Vector3(rect.anchoredPosition3D.x, rect.anchoredPosition3D.y);

        panel.transform.SetParent(transform.Find("PlayerPanelArea"), false);

        // determine the row / col
        int row = (int)(playerPanels.Count / gridCol);
        int col = (int)(playerPanels.Count % gridCol);

        // offset based on row / col
        Vector3 offset = new Vector3(
            (250f * col) + (12f * (col + 1)),
            (-50f) -(130f * row) - (10f * (row + 1)));
        // add to current position (top left corner)
        pos += offset;

        // set position of panel
        panel.GetComponent<RectTransform>().anchoredPosition3D = pos;
        #endregion

        // add to list
        playerPanels.Add(panel);

        // in position, fade in
        panel.GetComponent<FadeControllerUI>().BeginFadingIn();

        // set colour of player
        PlayerSetupInfo info = panel.GetComponent<PlayerSetupInfo>();
        info.colour = (Player.PlayerColour)(playerPanels.Count - 1);
        info.playerName = info.colour + " Player";
    }

    /// <summary>
    /// Removes a player panel.
    /// </summary>
    private void RemovePlayerPanel() {
        playerPanels[playerPanels.Count - 1].GetComponent<ActiveStateFade>().FadeAndDestory();
        playerPanels[playerPanels.Count - 1] = null;
        playerPanels.RemoveAt(playerPanels.Count - 1);
    }

    /// <summary>
    /// Sets the players in turn handler.
    /// </summary>
    public void SetPlayersInTurnHandler() {
        Player[] players = new Player[playerPanels.Count];

        for (int i = 0; i < playerPanels.Count; i++) {
            PlayerSetupInfo info = playerPanels[i].GetComponent<PlayerSetupInfo>();

            switch (info.type) {
                case PlayerSetupInfo.Type.Human:
                    players[i] = new LocalHumanPlayer();
                    break;
                case PlayerSetupInfo.Type.AIEasy:
                    players[i] = new WeightedAIPlayer(WeightedAIPlayer.Difficulty.Easy);
                    break;
                case PlayerSetupInfo.Type.AINormal:
                    players[i] = new WeightedAIPlayer(WeightedAIPlayer.Difficulty.Normal);
                    break;
                case PlayerSetupInfo.Type.AIHard:
                    players[i] = new WeightedAIPlayer(WeightedAIPlayer.Difficulty.Hard);
                    break;
            }

            players[i].colour = info.colour;
            players[i].playerName = info.playerName;
        }

        TurnHandler.players = players;

        SceneManager.LoadScene("GameBoard");
    }
}
