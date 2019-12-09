using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum enemyState
{
    PARADO,
    ALERTA,
    PATRULHA,
    ATACK,
    RECUAR
}

public class Goblin : MonoBehaviour
{
    private _GameController gameController;
    private PlayerScript playerScript;
    private Rigidbody2D rBody;
    private SpriteRenderer sRenderer;
    private Vector3 dir = Vector3.right;
    private Animator animator;

    public enemyState stateInicial;
    public enemyState currentEnemyState;
    public LayerMask layerObstaculos;
    public LayerMask layerPersonagem;
    public GameObject alert;
    public bool lookLeft;
    public float distanciaMudarRota;
    public float velocidadeBase;
    public float velocidade;
    public float tempoEsperaIdle;
    public float tempoRecuo;
    public float distanciaVerPersonagem;
    public float distanciaAtaque;
    public float distanciaSairAlerta;

    private bool attacking;
    public int idArma;
    public float idClasse;
    public GameObject[] armas;
    public GameObject[] arcos;
    public GameObject[] staffs;
    public GameObject[] flechaArco;

    public bool ambienteEscuro;
    public bool emAlertaHit;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        playerScript = FindObjectOfType(typeof(PlayerScript)) as PlayerScript;

        sRenderer = GetComponent<SpriteRenderer>();
        rBody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if(lookLeft == true) { flip(); }
        changeState(stateInicial);
        trocarArma((int) idClasse);

