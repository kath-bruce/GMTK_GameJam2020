using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Core;
using TMPro;

namespace Managers
{
    public class ComputerManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }

        public GameFolder Root { get; set; }

        public GameObject EventLog;
        public TextMeshProUGUI EventPrefab;

        void Start()
        {
            Root = new GameFolder("Root", null, InfectionState.Clean);

            var desktop = new GameFolder("Desktop", Root, InfectionState.Clean);
            var secret = new GameFolder("SECRET DOCUMENTS", desktop, InfectionState.Clean);
            var thing = new GameFile("virus", GameFileExtension.BAT, secret, InfectionState.Infected, true);
            var innerfolder = new GameFolder("test inner", secret, InfectionState.Clean);
            var innerinnerFolder = new GameFolder("test inner inner", innerfolder, InfectionState.Clean);
            innerfolder.Folders.Add(innerinnerFolder);
            secret.Folders.Add(innerfolder);
            secret.Files.Add(thing);
            desktop.Folders.Add(secret);
            Root.Folders.Add(desktop);

            var documents = new GameFolder("Documents", Root, InfectionState.Clean);
            documents.Files.Add(new GameFile("password", GameFileExtension.TXT, documents, InfectionState.Clean, false));

            var docFiles = new List<GameFile>();

            for (int i = 0; i < 10; i++)
            {
                docFiles.Add(new GameFile($"doc{i}", GameFileExtension.TXT, documents, InfectionState.Clean, false));
            }

            documents.Files.AddRange(docFiles);
            Root.Folders.Add(documents);

            var images = new GameFolder("Images", Root, InfectionState.Clean);
            images.Files.Add(new GameFile("q0rn", GameFileExtension.PNG, images, InfectionState.Clean, false));
            images.Files.Add(new GameFile("hill", GameFileExtension.JPEG, images, InfectionState.Clean, false));
            Root.Folders.Add(images);

            var bin = new GameFolder("Recycle Bin", Root, InfectionState.Clean);
            bin.Files.Add(new GameFile("actual password", GameFileExtension.TXT, bin, InfectionState.Clean, false));
            Root.Folders.Add(bin);

            Helpers.JsonHelper.SaveJson(Root, Path.Combine(Application.dataPath, "filesystem.json"));

            Root = Helpers.JsonHelper.LoadJson(Path.Combine(Application.dataPath, "filesystem.json"));
            Root.SetParents();

            GameManager.UIManager.SetUpFileSystem(Root);
            GameManager.VirusManager.SetVirusFile(Root.GetVirus());
        }

        public void AddToEventLog(string logText, Color textColor)
        {
            var eventText = Instantiate(EventPrefab, Vector3.zero, Quaternion.identity, EventLog.transform);
            eventText.text = "> " + logText;
            eventText.color = textColor;
        }

    }
}
