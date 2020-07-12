using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Managers
{
    public class VirusManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }
        public float Timer;
        public GameFile VirusFile { get; set; }

        private float timeElapsed = 0f;

        public void SetVirusFile(GameFile virus)
        {
            VirusFile = virus;
        }

        void Update()
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= Timer)
            {
                //infected file
                Debug.Log($"infect file");
                
                timeElapsed = 0f;
            }

            GameManager.UIManager.SetVirusCountdown(Timer - timeElapsed);
        }

    }
}
