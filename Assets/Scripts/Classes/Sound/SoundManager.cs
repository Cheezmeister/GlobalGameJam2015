﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour {
  public bool isPaused = false;

  public List<GameObject> audioObjectsAvailable = new List<GameObject>();
  public List<GameObject> audioObjectsInUse = new List<GameObject>();

  public Dictionary<AudioType, string> soundDictionary = new Dictionary<AudioType, string>();

  public bool disableSoundCalls = false;
  public GameObject audioObjectForMusic = null;
  public float audioObjectTimeAtWhenPauseToggled = 0f;
  public GameObject originalAudioObject = null;

  //BEGINNING OF SINGLETON CODE CONFIGURATION5
  private static volatile SoundManager _instance;
  private static object _lock = new object();

  //Stops the lock being created ahead of time if it's not necessary
  static SoundManager() {
  }

  public static SoundManager Instance {
    get {
      if(_instance == null) {
        lock(_lock) {
          if (_instance == null) {
            GameObject levelManagerGameObject = new GameObject("SoundManagerGameObject");
            _instance = (levelManagerGameObject.AddComponent<SoundManager>()).GetComponent<SoundManager>();
          }
        }
      }
      return _instance;
    }
  }

  private SoundManager() {
  }

  // Use this for initialization
  void Awake() {
    //There's a lot of magic happening right here. Basically, the THIS keyword is a reference to
    //the script, which is assumedly attached to some GameObject. This in turn allows the instance
    //to be assigned when a game object is given this script in the scene view.
    //This also allows the pre-configured lazy instantiation to occur when the script is referenced from
    //another call to it, so that you don't need to worry if it exists or not.
    _instance = this;
    ConfigureSoundDictionary();
  }
  //END OF SINGLETON CODE CONFIGURATION

  // Use this for initialization
  void Start() {
    // TweenExecutor.TweenObjectColor(colorGUIScoreRating, Color.red, Color.green, 0, 10, UITweener.Method.Linear, null);
  }

  // Update is called once per frame
  void Update () {
    if(!isPaused) {
      List<GameObject> audioObjectsThatFinishedPlaying = new List<GameObject>();
      foreach(GameObject gameObj in audioObjectsInUse) {
          if(gameObj.GetComponent<AudioSourceManager>().hasFinished) {
            audioObjectsThatFinishedPlaying.Add(gameObj);
          }
      }
      foreach(GameObject gameObj in audioObjectsThatFinishedPlaying) {
        audioObjectsInUse.Remove(gameObj);
        if(!audioObjectsAvailable.Contains(gameObj)) {
          audioObjectsAvailable.Add(gameObj);
        }
      }
    }
  }

  public void PerformAudioObjectRecyleCheck() {
  }

  public void TogglePause(bool newPauseState, AudioType pauseMusicToPlay = AudioType.None) {
    isPaused = newPauseState;

    // iterate over each object for sound and toggle to new is paused state
    if(isPaused) {
      foreach(GameObject gameObj in audioObjectsInUse) {
        gameObj.GetComponent<AudioSourceManager>().Pause();
      }
      originalAudioObject = audioObjectForMusic;
      if(audioObjectForMusic != null) {
        audioObjectTimeAtWhenPauseToggled = audioObjectForMusic.GetComponent<AudioSource>().time;
      }
      PlayMusic(pauseMusicToPlay);
    }
    else {
      foreach(GameObject gameObj in audioObjectsInUse) {
        gameObj.GetComponent<AudioSourceManager>().Play();
      }
      audioObjectForMusic.GetComponent<AudioSourceManager>().Stop();
      audioObjectsInUse.Remove(audioObjectForMusic);
      audioObjectsAvailable.Add(audioObjectForMusic);

      audioObjectForMusic = originalAudioObject;
      originalAudioObject = null;
      if(audioObjectForMusic != null) {
        audioObjectForMusic.GetComponent<AudioSource>().Play();
        audioObjectForMusic.GetComponent<AudioSource>().time = audioObjectTimeAtWhenPauseToggled;
      }
      audioObjectTimeAtWhenPauseToggled = 0f;
    }
  }

  public void ConfigureSoundDictionary() {
    soundDictionary[AudioType.None] = "path/to/file/from/root level";

    soundDictionary[AudioType.Jump1] = "Sound/jump1";
    soundDictionary[AudioType.Jump2] = "Sound/jump2";
    soundDictionary[AudioType.Correct1] = "Sound/success1";
    soundDictionary[AudioType.Correct2] = "Sound/success2";
    soundDictionary[AudioType.RobotDemo] = "Sound/robot_demo";
    soundDictionary[AudioType.Music] = "Sound/RobotMusic/mx_full";
    soundDictionary[AudioType.Melody] = "Sound/RobotMusic/mx_melody";

  }

  public GameObject Play(AudioType audioToPlay, bool loop = false, float delay = 0) {
    GameObject audioGameObject = null;
    // AudioSource audioSourceReference = null;
    AudioSourceManager audioSourceManager = null;

    if(!disableSoundCalls) {
      if(audioObjectsAvailable.Count == 0) {
        audioGameObject = new GameObject("SoundObject");
        audioGameObject.transform.parent = this.gameObject.transform;
        audioGameObject.AddComponent<AudioSource>();
        audioGameObject.AddComponent<AudioSourceManager>();
        audioObjectsInUse.Add(audioGameObject);
      }
      else {
        audioGameObject = audioObjectsAvailable[0];
        audioObjectsAvailable.Remove(audioGameObject);
        audioObjectsInUse.Add(audioGameObject);
      }

      // audioSourceReference  = audioGameObject.GetComponent<AudioSource>();
      // audioSourceReference.clip = (AudioClip)Resources.Load(soundDictionary[audioToPlay], typeof(AudioClip));

      // audioSourceReference.loop = loop;
      // audioSourceReference.PlayDelayed(delay);

      audioSourceManager = audioGameObject.GetComponent<AudioSourceManager>();
      audioSourceManager.audioSourceReference = audioGameObject.GetComponent<AudioSource>();
      audioSourceManager.SetClip((AudioClip)Resources.Load(soundDictionary[audioToPlay], typeof(AudioClip)));
      audioSourceManager.SetLoop(loop);
      audioSourceManager.PlayDelayed(delay);
    }

    return audioGameObject;
  }

  public GameObject PlayMusic(AudioType audioToPlay, bool loop = true, float delay = 0) {
    audioObjectForMusic = Play(audioToPlay, loop, delay);
    return audioObjectForMusic;
  }
}
