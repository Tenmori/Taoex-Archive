using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TotalPanelScore : MonoBehaviour
{
    /// <summary>
    /// All the buttons for switching the panels.
    /// </summary>
    private static Button[] btns;

    /// <summary>
    /// Array of all the players in the game.
    /// </summary>
	public Player[] players;

    /// <summary>
    /// Array of all the panels in the endMenu.
    /// </summary>
    private static GameObject[] panels;

	void Start()
	{
        // Change the winner text
        if (PieceMove.winner != null)
        {
            Text DisplayWinner = (Text)GameObject.Find("Winner").GetComponent<Text>();
            DisplayWinner.text = PieceMove.winner.playerName + " Wins";
        }
        Text ReasonWin = GameObject.Find("WinCon").GetComponent<Text>();

        // Get all the panels in end screen
        panels = new GameObject[4];
        panels[0] = GameObject.Find("TotalPanel");
        panels[1] = GameObject.Find("CapturePanel");
        panels[2] = GameObject.Find("HighPanel");
        panels[3] = GameObject.Find("CountTowerPanel");

        // Get the player information in the game
        players = TurnHandler.players;

        // Calculate the extra score for winner
        PieceMove.winner.score += players.Length * 6;
            switch (PieceMove.WIN)
            {
                case PieceMove.WinType.ContainFive:
                    PieceMove.winner.score += 10;
                    ReasonWin.text = "Capture an opposing tower containing 5 of his own tiles";
                    break;
                case PieceMove.WinType.ElimAll:
                    PieceMove.winner.score += players.Length * 5;
                    ReasonWin.text = "Eliminate all opponent colors from the board for a win by attrition";
                    break;
                case PieceMove.WinType.CaptureSix:
                    ReasonWin.text = "Capture 6 tiles of the same opposing color";
                    break;
                case PieceMove.WinType.Undetermined:
                    break;
            }

        // sort by score and place it in score panel
        players = sortByScore(players);
		for (var i = 0; i < players.Length; i++)
		{
			GameObject obj = Object.Instantiate(Resources.Load("Prefabs/GameObject/playerScoreWText")) as GameObject;
            obj.transform.SetParent(panels[0].transform, false);
            Vector3 objSize = panels[0].transform.localPosition;
            RectTransform rect = panels[0].transform as RectTransform;
            float width = rect.rect.width;
            float height = rect.rect.height;

            int j = i / 3 - 1;
            obj.transform.localPosition = new Vector3(0 - width/3 * -j,0 - height/3 * (i % 3 - 1));
            print(objSize.x + ", " + objSize.y);
            obj.GetComponent<Image>().color = players[i].getRealColor();
			obj.GetComponentInChildren<Text>().text = players[i].score + "";
		}

        //sort by capture and place in Capture panel
        players = sortByCapture(players);
        for (var i = 0; i < players.Length; i++)
        {
            GameObject obj1 = Object.Instantiate(Resources.Load("Prefabs/GameObject/playerScoreWText")) as GameObject;
            obj1.transform.SetParent(panels[1].transform, false);
            Vector3 objSize = panels[1].transform.localPosition;
            RectTransform rect = panels[1].transform as RectTransform;
            float width = rect.rect.width;
            float height = rect.rect.height;

            int j = i / 3 - 1;
            obj1.transform.localPosition = new Vector3(0 - width/3 * -j,0 - height/3 * (i % 3 - 1));
            print(objSize.x + ", " + objSize.y);
            obj1.GetComponent<Image>().color = players[i].getRealColor();
            obj1.GetComponentInChildren<Text>().text = players[i].capture + "";
        }

        //sort by Highest Tower and place in HighestTower panel
        players = sortByTowerCount(players);
        for (var i = 0; i < players.Length; i++)
        {
            GameObject obj3 = Object.Instantiate(Resources.Load("Prefabs/GameObject/playerScoreWText")) as GameObject;
            obj3.transform.SetParent(panels[2].transform, false);
            Vector3 objSize = panels[2].transform.localPosition;
            RectTransform rect = panels[2].transform as RectTransform;
            float width = rect.rect.width;
            float height = rect.rect.height;

            int j = i / 3 - 1;
            obj3.transform.localPosition = new Vector3(0 - width/3 * -j,0 - height/3 * (i % 3 - 1));
            print(objSize.x + ", " + objSize.y);
            obj3.GetComponent<Image>().color = players[i].getRealColor();
            obj3.GetComponentInChildren<Text>().text = players[i].HighestTower + "";
        }

        //sort by Tower Numbers and place in TowerCount panel
        players = sortByTowerCount(players);
        for (var i = 0; i < players.Length; i++)
        {
            GameObject obj2 = Object.Instantiate(Resources.Load("Prefabs/GameObject/playerScoreWText")) as GameObject;
            obj2.transform.SetParent(panels[3].transform, false);
            Vector3 objSize = panels[3].transform.localPosition;
            RectTransform rect = panels[3].transform as RectTransform;
            float width = rect.rect.width;
            float height = rect.rect.height;

            int j = i / 3 - 1;
            obj2.transform.localPosition = new Vector3(0 - width/3 * -j,0 - height/3 * (i % 3 - 1));
            print(objSize.x + ", " + objSize.y);
            obj2.GetComponent<Image>().color = players[i].getRealColor();
            obj2.GetComponentInChildren<Text>().text = players[i].Towers.Count + "";
        }

        // Active the default panel
        panels[0].SetActive(true);
        panels[1].SetActive(false);
        panels[2].SetActive(false);
        panels[3].SetActive(false);

        // Change the color of the button related to the current panel
        btns = GetComponentsInChildren<Button>();
        btns[1].image.color = new Color32(144, 144, 144, 255);
        for (int i = 1; i < btns.Length; i++)
        {
            btns[i].onClick.AddListener(changeBtnColor);
        }
	}

    /// <summary>
    /// This function is for testing, it generates num players.
    /// </summary>
    /// <param name="num">numbers of players</param>
	public void generatePlayer(int num)
	{
		players = new Player[num];
        System.Random a = new System.Random();
		for (var i = 0; i < num; i++)
		{
            players[i] = new LocalHumanPlayer();
//			players [i].colour = (Player.PlayerColour)Enum.ToObject (typeof(Player.PlayerColour), i);
			players[i].colour = (Player.PlayerColour)i;

			players[i].score = i * 10;
            players[i].capture = a.Next(0, 100);

		}
	}

    /// <summary>
    /// Sort by score
    /// </summary>
    /// <param name="players">Players</param>
    /// <returns>Sorted by score players</returns>
	public Player[] sortByScore(Player[] players)
	{
		for (var i = 0; i < players.Length; i++)
		{
			for (var j = i + 1; j < players.Length; j++)
			{
				if (players[i].score < players[j].score)
				{
					Player temp = players[j];
					players[j] = players[i];
					players[i] = temp;
				}
			}
		}
		return players;
	}

    /// <summary>
    /// Sort by numbers of capture
    /// </summary>
    /// <param name="players">Players</param>
    /// <returns>Sorted by numbers of capture players</returns>
    public Player[] sortByCapture(Player[] players)
    {
        for (var i = 0; i < players.Length; i++)
        {
            for (var j = i + 1; j < players.Length; j++)
            {
                if (players[i].capture < players[j].capture)
                {
                    Player temp = players[j];
                    players[j] = players[i];
                    players[i] = temp;
                }
            }
        }
        return players;
    }

    /// <summary>
    /// Sort by the tower amount
    /// </summary>
    /// <param name="players">Players</param>
    /// <returns>Sorted by the amount of tower belong to each player</returns>
    public Player[] sortByTowerCount(Player[] players)
    {
        for (var i = 0; i < players.Length; i++)
        {
            for (var j = i + 1; j < players.Length; j++)
            {
                if (players[i].Towers.Count < players[j].Towers.Count)
                {
                    Player temp = players[j];
                    players[j] = players[i];
                    players[i] = temp;
                }
            }
        }
        return players;
    }

    /// <summary>
    /// Sort by the highest Tower belong to each player
    /// </summary>
    /// <param name="players">Players</param>
    /// <returns>Sorted player</returns>
    public Player[] sortByHighestTower(Player[] players)
    {
        for (var i = 0; i < players.Length; i++)
        {
            for (var j = i + 1; j < players.Length; j++)
            {
                if (players[i].HighestTower < players[j].HighestTower)
                {
                    Player temp = players[j];
                    players[j] = players[i];
                    players[i] = temp;
                }
            }
        }
        return players;
    }

    /// <summary>
    /// Change the color of the menu button to highlight the current panel.
    /// </summary>
    void changeBtnColor() {
        for (int i = 0; i < panels.Length; i++)
        {
            if (panels[i].activeInHierarchy == true)
            {
                btns[i + 1].image.color = new Color32(144, 144, 144, 255);
            }
            else
            {
                btns[i + 1].image.color = new Color32(255, 255, 255, 255);
            }
        }
    }
}
