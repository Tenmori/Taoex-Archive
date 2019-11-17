using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetupInfo : MonoBehaviour {

    public enum Type {
        Human,
        AIEasy,
        AINormal,
        AIHard
    };

    /// <summary>
    /// The colour.
    /// </summary>
    public Player.PlayerColour colour;

    /// <summary>
    /// The name of the player.
    /// </summary>
    public string playerName = "Player";

    /// <summary>
    /// The type.
    /// </summary>
    public Type type = Type.Human;
	
	/// <summary>
    /// Update is called once per frame
    /// </summary>
	void Update () {
        transform.Find("NamePanel").Find("NameText").gameObject.GetComponent<Text>().text = playerName;
        transform.Find("HexImg").gameObject.GetComponent<Image>().color = Player.ConvertToColor32(colour);
	}

    /// <summary>
    /// Sets the type.
    /// </summary>
    /// <param name="t">Type as an int.</param>
    public void SetType(int t) {
        type = (Type)t;
    }
}
