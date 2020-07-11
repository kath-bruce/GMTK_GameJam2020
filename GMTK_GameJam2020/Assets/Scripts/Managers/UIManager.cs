using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        public void SetUpFileSystem(GameFolder root)
        {
            var rootText = Instantiate(RootPrefab, Vector3.zero, Quaternion.identity, ContentView.transform).GetComponent<TextMeshProUGUI>();
            rootText.text = root.Name;
            rootText.GetComponent<CoreComponent>().CoreElement = root;

            foreach (var file in root.Files)
            {
                var text = Instantiate(RootPrefab, Vector3.zero, Quaternion.identity, ContentView.transform).GetComponent<TextMeshProUGUI>();
                text.text = "\t" + file.Name + "." + file.Extension.ToString();
                text.GetComponent<CoreComponent>().CoreElement = file;
            }

            foreach (var folder in root.Folders)
            {
                RecursiveInstantiation(folder, 0);
            }

            GameManager.PlayerManager.SetSelectedEntry(rootText.GetComponent<CoreComponent>());
        }

        private void RecursiveInstantiation(GameFolder folder, int iteration)
        {
            string tabs = "";

            for (int i = 0; i < iteration; i++)
            {
                tabs += "\t";
            }

            var innerTextFolder = Instantiate(RootPrefab, Vector3.zero, Quaternion.identity, ContentView.transform).GetComponent<TextMeshProUGUI>();
            innerTextFolder.text = tabs + "\t" + folder.Name;
            innerTextFolder.GetComponent<CoreComponent>().CoreElement = folder;

            foreach (var file in folder.Files)
            {
                var textFile = Instantiate(RootPrefab, Vector3.zero, Quaternion.identity, ContentView.transform).GetComponent<TextMeshProUGUI>();
                textFile.text = tabs + "\t\t" + file.Name + "." + file.Extension.ToString();
                textFile.GetComponent<CoreComponent>().CoreElement = file;
            }

            if (folder.Folders.Count > 0)
            {
                RecursiveInstantiation(folder.Folders[0], ++iteration);
            }
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
    }
}
