using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotInventario : MonoBehaviour
{
    private _GameController gameController;
    private PItemInfo pItemInfo;

    public int idSlot;
    public GameObject objetoSlot;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        pItemInfo = FindObjectOfType(typeof(PItemInfo)) as PItemInfo;
    }

    public void usarItem()
    {
        if(objetoSlot != null)
        {
            pItemInfo.objetoSlot = objetoSlot;
            pItemInfo.idSlot = idSlot;
            pItemInfo.carregarInfoIntem();

            gameController.openItemInfo();
        }     
    }
}