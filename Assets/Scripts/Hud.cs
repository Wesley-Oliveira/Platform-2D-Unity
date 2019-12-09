using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    private _GameController gameController;
    private PlayerScript playerScript;
    public Image[] hpBar;
    public Sprite half, full;
    public Image[] mpBar;
    public Sprite mHalf, mFull;
    public GameObject painelMana;
    public GameObject painelFlechas;
    public Text qtdFlechas;
    public Image icoFlechas;

    public GameObject boxHp;
    public Text qtdHpBox;
    public GameObject boxMp;
    public Text qtdMpBox;

    public RectTransform boxA, boxB;
    public Vector2 posA, posB;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        playerScript = FindObjectOfType(typeof(PlayerScript)) as PlayerScript;
        painelMana.SetActive(false);
        painelFlechas.SetActive(false);
        boxMp.SetActive(false);
        boxHp.SetActive(false);

        if (gameController.idClasse[gameController.idPersonagem] == 2)
        {
            painelMana.SetActive(true);            
        }
        else if(gameController.idClasse[gameController.idPersonagem] == 1)
        {
            icoFlechas.sprite = gameController.icoFlecha[gameController.idFlechaEquipada];
            painelFlechas.SetActive(true);
        }

        posA = boxA.anchoredPosition;
        posB = boxB.anchoredPosition;
    }

    void Update()
    {
        controleBarraVida();
        posicaoCaixaPocoes();
        if (painelMana.activeSelf == true)
        {
            controleBarraMana();
        }
        else if(painelFlechas.activeSelf == true)
        {
            if(Input.GetButtonDown("btnL"))
            {
                if(gameController.idFlechaEquipada == 0)
                {
                    gameController.idFlechaEquipada = gameController.icoFlecha.Length - 1;
                }
                else
                {
                    gameController.idFlechaEquipada -= 1;
                }
            }
            else if(Input.GetButtonDown("btnR"))
            {
                if (gameController.idFlechaEquipada == gameController.icoFlecha.Length - 1)
                {
                    gameController.idFlechaEquipada = 0;
                }
                else
                {
                    gameController.idFlechaEquipada += 1;
                }
            }

            icoFlechas.sprite = gameController.icoFlecha[gameController.idFlechaEquipada];
            qtdFlechas.text = "x " + gameController.qtdFlechas[gameController.idFlechaEquipada].ToString();
        }
        
        if(gameController.idClasse[gameController.idPersonagem] == 2)
        {
            painelMana.SetActive(true);
        }
        else if(gameController.idClasse[gameController.idPersonagem] == 1)
        {
            painelFlechas.SetActive(true);
        }
        else
        {
            painelFlechas.SetActive(false);
            painelMana.SetActive(false);
        }
        qtdHpBox.text = gameController.qtdPocoes[0].ToString();
        qtdMpBox.text = gameController.qtdPocoes[1].ToString();
    }

    void posicaoCaixaPocoes()
    {
        if(gameController.qtdPocoes[0] > 0)
        {
            boxHp.GetComponent<RectTransform>().anchoredPosition = posA;
            boxMp.GetComponent<RectTransform>().anchoredPosition = posB;
        }
        else
        {
            boxHp.GetComponent<RectTransform>().anchoredPosition = posB;
            boxMp.GetComponent<RectTransform>().anchoredPosition = posA;
        }
    }

    void controleBarraVida()
    {
        float percVida = (float)gameController.vidaAtual / (float)gameController.vidaMaxima;

        if (Input.GetButtonDown("itemA") && percVida < 1)
        {
            gameController.usarPocao(0); //POÇÃO DE CURA
        }

        foreach (Image img in hpBar)
        {
            img.enabled = true;
            img.sprite = full;
        }

        if(percVida == 1)
        {

        }
        else if(percVida >= 0.9f)
        {
            hpBar[4].sprite = half;
        }
        else if (percVida >= 0.8f)
        {
            hpBar[4].enabled = false;
        }
        else if (percVida >= 0.7f)
        {
            hpBar[4].enabled = false;
            hpBar[3].sprite = half;
        }
        else if (percVida >= 0.6f)
        {
            hpBar[4].enabled = false;
            hpBar[3].enabled = false;
        }
        else if (percVida >= 0.5f)
        {
            hpBar[4].enabled = false;
            hpBar[3].enabled = false;
            hpBar[2].sprite = half;
        }
        else if (percVida >= 0.4f)
        {
            hpBar[4].enabled = false;
            hpBar[3].enabled = false;
            hpBar[2].enabled = false;
        }
        else if (percVida >= 0.3f)
        {
            hpBar[4].enabled = false;
            hpBar[3].enabled = false;
            hpBar[2].enabled = false;
            hpBar[1].sprite = half;
        }
        else if (percVida >= 0.2f)
        {
            hpBar[4].enabled = false;
            hpBar[3].enabled = false;
            hpBar[2].enabled = false;
            hpBar[1].enabled = false;
        }
        else if (percVida >= 0.01f)
        {
            hpBar[4].enabled = false;
            hpBar[3].enabled = false;
            hpBar[2].enabled = false;
            hpBar[1].enabled = false;
            hpBar[0].sprite = half;
        }
        else if (percVida <= 0f)
        {
            hpBar[4].enabled = false;
            hpBar[3].enabled = false;
            hpBar[2].enabled = false;
            hpBar[1].enabled = false;
            hpBar[0].enabled = false;
        }

        if(gameController.qtdPocoes[0] > 0)
        {
            boxHp.SetActive(true);
        }
        else
        {
            boxHp.SetActive(false);
        }
    }

    void controleBarraMana()
    {
        float percMana = (float)gameController.manaAtual / (float)gameController.manaMax;

        if (Input.GetButtonDown("itemB") && percMana < 1)
        {
            gameController.usarPocao(1); //POÇÃO DE MANA
        }

        foreach (Image img in mpBar)
        {
            img.enabled = true;
            img.sprite = mFull;
        }

        if (percMana == 1)
        {

        }
        else if (percMana >= 0.9f)
        {
            mpBar[4].sprite = mHalf;
        }
        else if (percMana >= 0.8f)
        {
            mpBar[4].enabled = false;
        }
        else if (percMana >= 0.7f)
        {
            mpBar[4].enabled = false;
            mpBar[3].sprite = mHalf;
        }
        else if (percMana >= 0.6f)
        {
            mpBar[4].enabled = false;
            mpBar[3].enabled = false;
        }
        else if (percMana >= 0.5f)
        {
            mpBar[4].enabled = false;
            mpBar[3].enabled = false;
            mpBar[2].sprite = mHalf;
        }
        else if (percMana >= 0.4f)
        {
            mpBar[4].enabled = false;
            mpBar[3].enabled = false;
            mpBar[2].enabled = false;
        }
        else if (percMana >= 0.3f)
        {
            mpBar[4].enabled = false;
            mpBar[3].enabled = false;
            mpBar[2].enabled = false;
            mpBar[1].sprite = mHalf;
        }
        else if (percMana >= 0.2f)
        {
            mpBar[4].enabled = false;
            mpBar[3].enabled = false;
            mpBar[2].enabled = false;
            mpBar[1].enabled = false;
        }
        else if (percMana >= 0.01f)
        {
            mpBar[4].enabled = false;
            mpBar[3].enabled = false;
            mpBar[2].enabled = false;
            mpBar[1].enabled = false;
            mpBar[0].sprite = mHalf;
        }
        else if (percMana <= 0f)
        {
            mpBar[4].enabled = false;
            mpBar[3].enabled = false;
            mpBar[2].enabled = false;
            mpBar[1].enabled = false;
            mpBar[0].enabled = false;
        }

        if (gameController.qtdPocoes[1] > 0)
        {
            boxMp.SetActive(true);
        }
        else
        {
            boxMp.SetActive(false);
        }
    }

    public void verificarHudPersonagem()
    {
        painelMana.SetActive(false);
        painelFlechas.SetActive(false);

        if (gameController.idClasse[gameController.idPersonagem] == 2)
        {
            painelMana.SetActive(true);
        }
        else if (gameController.idClasse[gameController.idPersonagem] == 1)
        {
            icoFlechas.sprite = gameController.icoFlecha[gameController.idFlechaEquipada];
            painelFlechas.SetActive(true);
        }
    }
}
