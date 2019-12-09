using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private _GameController gameController;
    private AudioController audioController;

    private SpriteRenderer playerSpriteRenderer;
   
    //Movimentos básicos e controle de animações
    public Transform groundCheck;
    public LayerMask whatIsGround;
    public Collider2D standing, crounching;
    public float speed, jumpForce;
    public bool grounded, lookLeft, attacking, naoPodeAtacar;
    public int idAnimation;
    private float h, v;
    private Animator playerAnimator;
    private Rigidbody2D playerRb;

    //Interação com Itens e Objetos
    public Transform hand;
    private Vector3 dir = Vector3.right;
    private Vector3 dir2 = Vector3.right;   //Para não alterar o tamanho dos projéteis
    public LayerMask interacao;
    public GameObject objetoInteracao;

    //Armas
    public int idArma;
    public int idArmaAtual;
    public GameObject[] armas;
    public GameObject[] arcos, staffs, flechaArco;
    public GameObject magiaPrefab;
    public Transform spawnFlecha, spawnMagia;

    public int vidaMax, vidaAtual;
    public GameObject balaoAlerta;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        audioController = FindObjectOfType(typeof(AudioController)) as AudioController;

        //Carrega os dados iniciais do personagem
        vidaMax = gameController.vidaMaxima;
        idArma = gameController.idArma;
        gameController.manaAtual = gameController.manaMax;


        playerAnimator = GetComponent<Animator>();
        playerRb = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        vidaAtual = vidaMax;

        crounching.enabled = true;
        standing.enabled = false;

        foreach(GameObject o in armas)
        {
            o.SetActive(false);
        }

        foreach (GameObject o in arcos)
        {
            o.SetActive(false);
        }

        foreach (GameObject o in staffs)
        {
            o.SetActive(false);
        }
        //mudando arma
        trocarArma(idArma);
    }

    void FixedUpdate()
    {
        if(gameController.currentState != GameState.GAMEPLAY)
        {
            return;
        }

        grounded = Physics2D.OverlapCircle(groundCheck.position, 0.02f, whatIsGround);
        playerRb.velocity = new Vector2(h * speed, playerRb.velocity.y);
        interagir();
    }

    void Update()
    {
        if(gameController.currentState == GameState.DIALOGO && Input.GetButtonDown("Fire1"))
        {
            playerRb.velocity = new Vector2(0, playerRb.velocity.y);
            playerAnimator.SetInteger("idAnimation", 0);
            objetoInteracao.SendMessage("falar", SendMessageOptions.DontRequireReceiver);
        }

        if (gameController.currentState != GameState.GAMEPLAY)
        {
            return;
        }

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");

        if(h > 0 && lookLeft == true && attacking == false)
        {
            flip();
        }
        else if(h < 0 && lookLeft == false && attacking == false)
        {
            flip();
        }

        if(v < 0)
        {
            idAnimation = 2;
            if(grounded == true)
                h = 0;
        }
        else if(h != 0)
        {
            idAnimation = 1;
        }
        else
        {
            idAnimation = 0;
        }

        if(Input.GetButtonDown("Fire1") && v >= 0 && attacking == false && objetoInteracao == null && naoPodeAtacar == false)
        {
            naoPodeAtacar = true;
            playerAnimator.SetTrigger("atack");
        }

        if (Input.GetButtonDown("Fire1") && v >= 0 && attacking == false && objetoInteracao != null)
        {
            objetoInteracao.SendMessage("interacao", SendMessageOptions.DontRequireReceiver);
        }

        if (Input.GetButtonDown("Jump") && grounded == true && attacking == false)
        {
            playerRb.AddForce(new Vector2(0, jumpForce));
        }

        if(Input.GetKeyDown(KeyCode.Alpha1) && attacking == false)
        {
            trocarArma(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && attacking == false)
        {
            trocarArma(10);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && attacking == false)
        {
            trocarArma(20);
        }

        if (attacking == true && grounded == true)
        {
            h = 0;
        }

        if(v < 0 && grounded == true)
        {
            crounching.enabled = true;
            standing.enabled = false;
        }
        else if(v >= 0 && grounded == true)
        {
            crounching.enabled = false;
            standing.enabled = true;
        }
        else if(v != 0 && grounded == false)
        {
            crounching.enabled = false;
            standing.enabled = true;
        }

        playerAnimator.SetBool("grounded", grounded);
        playerAnimator.SetInteger("idAnimation", idAnimation);
        playerAnimator.SetFloat("speedY", playerRb.velocity.y);
        playerAnimator.SetFloat("idClasseArma", gameController.idClasseArmas[gameController.idArmaAtual]);

        if(gameController.qtdFlechas[gameController.idFlechaEquipada] > 0)
        {
            foreach(GameObject f in flechaArco)
            {
                f.SetActive(true);
            }
        }
        else
        {
            foreach (GameObject f in flechaArco)
            {
                f.SetActive(false);
            }
        }
    }

    void LateUpdate()
    {
        if(gameController.idArma != gameController.idArmaAtual)
        {
            trocarArma(gameController.idArma);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        switch(col.gameObject.tag)
        {
            case "Coletavel":
                col.gameObject.SendMessage("coletar", SendMessageOptions.DontRequireReceiver);
                break;
            case "Inimigo":
                gameController.vidaAtual -= 1;
                break;
        }
    }

    void flip()
    {
        lookLeft = !lookLeft;
        float x = transform.localScale.x;
        x *= -1;
        transform.localScale = new Vector3(x, transform.localScale.y, transform.localScale.z);
        dir.x = x;
        dir2.x *= -1;
    }

    public void atack(int atk)
    {
        switch(atk)
        {
            case 0:
                attacking = false;
                armas[2].SetActive(false);
                StartCoroutine("esperarNovoAtaque");
                break;
            case 1:
                if(gameController.idArma < 10)
                {
                    audioController.tocarFx(audioController.fxSword, 1);
                }
                else if(gameController.idArma >= 1 && gameController.idArma < 15)
                {
                    audioController.tocarFx(audioController.fxAxe, 1);
                }
                else if (gameController.idArma >= 18 && gameController.idArma < 23)
                {
                    audioController.tocarFx(audioController.fxHammer, 1);
                }
                else if (gameController.idArma >= 23 && gameController.idArma < 28)
                {
                    audioController.tocarFx(audioController.fxMace, 1);
                }

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
                StartCoroutine("esperarNovoAtaque");
                break;
            case 1:                
                attacking = true;
                break;
            case 2:
                if(gameController.qtdFlechas[gameController.idFlechaEquipada] > 0)
                {
                    audioController.tocarFx(audioController.fxBow, 1);
                    gameController.qtdFlechas[gameController.idFlechaEquipada] -= 1;
                    GameObject temPrefab = Instantiate(gameController.flechaPrefab[gameController.idFlechaEquipada], spawnFlecha.position, spawnFlecha.localRotation);
                    temPrefab.transform.localScale = new Vector3(temPrefab.transform.localScale.x * dir2.x, temPrefab.transform.localScale.y, temPrefab.transform.localScale.z);
                    temPrefab.GetComponent<Rigidbody2D>().velocity = new Vector2(gameController.velocidadeFlecha[gameController.idFlechaEquipada] * dir2.x, 0);
                    Destroy(temPrefab, 2f);
                }                
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
                StartCoroutine("esperarNovoAtaque");
                break;
            case 1:                
                attacking = true;
                break;
            case 2:
                if(gameController.manaAtual > 0)
                {
                    audioController.tocarFx(audioController.fxStaff, 1);
                    gameController.manaAtual -= 1;
                    GameObject temPrefab = Instantiate(magiaPrefab, spawnMagia.position, spawnMagia.localRotation);
                    temPrefab.GetComponent<Rigidbody2D>().velocity = new Vector2(3 * dir2.x, 0);
                    Destroy(temPrefab, 1f);
                }
                
                break;
        }
    }

    void interagir()
    {
        Debug.DrawRay(hand.position, dir * 0.1f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(hand.position, dir, 0.1f, interacao);
        
        if(hit == true)
        {
            objetoInteracao = hit.collider.gameObject;
            balaoAlerta.SetActive(true);
        }
        else
        {
            objetoInteracao = null;
            balaoAlerta.SetActive(false);
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

    public void changeMaterial(Material novoMaterial)


    {
        playerSpriteRenderer.material = novoMaterial;
        foreach(GameObject o in armas)
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

    public void trocarArma(int id)
    {
        gameController.idArma = id;

        switch(gameController.idClasseArmas[id]) 
        {
            case 0: //espadas, machados, macas, martelos
                armas[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[id];

                ArmaInfo tempInfoArma = armas[0].GetComponent<ArmaInfo>();
                tempInfoArma.danoMin = gameController.danoMinArma[idArma];
                tempInfoArma.danoMax = gameController.danoMaxArma[idArma];
                tempInfoArma.tipoDano = gameController.tipoDanoArma[idArma];

                armas[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[id];
                tempInfoArma = armas[1].GetComponent<ArmaInfo>();
                tempInfoArma.danoMin = gameController.danoMinArma[idArma];
                tempInfoArma.danoMax = gameController.danoMaxArma[idArma];
                tempInfoArma.tipoDano = gameController.tipoDanoArma[idArma];

                armas[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[id];
                tempInfoArma = armas[2].GetComponent<ArmaInfo>();
                tempInfoArma.danoMin = gameController.danoMinArma[idArma];
                tempInfoArma.danoMax = gameController.danoMaxArma[idArma];
                tempInfoArma.tipoDano = gameController.tipoDanoArma[idArma];
                break;

            case 1:
                arcos[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[id];
                arcos[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[id];
                arcos[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[id];
                break;

            case 2:
                staffs[0].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas1[id];
                staffs[1].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas2[id];
                staffs[2].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas3[id];
                staffs[3].GetComponent<SpriteRenderer>().sprite = gameController.spriteArmas4[id];
                break;
        }
        gameController.idArmaAtual = gameController.idArma;
    }

    IEnumerator esperarNovoAtaque()
    {
        yield return new WaitForSeconds(0.2f);
        naoPodeAtacar = false;
    }
}