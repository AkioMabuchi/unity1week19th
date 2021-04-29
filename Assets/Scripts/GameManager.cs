using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private IEnumerator Start()
    {
#if UNITY_EDITOR
        yield return null;
#elif UNITY_WEBGL
        SceneManager.LoadScene("TitleScene", LoadSceneMode.Additive);
        Scene scene = SceneManager.GetSceneByName("TitleScene");
        while (!scene.isLoaded)
        {
            yield return null;
        }

        SceneManager.SetActiveScene(scene);
#else
        yield return null;
#endif
    }
}
