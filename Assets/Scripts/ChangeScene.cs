using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeScene : MonoBehaviour
{
    public string sceneDestino;
    private Fade fade;
    private _GameController gameController;

    void Start()
    {
        fade = FindObjectOfType(typeof(Fade)) as Fade;
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void interacao()
    {
        StartCoroutine("mudandoScene");
    }

    IEnumerator mudandoScene()
    {
        fade.fadeIn();
        yield return new WaitWhile(() => fade.fume.color.a < 0.9f);

        if(sceneDestino == "titulo")
        {
            Destroy(gameController.gameObject);
        }

        SceneManager.LoadScene(sceneDestino);
    }
}
