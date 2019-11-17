using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollScript : MonoBehaviour {

    public enum State { Waiting, Rolling, End, Ready};

    public Sprite[] rollSprite;

    private int tick;
    private int result = -999;
    private State state = State.Waiting;
    private System.Random rand;

    /// <summary>
    /// How long the dice roll animation lasts.
    /// </summary>
    private readonly float rollTime = 0.5f;

    void Start() {
        rand = new System.Random();
    }

    /// <summary>
    /// The result of the roll. This value will be initally -999 before the roll has found a result
    /// </summary>
    public int Result {
        get {
            return result;
        }
    }

    /// <summary>
    /// Gets the state of the roll.
    /// </summary>
    /// <value>The state of the roll.</value>
    public State RollState {
        get {
            return state;
        }
    }


    /// <summary>
    /// Starts the roll.
    /// </summary>
    /// <returns>The roll.</returns>
    public IEnumerator StartRoll() {
        state = State.Rolling;
        yield return new WaitForSeconds(rollTime);
        state = State.End;
    }

    /// <summary>
    /// Fixed delay updates
    /// </summary>
    void FixedUpdate() {
        tick++;
        if (state == State.Rolling && tick > 4) {
            GetComponent<Image>().sprite = rollSprite[rand.Next(6)];
            tick -= 4;

        } else if (state == State.End && result == -999) {
            result = rand.Next(0, 10);
            int rollResult = result;

            if (rollResult == 9) {
                rollResult = 5;
            } else if (rollResult > 4) {
                rollResult -= 4;
            }

            GetComponent<Image>().sprite = rollSprite[rollResult];
            state = State.Ready;
        }
	}
}
