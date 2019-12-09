using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using UnityEngine.UI;

public class NPC_1 : MonoBehaviour
{
    private _GameController gameController;
    public string nomeArquivoXml;

    public GameObject canvasNPC;
    public Text caixaTexto;

    public int idFala;
    public int idDialogo;

    public List<string> fala0;
    public List<string> fala1;
    public List<string> fala2;
    public List<string> fala3;
    public List<string> fala4;
    public List<string> fala5;

    public List<string> respostaFala0;

    public List<string> linhasDialogo;

    private bool dialogoOn, respondendoPergunta;

    public GameObject painelResposta;
    public Button btnA;
    public Button btnB;
    public Text textBtnA, textBtnB;

    void Start()
    {
        gameController = FindObjectOfType(typeof(_GameController)) as _GameController;

        canvasNPC.SetActive(false);
        painelResposta.SetActive(false);
        LoadDialogoData();
    }

    public void interacao()
    {
        if(gameController.currentState == GameState.GAMEPLAY)
        {
            gameController.changeState(GameState.DIALOGO);
            idFala = 0;

            //verifica se a missão foi cumprida
            if(idDialogo == 3 && gameController.missao1 == true)
            {
                idDialogo = 4; // dialogo de missao cumprida
            }

            prepararDialogo();
            dialogo();
            canvasNPC.SetActive(true);
            dialogoOn = true;            
        }
    }

    public void falar()
    {
        if (dialogoOn == true && respondendoPergunta == false)
        {
            idFala += 1;
            dialogo();
        }
    }

    public void dialogo()
    {
        if(idFala < linhasDialogo.Count)
        {
            caixaTexto.text = linhasDialogo[idFala];

            if(idDialogo == 0 && idFala == 2)
            {
                textBtnA.text = respostaFala0[0];
                textBtnB.text = respostaFala0[1];
                respondendoPergunta = true;
                btnA.Select();
                painelResposta.SetActive(true);
            }
        }
        else //ENCERRA A CONVERSA
        {
            switch(idDialogo)
            {
                case 0:                    
                    break;
                case 1:
                    idDialogo = 3;
                    break;
                case 2:
                    idDialogo = 0;
                    break;
                case 4:
                    idDialogo = 5;
                    break;
            }

            canvasNPC.SetActive(false);
            dialogoOn = false;
            gameController.changeState(GameState.FIMDIALOGO);
        } 
    }

    void prepararDialogo()
    {
        linhasDialogo.Clear();
        switch (idDialogo)
        {
            case 0:
                foreach (string s in fala0)
                {
                    linhasDialogo.Add(s);
                }
                break;
            case 1:
                foreach (string s in fala1)
                {
                    linhasDialogo.Add(s);
                }
                break;
            case 2:
                foreach (string s in fala2)
                {
                    linhasDialogo.Add(s);
                }
                break;
            case 3:
                foreach (string s in fala3)
                {
                    linhasDialogo.Add(s);
                }
                break;
            case 4:
                foreach (string s in fala4)
                {
                    linhasDialogo.Add(s);
                }
                break;
            case 5:
                foreach (string s in fala5)
                {
                    linhasDialogo.Add(s);
                }
                break;
        }
    }

    public void btnRespostaA()
    {
        idDialogo = 1;
        prepararDialogo();
        idFala = 0;
        respondendoPergunta = false;
        painelResposta.SetActive(false);
        dialogo();
    }

    public void btnRespostaB()
    {
        idDialogo = 2;
        prepararDialogo();
        idFala = 0;
        respondendoPergunta = false;
        painelResposta.SetActive(false);
        dialogo();
    }

    //Ler o arquivo XML do NPC
    void LoadDialogoData()
    {
        TextAsset xmlData = (TextAsset)Resources.Load(gameController.idiomaFolder[gameController.idioma] + "/" + nomeArquivoXml);
        XmlDocument XmlDocument = new XmlDocument();
        XmlDocument.LoadXml(xmlData.text);

        foreach(XmlNode dialogo in XmlDocument["dialogos"].ChildNodes)
        {
            string dialogoName = dialogo.Attributes["name"].Value;

            foreach(XmlNode f in dialogo["falas"].ChildNodes)
            {
                switch(dialogoName)
                {
                    case "fala0":
                        fala0.Add(gameController.textoFormatado(f.InnerText));
                        break;
                    case "fala1":
                        fala1.Add(gameController.textoFormatado(f.InnerText));
                        break;
                    case "fala2":
                        fala2.Add(gameController.textoFormatado(f.InnerText));
                        break;
                    case "fala3":
                        fala3.Add(gameController.textoFormatado(f.InnerText));
                        break;
                    case "fala4":
                        fala4.Add(gameController.textoFormatado(f.InnerText));
                        break;
                    case "fala5":
                        fala5.Add(gameController.textoFormatado(f.InnerText));
                        break;
                    case "resposta0":
                        respostaFala0.Add(gameController.textoFormatado(f.InnerText));
                        break;
                }
            }
        }
    }
}
