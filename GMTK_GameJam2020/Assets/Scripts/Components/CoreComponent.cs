using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;
using System;
using TMPro;
using Managers;

public class CoreComponent : MonoBehaviour
{
    public CoreBase CoreElement { get; set; }
    private float timer = 0f;
    private float elapsedTime = 0f;
    private bool startInfecting = false;
    private float quarantineTime = 0f;
    private float elapsedQTime = 0f;

    Func<bool> hasBreakQuarantineMutation;
    Func<CoreBase, CoreComponent> getCoreComponent;
    Action<string, Color> log;

    GameManager manager;
    InfectionState prevState;

    public void SetHighlight(bool highlight)
    {
        transform.GetChild(0).gameObject.SetActive(highlight);
    }

    public void SetMenuHighlight(bool highlight)
    {
        GetComponent<Image>().enabled = highlight;
    }

    void Update()
    {
        if (CoreElement.InfectionState == InfectionState.Infected && startInfecting && manager.State == GameState.Playing && !manager.VirusManager.IsDelayed())
        {
            //only infect siblings and children
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timer)
            {
                if (CoreElement.ContainingFolder.Files.Count > 0)
                {
                    //loop through and infect through core component in not infected
                    //virus can break quarantine if upgraded
                    foreach (var file in CoreElement.ContainingFolder.Files)
                    {
                        if (file.InfectionState != InfectionState.Infected)
                        {
                            if (file.InfectionState == InfectionState.Clean)
                            {
                                getCoreComponent(file).Infect(0f, manager);
                                log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                return;
                            }
                            else if (file.InfectionState == InfectionState.Quarantined && hasBreakQuarantineMutation())
                            {
                                getCoreComponent(file).Infect(0f, manager);
                                log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                return;
                            }
                        }
                    }
                }

                if (CoreElement.ContainingFolder.Folders.Count > 0)
                {
                    //see above
                    foreach (var folder in CoreElement.ContainingFolder.Folders)
                    {
                        if (folder.InfectionState != InfectionState.Infected)
                        {
                            if (folder.InfectionState == InfectionState.Clean)
                            {
                                getCoreComponent(folder).Infect(0f, manager);
                                log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                return;
                            }
                            else if (folder.InfectionState == InfectionState.Quarantined && hasBreakQuarantineMutation())
                            {
                                getCoreComponent(folder).Infect(0f, manager);
                                log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                return;
                            }
                        }
                    }
                }

                if (CoreElement is GameFolder)
                {
                    var folder = CoreElement as GameFolder;

                    if (folder.Files.Count > 0)
                    {
                        foreach (var file in folder.Files)
                        {
                            if (file.InfectionState != InfectionState.Infected)
                            {
                                if (file.InfectionState == InfectionState.Clean)
                                {
                                    getCoreComponent(file).Infect(0f, manager);
                                    log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                    return;
                                }
                                else if (file.InfectionState == InfectionState.Quarantined && hasBreakQuarantineMutation())
                                {
                                    getCoreComponent(file).Infect(0f, manager);
                                    log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                    return;
                                }
                            }
                        }
                    }

                    if (folder.Folders.Count > 0)
                    {
                        foreach (var innerFolder in folder.Folders)
                        {
                            if (innerFolder.InfectionState != InfectionState.Infected)
                            {
                                if (innerFolder.InfectionState == InfectionState.Clean)
                                {
                                    getCoreComponent(folder).Infect(0f, manager);
                                    log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                    return;
                                }
                                else if (innerFolder.InfectionState == InfectionState.Quarantined && hasBreakQuarantineMutation())
                                {
                                    getCoreComponent(folder).Infect(0f, manager);
                                    log($"Infected {CoreElement.Name}", new Color(0.75f, 0f, 1f));
                                    return;
                                }
                            }
                        }
                    }
                }

                elapsedTime = 0f;
            }
        }
        else if (CoreElement.InfectionState == InfectionState.Quarantined && manager.State == GameState.Playing)
        {
            elapsedQTime += Time.deltaTime;

            if (elapsedQTime >= quarantineTime)
            {
                CoreElement.InfectionState = prevState;
                if (CoreElement.InfectionState == InfectionState.Clean)
                {
                    GetComponent<TextMeshProUGUI>().color = Color.white;
                    manager.ComputerManager.AddToEventLog($"Quarantined on {CoreElement.Name} expired ", Color.white);
                }
                else if (CoreElement.InfectionState == InfectionState.Infected)
                {
                    GetComponent<TextMeshProUGUI>().color = new Color(0.75f, 0f, 1f);
                    manager.ComputerManager.AddToEventLog($"Quarantined on {CoreElement.Name} expired ", new Color(0.75f, 0f, 1f));
                }

                elapsedQTime = 0f;
            }
        }
    }

    public void Infect(float time, GameManager gmanager) //time is usually 0, unless virus has upgraded
    {
        manager = gmanager;
        hasBreakQuarantineMutation = manager.VirusManager.HasBreakQuarantineMutation;
        getCoreComponent = manager.UIManager.GetCoreComponent;
        log = manager.ComputerManager.AddToEventLog;
        CoreElement.InfectionState = InfectionState.Infected;
        GetComponent<TextMeshProUGUI>().color = new Color(0.75f, 0f, 1f); //purple
        if (time > 0f)
        {
            startInfecting = true;
            timer = time;
        }
    }

    public void Quarantine(float time, GameManager gManager)
    {
        manager = gManager;
        if (CoreElement.InfectionState != InfectionState.Quarantined)
        {
            prevState = CoreElement.InfectionState;
        }
        CoreElement.InfectionState = InfectionState.Quarantined;
        quarantineTime = time;
        elapsedQTime = 0f;
        GetComponent<TextMeshProUGUI>().color = Color.cyan;
    }
}
