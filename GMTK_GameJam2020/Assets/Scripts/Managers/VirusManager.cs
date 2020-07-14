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
        private float initTimer;
        public GameFile VirusFile { get; set; }
        public VirusMutations Mutations { get; set; }
        public int ChanceToMutate;
        public int ChanceToInfectParent;
        public int ChanceToInfectRoot;
        private CoreComponent currentInfection;

        private float timeElapsed = 0f;
        private float delay = 5f;
        private float delayElapsed = 10f;

        void Start()
        {
            initTimer = Timer;
        }

        public void Restart()
        {
            //
            if (Mutations.HasFlag(VirusMutations.BreakQuarantine))
            {
                Mutations &= ~VirusMutations.BreakQuarantine;
            }
            if (Mutations.HasFlag(VirusMutations.SpreadQuicker))
            {
                Mutations &= ~VirusMutations.SpreadQuicker;
            }
            if (Mutations.HasFlag(VirusMutations.SpreadEasier))
            {
                Mutations &= ~VirusMutations.SpreadEasier;
            }
            Mutations = VirusMutations.None;
            delayElapsed = 10f;
            currentInfection = null;
            Timer = initTimer;
        }

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

            if (parentInfectChance > ChanceToInfectParent || currentInfection.CoreElement.ContainingFolder.InfectionState == InfectionState.Infected)
            {
                if (currentInfection.CoreElement.ContainingFolder.InfectionState == InfectionState.Quarantined)
                {
                    if (Mutations.HasFlag(VirusMutations.BreakQuarantine))
                    {
                        var temp = currentInfection;
                        currentInfection = GameManager.UIManager.GetCoreComponent(currentInfection.CoreElement.ContainingFolder);

                        if (currentInfection.CoreElement.Name == "Root" && currentInfection.CoreElement.ContainingFolder == null)
                        {
                            if (parentInfectChance > ChanceToInfectRoot)
                            {
                                currentInfection.CoreElement.InfectionState = InfectionState.Infected;
                                GameManager.State = GameState.Lose;
                                return;
                            }
                            else
                            {
                                currentInfection = temp;

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
                                                GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                                return;
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
                                                GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                                return;
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
                                                    GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                                    return;
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
                                                    GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!infectedChild)
                                {
                                    InfectSibling();
                                }
                            }
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
                            GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
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
                                        GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                        return;
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
                                        GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                        return;
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
                                            GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                            return;
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
                                            GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                        if (!infectedChild)
                        {
                            InfectSibling();
                        }
                    }
                }
                else
                {
                    var temp = currentInfection;

                    currentInfection = GameManager.UIManager.GetCoreComponent(currentInfection.CoreElement.ContainingFolder);

                    if (currentInfection.CoreElement.Name == "Root" && currentInfection.CoreElement.ContainingFolder == null)
                    {
                        if (parentInfectChance > ChanceToInfectRoot)
                        {
                            currentInfection.CoreElement.InfectionState = InfectionState.Infected;
                            GameManager.State = GameState.Lose;
                            return;
                        }
                        else
                        {
                            currentInfection = temp;

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
                                            GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                            return;
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
                                            GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                            return;
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
                                                GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                                return;
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
                                                GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                                return;
                                            }
                                        }
                                    }
                                }
                            }

                            if (!infectedChild)
                            {
                                InfectSibling();
                            }
                        }
                    }
                    else if (currentInfection.CoreElement.ContainingFolder.InfectionState == InfectionState.Infected)
                    {
                        if (Mutations.HasFlag(VirusMutations.SpreadEasier))
                        {
                            currentInfection.Infect(20f, GameManager);
                        }
                        else
                        {
                            currentInfection.Infect(0f, GameManager);
                        }
                        GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                        return;
                    }
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
                                GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                return;
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
                                GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                return;
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
                                    GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                    return;
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
                                    GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                    return;
                                }
                            }
                        }
                    }
                }

                if (!infectedChild)
                {
                    InfectSibling();
                }
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

                        GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                        infectedSibling = true;
                        return;
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
                        GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));

                        infectedSibling = true;
                        return;
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
                            GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));

                            return;
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
                            GameManager.ComputerManager.AddToEventLog($"Infected {currentInfection.CoreElement.Name}", new Color(0.75f, 0f, 1f));
                            return;
                        }
                    }
                }

            }
        }

        public void Delay()
        {
            delayElapsed = 0f;
        }

        public bool IsDelayed()
        {
            return (delay > delayElapsed);
        }

        void Update()
        {
            if (Timer < 0f)
            {
                Timer = 0f;
            }

            if (GameManager.State == GameState.Playing)
            {
                if (delay > delayElapsed) //if not finished delay
                {
                    delayElapsed += Time.deltaTime;
                    return;
                }

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
                                GameManager.UIManager.SpreadQuicker.SetActive(true);
                            }
                            else if (!Mutations.HasFlag(VirusMutations.SpreadEasier))
                            {
                                Mutations |= VirusMutations.SpreadEasier;
                                GameManager.UIManager.SpreadEasier.SetActive(true);
                            }
                            else if (!Mutations.HasFlag(VirusMutations.BreakQuarantine))
                            {
                                Mutations |= VirusMutations.BreakQuarantine;
                                GameManager.UIManager.BreakQuarantine.SetActive(true);
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
