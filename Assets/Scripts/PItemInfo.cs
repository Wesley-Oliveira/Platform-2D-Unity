using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PItemInfo : MonoBehaviour
{
    private _GameController gameController;
    public int idSlot;
    public GameObject objetoSlot;

    [Header("Configuração Hud")]
    public Image imgItem;
    public Text nomeItem;
    public Text danoArma;
    public GameObject[] aprimoramentos;

    [Header("Botões")]
    public Button btnAprimorar;
    public Button btnEquipar;
    public Button btnExcluir;

    private int idArma;
    private int aprimoramento;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void carregarInfoIntem()
    {
        Item itemInfo = objetoSlot.GetComponent<Item>();
        idArma = itemInfo.idItem;
        imgItem.sprite = gameController.imgInventario[idArma];
        nomeItem.text = gameController.nomeArma[idArma];
        string tipoDano = gameController.tiposDano[gameController.tipoDanoArma[idArma]];
        int danoMin = gameController.danoMinArma[idArma];
        int danoMax = gameController.danoMaxArma[idArma];
        danoArma.text = "Dano: " + danoMin.ToString() + "-" + danoMax.ToString() + " / " + tipoDano;
        carregarAprimoramento();

        if(idSlot == 0)
        {
            btnEquipar.interactable = false;
            btnExcluir.interactable = false;
        }
        else
        {
            int idClasseArma = gameController.idClasseArmas[idArma];
            int idClassePersonagem = gameController.idClasse[gameController.idPersonagem];

            if (idClasseArma == idClassePersonagem)
            {
                btnEquipar.interactable = true;
            }
            else
            {
                btnEquipar.interactable = false;
            }

            btnExcluir.interactable = true;
        }
    }

    public void bAprimorar()
    {
        gameController.aprimorarArma(idArma);
        carregarAprimoramento();
    }

    public void bEquipar()
    {
        objetoSlot.SendMessage("usarItem", SendMessageOptions.DontRequireReceiver);
        gameController.swap(idSlot);
    }

    public void bExcluir()
    {
        gameController.excluirItem(idSlot);
    }

    void carregarAprimoramento()
    {
        aprimoramento = gameController.aprimoramentoArma[idArma];

        if(aprimoramento >= 10)
        {
            btnAprimorar.interactable = false;
        }
        else
        {
            btnAprimorar.interactable = true;
        }

        foreach (GameObject a in aprimoramentos)
        {
            a.SetActive(false);
        }

        for (int i = 0; i < aprimoramento; i++)
        {
            aprimoramentos[i].SetActive(true);
        }
    }
}