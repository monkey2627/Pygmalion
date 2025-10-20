using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scene
{
    public class PSceneManager : MonoBehaviour
    {
        public static PSceneManager Instance;
        Dictionary<String,Scene> scenesDictionary = new Dictionary<String, Scene>();
        public Scene[] scenes;
        public Scene _currentScene;
        private void Awake()
        {
            Instance = this;
            foreach (var t in scenes)
            {
                scenesDictionary.Add(t.name, t);
            }
        }

        public void LoadScene(String sceneName)
        {
            _currentScene = scenesDictionary[sceneName];
            _currentScene.Load();
        }
    }
}
