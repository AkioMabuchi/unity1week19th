using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : SingletonMonoBehaviour<SceneController>
{
    public void ChangeScene(string fromScene, string toScene)
    {
        StartCoroutine(CoroutineChangeScene(fromScene, toScene));
    }

    IEnumerator CoroutineChangeScene(string fromScene, string toScene)
    {
        SceneManager.LoadScene(toScene, LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneByName(toScene);
        while (!scene.isLoaded)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(fromScene);
        SceneManager.SetActiveScene(scene);
    }
}
