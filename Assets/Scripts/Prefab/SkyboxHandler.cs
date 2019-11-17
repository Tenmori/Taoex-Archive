using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxHandler : MonoBehaviour
{
    public string skyboxName = "purplenebula";

    void ChangeSkybox(string newSkyboxName)
    {
        skyboxName = newSkyboxName;
        RenderSettings.skybox = (Material)Resources.Load("Images/Skybox/" + skyboxName + "/" + skyboxName);
    }
}
