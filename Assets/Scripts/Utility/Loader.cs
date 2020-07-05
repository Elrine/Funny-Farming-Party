using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    public enum Scene {
        OuterWorld,
        Home,
        Loading
    }

    private static Action onLoaderCallback;

    public static void LoadScrene(Scene scene) {
        onLoaderCallback = () => {
            SceneManager.LoadSceneAsync(scene.ToString());
        };
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static void LoaderCallback() {
        onLoaderCallback();
    }
}
