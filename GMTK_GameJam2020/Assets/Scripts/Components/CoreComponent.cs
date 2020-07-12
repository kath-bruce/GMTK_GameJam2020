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

    Func<bool> hasBreakQuarantineMutation;
    Func<CoreBase, CoreComponent> getCoreComponent;
    Action<string, Color> log;

    GameManager manager;

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
        if (CoreElement.InfectionState == InfectionState.Infected && startInfecting && manager.State == GameState.Playing)
        {
            //only infect siblings
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
                            }
                            else if (file.InfectionState == InfectionState.Quarantined && hasBreakQuarantineMutation())
                            {
                                getCoreComponent(file).Infect(0f, manager);
                            }
                            log("Infected", Color.red);
                        }
                    }
                }
                else if (CoreElement.ContainingFolder.Folders.Count > 0)
                {
                    //see above
                    foreach (var folder in CoreElement.ContainingFolder.Folders)
                    {
                        if (folder.InfectionState != InfectionState.Infected)
                        {
                            if (folder.InfectionState == InfectionState.Clean)
                            {
                                getCoreComponent(folder).Infect(0f, manager);
                            }
                            else if (folder.InfectionState == InfectionState.Quarantined && hasBreakQuarantineMutation())
                            {
                                getCoreComponent(folder).Infect(0f, manager);
                            }
                            log("Infected", Color.red);
                        }
                    }
                }
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
}
