using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public enum GameState
{
    PAUSE,
    GAMEPLAY,
    ITENS,
    DIALOGO,
    FIMDIALOGO,
    LOADGAME
}

public class _GameController : MonoBehaviour
{
    public int idioma;
    public string[] idiomaFolder;
    public GameState currentState;

    public string[] tiposDano;
    public GameObject[] fxDano;
    public GameObject fxMorte;

    public int gold;
    public Text goldTxt;

    [Header("Informações Player")]
    public int idPersonagem;
    public int idPersonagemAtual;
    public int vidaMaxima;
    public int vidaAtual;
    public int manaMax;
    public int manaAtual;
    public int idArma, idArmaAtual;
    public int idFlechaEquipada;
    public int[] qtdFlechas;                        // 0 - flecha comum, 1 - flecha de prata, 2 - flecha ouro
    public int[] qtdPocoes;                         // 0 - Poção de Cura, 1 - Poção Mana

    [Header("Banco de Personagens")]
    public string[] nomePersonagem;
    public Texture[] spriteSheetName;
    public int[] idClasse;
    public GameObject[] ArmaInicial;

    public ItemModelo[] armaInicialPersonagem;

    public int idArmaInicial;

    [Header("Banco de dados Armas")]
    public List<string> nomeArma;
    public List<Sprite> imgInventario;
    public List<int> custoArma;
    public List<int> idClasseArmas;                   // 0: Machados, martelos, espadas - 1: Arcos - 2: Staffs

    public List<Sprite> spriteArmas1;
    public List<Sprite> spriteArmas2;
    public List<Sprite> spriteArmas3;
    public List<Sprite> spriteArmas4;

    public List<int> danoMinArma;
    public List<int> danoMaxArma;
    public List<int> tipoDanoArma;

    public List<int> aprimoramentoArma;

    [Header("Flechas")]
    public Sprite[] icoFlecha;
    public Sprite[] imgFlecha;
    public GameObject[] flechaPrefab;
    public float[] velocidadeFlecha;

    [Header("Paineis")]
    public GameObject painelPause;
    public GameObject painelItens;
    public GameObject painelItemInfo;

    [Header("Primeiro Elemento de Cada Painel")]
    public Button firstPainelPause;
    public Button firstPainelItens;
    public Button firstPainelItemInfo;

    private PlayerScript playerScript;
    private Inventario inventario;
    private Hud hud;
    private AudioController audioController;

    public Material luz2D, padrao2D;

    //Controle de missão
    public bool missao1;// indica que a missão foi concluida

    public List<string> itensInventario;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        inventario = FindObjectOfType(typeof(Inventario)) as Inventario;
        hud = FindObjectOfType(typeof(Hud)) as Hud;
        audioController = FindObjectOfType(typeof(AudioController)) as AudioController;

        painelPause.SetActive(false);
        painelItens.SetActive(false);
        painelItemInfo.SetActive(false);

        validarArma();

