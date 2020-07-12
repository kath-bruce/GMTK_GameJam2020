using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Core;

namespace Managers
{
    public class UIManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }

        public Canvas Canvas;
        public GameObject ContentView;
        public GameObject RootPrefab;
        public GameObject Popup;
        public int XOffset;
        public TextMeshProUGUI VirusCountdown;

        public void SetVirusCountdown(float time)
        {
            VirusCountdown.text = time.ToString("#0.00s");
        }

        public void ShowPopup(Vector3 selectionPos, List<MenuAction> options, bool isInfected)
        {
            selectionPos.x += XOffset;

            Popup.SetActive(true);

            Popup.transform.position = selectionPos;

            var coreComponents = Popup.GetComponentsInChildren<CoreComponent>(true);

            //set text and set core element
            for (int i = 0; i < options.Count; i++)
            {
                coreComponents[i].CoreElement = options[i];
                coreComponents[i].GetComponentInChildren<TextMeshProUGUI>().text = ">" + options[i].Name;

                if (isInfected)
                {
                    coreComponents[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.gray;
                }
            }

            GameManager.PlayerManager.SetSelectedMenuEntry(coreComponents[0]);
        }

        public void ClosePopup()
        {
            Popup.SetActive(false);
        }

        public void SetUpFileSystem(GameFolder root)
        {
            var rootComponent = InstantiateGameElement(root, "");

            foreach (var file in root.Files)
            {
                InstantiateGameElement(file, "\t", file.Extension.ToString());
            }

            foreach (var folder in root.Folders)
            {
                RecursiveInstantiation(folder, 0);
            }

            GameManager.PlayerManager.SetSelectedEntry(rootComponent.GetComponent<CoreComponent>());
        }

        private void RecursiveInstantiation(GameFolder folder, int iteration)
        {
            string tabs = "";

            for (int i = 0; i < iteration; i++)
            {
                tabs += "\t";
            }

            InstantiateGameElement(folder, tabs + "\t");

            foreach (var file in folder.Files)
            {
                InstantiateGameElement(file, tabs + "\t\t", file.Extension.ToString());
            }

            if (folder.Folders.Count > 0)
            {
                RecursiveInstantiation(folder.Folders[0], ++iteration);
            }
        }

        private CoreComponent InstantiateGameElement(CoreBase coreElement, string tabs, string extension = null)
        {
            var text = Instantiate(RootPrefab, Vector3.zero, Quaternion.identity, ContentView.transform).GetComponent<TextMeshProUGUI>();

            if (extension != null)
            {
                text.text = tabs + coreElement.Name + "." + extension;
            }
            else
            {
                text.text = tabs + coreElement.Name;
            }

            text.GetComponent<CoreComponent>().CoreElement = coreElement;

            return text.GetComponent<CoreComponent>();
        }

        public CoreComponent GetCoreComponent(CoreBase coreElement)
        {
            foreach (var child in ContentView.GetComponentsInChildren<CoreComponent>())
            {
                if (child.CoreElement.Name == coreElement.Name && child.CoreElement.ContainingFolder == coreElement.ContainingFolder)
                {
                    return child;
                }
            }

            return null;
        }

        public CoreComponent GetMenuCoreComponent(MenuAction option)
        {
            foreach (var child in Popup.GetComponentsInChildren<CoreComponent>())
            {
                if (!(child.CoreElement is MenuAction))
                {
                    return null;
                }

                var childOption = child.CoreElement as MenuAction;

                if (childOption.Name == option.Name
                && childOption.ContainingFolder == option.ContainingFolder
                && childOption.SelectionAction == option.SelectionAction)
                {
                    return child;
                }
            }

            return null;
        }
    }
}
