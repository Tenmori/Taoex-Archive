using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour {

    /// <summary>
    /// Menus.
    /// </summary>
    public enum Menus {
        // Enter the exact name of the menu here
        MainMenuScreen,     // main menu
        GameSetupScreen     // menu for setting up a game
    }

    /// <summary>
    /// The name of the starting menu.
    /// </summary>
    public string startingMenu;

    /// <summary>
    /// Array with all of the menus avaible
    /// </summary>
    private GameObject[] menuList;

	/// <summary>
    /// Awake this instance.
    /// </summary>
    void Awake() {
        // number of menus 
        int numMenu = System.Enum.GetNames(typeof(Menus)).Length;
        menuList = new GameObject[numMenu];

        // populate menu list
        for (int i = 0; i < menuList.Length; i++) {
            menuList[i] = GameObject.Find(((Menus)i).ToString());
            Debug.Assert(menuList[i] != null, "Menu [" + ((Menus)i).ToString() + "] not found");
        }

        ChangeMenu(startingMenu);
    }

    /// <summary>
    /// Changes the menu.
    /// </summary>
    /// <param name="menu">Menu.</param>
    public void ChangeMenu(Menus menu) {
        for (int i = 0; i < menuList.Length; i++) {
            // check name
            if (menuList[i].name.Equals(menu.ToString())) {
                menuList[i].GetComponent<ActiveStateFade>().FadeAndEnable();
            } 
            else {
                menuList[i].GetComponent<ActiveStateFade>().FadeAndDisable();
            }
        }
    }

    /// <summary>
    /// Changes the menu.
    /// </summary>
    /// <param name="menuName">Menu name.</param>
    public void ChangeMenu(string menuName) {
        Menus m = (Menus)System.Enum.Parse(typeof(Menus), menuName);
        ChangeMenu(m);
    }
}
