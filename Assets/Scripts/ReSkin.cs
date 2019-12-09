using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ReSkin : MonoBehaviour
{
    private _GameController gameController;
    public bool isPlayer;                       //Indica se o script está associado ao personagem jogável
    private SpriteRenderer sRender;

    public Sprite[] sprites;
    public string spriteSheetName;              //Nome do spritesheet que queira utilizar
    public string loadedSpriteSheetName;        //Nome do sprisheet em uso

    private Dictionary<string, Sprite> spriteSheet;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        if(isPlayer)
        {
            spriteSheetName = gameController.spriteSheetName[gameController.idPersonagem].name;
        }
        sRender = GetComponent<SpriteRenderer>();
        LoadSpriteSheet();
    }

    void LateUpdate()
    {
        if (isPlayer)
        {
            if(gameController.idPersonagem != gameController.idPersonagemAtual)
            {
                spriteSheetName = gameController.spriteSheetName[gameController.idPersonagem].name;
                gameController.idPersonagemAtual = gameController.idPersonagem;
            }
            gameController.validarArma();
        }

        if (loadedSpriteSheetName != spriteSheetName)
        {
            LoadSpriteSheet();
        }

        sRender.sprite = spriteSheet[sRender.sprite.name];
    }

    private void LoadSpriteSheet()
    {
        sprites = Resources.LoadAll<Sprite>(spriteSheetName);
        spriteSheet = sprites.ToDictionary(x => x.name, x => x);
        loadedSpriteSheetName = spriteSheetName;
    }
}
