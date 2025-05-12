using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{

    public AudioClip startingClip;
    public static AudioManager instance;
    public AudioPlayer currentAudioSource;
    public AudioPlayer source1, source2;
    public Slider musicSlider, sfxSlider;
    public TextMeshProUGUI musicText, soundText;

    public const string musicKey = "music", sfxKey = "sfxKey";
    public static float musicVolume = 0.5f, sfxVolume = 0.5f;
    public static List<AudioPlayer> musicSources = new List<AudioPlayer>();
    public static List<AudioPlayer> sfxSources = new List<AudioPlayer>();

    #if !UNITY_SWITCH
    private void Awake() {
        Setup();
    }
    #endif
    
    #if UNITY_SWITCH
    private void Start()
    {
        Setup();
    }
    #endif

    private void Setup()
    {
        if (instance == null) {
            instance = this;
            transform.SetParent(null);
            DontDestroyOnLoad(gameObject);
        } else {
            if (this != instance) {
                Destroy(gameObject);
            }
        }
        
        Initialize();
    }

    public static bool initialized = false;
    public void Initialize() {
        source1.GetSource();
        source2.GetSource();
        PlayTrack(startingClip);
        
#if !UNITY_SWITCH
        if (!PlayerPrefs.HasKey(musicKey)) {
            PlayerPrefs.SetFloat(musicKey, 0.5f);
        } else {
            musicVolume = PlayerPrefs.GetFloat(musicKey);
        }
        if (!PlayerPrefs.HasKey(sfxKey)) {
            PlayerPrefs.SetFloat(sfxKey, 0.5f);
        } else {
            sfxVolume = PlayerPrefs.GetFloat(sfxKey);
        }
#else
        musicVolume = GameMasterManager.gameSaveFile.musicVol;
        sfxVolume = GameMasterManager.gameSaveFile.sfxVol;
#endif
        
        musicSlider.SetValueWithoutNotify(musicVolume);
        sfxSlider.SetValueWithoutNotify(sfxVolume);
        SetVolumes();
    }

    public void SetMusic(float value) {
#if !UNITY_SWITCH
        PlayerPrefs.SetFloat(musicKey, value);
        musicVolume = value;
#else
        GameMasterManager.gameSaveFile.musicVol = value;
        GameMasterManager.instance.SaveGameFile();
#endif
        SetVolumes();
    }

    public void SetSound(float value) {
#if !UNITY_SWITCH
        PlayerPrefs.SetFloat(sfxKey, value);
        sfxVolume = value;
#else
        GameMasterManager.gameSaveFile.sfxVol = value;
        GameMasterManager.instance.SaveGameFile();
#endif
        SetVolumes();
    }


    public void SetVolumes() {
        #if !UNITY_SWITCH
        musicVolume = PlayerPrefs.GetFloat(musicKey);
        sfxVolume = PlayerPrefs.GetFloat(sfxKey);
        #else
        musicVolume = GameMasterManager.gameSaveFile.musicVol;
        sfxVolume = GameMasterManager.gameSaveFile.sfxVol;
        #endif

        musicText.text = ((int)(musicVolume * 100)).ToString() + "%";
        soundText.text = ((int)(sfxVolume * 100)).ToString() + "%";

        foreach (AudioPlayer player in musicSources) {
            player.SetVolume(musicVolume);
        }

        foreach (AudioPlayer player in sfxSources) {
            player.SetVolume(sfxVolume);
        }

    }

    public static void PlayTrack(AudioClip c) {
        if (c == null) return;
        instance.PTrack(c);
    }


    private void PTrack(AudioClip c) {


        if (currentAudioSource == null) {
            currentAudioSource = source1;
            currentAudioSource.source.clip = c;
            currentAudioSource.SetVolume(musicVolume);
            currentAudioSource.source.Play();
            return;
        } else if (currentAudioSource.source.clip == c) {
            return;
        } else {
            StartCoroutine(SwitchTracks(c));
        }



    }


   
    public IEnumerator SwitchTracks(AudioClip clip) {

        AudioPlayer oldSource = null;


        if (currentAudioSource == source1) {

            oldSource = source1;
            currentAudioSource = source2;
        } else {
            oldSource = source2;
            currentAudioSource = source1;
        }

        float val = 0;
        currentAudioSource.source.clip = clip;
        currentAudioSource.SetVolume(0);
        oldSource.SetVolume(musicVolume);
        currentAudioSource.source.Play();
        

        while (val < musicVolume) {
            val += Time.deltaTime * musicVolume;
            currentAudioSource.SetVolume(val);
            oldSource.SetVolume(musicVolume - val);
            yield return null;
        }

        currentAudioSource.SetVolume(musicVolume);
        oldSource.SetVolume(0);
        oldSource.source.Stop();
        oldSource.source.clip = null;

        
    }


}
