using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] imagemObjeto;
    public bool open;
    public GameObject[] loots;
    public int qtdMinItens, qtdMaxItens;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        open = false;
    }

    public void interacao()
    {
        if(open == false)
        {
            open = true;
            spriteRenderer.sprite = imagemObjeto[1];
            StartCoroutine("gerarLoot");
            GetComponent<Collider2D>().enabled = false;
        }
    }

    IEnumerator gerarLoot()
    {
        int qtdMoedas = Random.Range(qtdMinItens, qtdMaxItens);
        for (int l = 0; l < qtdMoedas; l++)
        {
            int rand = Random.Range(0, 100);
            int idLoot = 0;

            GameObject lootTemp = Instantiate(loots[idLoot], transform.position, transform.localRotation);
            lootTemp.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30, 30), 80));
            yield return new WaitForEndOfFrame();
        }
    }
}
