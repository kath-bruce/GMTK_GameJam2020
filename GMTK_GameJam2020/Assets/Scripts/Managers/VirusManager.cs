using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;
using TMPro;

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
        public int ChanceToMutate;
        public int ChanceToInfectParent;
        private CoreComponent currentInfection;

        private float timeElapsed = 0f;

        public void SetVirusFile(GameFile virus)
        {
            VirusFile = virus;
            currentInfection = GameManager.UIManager.GetCoreComponent(VirusFile);
            currentInfection.GetComponent<TextMeshProUGUI>().color = new Color(0.75f, 0f, 1f);
        }

        public bool HasBreakQuarantineMutation()
        {
            return (Mutations.HasFlag(VirusMutations.BreakQuarantine));
        }

        void Infect()
        {
            int parentInfectChance = Helpers.RandomHelper.RandomInt(0, 100);

            if (parentInfectChance > ChanceToInfectParent)
            {
                if (currentInfection.CoreElement.ContainingFolder.InfectionState == InfectionState.Quarantined)
                {
                    if (Mutations.HasFlag(VirusMutations.BreakQuarantine))
                    {
                        currentInfection = GameManager.UIManager.GetCoreComponent(currentInfection.CoreElement.ContainingFolder);

                        if (currentInfection.CoreElement.Name == "Root" && currentInfection.CoreElement.ContainingFolder == null)
                        {
                            GameManager.State = GameState.Lose;
                            return;
                        }
                        else
                        {
                            if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                            {
                                currentInfection.Infect(20f, GameManager);
                            }
                            else
                            {
                                currentInfection.Infect(0f, GameManager);
                            }
                            GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                        }
                    }
                    else
                    {
                        //infect children

                        bool infectedChild = false;

                        if (currentInfection.CoreElement is GameFolder)
                        {
                            var currentFolder = currentInfection.CoreElement as GameFolder;
                            foreach (var file in currentFolder.Files)
                            {
                                if (file.InfectionState != InfectionState.Infected)
                                {
                                    if (file.InfectionState == InfectionState.Quarantined && Mutations.HasFlag(VirusMutations.BreakQuarantine))
                                    {
                                        currentInfection = GameManager.UIManager.GetCoreComponent(file);
                                        if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                                        {
                                            currentInfection.Infect(20f, GameManager);
                                        }
                                        else
                                        {
                                            currentInfection.Infect(0f, GameManager);
                                        }
                                        infectedChild = true;
                                        GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                                        break;
                                    }
                                    else if (file.InfectionState == InfectionState.Clean)
                                    {
                                        currentInfection = GameManager.UIManager.GetCoreComponent(file);
                                        if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                                        {
                                            currentInfection.Infect(20f, GameManager);
                                        }
                                        else
                                        {
                                            currentInfection.Infect(0f, GameManager);
                                        }
                                        infectedChild = true;
                                        GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                                        break;
                                    }
                                }
                            }

                            if (!infectedChild)
                            {
                                foreach (var folder in currentFolder.Folders)
                                {
                                    if (folder.InfectionState != InfectionState.Infected)
                                    {
                                        if (folder.InfectionState == InfectionState.Quarantined && Mutations.HasFlag(VirusMutations.BreakQuarantine))
                                        {
                                            currentInfection = GameManager.UIManager.GetCoreComponent(folder);
                                            if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                                            {
                                                currentInfection.Infect(20f, GameManager);
                                            }
                                            else
                                            {
                                                currentInfection.Infect(0f, GameManager);
                                            }
                                            infectedChild = true;
                                            GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                                            break;
                                        }
                                        else if (folder.InfectionState == InfectionState.Clean)
                                        {
                                            currentInfection = GameManager.UIManager.GetCoreComponent(folder);
                                            if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                                            {
                                                currentInfection.Infect(20f, GameManager);
                                            }
                                            else
                                            {
                                                currentInfection.Infect(0f, GameManager);
                                            }
                                            infectedChild = true;
                                            GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                                            break;
                                        }

                                    }
                                }
                            }
                        }

                        InfectSibling();
                    }
                }
                else
                {
                    currentInfection = GameManager.UIManager.GetCoreComponent(currentInfection.CoreElement.ContainingFolder);

                    if (currentInfection.CoreElement.Name == "Root" && currentInfection.CoreElement.ContainingFolder == null)
                    {
                        GameManager.State = GameState.Lose;
                    }
                    else
                    {
                        if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                        {
                            currentInfection.Infect(20f, GameManager);
                        }
                        else
                        {
                            currentInfection.Infect(0f, GameManager);
                        }
                        GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                    }
                }
            }
            else
            {
                //
                InfectSibling();
            }
        }

        void InfectSibling()
        {
            //var parent = GameManager.UIManager.GetCoreComponent(currentInfection.CoreElement.ContainingFolder);

            //infect files first
            //then go into sibling folder and infect deepest file

            bool infectedSibling = false;

            foreach (var sibling in currentInfection.CoreElement.ContainingFolder.Files)
            {
                if (sibling.InfectionState != InfectionState.Infected)
                {
                    if (sibling.InfectionState == InfectionState.Clean)
                    {
                        currentInfection = GameManager.UIManager.GetCoreComponent(sibling);

                        if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                        {
                            currentInfection.Infect(20f, GameManager);
                        }
                        else
                        {
                            currentInfection.Infect(0f, GameManager);
                        }

                        GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                        infectedSibling = true;
                        break;
                    }
                    else if (sibling.InfectionState == InfectionState.Quarantined
                    && Mutations.HasFlag(VirusMutations.BreakQuarantine))
                    {
                        currentInfection = GameManager.UIManager.GetCoreComponent(sibling);

                        if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                        {
                            currentInfection.Infect(20f, GameManager);
                        }
                        else
                        {
                            currentInfection.Infect(0f, GameManager);
                        }
                        GameManager.ComputerManager.AddToEventLog("Infected", Color.red);

                        infectedSibling = true;
                        break;
                    }
                }
            }

            if (!infectedSibling && currentInfection.CoreElement.ContainingFolder.Folders.Count > 0)
            {
                foreach (var sibling in currentInfection.CoreElement.ContainingFolder.Folders)
                {
                    if (sibling.InfectionState != InfectionState.Infected)
                    {
                        if (sibling.InfectionState == InfectionState.Clean)
                        {
                            currentInfection = GameManager.UIManager.GetCoreComponent(sibling);

                            if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                            {
                                currentInfection.Infect(20f, GameManager);
                            }
                            else
                            {
                                currentInfection.Infect(0f, GameManager);
                            }
                            GameManager.ComputerManager.AddToEventLog("Infected", Color.red);

                            break;
                        }
                        else if (sibling.InfectionState == InfectionState.Quarantined
                        && Mutations.HasFlag(VirusMutations.BreakQuarantine))
                        {
                            currentInfection = GameManager.UIManager.GetCoreComponent(sibling);

                            if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                            {
                                currentInfection.Infect(20f, GameManager);
                            }
                            else
                            {
                                currentInfection.Infect(0f, GameManager);
                            }
                            GameManager.ComputerManager.AddToEventLog("Infected", Color.red);
                            break;
                        }
                    }
                }

            }
        }

        void Update()
        {
            if (GameManager.State == GameState.Playing)
            {

                timeElapsed += Time.deltaTime;

                if (timeElapsed >= Timer)
                {
                    //chance to infect other sibling file
                    //just infect corecomponents
                    //but 
                    //or random mutation


                    //if thing is root folder
                    //lose

                    if (Mutations.HasFlag(VirusMutations.BreakQuarantine | VirusMutations.SpreadEasier | VirusMutations.SpreadQuicker))
                    {
                        //just infect - add to event log
                        Infect();
                    }
                    else
                    {
                        int mutationChance = Helpers.RandomHelper.RandomInt(0, 100);

                        if (mutationChance > ChanceToMutate)
                        {
                            //mutate
                            if (!Mutations.HasFlag(VirusMutations.SpreadQuicker))
                            {
                                Mutations |= VirusMutations.SpreadQuicker;
                                Timer -= 1f;
                            }
                            else if (!Mutations.HasFlag(VirusMutations.SpreadEasier))
                            {
                                Mutations |= VirusMutations.SpreadEasier;
                            }
                            else if (!Mutations.HasFlag(VirusMutations.BreakQuarantine))
                            {
                                Mutations |= VirusMutations.BreakQuarantine;
                            }
                            //update mutations display
                            //add to event log
                            GameManager.ComputerManager.AddToEventLog("Mutated", Color.red);
                        }
                        else
                        {
                            Infect(); //add to event log
                        }
                    }

                    timeElapsed = 0f;
                }

                GameManager.UIManager.SetVirusCountdown(Timer - timeElapsed);
            }
        }

    }
}
