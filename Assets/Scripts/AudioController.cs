using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioController : MonoBehaviour
{
    public AudioSource sMusic;  // FONTE DE MUSICA
    public AudioSource sFx;     // FONTE DE EFEITOS SONOROS

    [Header("Musicas")]
    public AudioClip musicaTitulo;
    public AudioClip musicaFase1;

    [Header("FX")]
    public AudioClip fxClick;

    public AudioClip fxSword;
    public AudioClip fxAxe;
    public AudioClip fxBow;
    public AudioClip fxHammer;
    public AudioClip fxMace;
    public AudioClip fxStaff;

    // configurações dos audios
    public float volumeMaximoMusica;
    public float volumeMaximoFx;

    // configurações da troca de musica
    private AudioClip novaMusica;
    private string novaCena;
    private bool trocarCena;

    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if(PlayerPrefs.GetInt("valoresIniciais") == 0)
        {
            PlayerPrefs.SetInt("valoresIniciais", 1);
            PlayerPrefs.SetFloat("volumeMaximoMusica", 1);
            PlayerPrefs.SetFloat("volumeMaximoFx", 1);
        }

        //CARREGA AS CONFIGURAÇÕES DE AUDIO DO APARELHO
        volumeMaximoMusica = PlayerPrefs.GetFloat("volumeMaximoMusica");
        volumeMaximoFx = PlayerPrefs.GetFloat("volumeMaximoFx");

        trocarMusica(musicaTitulo, "titulo", true);
    }

    public void trocarMusica(AudioClip clip, string nomeCena, bool mudarCena)
    {
        novaMusica = clip;
        novaCena = nomeCena;
        trocarCena = mudarCena;

        StartCoroutine("changeMusic");
    }

    IEnumerator changeMusic()
    {
        for(float volume = volumeMaximoMusica; volume >= 0; volume -= 0.1f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            sMusic.volume = volume;
        }
        sMusic.volume = 0;

        sMusic.clip = novaMusica;
        sMusic.Play();

        for (float volume = 0; volume < volumeMaximoMusica; volume += 0.1f)
        {
            yield return new WaitForSecondsRealtime(0.1f);
            sMusic.volume = volume;
        }
        sMusic.volume = volumeMaximoMusica;

        if(trocarCena == true)
        {
            print(novaCena);
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(novaCena);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
        }
    }

    public void tocarFx(AudioClip fx, float volume)
    {
        float tempVolume = volume;
        if(volume > volumeMaximoFx)
        {
            tempVolume = volumeMaximoFx;
        }
        sFx.volume = tempVolume;
        sFx.PlayOneShot(fx);
    }
}
