using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControleDanoInimigo : MonoBehaviour
{
    private _GameController gameController;
    private PlayerScript playerScript;
    private SpriteRenderer spriteRenderer;
    private Animator inimigoAnimator;

    [Header("Configuração de Vida")]
    public int vidaInimigo;
    public int vidaAtual;
    public GameObject barrasVida;
    public Transform hpBar;
    private float percVida;
    public GameObject danoTxtPrefab;

    [Header("Configuração de Resistência/Fraqueza")]
    public float[] ajusteDano;                          //sistema de resistência/fraqueza contra danos

    [Header("Configuração de KnockBack")]
    public bool olhandoEsquerda;
    public bool playerEsquerda;
    public GameObject knockForcePrefab;                 //força repulsão 
    public Transform knockPosition;                     //ponto de origem da força
    public float knockX;                                //valor padrão do position x
    private float kx;

    [Header("Configuração de Hit")]
    private bool getHit;                                // indica se tomou hit
    public Color[] characterColor;
    private bool died;                                  //Responsável por deixar o corpo do inimigo quando morrer sumir ou deixar o player continuar batendo

    [Header("Configuração de Chão")]
    public Transform groundCheck;
    public LayerMask whatIsGround;

    [Header("Configuração de Loot")]
    public GameObject loots;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        playerScript = FindObjectOfType(typeof(PlayerScript)) as PlayerScript;
        spriteRenderer = GetComponent<SpriteRenderer>();
        inimigoAnimator = GetComponent<Animator>();

        spriteRenderer.color = characterColor[0];
        barrasVida.SetActive(false);
        vidaAtual = vidaInimigo;
        hpBar.localScale = new Vector3(1, 1, 1);

        if(olhandoEsquerda == true)
        {
            float x = transform.localScale.x;
            x *= -1;
            transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
            barrasVida.transform.localScale = new Vector3(x, barrasVida.transform.localScale.y, barrasVida.transform.localScale.z);
        }
    }

    void Update()
    {
        //Verifica se o player está a direita ou esquerda do inimigo
        float xPLayer = playerScript.transform.position.x;

        if (xPLayer < transform.position.x)
        {            
            playerEsquerda = true;
        }
        else if(xPLayer > transform.position.x)
        {         
            playerEsquerda = false;
        }

        /*
          Personagem a esquerda, Inimigo esquerda = kposition +
          Personagem a esquerda, Inimigo direita = kposition -
          Personagem a direita, Inimigo esquerda = kposition -
          Personagem a direita, Inimigo direita = kposition +
        */

        if (olhandoEsquerda == true && playerEsquerda == true)
        {
            kx = knockX;
        }
        else if(olhandoEsquerda == false && playerEsquerda == true)
        {
            kx = knockX * -1;
        }
        else if (olhandoEsquerda == true && playerEsquerda == false)
        {
            kx = knockX * -1;
        }
        else if (olhandoEsquerda == false && playerEsquerda == false)
        {
            kx = knockX;
        }

        knockPosition.localPosition = new Vector3(kx, knockPosition.localPosition.y, 0);
        inimigoAnimator.SetBool("grounded", true);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (died == true)
            return; // se remover esse bloco e a variável died, o cadáver ainda pode ser espancado

        switch(col.gameObject.tag)
        {
            case "Arma":

                if(getHit == false)
                {
                    getHit = true;
                    barrasVida.SetActive(true);
                    ArmaInfo armaInfo = col.gameObject.GetComponent<ArmaInfo>();

                    inimigoAnimator.SetTrigger("hit");

                    float danoArma = Random.Range(armaInfo.danoMin, armaInfo.danoMax);
                    int tipoDano = armaInfo.tipoDano;
                    float danoTomado = danoArma + (danoArma * (ajusteDano[tipoDano] / 100));
                    vidaAtual -= Mathf.RoundToInt(danoTomado);                                      //reduz da vida a quantidade de dano tomado

                    percVida = (float) vidaAtual / (float) vidaInimigo;

                    if(percVida < 0)
                    {
                        percVida = 0;
                    }

                    hpBar.localScale = new Vector3(percVida, 1, 1);

                    if (vidaAtual <= 0)
                    {
                        //deixar o corpo do inimigo sem poder ser arrastado depois de morto
                        this.gameObject.layer = 12;
                        died = true;
                        inimigoAnimator.SetInteger("idAnimation", 3);
                        
                        StartCoroutine("loot");
                    }

                    GameObject danoTemp = Instantiate(danoTxtPrefab, transform.position, transform.localRotation);
                    danoTemp.GetComponent<TextMesh>().text = Mathf.RoundToInt(danoTomado).ToString();
                    //danoTemp.GetComponent<MeshRenderer>().sortingLayerName = "HUD"; //fazer o texto ficar em primeiro plano

                    GameObject fxTemp = Instantiate(gameController.fxDano[tipoDano], transform.position, transform.localRotation);
                    Destroy(fxTemp, 1);

                    int forcaX = 50;
                    if (playerEsquerda == false)
                        forcaX *= -1;
                    danoTemp.GetComponent<Rigidbody2D>().AddForce(new Vector2(forcaX, 210));
                    Destroy(danoTemp, 1f);

                    GameObject knockTemp = Instantiate(knockForcePrefab, knockPosition.position, knockPosition.localRotation);
                    Destroy(knockTemp, 0.02f);

                    StartCoroutine("invuneravel");

                    this.gameObject.SendMessage("tomeiHit", SendMessageOptions.DontRequireReceiver);
                }
                break;
        }
    }

    void flip()
    {
        olhandoEsquerda = !olhandoEsquerda;
        float x = transform.localScale.x;
        x *= -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        barrasVida.transform.localScale = new Vector3(x, barrasVida.transform.localScale.y, barrasVida.transform.localScale.z);
    }

    IEnumerator loot()
    {
        yield return new WaitForSeconds(1);
        GameObject fxMorte = Instantiate(gameController.fxMorte, groundCheck.position, transform.localRotation);
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.enabled = false;

        //Controle de loot
        int qtdMoedas = Random.Range(1, 5);
        for(int l = 0; l < qtdMoedas; l++)
        {
            GameObject lootTemp = Instantiate(loots, transform.position, transform.localRotation);
            lootTemp.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-30, 30), 80));
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(0.7f);
        Destroy(fxMorte, 1);
        Destroy(this.gameObject);
    }

        IEnumerator invuneravel()
    {
        spriteRenderer.color = characterColor[1];
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = characterColor[0];
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = characterColor[1];
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = characterColor[0];
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = characterColor[1];
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = characterColor[0];
        yield return new WaitForSeconds(0.2f);
        getHit = false;
        barrasVida.SetActive(false);
    }
}
