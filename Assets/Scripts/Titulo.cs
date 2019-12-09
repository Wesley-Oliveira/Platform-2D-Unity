using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.UI;

public class Titulo : MonoBehaviour
{
    public Button btnCarregarJogo;

    //Botões que representam os slots onde há save
    public Button btnCarregarSlot1;
    public Button btnCarregarSlot2;
    public Button btnCarregarSlot3;

    //Botões para novo jogo no slot selecionado
    public Button btnNovoSlot1;
    public Button btnNovoSlot2;
    public Button btnNovoSlot3;

    //Botões que deletam o savegame
    public GameObject btnDelete1;
    public GameObject btnDelete2;
    public GameObject btnDelete3;

    private AudioController audioController;

    void Start()
    {
        audioController = FindObjectOfType(typeof(AudioController)) as AudioController;

        verificarSaveGame();
    }

    public void selecionarPersonagem(int idPersonagem)
    {
        PlayerPrefs.SetInt("idPersonagem", idPersonagem);
        SceneManager.LoadScene("load");
    }

    void verificarSaveGame()
    {
        btnCarregarJogo.interactable = false;

        btnCarregarSlot1.interactable = false;
        btnCarregarSlot2.interactable = false;
        btnCarregarSlot3.interactable = false;

        btnNovoSlot1.interactable = true;
        btnNovoSlot2.interactable = true;
        btnNovoSlot3.interactable = true;

        btnDelete1.SetActive(false);
        btnDelete2.SetActive(false);
        btnDelete3.SetActive(false);

        if (File.Exists(Application.persistentDataPath + "/playerdata1.dat"))
        {
            btnCarregarSlot1.interactable = true;

            btnNovoSlot1.interactable = false;
            btnDelete1.SetActive(true);
        }

        if (File.Exists(Application.persistentDataPath + "/playerdata2.dat"))
        {
            btnCarregarSlot2.interactable = true;

            btnNovoSlot2.interactable = false;
            btnDelete2.SetActive(true);
        }

        if (File.Exists(Application.persistentDataPath + "/playerdata3.dat"))
        {
            btnCarregarSlot3.interactable = true;

            btnNovoSlot3.interactable = false;
            btnDelete3.SetActive(true);
        }

        if (btnCarregarSlot1.interactable == true || btnCarregarSlot2.interactable == true || btnCarregarSlot3.interactable == true)
        {
            btnCarregarJogo.interactable = true;
        }
    }

    public void novoJogo(int slot)
    {
        switch(slot)
        {
            case 1:                
                PlayerPrefs.SetString("slot", "playerdata1.dat");
                break;
            case 2:
                PlayerPrefs.SetString("slot", "playerdata2.dat");
                break;
            case 3:
                PlayerPrefs.SetString("slot", "playerdata3.dat");
                break;
        }
    }

    public void carregarJogo(int slot)
    {
        switch (slot)
        {
            case 1:
                PlayerPrefs.SetString("slot", "playerdata1.dat");
                break;
            case 2:
                PlayerPrefs.SetString("slot", "playerdata2.dat");
                break;
            case 3:
                PlayerPrefs.SetString("slot", "playerdata3.dat");
                break;
        }
        SceneManager.LoadScene("load");
    }

    public void deleteSave(int slot)
    {
        switch (slot)
        {
            case 1:
                if(File.Exists(Application.persistentDataPath + "/playerdata1.dat"))
                {
                    File.Delete(Application.persistentDataPath + "/playerdata1.dat");                   
                }
                break;
            case 2:
                PlayerPrefs.SetString("slot", "playerdata2.dat");
                if (File.Exists(Application.persistentDataPath + "/playerdata2.dat"))
                {
                    File.Delete(Application.persistentDataPath + "/playerdata2.dat");
                }
                break;
            case 3:
                if (File.Exists(Application.persistentDataPath + "/playerdata3.dat"))
                {
                    File.Delete(Application.persistentDataPath + "/playerdata3.dat");
                }
                break;
        }
        verificarSaveGame();
    }

    public void click()
    {
        audioController.tocarFx(audioController.fxClick, 1);
    }
}