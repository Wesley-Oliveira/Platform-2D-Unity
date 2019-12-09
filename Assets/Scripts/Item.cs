using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private _GameController gameController;
    public int idItem;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void usarItem()
    {
        gameController.usarItemArma(idItem);
    }
}