        if(ambienteEscuro == true)
        {
            changeMaterial(gameController.luz2D);
        }
        else
        {
            changeMaterial(gameController.padrao2D);
        }
    }

    void Update()
    {
        
        if(currentEnemyState != enemyState.ATACK && currentEnemyState != enemyState.RECUAR)
        {
            RaycastHit2D hitPersonagem = Physics2D.Raycast(transform.position, dir, distanciaVerPersonagem, layerPersonagem);
            Debug.DrawRay(transform.position, dir * distanciaVerPersonagem, Color.red);

            if (hitPersonagem == true)
            {
                changeState(enemyState.ALERTA);
            }
        }        

        if (currentEnemyState == enemyState.PATRULHA)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distanciaMudarRota, layerObstaculos);

            if (hit == true)
            {
                changeState(enemyState.PARADO);
            }
        }

        if (currentEnemyState == enemyState.RECUAR)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, distanciaMudarRota, layerObstaculos);

            if (hit == true)
            {
                changeState(enemyState.PATRULHA);// flip();
            }
        }

        if (currentEnemyState == enemyState.ALERTA)
        {
            alert.SetActive(true);
            float dist = Vector3.Distance(transform.position, playerScript.transform.position);

            if(dist <= distanciaAtaque)
            {
                changeState(enemyState.ATACK);
            }
            else if(dist >= distanciaSairAlerta && emAlertaHit == false)
            {
                changeState(enemyState.PARADO);
            }
        }

        if(currentEnemyState != enemyState.ALERTA)
        {
            alert.SetActive(false);
        }

        rBody.velocity = new Vector2(velocidade, rBody.velocity.y);

        if (velocidade == 0)
        {
            animator.SetInteger("idAnimation", 0);
        }
        else if (velocidade != 0)
        {
            animator.SetInteger("idAnimation", 1);
        }

        animator.SetFloat("idClasse", idClasse);
    }

    void flip()
    {
        this.gameObject.GetComponent<ControleDanoInimigo>().olhandoEsquerda = !this.gameObject.GetComponent<ControleDanoInimigo>().olhandoEsquerda;
        lookLeft = !lookLeft;
        float x = transform.localScale.x;
        x *= -1;
        dir.x = x;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        velocidadeBase *= -1;

        float vAtual = velocidade * -1;
        velocidade = vAtual;
    }

    IEnumerator idle()
    {
        yield return new WaitForSeconds(tempoEsperaIdle);
        flip();
        changeState(enemyState.PATRULHA);
    }

    IEnumerator recuar()
    {
        yield return new WaitForSeconds(tempoRecuo);
        flip();
        changeState(enemyState.ALERTA);
    }

    void changeState(enemyState newState)
    {
        currentEnemyState = newState;

        switch(newState)
        {
            case enemyState.PARADO:
                velocidade = 0;
                StartCoroutine("idle");
                break;
            case enemyState.PATRULHA:
                velocidade = velocidadeBase;
                break;
            case enemyState.ALERTA:
                velocidade = 0;
                alert.SetActive(true);
                break;
            case enemyState.ATACK:
                animator.SetTrigger("atack");
                break;
            case enemyState.RECUAR:
                flip();
                velocidade = velocidadeBase * 2; // modificar para distância ao invés de velocidade de movimento
                StartCoroutine("recuar");
                break;
        }
    }

    public void trocarArma(int id)
    {
        switch (id)
        {
            case 0: //espadas, machados, macas, martelos
                armas[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[idArma];

                ArmaInfo tempInfoArma = armas[0].GetComponent<ArmaInfo>();
                tempInfoArma.danoMin = gameController.danoMinArma[idArma];
                tempInfoArma.danoMax = gameController.danoMaxArma[idArma];
                tempInfoArma.tipoDano = gameController.tipoDanoArma[idArma];

                armas[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[idArma];
                tempInfoArma = armas[1].GetComponent<ArmaInfo>();
                tempInfoArma.danoMin = gameController.danoMinArma[idArma];
                tempInfoArma.danoMax = gameController.danoMaxArma[idArma];
                tempInfoArma.tipoDano = gameController.tipoDanoArma[idArma];

                armas[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[idArma];
                tempInfoArma = armas[2].GetComponent<ArmaInfo>();
                tempInfoArma.danoMin = gameController.danoMinArma[idArma];
                tempInfoArma.danoMax = gameController.danoMaxArma[idArma];
                tempInfoArma.tipoDano = gameController.tipoDanoArma[idArma];
                break;

            case 1:
                arcos[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[idArma];
                arcos[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[idArma];
                arcos[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[idArma];
                break;

            case 2:
                staffs[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[idArma];
                staffs[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[idArma];
                staffs[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[idArma];
                staffs[3].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas4[idArma];
                break;
        }
    }

    public void atack(int atk)
    {
        switch (atk)
        {
            case 0:
                attacking = false;
                armas[2].SetActive(false);
                changeState(enemyState.RECUAR);
                break;
            case 1:
                attacking = true;
                break;
        }
    }

    public void atackFlecha(int atk)
    {
        switch (atk)
        {
            case 0:
                attacking = false;
                arcos[2].SetActive(false);
                break;
            case 1:
                attacking = true;
                break;
            case 2:
                // Caso queira adicionar um goblin que atire flechas, configurar aqui
                //if (gameController.qtdFlechas[gameController.idFlechaEquipada] > 0)
                //{
                    //gameController.qtdFlechas[gameController.idFlechaEquipada] -= 1;
                    //GameObject temPrefab = Instantiate(gameController.flechaPrefab[gameController.idFlechaEquipada], spawnFlecha.position, spawnFlecha.localRotation);
                    //temPrefab.transform.localScale = new Vector3(temPrefab.transform.localScale.x * dir2.x, temPrefab.transform.localScale.y, temPrefab.transform.localScale.z);
                    //temPrefab.GetComponent<Rigidbody2D>().velocity = new Vector2(gameController.velocidadeFlecha[gameController.idFlechaEquipada] * dir2.x, 0);
                    //Destroy(temPrefab, 2f);
               // }
                break;
        }
    }

    public void atackStaff(int atk)
    {
        switch (atk)
        {
            case 0:
                attacking = false;
                staffs[3].SetActive(false);
                break;
            case 1:
                attacking = true;
                break;
            case 2:
                // Caso queira adicionar um goblin mago, configurar aqui
                //if (gameController.manaAtual > 0)
                //{
                // gameController.manaAtual -= 1;
                //GameObject temPrefab = Instantiate(magiaPrefab, spawnMagia.position, spawnMagia.localRotation);
                //temPrefab.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * dir2.x, 0);
                //Destroy(temPrefab, 1f);
                //}

                break;
        }
    }

    void controleArma(int id)
    {
        foreach (GameObject o in armas)
        {
            o.SetActive(false);
        }

        armas[id].SetActive(true);
    }

    void controleArco(int id)
    {
        foreach (GameObject o in arcos)
        {
            o.SetActive(false);
        }

        arcos[id].SetActive(true);
    }

    void controleStaff(int id)
    {
        foreach (GameObject o in staffs)
        {
            o.SetActive(false);
        }

        staffs[id].SetActive(true);
    }

    public void tomeiHit()
    {
        emAlertaHit = true;
        StartCoroutine("hitAlerta");
        changeState(enemyState.ALERTA);
    }

    IEnumerator hitAlerta()
    {
        yield return new WaitForSeconds(2);
        emAlertaHit = false;
    }

    public void changeMaterial(Material novoMaterial)
    {
        sRenderer.material = novoMaterial;
        foreach (GameObject o in armas)
        {
            o.GetComponent<SpriteRenderer>().material = novoMaterial;
        }

        foreach (GameObject o in arcos)
        {
            o.GetComponent<SpriteRenderer>().material = novoMaterial;
        }

        foreach (GameObject o in flechaArco)
        {
            o.GetComponent<SpriteRenderer>().material = novoMaterial;
        }

        foreach (GameObject o in staffs)
        {
            o.GetComponent<SpriteRenderer>().material = novoMaterial;
        }
    }
}
