using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventario : MonoBehaviour
{
    private _GameController gameController;

    public Button[] slot;
    public Image[] iconItem;

    public Text qtdPorcao, qtdMana, qtdFlechaA, qtdFlechaB, qtdFlechaC;
    public int qPorcao, qMana, qFlechaA, qFlechaB, qFlechaC;

    public List<GameObject> itemInventario;
    public List<GameObject> itensCarregados;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
    }

    public void carregarInventario()
    {
        limparItensCarregados();

        foreach (Button b in slot)
        {
            b.interactable = false;
        }

        foreach (Image i in iconItem)
        {
            i.sprite = null;
            i.gameObject.SetActive(false);
        }

        qtdPorcao.text = "x " + gameController.qtdPocoes[0].ToString();
        qtdMana.text = "x" + gameController.qtdPocoes[1].ToString();
        qtdFlechaA.text = "x" + gameController.qtdFlechas[0].ToString();
        qtdFlechaB.text = "x" + gameController.qtdFlechas[1].ToString(); ;
        qtdFlechaC.text = "x" + gameController.qtdFlechas[2].ToString(); ;

        int s = 0; // ID DO SLOT
        foreach(GameObject i in itemInventario)
        {
            GameObject temp = Instantiate(i);
            Item itemInfo = temp.GetComponent<Item>();

            itensCarregados.Add(temp);

            slot[s].GetComponent<SlotInventario>().objetoSlot = temp;
            slot[s].interactable = true;

            iconItem[s].sprite = gameController.imgInventario[itemInfo.idItem];
            iconItem[s].gameObject.SetActive(true);
            s++;
        }
    }

    public void limparItensCarregados()
    {
        foreach (GameObject ic in itensCarregados)
        {
            Destroy(ic);
        }
        itensCarregados.Clear();
    }
}
