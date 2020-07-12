using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

namespace Managers
{
    [Flags]
    public enum VirusMutations { None = 0, BreakQuarantine = 1, SpreadQuicker = 2, SpreadEasier = 4 }

    public class VirusManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }
        public float Timer;
        public GameFile VirusFile { get; set; }
        public VirusMutations Mutations { get; set; }

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
                //chance to infect other sibling file
                //just infect corecomponents
                //but 
                //or random mutation


                //set colour on thing
                //if thing is root folder
                //lose
                timeElapsed = 0f;
            }

            GameManager.UIManager.SetVirusCountdown(Timer - timeElapsed);
        }

    }
}
