using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class UserManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }
        public int MinTimeBetweenActions;
        public int MaxTimeBetweenActions;
        private float elapsedTime = 0f;
        private float currentTimeBetweenActions = 10f;

        public void Restart()
        {
            elapsedTime = 0f;
        }

        void Start()
        {
            if (MaxTimeBetweenActions < MinTimeBetweenActions)
            {
                throw new System.Exception("Max time between actions should be more than or equal to Min time between actions");
            }
        }

        void Update()
        {
            //every 10 - 15s, do something weird

            elapsedTime += Time.deltaTime;

            if (elapsedTime >= currentTimeBetweenActions)
            {
                //do something
                if (GameManager.VirusManager.Mutations.HasFlag(VirusMutations.BreakQuarantine)
                 || GameManager.VirusManager.Mutations.HasFlag(VirusMutations.SpreadEasier)
                 || GameManager.VirusManager.Mutations.HasFlag(VirusMutations.SpreadQuicker))
                {
                    ChooseBetweenThreeActions();
                }
                else
                {
                    ChooseBetweenTwoActions();
                }

                currentTimeBetweenActions = Helpers.RandomHelper.RandomInt(MinTimeBetweenActions, MaxTimeBetweenActions);
                elapsedTime = 0f;
            }
        }

        void ChooseBetweenTwoActions()
        {
            var thingToDo = Helpers.RandomHelper.RandomInt(1, 3); //1 or 2 

            switch (thingToDo)
            {
                case 1:
                    Reboot();
                    break;

                case 2:
                    Restore();
                    break;

                default:
                    break;
            }
        }

        void ChooseBetweenThreeActions()
        {
            var thingToDo = Helpers.RandomHelper.RandomInt(1, 4); //1, 2 or 3

            switch (thingToDo)
            {
                case 1:
                    Reboot();
                    break;

                case 2:
                    Restore();
                    break;

                case 3:
                    OpenInfectedFile();
                    break;

                default:
                    break;
            }
        }

        void Reboot()
        {
            //reboot - 50/50 either you or the virus are delayed
            var helpChance = Helpers.RandomHelper.RandomInt(0, 2); //0 or 1

            if (helpChance == 0)
            {
                //help player - delay virus
                GameManager.VirusManager.Delay();
                GameManager.ComputerManager.AddToEventLog("User has rebooted the PC - Virus spread delayed!", Color.green);
            }
            else if (helpChance == 1)
            {
                //help virus - delay user
                GameManager.PlayerManager.Delay();
                GameManager.ComputerManager.AddToEventLog("User has rebooted the PC - Anti virus delayed!", Color.red);
            }
        }

        void Restore()
        {
            //restore - virus is weaker - remove mutation
            if (GameManager.VirusManager.Mutations.HasFlag(VirusMutations.BreakQuarantine))
            {
                GameManager.VirusManager.Mutations &= ~VirusMutations.BreakQuarantine;
                GameManager.UIManager.BreakQuarantine.SetActive(false);
            }
            else if (GameManager.VirusManager.Mutations.HasFlag(VirusMutations.SpreadEasier))
            {
                GameManager.VirusManager.Mutations &= ~VirusMutations.SpreadEasier;
                GameManager.UIManager.SpreadEasier.SetActive(false);
            }
            else if (GameManager.VirusManager.Mutations.HasFlag(VirusMutations.SpreadQuicker))
            {
                GameManager.VirusManager.Mutations &= ~VirusMutations.SpreadQuicker;
                GameManager.VirusManager.Timer += 1f;
                GameManager.UIManager.SpreadQuicker.SetActive(false);
            }

            GameManager.ComputerManager.AddToEventLog("User has restored PC - Virus has lost a mutation!", Color.green);
        }

        void OpenInfectedFile()
        {
            //virus spreads faster
            GameManager.VirusManager.Timer -= 1f;
            GameManager.ComputerManager.AddToEventLog("User has opened an infected file - Virus now spreads faster!", Color.red);
        }
    }
}
