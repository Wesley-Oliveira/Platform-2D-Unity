using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arma : MonoBehaviour
{
    private _GameController gameController;
    public GameObject[] itemColetar;

    private bool coletado;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void coletar()
    {
        if(coletado == false)
        {
            coletado = true;
            gameController.coletarArma(itemColetar[Random.Range(0, itemColetar.Length)]);
        }
        
        Destroy(this.gameObject);
    }
}
