using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public abstract class Player {
    /// <summary>
    /// Player colour.
    /// </summary>
    public enum PlayerColour { Brown, Purple, Black, Red, Blue, Green, Hook };

    /// <summary>
    /// The colour.
    /// </summary>
    public PlayerColour colour;

    /// <summary>
    /// The towers.
    /// </summary>
    private List<PieceTower> towers;

    /// <summary>
    /// Gets or sets the towers.
    /// </summary>
    /// <value>The towers.</value>
    public List<PieceTower> Towers { get { return towers; } }

    // Pieces that have not been placed yet
    private List<PlacementPiece> placementPieces;

    /// <summary>
    /// The score.
    /// </summary>
    public int score;

    /// <summary>
    /// Numbers of capture
    /// </summary>
    public int capture;

    private int highestTower;

    /// <summary>
    /// The highest tower.
    /// </summary>
    public int HighestTower
    {
        get {
            return highestTower;
        }
        set {
            if (highestTower < value)
            {
                highestTower = value;
            }
        }
    }

    /// <summary>
    /// The graveyard of pieces owned by this player.
    /// </summary>
    private List<PieceData> takenPieces;

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string playerName;

    /// <summary>
    /// The score display.
    /// </summary>
    public GameObject scoreDisplay;

    /// <summary>
    /// Whether the player is AI or not.
    /// </summary>
    protected bool isAI;

    /// <summary>
    /// Accessor for isAI.
    /// </summary>
    public bool IsAI { get { return isAI; } }

    public List<PlacementPiece> PlacementPieces { get { return placementPieces; } }

    /// <summary>
    /// Initializes a new instance of the <see cref="Player"/> class.
    /// </summary>
    public Player() {
        towers = new List<PieceTower>();
        takenPieces = new List<PieceData>();

        // List to store the twelve starting pieces
        placementPieces = new List<PlacementPiece>(12);
        isAI = false;
        HighestTower = 1;
    }

    /// <summary>
    /// called when this player's turn starts
    /// </summary>
    public abstract void TurnStart();

    /// <summary>
    /// Called when this player's turn starts
    /// </summary>
    public abstract void TurnEnd();

    /// <summary>
    /// called when this player's turn was skipped
    /// </summary>
    public abstract void TurnSkipped();

    /// <summary>
    /// Updates the score and display for this player
    /// </summary>
    public void updateScore() {
        if (colour != PlayerColour.Hook) {
            // calculate the score
            score = 0 + takenPieces.Count * 2;
            for (int i = 0; i < towers.Count; i++) {
                for (int p = 0; p < towers[i].pieces.Count; p++) {
                    PieceData piece = towers[i].pieces[p];
                    score += piece.value;
                }
            }

            // update score with towers
            scoreDisplay.GetComponentInChildren<Text>().text = "" + score;
        }
    }

    /// <summary>
    /// Called when it is this player's turn.
    /// </summary>
	public virtual WeightedMove DoTurn() {
        Debug.Log("Player Moved");

		return null;
    }

    /// <summary>
    /// Adds a tower to this player for scoring. Only used for adding starting towers
    /// </summary>
    /// <param name="tower"></param>
    public void AddTower(PieceTower tower) {
        towers.Add(tower);
    }

    public void RemoveTower(PieceTower tower)
    {
        towers.Remove(tower);
    }

	public Color32 getRealColor(){
		switch (colour) {
		case PlayerColour.Brown:
			return new Color32 (143, 90, 60, 255);
		case PlayerColour.Purple:
			return new Color32 (152, 109, 178, 255);
		case PlayerColour.Black:
			return new Color32 (120, 120, 120, 255);
		case PlayerColour.Red:
			return new Color32 (203, 64, 66, 255);
		case PlayerColour.Blue:
			return new Color32 (46, 169, 233, 255);
		case PlayerColour.Green:
			return new Color32 (0, 170, 144, 255);
        case PlayerColour.Hook:
            return new Color32 (255, 255, 0, 255);
		default:
			return new Color32 (0, 0, 0, 255);
		}
	}

    /// <summary>
    /// Converts the given playerColour into a usable Color object
    /// </summary>
    /// <returns>The real color.</returns>
    /// <param name="c">PlayerColour(Enum)</param>
    public static Color32 ConvertToColor32(Player.PlayerColour c){
        switch (c) {
            case PlayerColour.Brown:
                return new Color32 (143, 90, 60, 255);
            case PlayerColour.Purple:
                return new Color32 (150, 100, 255, 255);
            case PlayerColour.Black:
                return new Color32 (70, 70, 70, 255);
            case PlayerColour.Red:
                return new Color32 (203, 64, 66, 255);
            case PlayerColour.Blue:
                return new Color32 (46, 169, 233, 255);
            case PlayerColour.Green:
                return new Color32 (0, 155, 0, 255);
            case PlayerColour.Hook:
                return new Color32 (255, 255, 0, 255);
            default:
                return new Color32 (0, 0, 0, 255);
        }
    }

    /// <summary>
    /// Totals the number of moves.
    /// </summary>
    /// <returns>The number of moves.</returns>
    public int TotalNumberOfMoves() {
        int total = 0;

        foreach (PieceTower tower in towers) {
            total += tower.GetMoves().Destinations.Count;
        }

        return total;
    }


    /// <summary>
    /// Displaies the turn display on to the UICanvas
    /// </summary>
    public void DisplayTurnDisplay() {
        GameObject exDisplay = GameObject.Find("TurnDisplay(Clone)");

        // remove existing displays
        if (exDisplay != null) {
            GameObject.Destroy(exDisplay);
        }

        GameObject canvas = GameObject.Find("UICanvas");
        GameObject turnDisplay = Object.Instantiate(Resources.Load("Prefabs/UI/TurnDisplay")) as GameObject;

        turnDisplay.transform.SetParent(canvas.transform, false);

        GameObject nameText = turnDisplay.transform.Find("PlayerNameText").gameObject;
        nameText.GetComponent<Text>().text = playerName;

        GameObject hexagon = turnDisplay.transform.Find("HexagonImg").gameObject;
        hexagon.GetComponent<Image>().color = getRealColor();
    }

    /// Places the specified piece onto the node.
    /// </summary>
    /// <param name="node">the node to place the piece onto</param>
    public void PlacePiece(PlacementPiece placementPiece, TileNode node)
    {
        // Add the piece to the game
        AddTower(new PieceTower(this, placementPiece.Piece, node));

        // Must remove the placed piece from the available placement pieces list
        placementPieces.Remove(placementPiece);
    }

    /// <summary>
    /// Takes a piece owned by an opposing player.
    /// </summary>
    /// <param name="piece"></param>
    public void TakePiece(PieceData piece) {
        // Can only take pieces owned by other players
        if (piece.colour != colour && piece.type == PieceData.Type.Normal) {
            takenPieces.Add(piece);
        }
    }
}