        Load(PlayerPrefs.GetString("slot")); 
    }

    void Update()
    {
        if(currentState == GameState.GAMEPLAY)
        {
            if (playerScript == null)
            {
                playerScript = FindObjectOfType(typeof(PlayerScript)) as PlayerScript;
            }

            if (Input.GetButtonDown("Cancel") && currentState != GameState.ITENS)
            {
                pauseGame();
            }

            string s = gold.ToString();
            //goldTxt.text = s.Replace(",", "."); //caso não tenha funcionado o padrão abaixo
            goldTxt.text = gold.ToString("N0");
        }
    }

    public void validarArma()
    {
        if(idClasseArmas[idArma] != idClasse[idPersonagem])
        {
            idArma = idArmaInicial;
            playerScript.trocarArma(idArma);
        }
    }

    public void pauseGame()
    {
        bool pauseState = painelPause.activeSelf;
        pauseState = !pauseState;
        painelPause.SetActive(pauseState);

        switch(pauseState)
        {
            case true:
                audioController.tocarFx(audioController.fxClick, 1);
                audioController.trocarMusica(audioController.musicaTitulo, "", false);
                changeState(GameState.PAUSE);                
                firstPainelPause.Select();
                break;
            case false:
                audioController.trocarMusica(audioController.musicaFase1, "", false);
                changeState(GameState.GAMEPLAY);                
                break;
        }        
    }

    public void changeState(GameState newState)
    {
        currentState = newState;
        switch(newState)
        {
            case GameState.GAMEPLAY:
                Time.timeScale = 1;
                break;

            case GameState.PAUSE:
                Time.timeScale = 0;
                break;

            case GameState.ITENS:
                Time.timeScale = 0;
                break;
            case GameState.FIMDIALOGO:
                StartCoroutine("fimConversa");
                break;
        }
    }

    public void btnItensDown()
    {
        painelPause.SetActive(false);
        painelItens.SetActive(true);
        firstPainelItens.Select();
        inventario.carregarInventario();
        changeState(GameState.ITENS);
    }

    public void fecharPainel()
    {
        painelItens.SetActive(false);
        painelPause.SetActive(true);
        painelItemInfo.SetActive(false);
        firstPainelPause.Select();
        inventario.limparItensCarregados();
        changeState(GameState.PAUSE);
    }

    public void usarItemArma(int idArma)
    {
        playerScript.trocarArma(idArma);
    }

    public void openItemInfo()
    {
        painelItemInfo.SetActive(true);
        firstPainelItemInfo.Select();
    }

    public void fecharItemInfo()
    {
        painelItemInfo.SetActive(false);
    }

    public void voltarGamePlay()
    {
        painelItens.SetActive(false);
        painelPause.SetActive(false);
        painelItemInfo.SetActive(false);
        changeState(GameState.GAMEPLAY);
    }

    public void excluirItem(int idSlot)
    {
        inventario.itemInventario.RemoveAt(idSlot);
        inventario.carregarInventario();
        painelItemInfo.SetActive(false);
        firstPainelItens.Select();
    }

    public void aprimorarArma(int idArma)
    {
        int ap = aprimoramentoArma[idArma];
        if (ap < 10)
        {
            ap += 1;
            aprimoramentoArma[idArma] = ap;
        }
    }

    public void swap(int idSlot)
    {
        GameObject t1 = inventario.itemInventario[0];
        GameObject t2 = inventario.itemInventario[idSlot];

        inventario.itemInventario[0] = t2;
        inventario.itemInventario[idSlot] = t1;

        voltarGamePlay();
    }

    public void coletarArma(GameObject objetoColetado)
    {
        inventario.itemInventario.Add(objetoColetado);
    }

    public void usarPocao(int idPocao)
    {
        if(qtdPocoes[idPocao] > 0)
        {
            qtdPocoes[idPocao] -= 1;
            switch (idPocao)
            {
                case 0:
                    vidaAtual += 3;
                    if (vidaAtual > vidaMaxima)
                    {
                        vidaAtual = vidaMaxima;
                    }
                    break;
                case 1:
                    manaAtual += 3;
                    if(manaAtual > manaMax)
                    {
                        manaAtual = manaMax;
                    }
                    break;
            }
        }
    }

    IEnumerator fimConversa()
    {
        yield return new WaitForEndOfFrame();
        changeState(GameState.GAMEPLAY);
    }

    public string textoFormatado(string frase)
    {
        //cor=nomecor     <color=#corcorrespondente>
        //fimcor          /color>
        
        string temp = frase;
            
        temp = temp.Replace("cor=yellow", "<color=#FFFF00FF>");
        temp = temp.Replace("cor=red", "<color=#FF0000FF>");
        temp = temp.Replace("cor=green", "<color=#00ff00ff>");
        temp = temp.Replace("fimcor", "</color>");
        temp = temp.Replace("fimcor", "</color>");
        temp = temp.Replace("fimcor", "</color>");

        return temp;
    }

    public void Save()
    {
        string nomeArquivoSave = PlayerPrefs.GetString("slot");

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/" + nomeArquivoSave);

        PlayerData data = new PlayerData();
        data.idioma = idioma;
        data.idPersonagem = idPersonagem;
        data.gold = gold;
        data.idArma = idArma;

        data.idFlechaEquipada = idFlechaEquipada;
        data.qtdFlechas = qtdFlechas;
        data.qtdPocoes = qtdPocoes;
        data.aprimoramentoArma = aprimoramentoArma;

        if (itensInventario.Count != 0) { itensInventario.Clear(); } 
        foreach(GameObject i in inventario.itemInventario)
        {
            itensInventario.Add(i.name);
        }

        data.itensInventario = itensInventario;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load(string slot)
    {
        string nomeArquivoSave = PlayerPrefs.GetString("slot");

        if (File.Exists(Application.persistentDataPath + "/" + nomeArquivoSave))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + nomeArquivoSave, FileMode.Open);

            PlayerData data = (PlayerData)bf.Deserialize(file);

            idioma = data.idioma;
            gold = data.gold;
            idPersonagem = data.idPersonagem;
            idFlechaEquipada = data.idFlechaEquipada;
            qtdFlechas = data.qtdFlechas;
            itensInventario = data.itensInventario;
            aprimoramentoArma = data.aprimoramentoArma;

            idArma = data.idArma;
            idArmaAtual = data.idArma;
            idArmaInicial = data.idArma;

            inventario.itemInventario.Clear();

            foreach(string i in itensInventario)
            {
                inventario.itemInventario.Add(Resources.Load<GameObject>("Armas/" + i));
            }

            inventario.itemInventario.Add(ArmaInicial[idPersonagem]);
            GameObject tempArma = Instantiate(ArmaInicial[idPersonagem]);
            inventario.itensCarregados.Add(tempArma);

            vidaAtual = vidaMaxima;
            manaAtual = manaMax;

            file.Close();

            changeState(GameState.GAMEPLAY);
            hud.verificarHudPersonagem();

            string nomeCena = "cena1";
            audioController.trocarMusica(audioController.musicaFase1, nomeCena, true);
        }
        else
        {
            newGame(); 
        }
    }

    void newGame()
    {
        //definir os valores iniciais do jogo
        gold = 0;
        idPersonagem = PlayerPrefs.GetInt("idPersonagem");
        idArma = armaInicialPersonagem[idPersonagem].idArma;

        idFlechaEquipada = 0;
        qtdFlechas[0] = 25;
        qtdFlechas[1] = 0;
        qtdFlechas[2] = 0;

        qtdPocoes[0] = 3;
        qtdPocoes[1] = 3;

        Save(); 
        Load(PlayerPrefs.GetString("slot"));
    }

    public void click()
    {
        audioController.tocarFx(audioController.fxClick, 1);
    }

}

[Serializable]
class PlayerData
{
    public int idioma;
    public int gold;
    public int idPersonagem;
    public int idArma;
    public int idFlechaEquipada;
    public int[] qtdFlechas;
    public int[] qtdPocoes;
    public List<string> itensInventario;
    public List<int> aprimoramentoArma;
}
