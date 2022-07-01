using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    // Start is called before the first frame update
     private AudioSource _audioSource;
    [SerializeField] AudioClip mainMenuClip;
    [SerializeField] AudioClip gameClip;

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
        _audioSource = GetComponent<AudioSource>();
        
        PlayMusic(SceneManager.GetActiveScene());
        SceneManager.sceneLoaded += PlayMusic;
    }
    public void PlayMusic(Scene scene, LoadSceneMode sceneLoad = LoadSceneMode.Single)
    {
        if (scene.buildIndex == 0 && _audioSource.clip != mainMenuClip)
        {
            _audioSource.Stop();
            _audioSource.clip = mainMenuClip;
            _audioSource.Play();
        } else if (scene.buildIndex != 0 && _audioSource.clip != gameClip) //...and buildIndex is not 0
        {
            _audioSource.Stop();
            _audioSource.clip = gameClip;
            _audioSource.Play();
        }
    }
    public IEnumerator FadeOutMusic(AudioSource src, float initialVolume, float fadeTime)
    {
        float currentTime = 0;

        while (currentTime < fadeTime)
        {
            currentTime += Time.deltaTime;
            src.volume = Mathf.Lerp(initialVolume, 0, currentTime / fadeTime);
            yield return new WaitForSeconds(1f);
        }
        yield break;
    }
    public void StopMusic()
    {
        _audioSource.Stop();
    }
}
