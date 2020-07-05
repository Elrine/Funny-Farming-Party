using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScene : MonoBehaviour
{
    private bool isFistUpdate = true;
    // Update is called once per frame
    void Update()
    {
        if (isFistUpdate) {
            isFistUpdate = false;
            Loader.LoaderCallback();
        }
    }
}
