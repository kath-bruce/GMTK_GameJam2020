using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core;

public class CoreComponent : MonoBehaviour
{
    public CoreBase CoreElement { get; set; }
    private float timer = 0f;
    private float elapsedTime = 0f;
    private bool startInfecting = false;

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
        if (CoreElement.InfectionState == InfectionState.Infected && startInfecting)
        {
            //random chance every virus action time to infect another file
            //chance is determined by virus strength
            //cannot go up into parent
            //check if root
            //get sibling - if not siblings, get parent
            elapsedTime += Time.deltaTime;

            if (elapsedTime >= timer)
            {
                if (CoreElement.ContainingFolder.Files.Count > 0)
                {
                    //loop through and infect through core component in not infected
                    //virus can break quarantine if upgraded
                    foreach (var file in CoreElement.ContainingFolder.Files)
                    {
                        if (file.InfectionState == InfectionState.Clean)
                        {
                            
                        }
                    }
                }
                else if (CoreElement.ContainingFolder.Folders.Count > 0)
                {
                    //see above
                }
            }
        }
    }

    public void Infect(float time) //time is usually 0, unless virus has upgraded
    {
        CoreElement.InfectionState = InfectionState.Infected;
        if (time > 0f)
        {
            startInfecting = true;
            timer = time;
        }
    }
}
