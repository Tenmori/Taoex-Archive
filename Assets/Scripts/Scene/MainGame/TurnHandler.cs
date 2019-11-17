using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TurnHandler : MonoBehaviour {

    /// <summary>
    /// Game state
    /// </summary>
    public enum State { Placement, Playing, PlayingForceWay };

    /// <summary>
    /// List of players
    /// </summary>
    public static Player[] players;

    /// <summary>
    /// Random placement flag for testing purposes
    /// </summary>
    private readonly bool randomPlacement = false;

    /// <summary>
    /// Turn index
    /// </summary>
    private int turnIndex;

    /// <summary>
    /// The total turns.
    /// </summary>
    private int totalTurns;

    /// <summary>
    /// The current cycle. All players must have a turn before the next cycle
    /// </summary>
    private int currentCycle; 

    /// <summary>
    /// The move history.
    /// </summary>
    private List<PieceMove> moveHistory;

    /// <summary>
    /// The first tile selection.
    /// </summary>
    private TileNode firstSelection;

    /// <summary>
    /// The current moveset for the selected tower
    /// </summary>
    private HashSet<TileNode> moves;

    /// <summary>
    /// Random object
    /// </summary>
    private System.Random rand;

    // used for the random placement function
    private int[] dirWheel = { 0, 0, 5, 5, 4, 4, 3, 3, 2, 2, 1, 1 };
    private int[,] dirWheelOffset = { { 1, 9 }, { 11, 7 }, { 9, 1 }, { 7, 3 }, { 5, 1 }, { 3, 11 } };

    /// <summary>
    /// The result panel.
    /// </summary>
    private GameObject resultPanel;

    /// <summary>
    /// The game current state.
    /// </summary>
    public State state;

    /// <summary>
    /// Float to offset player placement piece.
    /// </summary>
    private float playerPlacementOffset;

    /// <summary>
    /// List of all the available tiles for placement.
    /// </summary>
    private List<TileNode> placementTiles;

    /// <summary>
    /// Accessor for placementTiles.
    /// </summary>
    public List<TileNode> PlacementTiles { get { return placementTiles; } }

    /// <summary>
    /// The placement piece selection.
    /// </summary>
    private PlacementPiece placementPiece;

    /// <summary>
    /// Accessor for placementPiece.
    /// </summary>
    public PlacementPiece PlacementPiece { set { placementPiece = value; } }

    /// <summary>
    /// Number of turns skipped in a row
    /// </summary>
    private int turnsSkipped;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start() {
    }

    /// <summary>
    /// Initalize this instance.
    /// </summary>
	public void Initalize () {
        turnIndex = -1;
        totalTurns = 0;
        currentCycle = 1;
        moveHistory = new List<PieceMove>();
        state = State.Placement;
        rand = new System.Random();

        #region quick ini players
        if (players == null) {
            // ini players 
            players = new Player[3];

            // player 1
            players[0] = new LocalHumanPlayer();
            players[0].colour = Player.PlayerColour.Brown;
            players[0].playerName = players[0].colour + "";

            // player 2
            players[1] = new LocalHumanPlayer();
            players[1].colour = Player.PlayerColour.Blue;
            players[1].playerName = players[1].colour + "";

            // player 3
            players[2] = new LocalHumanPlayer();
            players[2].colour = Player.PlayerColour.Green;
            players[2].playerName = players[2].colour + "";
        }
        #endregion

        #region generate and attach score displays to player
        for (int i = 0 ; i < players.Length; i++) {
            GameObject display = Instantiate(Resources.Load("Prefabs/UI/ScoreDisplay")) as GameObject;

            display.transform.SetParent(GameObject.Find("UICanvas").transform, false);

            // positioning the display
            RectTransform rect = display.GetComponent<RectTransform>();

            Vector3 pos = new Vector3(rect.anchoredPosition3D.x, rect.anchoredPosition3D.y);
            Vector3 offset = new Vector3(
                -(45f * (players.Length / 2f)) + (45f * i) + (10f * i), 
                0f);

            pos += offset;

            rect.anchoredPosition3D = pos;

            // setting display colour
            display.GetComponent<Image>().color = players[i].getRealColor();

            players[i].scoreDisplay = display;
            players[i].updateScore();
        }
        #endregion
        
        // Generate placement pieces for each player
        foreach (Player p in players)
        {
            GeneratePlacementPieces(p);
        }

        // Initialize the placement tiles
        TaoexBoard board = GameObject.Find("Taoex").GetComponent<TaoexBoard>();
        placementTiles = new List<TileNode>();

        // Add all edge tiles to the placement tiles list
        for (int i = 0; i < board.edgeTiles.GetLength(0); i++)
        {
            for (int j = 0; j < board.edgeTiles.GetLength(1); j++)
            {
                placementTiles.Add(board.edgeTiles[i, j]);
            }
        }

        if (randomPlacement)
        {
            // randomly places the pieces
            RandomPlacement();
        }

        // increments the turn from -1 to 0 which is the first player
        NextTurn();
    }

    /// <summary>
    /// handles tile selection event based on game state
    /// </summary>
    /// <param name="node">A selected tilenode</param>
    public void SelectedTile(TileNode node) {
        switch (state) {
            case State.Placement:
                PlacementAction(node);
                break;
            
            case State.Playing:
                PlayingAction(node);
                break;

            case State.PlayingForceWay:
                StartCoroutine(ForcedWayAction(node));
                break;
        }
    }

    /// <summary>
    /// if placement state then this will handle the selection event
    /// </summary>
    /// <param name="node">A selected tilenode</param>
    private void PlacementAction(TileNode node) {

        // Can only place pieces on empty edge tiles
        if (node.type != TileNode.Type.Edge || node.tower != null || placementPiece == null)
        {
            UnhighlightPlacementTiles();
            return;
        }

        // Node direction as an int
        int nodeDir = (node.edgeId + 3) % 6;

        // Invalid starting position for this piece
        if (nodeDir != placementPiece.Piece.direction)
        {
            UnhighlightPlacementTiles();
            return;
        }

        // Validation passed, so place the piece
        players[turnIndex].PlacePiece(placementPiece, node);
        placementPiece.Placed = true;

        // Disable components that were used for placement purposes
        Destroy(placementPiece.gameObject.GetComponent<MeshCollider>());
        //Destroy(placementPiece.gameObject.GetComponent<PlacementPiece>());

        // Setup for the next placement
        placementPiece = null;
        UnhighlightPlacementTiles();
        placementTiles.Remove(node);

        NextTurn();
    }

    /// <summary>
    /// if playing state then this is called
    /// </summary>
    /// <param name="node">A selected tilenode</param>
    private void PlayingAction(TileNode node) {

        // no tile selected
        if (firstSelection == null) {
            #region first Selection
            // check if the selected tower is valid for the player's turn
            if (node.tower != null && node.tower.owningColour == players[turnIndex].colour) {
                firstSelection = node;

                // for a valid tower, highlight the legal moves for it.
                foreach (TileNode moveTile in node.tower.GetMoves().Destinations) {
                    moveTile.highlight();
                }
            }
            #endregion

        // player has selected a tower already (first selection)
        } else {
            #region second selection

            // fix attempt for mystery firstSelection not clearing
            TileNode cfirstSelection = this.firstSelection;
            this.firstSelection = null;

            PieceTower tower = cfirstSelection.tower;
            moves = tower.GetMoves().Destinations;

            // unhighlights the legal moves from the first selected tower
            foreach (TileNode moveTile in moves) {
                moveTile.unhighlight();
            }

            // request move to the next selected tile.
            PieceMove requestedMove = new PieceMove(cfirstSelection, tower.GetMoves().GetMoveObject(node), moves);

            // requestMove.Execute() both validates and execute it.
            if (requestedMove.Execute() == true) {

                // add the requested move (its a legal move) to the history
                moveHistory.Add(requestedMove);
                GameObject.Find("MoveHistoryPanel").GetComponent<MoveHistoryHandler>().addMove(moveHistory[moveHistory.Count - 1]);
                
                // Tower came off of the edge tile
                if (cfirstSelection.type == TileNode.Type.Edge && requestedMove.dest.type != TileNode.Type.Edge)
                {
                    placementTiles.Add(cfirstSelection);
                }

                StartCoroutine(HandleRequestResult(requestedMove.GetResult(), node, tower));
            }

            // reset the selection
            firstSelection = null;

            #endregion
        }
    }

    /// <summary>
    /// Handles the result type of a piece move.
    /// </summary>
    /// <param name="result">the result type</param>
    /// <param name="node">the destination node</param>
    /// <param name="tower">the attacking tower</param>
    /// <returns></returns>
    private IEnumerator HandleRequestResult(PieceMove.ResultType result, TileNode node, PieceTower tower)
    {

        // Do something based on the result of the way move
        switch (result)
        {
            case PieceMove.ResultType.Normal:
                // Wait for overstack to finish
                yield return new WaitUntil(() => OverstackUI.done);

                NextTurn();
                break;

            case PieceMove.ResultType.ForcedWay:
                // Wait for overstack to finish
                yield return new WaitUntil(() => OverstackUI.done);

                StartCoroutine(WayRoll(node, tower, node.wayLine));
                break;

            case PieceMove.ResultType.ForcedWayCross:
                // Wait for overstack to finish
                yield return new WaitUntil(() => OverstackUI.done);

                StartCoroutine(WayCrossRoll(node, tower));
                break;

            case PieceMove.ResultType.Win:
                SceneManager.LoadScene("EndMenu");
                break;
        }
    }

    /// <summary>
    /// Logic for handling the case for way roll on a cross or intersection on the way
    /// </summary>
    /// <returns>A thing used for courtine</returns>
    /// <param name="node">Node.</param>
    /// <param name="tower">Tower.</param>
    private IEnumerator WayCrossRoll(TileNode node, PieceTower tower)
	{

		// create the picker for player to choose direction on the way
		WayCrossPicker wcp = new WayCrossPicker(node);

        // AI picks a random arrow
        if (players[turnIndex].IsAI)
        {
            // AI delay before continuing
            yield return new WaitForSeconds(AIPlayer.AIDelay);

            // Get a random arrow
            GameObject arrow = wcp.Arrows[new System.Random().Next(0, wcp.Arrows.Length)];

            // Pick the arrow
            arrow.GetComponent<WayCrossArrowClick>().OnMouseUpAsButton();
        }

		// wait until the user has picked a direction
		yield return new WaitUntil(() => wcp.decided == true);

		// if resultLine is -1 then somehow the user has not picked a direction
		Debug.Assert(wcp.resultLine != -1);

		// Start a roll along 
		StartCoroutine(WayRoll(node, tower, wcp.resultLine));	
    }

    /// <summary>
    /// Logic for handling a way roll move along a way line
    /// </summary>  
    /// <returns>The roll.</returns>
    /// <param name="node">Node.</param>
    /// <param name="tower">Tower.</param>
    /// <param name="wayLineID">Way line ID.</param>
    private IEnumerator WayRoll(TileNode node, PieceTower tower, int wayLineID) {

        //moves = tower.GetWayMove(9);

        // Wait for overstack to finish
        yield return new WaitUntil(() => OverstackUI.done);

        // Create a way roll dice
        GameObject diceObj = Instantiate(Resources.Load("Prefabs/UI/WayDice")) as GameObject;
 
        // place the way roll dice on the UICanvas
        diceObj.transform.SetParent(GameObject.Find("UICanvas").transform, false);

        // Starts the roll
        DiceRollScript diceScript = diceObj.GetComponent<DiceRollScript>();
        StartCoroutine(diceScript.StartRoll());

        // wait until roll is complete
        yield return new WaitUntil(() => diceScript.RollState == DiceRollScript.State.Ready);

        // if the result is -999, some how the diceroll did not reach a result.
        Debug.Assert(diceObj.GetComponent<DiceRollScript>().Result != -999);

        // Overstacking onto way cross, fix the reference for the tower
        if (tower.GetNode() == null)
        {
            // Get the new tower
            tower = GameObject.Find("Overstack").GetComponent<OverstackUI>().NewTower;

            // Clear reference from overstack interface
            GameObject.Find("Overstack").GetComponent<OverstackUI>().NewTower = null;
        }

        // get moves along the way line with the range from the result of the roll
        moves = tower.GetWayMove(diceObj.GetComponent<DiceRollScript>().Result, wayLineID).Destinations;

        // if no moves are avalible or rolled a zero
        if (moves.Count == 0) {

            // add move to history
            moveHistory.Add(new PieceMove(node, tower.GetMoves().GetMoveObject(node), moves));
            GameObject.Find("MoveHistoryPanel").GetComponent<MoveHistoryHandler>().addMove(moveHistory[moveHistory.Count - 1]);

            // reset selection  
            state = State.Playing;
            firstSelection = null;

            // move to next player's turn
            NextTurn();

        } else {
            // highlight the way move options
			foreach (TileNode wayTile in moves) {
				wayTile.highlight ();
			}

            // set state to special way state
            state = State.PlayingForceWay;

            // set first selection to current position
            firstSelection = node;

            // If the player is an AI
            if (players[turnIndex].IsAI)
            {
                // AI delay before continuing
                yield return new WaitForSeconds(AIPlayer.AIDelay);

                // Forced way move for AI
                StartCoroutine(ForcedWayAction(((AIPlayer)players[turnIndex]).ForcedWay(tower, moves).dest));
            }
        }

        // destory the dice roll ui game object
        Destroy(diceObj, 1f);
    }

    /// <summary>
    /// If forced a roll
    /// </summary>
    /// <param name="node"></param>
    private IEnumerator ForcedWayAction(TileNode node)
    {
        PieceMove requestedMove = new PieceMove(firstSelection, new MoveSet.Move(firstSelection, false, node), moves);

        if (requestedMove.Execute()) {
            moveHistory.Add(requestedMove);
            GameObject.Find("MoveHistoryPanel").GetComponent<MoveHistoryHandler>().addMove(moveHistory[moveHistory.Count - 1]);
            
			// unhighlight the way moves
			foreach (TileNode moveTile in moves)
			{
				moveTile.unhighlight();
			}

            if (requestedMove.Result == PieceMove.ResultType.Win) {
                SceneManager.LoadScene("EndMenu");
            }

            // Wait for overstack
            yield return new WaitUntil(() => OverstackUI.done);

            state = State.Playing;
            firstSelection = null;
            NextTurn();
        }
    }

    /// <summary>
    /// Changes the current player's turn to the next player
    /// </summary>
    private void NextTurn() {
        if (turnIndex != -1) {
            players[turnIndex].TurnEnd();
        }

        turnIndex = (turnIndex + 1) % players.Length;
        totalTurns++;

        // move arrow to next player's display
        GameObject.Find("TurnArrow").GetComponent<RectTransform>().anchoredPosition3D = new Vector3(
            players[turnIndex].scoreDisplay.GetComponent<RectTransform>().anchoredPosition3D.x, 
            players[turnIndex].scoreDisplay.GetComponent<RectTransform>().anchoredPosition3D.y - 25f, 
            players[turnIndex].scoreDisplay.GetComponent<RectTransform>().anchoredPosition3D.z);

        if (turnIndex == 0) {
            currentCycle++;
        }

        // There are placement tiles available
        if (placementTiles.Count > 0)
        {
            // Check if the current player must make a placement move
            if (ForcedPlacementCheck())
            {
                state = State.Placement;
                CreatePlacementArrows();
            } else
            {
                state = State.Playing;
            }
        }
        else
        {
            // No placement tiles available
            state = State.Playing;
        }

        Debug.Log("State: " + state);
          
        // check if player has any moves
        if (state == State.Playing && players[turnIndex].TotalNumberOfMoves() == 0) {
            int moves = players[turnIndex].TotalNumberOfMoves();
            turnsSkipped++;

            // check if all but one player can make a turn;
            if (turnsSkipped >= players.Length - 1) {
                turnIndex = (turnIndex + 1) % players.Length;
                totalTurns++;
                PieceMove.winner = players[turnIndex];
                PieceMove.WIN = PieceMove.WinType.ElimAll;
                SceneManager.LoadScene("EndMenu");
                return;
            }
                
            players[turnIndex].TurnSkipped();
            NextTurn(); // Skip turn!

        } 

        // Player has move
        else {
            players[turnIndex].TurnStart();
            turnsSkipped = 0;
			if (!players[turnIndex].IsAI) {
				players[turnIndex].DoTurn();
			} 
            else {
                StartCoroutine(AITurn());
			}
        }
    }

    /// <summary>
    /// Creates arrows above the placement positions and tiles
    /// </summary>
    private void CreatePlacementArrows() {
        HashSet<TileNode> placementSpots = new HashSet<TileNode>();
        foreach (PlacementPiece p in players[turnIndex].PlacementPieces) {
            // create arrow on placement piece
            GameObject arrow = GameObject.Instantiate(Resources.Load("Prefabs/GameObject/FloatingArrowPointer")) as GameObject;
            Vector3 pos = new Vector3(p.transform.position.x, p.transform.position.y, p.transform.position.z);
            pos.y += 90f;
            arrow.transform.position = pos;

            foreach (TileNode tn in p.GetAvailableTiles()) {
                placementSpots.Add(tn);
            }
        }
        
        /*
        // create arrows over placement spots
        foreach (TileNode tn in placementSpots) {
            GameObject arrow = GameObject.Instantiate(Resources.Load("Prefabs/GameObject/FloatingArrowPointer")) as GameObject;
            Vector3 pos = tn.BasePosition;
            pos.y += 100f;
            arrow.transform.position = pos;
        }
        */
    }

    /// <summary>
    /// Handles turns for AI players.
    /// </summary>
    private IEnumerator AITurn()
    {
        // AI delay before continuing
        yield return new WaitForSeconds(AIPlayer.AIDelay);

        if (state == State.Placement)
        {
            // Get a placement move
            PlacementMove pm = ((AIPlayer)players[turnIndex]).PlacementMove();
            placementPiece = pm.PlacementPiece;

            // Click the placement piece
            placementPiece.OnMouseUpAsButton();

            yield return new WaitForSeconds(AIPlayer.AIDelay);

            // Click the placement tile
            SelectedTile(pm.Destination);
        }
        else
        {
            // Get a weighted move
            WeightedMove wm = players[turnIndex].DoTurn();

            // Do the turn
            SelectedTile(wm.piece.GetNode());

            yield return new WaitForSeconds(AIPlayer.AIDelay);

            SelectedTile(wm.dest);
        }
    }

    /// <summary>
    /// Randomly places pieces for the players.
    /// </summary>
    private void RandomPlacement() {
        turnIndex = 0;
        
        // Keep placing until done
        while (placementTiles.Count > 0 && players[turnIndex].PlacementPieces.Count > 0)
        {
            // Get a random piece
            List<PlacementPiece> placementPieces = players[turnIndex].PlacementPieces;
            PlacementPiece randomPiece = placementPieces[new System.Random().Next(0, placementPieces.Count)];
            List<TileNode> availableTiles = randomPiece.GetAvailableTiles();

            // If this piece can't be placed, get one that can
            if (availableTiles.Count == 0)
            {
                for (int i = 0; i < placementPieces.Count; i++)
                {
                    availableTiles = placementPieces[i].GetAvailableTiles();
                    if (availableTiles.Count > 0)
                    {
                        randomPiece = placementPieces[i];
                        break;
                    }
                }
            }

            TileNode randomTile = availableTiles[new System.Random().Next(0, availableTiles.Count)];

            // Place the random piece onto a random edge tile for its direction
            getPlayerByColour(randomPiece.Piece.colour).PlacePiece(randomPiece, randomTile);
            randomPiece.Placed = true;

            // Cleanup, can't use PlacementAction for this because we dont want to call next turn yet
            Destroy(randomPiece.gameObject.GetComponent<MeshCollider>());
            placementTiles.Remove(randomTile);

            // Increment turn index
            turnIndex = (turnIndex + 1) % players.Length;
            totalTurns++;

            if (turnIndex == 0)
            {
                currentCycle++;
            }
        }

        // Setup for next turn
        turnIndex--;
    }

    /// <summary>
    /// Gets the player by colour.
    /// </summary>
    /// <returns>The player by colour.</returns>
    /// <param name="colour">A playters colour enum. Does not work with hook "colour"</param>
    public Player getPlayerByColour(Player.PlayerColour colour) {

        // search each player for one with the correct colour
        foreach (Player p in players) {
            if (p.colour == colour) {
                return p;
            }
        }

        // did not find a matching player
        return null;
    }

    /// <summary>
    /// Getter for the current player.
    /// </summary>
    /// <returns>the current player</returns>
    public Player GetCurrentPlayer()
    {
        return players[turnIndex];
    }

    /// <summary>
    /// Checks if the current player must do a placement action.
    /// </summary>
    public bool ForcedPlacementCheck()
    {
        foreach (PlacementPiece placementPiece in players[turnIndex].PlacementPieces)
        {
            if (placementPiece.GetAvailableTiles().Count > 0)
            {
                Debug.Log(players[turnIndex].colour + " must place");
                return true;
            }
        }

        // Player doesn't have to place
        return false;
    }

    /// <summary>
    /// Unhighlights the placement tiles
    /// </summary>
    public void UnhighlightPlacementTiles()
    {
        foreach (TileNode placementTile in placementTiles)
        {
            placementTile.unhighlight();
        }
    }

    /// <summary>
    /// Generates all twelve starting pieces for the player.
    /// </summary>
    /// <param name="player">the player to generate pieces for</param>
    public void GeneratePlacementPieces(Player player)
    {
        // Directions as ints
        int N = 0, NE = 1, SE = 2, S = 3, SW = 4, NW = 5;

        // List of pieces to place
        List<PieceData> pieces = new List<PieceData>();
        PieceCreator pieceCreator = GameObject.Find("Taoex").GetComponent<PieceCreator>();

        // Create the pieces
        for (int dir = 0; dir < 6; dir++)
        {
            pieces.Add(pieceCreator.CreatePiece(player.colour, dir, 2));
            pieces.Add(pieceCreator.CreatePiece(player.colour, dir, 3));
        }

        // Reference to the board for getting way cross coordinates
        TaoexBoard board = GameObject.Find("Taoex").GetComponent<TaoexBoard>();
        Vector3 center = GameObject.Find("Compass").transform.position;

        Vector3[] crosses = new Vector3[6];

        // Positions of the way crosses
        crosses[0] = board.GetTiles()[13, 5].BasePosition;     // S
        crosses[1] = board.GetTiles()[7, 8].BasePosition;      // SW
        crosses[2] = board.GetTiles()[7, 14].BasePosition;     // NW
        crosses[3] = board.GetTiles()[13, 17].BasePosition;    // N
        crosses[4] = board.GetTiles()[19, 14].BasePosition;    // NE
        crosses[5] = board.GetTiles()[19, 8].BasePosition;     // SE

        // Pieces per direction are in sets of two, so we need to offset them
        float offset = 0f;
        float angleOffset = 0f;

        // Counter for the piece number for the current direction
        int count = 0;

        foreach (PieceData piece in pieces)
        {
            // Setup the textures
            piece.SetupTextures();

            // Placement piece component so that the piece can handle click events correctly
            player.PlacementPieces.Add(piece.getObj().AddComponent<PlacementPiece>());
            piece.getObj().GetComponent<PlacementPiece>().Piece = piece;

            // Spawn position of the piece
            Vector3 spawnPosition;
            Transform pieceTrans = piece.getObj().transform;

            // Direction of the piece
            int direction = piece.direction;

            // Spawn position starts at the way cross
            spawnPosition = crosses[direction];

            // Vector going outward from the center to the cross
            Vector3 centerToCross = spawnPosition - center;
            centerToCross.Normalize();

            // Move the spawn position back and up
            spawnPosition += centerToCross * 500f;
            spawnPosition.y += 40f;

            // Spawn the piece texture there
            piece.getObj().transform.position = spawnPosition;

            // Make the piece look towards the center
            pieceTrans.LookAt(center);

            // Move along the side of the board using uvn coordinates
            pieceTrans.InverseTransformDirection(pieceTrans.position);
            pieceTrans.Translate(offset - 400f + playerPlacementOffset, 0, 0f);
            pieceTrans.TransformDirection(pieceTrans.position);

            // Rotate the pieces
            piece.getObj().transform.eulerAngles = new Vector3(0, angleOffset, 0);

            // Save the transform
            piece.getObj().GetComponent<PlacementPiece>().PlacementPosition = piece.getObj().transform.position;
            piece.getObj().GetComponent<PlacementPiece>().PlacementRotation = piece.getObj().transform.eulerAngles;

            // Increment the offset
            offset += 75f;
            count++;

            // If this is the second piece, reset offsets for the next set of two
            if (count % 2 == 0)
            {
                offset = 0f;
            }
            
            if (count % 2 == 0)
            {
                angleOffset += 60f;
            }
            
        }
        // Next player's offset
        playerPlacementOffset += 200f;
    }
}
