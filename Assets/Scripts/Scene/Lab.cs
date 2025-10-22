using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lab : Scene.Scene
{
    public override void Load()
    {
        gameObject.SetActive(true);
    }

    public override void Unload()
    {
        gameObject.SetActive(false);
    }
}
