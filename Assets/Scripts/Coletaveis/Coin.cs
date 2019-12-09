using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private _GameController gameController;
    public int valor;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void coletar()
    {
        gameController.gold += valor;
        Destroy(this.gameObject);
    }
}
