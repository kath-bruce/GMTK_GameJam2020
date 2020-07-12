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
                //find random file to infect
                //where set colours
                Debug.Log($"infect file or folder");
                
                //set colour on thing
                //if thing is root folder
                //lose
                timeElapsed = 0f;
            }

            GameManager.UIManager.SetVirusCountdown(Timer - timeElapsed);
        }

    }
}
