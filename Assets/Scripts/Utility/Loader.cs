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
        Loading,
        Default,
        Menu
    }

    private static Action onLoaderCallback;

    public static void LoadScrene(Scene scene) {
        if (getCurrentScene() == Scene.OuterWorld) {
            RessouceGenerator.Instance.clearRessource();
        }
        onLoaderCallback = () => {
            SceneManager.LoadSceneAsync(scene.ToString());
        };
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    public static Scene getCurrentScene() {
        string currentSceneName =  SceneManager.GetActiveScene().name;
        foreach (var scene in (Scene[]) Enum.GetValues(typeof(Scene)))
        {
            if (scene.ToString() == currentSceneName) {
                return scene;
            }
        }
        return Scene.Default;
    }

    public static void LoaderCallback() {
        onLoaderCallback();
    }
}
