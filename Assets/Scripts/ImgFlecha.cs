using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImgFlecha : MonoBehaviour
{
    private _GameController gameController;
    private SpriteRenderer sRender;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        sRender = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        sRender.sprite = gameController.imgFlecha[gameController.idFlechaEquipada];
    }
}
