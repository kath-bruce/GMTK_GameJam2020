using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Managers
{
    public class ComputerManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }

        public GameFolder Root { get; set; }

        void Start()
        {
            Root = new GameFolder("Root", null);

            var desktop = new GameFolder("Desktop", Root);
            var secret = new GameFolder("SECRET DOCUMENTS", desktop);
            var thing = new GameFile("virus", GameFileExtension.BAT, secret);
            var innerfolder = new GameFolder("test inner", secret);
            var innerinnerFolder = new GameFolder("test inner inner", innerfolder);
            innerfolder.Folders.Add(innerinnerFolder);
            secret.Folders.Add(innerfolder);
            secret.Files.Add(thing);
            desktop.Folders.Add(secret);
            Root.Folders.Add(desktop);

            var documents = new GameFolder("Documents", Root);
            documents.Files.Add(new GameFile("password", GameFileExtension.TXT, documents));
            Root.Folders.Add(documents);

            var images = new GameFolder("Images", Root);
            images.Files.Add(new GameFile("q0rn", GameFileExtension.PNG, images));
            images.Files.Add(new GameFile("hill", GameFileExtension.JPEG, images));
            Root.Folders.Add(images);

            var bin = new GameFolder("Recycle Bin", Root);
            bin.Files.Add(new GameFile("actual password", GameFileExtension.TXT, bin));
            Root.Folders.Add(bin);

            GameManager.UIManager.SetUpFolders(Root);
        }

    }
}
