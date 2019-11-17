using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MultiSpriteChanger : MonoBehaviour {

    /// <summary>
    /// A list of sprites
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// Image componet reference
    /// </summary>
    private Image img;

    private int index;

    /// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake() {
        img = GetComponent<Image>();
        Debug.Assert(img != null);
    }

    /// <summary>
    /// Changes the sprite.
    /// </summary>
    /// <param name="index">Index.</param>
    public void ChangeSprite(int i) {
        Debug.Assert(sprites != null);
        img.sprite = sprites[i];
        index = i;
    }

    /// <summary>
    /// Changes the sprite to the next one. (circular)
    /// </summary>
    public void NextSprite() {
        index++;
        index = index % sprites.Length;
        img.sprite = sprites[index];
    }

    /// <summary>
    /// Changes the sprite to the previous one. (circular)
    /// </summary>
    public void PrevSprite() {
        index += sprites.Length;
        index--;
        index = index % sprites.Length;
        img.sprite = sprites[index];
    }
}
