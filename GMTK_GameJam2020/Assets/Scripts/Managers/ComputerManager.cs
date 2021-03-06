﻿using System.Collections;
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
            Root = new GameFolder("Root", null, InfectionState.Clean, ScanResult.Joke);

            var desktop = new GameFolder("Desktop", Root, InfectionState.Clean, ScanResult.Joke);
            var secret = new GameFolder("SECRET DOCUMENTS", desktop, InfectionState.Clean, ScanResult.Joke);
            var thing = new GameFile("virus", GameFileExtension.BAT, secret, InfectionState.Infected, ScanResult.Joke, true);
            var innerfolder = new GameFolder("virus sourcecode", secret, InfectionState.Clean, ScanResult.License);
            secret.Folders.Add(innerfolder);
            secret.Files.Add(thing);
            desktop.Folders.Add(secret);
            Root.Folders.Add(desktop);

            var documents = new GameFolder("Documents", Root, InfectionState.Clean, ScanResult.Joke);
            documents.Files.Add(new GameFile("password", GameFileExtension.TXT, documents, InfectionState.Clean, ScanResult.Joke, false));

            var docFiles = new List<GameFile>();

            for (int i = 0; i < 10; i++)
            {
                if (i == 9)
                {
                    docFiles.Add(new GameFile($"doc{i}", GameFileExtension.TXT, documents, InfectionState.Clean, ScanResult.Joke, false));
                    continue;
                }
                docFiles.Add(new GameFile($"doc{i}", GameFileExtension.TXT, documents, InfectionState.Clean, ScanResult.PasswordLetter, false));
            }

            documents.Files.AddRange(docFiles);
            Root.Folders.Add(documents);

            var images = new GameFolder("Images", Root, InfectionState.Clean, ScanResult.License);
            images.Files.Add(new GameFile("q0rn", GameFileExtension.PNG, images, InfectionState.Clean, ScanResult.Joke, false));
            images.Files.Add(new GameFile("hill", GameFileExtension.JPEG, images, InfectionState.Clean, ScanResult.Joke, false));
            Root.Folders.Add(images);

            var bin = new GameFolder("Recycle Bin", Root, InfectionState.Clean, ScanResult.License);
            bin.Files.Add(new GameFile("actual password", GameFileExtension.TXT, bin, InfectionState.Clean, ScanResult.PasswordLetter, false));
            Root.Folders.Add(bin);

            //Helpers.JsonHelper.SaveJson(Root, Path.Combine(Application.dataPath, "filesystem.json"));

            //Root = Helpers.JsonHelper.LoadJson(Path.Combine(Application.dataPath, "filesystem.json"));
            //Root.SetParents();

            GameManager.UIManager.SetUpFileSystem(Root);
            GameManager.VirusManager.SetVirusFile(Root.GetVirus());
        }

        public void Restart()
        {
            //clear log
            for (int i = 0; i < EventLog.transform.childCount; i++)
            {
                Destroy(EventLog.transform.GetChild(i).gameObject);
            }

            Start();
        }

        public void AddToEventLog(string logText, Color textColor)
        {
            var eventText = Instantiate(EventPrefab, Vector3.zero, Quaternion.identity, EventLog.transform);
            eventText.text = $"> " + logText;
            eventText.color = textColor;

            GameManager.UIManager.SetEventLogScroll();
            eventText.gameObject.transform.SetAsFirstSibling();
        }

    }
}
