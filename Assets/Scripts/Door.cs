using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    private Fade fade;
    private PlayerScript playerScript;
    
    public Transform destino;

    public bool escuro;
    public Material luz2D, padrao2D;

    void Start()
    {
        fade = FindObjectOfType(typeof(Fade)) as Fade;
        playerScript = FindObjectOfType(typeof(PlayerScript)) as PlayerScript;
    }

    public void interacao()
    {
        StartCoroutine("acionarPorta");
    }

    IEnumerator acionarPorta()
    {
        fade.fadeIn();
        yield return new WaitWhile(() => fade.fume.color.a < 0.9f);
        playerScript.gameObject.SetActive(false);

        switch(escuro)
        {
            case true:
                playerScript.changeMaterial(luz2D);
                break;
            case false:
                playerScript.changeMaterial(padrao2D);
                break;
        }

        playerScript.transform.position = destino.position;
        playerScript.gameObject.SetActive(true);
        fade.fadeOut();
    }
}
