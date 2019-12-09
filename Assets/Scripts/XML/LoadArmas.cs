using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using UnityEngine;

public class LoadArmas : MonoBehaviour
{
    private _GameController gameController;

    public string nomeArquivoXml;           // Nome do arquivo XML que será feito a leitura
    public List<string> nomeArma;           // Armazena o nome da arma para exibir no inventário e na loja
    public List<string> nomeIconeArma;      // Nome do icone no arquivo spritesheet de icones das armas 

    public List<Sprite> iconeArma;          // Icone exibido no inventário e loja
    public List<string> categoriaArma;      // Espada, Machado, Martelo, Arco, Cajado, Maca
    public List<int> idClasseArmas;         // 0: Machados, martelos, espadas - 1: Arcos - 2: Staffs

    public List<int> danoMinArma;           // Dano mínimo natural
    public List<int> danoMaxArma;           // Dano máximo caudado pela arma
    public List<int> tipoDanoArma;          // Tipo de dano 

    public List<Sprite> spriteArmas1;
    public List<Sprite> spriteArmas2;
    public List<Sprite> spriteArmas3;
    public List<Sprite> spriteArmas4;

    public List<Sprite> bancoDeSpritesArma;  //armazena todos os sprites de todas as armas de forma temporária
    public Sprite[] SpriteSheetIconesArmas;
    public Sprite[] espadas;
    public Sprite[] machados;
    public Sprite[] arcos;
    public Sprite[] macas;
    public Sprite[] martelos;
    public Sprite[] staffs;

    private Dictionary<string, Sprite> SpriteSheetArmas;

    public Texture ssIcones;
    public Texture ssEspadas;
    public Texture ssMachados;
    public Texture ssArcos;
    public Texture ssMacas;
    public Texture ssMartelos;
    public Texture ssStaffs;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;
        loadData();
    }

    //Função responsável pela leitura do arquivo XML e carregamento das imagens
    void loadData()
    {
        SpriteSheetIconesArmas = Resources.LoadAll<Sprite>(ssIcones.name);
        espadas = Resources.LoadAll<Sprite>(ssEspadas.name);
        machados = Resources.LoadAll<Sprite>(ssMachados.name);
        arcos = Resources.LoadAll<Sprite>(ssArcos.name);
        macas = Resources.LoadAll<Sprite>(ssMacas.name);
        martelos = Resources.LoadAll<Sprite>(ssMartelos.name);
        staffs = Resources.LoadAll<Sprite>(ssStaffs.name);

        foreach (Sprite s in espadas)
        {
            bancoDeSpritesArma.Add(s);
        }

        foreach (Sprite s in machados)
        {
            bancoDeSpritesArma.Add(s);
        }

        foreach (Sprite s in arcos)
        {
            bancoDeSpritesArma.Add(s);
        }

        foreach (Sprite s in macas)
        {
            bancoDeSpritesArma.Add(s);
        }

        foreach (Sprite s in martelos)
        {
            bancoDeSpritesArma.Add(s);
        }

        foreach (Sprite s in staffs)
        {
            bancoDeSpritesArma.Add(s);
        }

        SpriteSheetArmas = bancoDeSpritesArma.ToDictionary(x => x.name, x => x);


        //Leitura do XML
        TextAsset xmlData = (TextAsset)Resources.Load(gameController.idiomaFolder[gameController.idioma] + "/" + nomeArquivoXml);
        XmlDocument XmlDocument = new XmlDocument();
        XmlDocument.LoadXml(xmlData.text);

        foreach (XmlNode atributo in XmlDocument["Armas"].ChildNodes)
        {
            string att = atributo.Attributes["atributo"].Value;

            foreach (XmlNode a in atributo["armas"].ChildNodes)
            {
                switch(att)
                {
                    case "nome":
                        nomeArma.Add(a.InnerText);
                        break;
                    case "icone":
                        nomeIconeArma.Add(a.InnerText);

                        //carrega o icone respectivo ao nome
                        for(int i = 0; i < SpriteSheetIconesArmas.Length; i++)
                        {
                            if(SpriteSheetIconesArmas[i].name == a.InnerText)
                            {
                                iconeArma.Add(SpriteSheetIconesArmas[i]);
                                break;
                            }                      
                        }
                        break;
                    case "categoria":
                        categoriaArma.Add(a.InnerText);

                        switch(a.InnerText)
                        {
                            case "Staff":
                                idClasseArmas.Add(2);
                                break;
                            case "Arco":
                                idClasseArmas.Add(1);
                                break;
                            default:
                                idClasseArmas.Add(0);
                                break;
                        }

                        break;
                    case "danoMin":
                        danoMinArma.Add(int.Parse(a.InnerText));
                        break;
                    case "danoMax":
                        danoMaxArma.Add(int.Parse(a.InnerText));
                        break;
                    case "tipoDano":
                        tipoDanoArma.Add(int.Parse(a.InnerText));
                        break;
                }
            }
        }

        for (int i = 0; i < iconeArma.Count; i++)
        {
            spriteArmas1.Add(SpriteSheetArmas[nomeIconeArma[i] + "0"]);
            spriteArmas2.Add(SpriteSheetArmas[nomeIconeArma[i] + "1"]);
            spriteArmas3.Add(SpriteSheetArmas[nomeIconeArma[i] + "2"]);

            if (categoriaArma[i] != "Staff")
            {
                spriteArmas4.Add(null);
            }
            else
            {
                spriteArmas4.Add(SpriteSheetArmas[nomeIconeArma[i] + "3"]);
            }
        }

        atualizarGameController();
    }

    public void atualizarGameController()
    {
        gameController.nomeArma = nomeArma;
        gameController.idClasseArmas = idClasseArmas;

        gameController.danoMinArma = danoMinArma;
        gameController.danoMaxArma = danoMaxArma;
        gameController.tipoDanoArma = tipoDanoArma;

        gameController.imgInventario = iconeArma;

        gameController.spriteArmas1 = spriteArmas1;
        gameController.spriteArmas2 = spriteArmas2;
        gameController.spriteArmas3 = spriteArmas3;
        gameController.spriteArmas4 = spriteArmas4;
    }
}
