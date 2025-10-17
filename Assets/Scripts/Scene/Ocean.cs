using System.Collections;
using System.Collections.Generic;
using Scene;
using UnityEngine;

public class Ocean : SceneManager
{
    public GameObject BackGround;
    public GameObject vp;
    public override void Load()
    {
        gameObject.SetActive(true);
    }

    public override void Unload()
    {
        gameObject.SetActive(false);
    }

   
}
