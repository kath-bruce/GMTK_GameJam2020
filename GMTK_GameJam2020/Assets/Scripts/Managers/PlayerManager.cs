using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
using System;
using System.Linq;
using TMPro;

namespace Managers
{
    public enum PlayerNavigationState { FileSystem, MenuPopup }

    [Flags]
    public enum Licenses { None = 0, QuarantineVirus = 1, BetterQuarantine = 2, CleanInfected = 4 }

    public class PlayerManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }

        public PlayerNavigationState NavState { get; private set; } = PlayerNavigationState.FileSystem;
        public Licenses Licenses { get; set; }

        private CoreComponent selectedEntry;
        private CoreComponent selectedMenuEntry;

        private List<MenuAction> menuOptions = new List<MenuAction>();

        public float QuarantineTime;
        private float initQTime;

        public PasswordComponent Password;

        List<CoreComponent> usedLetters = new List<CoreComponent>();
        private float delay = 5f;
        private float delayElapsed = 10f;

        void Start()
        {
            menuOptions.Add(new MenuAction("Scan", Scan)); //0
            menuOptions.Add(new MenuAction("Quarantine", Quarantine)); //1
            menuOptions.Add(new MenuAction("Clean", Clean)); //2

            initQTime = QuarantineTime;
        }

        public void SetSelectedEntry(CoreComponent coreComp)
        {
            if (coreComp == null)
            {
                throw new InvalidOperationException("Attempting to set selected entry to null");
            }

            selectedEntry = coreComp;
            selectedEntry.SetHighlight(true);
        }

        public CoreComponent GetSelectedEntry()
        {
            return selectedEntry;
        }

        public void SetSelectedMenuEntry(CoreComponent option)
        {
            if (option == null)
            {
                throw new InvalidOperationException("Attempting to set selected menu entry to null");
            }

            selectedMenuEntry = option;
            selectedMenuEntry.SetMenuHighlight(true);
        }

        public void DeselectMenuEntry()
        {
            selectedMenuEntry.SetMenuHighlight(false);
        }

        void OnNavigate(InputValue value)
        {
            if (GameManager.State != GameState.Playing || (delayElapsed < delay))
            {
                return;
            }

            var dir = value.Get<Vector2>();

            if (NavState == PlayerNavigationState.FileSystem)
            {
                NavigateFileSystem(dir);
            }
            else if (NavState == PlayerNavigationState.MenuPopup)
            {
                NavigateMenuPopup(dir);
            }
        }

        private void NavigateFileSystem(Vector2 dir)
        {

            //check if can move - possibly need to scroll so change content - ScrollRect.verticalNormalizedPosition
            //can only move in one dir so return on success (i.e., on every set selected entry call)
            if (dir.x > 0) //left
            {
                //if folder and has folders, go to first file, if it exists
                if (selectedEntry.CoreElement is GameFolder)
                {
                    if ((selectedEntry.CoreElement as GameFolder).Files.Count > 0)
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent((selectedEntry.CoreElement as GameFolder).Files[0]));
                        GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                        return;
                    }
                    else if ((selectedEntry.CoreElement as GameFolder).Folders.Count > 0)
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent((selectedEntry.CoreElement as GameFolder).Folders[0]));
                        GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                        return;
                    }
                }
            }
            else if (dir.x < 0) //right
            {
                if (selectedEntry.CoreElement.ContainingFolder == null) //is root so can't go right
                {
                    return;
                }

                //exit folder, go to parent
                if (selectedEntry.CoreElement.ContainingFolder != null)
                {
                    selectedEntry.SetHighlight(false);
                    SetSelectedEntry(GameManager.UIManager.GetCoreComponent(selectedEntry.CoreElement.ContainingFolder));
                    GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                    return;
                }
            }

            if (selectedEntry.CoreElement.ContainingFolder == null) //is root so can't go up or down (may change this later)
            {
                return;
            }

            if (dir.y > 0) //up
            {
                //go up in parents list of children - loop
                //files are at the top so 
                //if current element is folder 0 in parent folder list, go to last of parent file list
                //if current element is file 0 in parent file list, go to last of parent folder list

                if (selectedEntry.CoreElement is GameFile)
                {
                    var file = selectedEntry.CoreElement as GameFile;

                    if (file.Equals(file.ContainingFolder.Files[0]))
                    {
                        //goto last folder if exists
                        //else go to last file
                        if (selectedEntry.CoreElement.ContainingFolder.Folders.Count > 0)
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Folders.Last()
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Files.Last()
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetOlderSibling(selectedEntry.CoreElement as GameFile)
                        ));
                        GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                        return;
                    }
                }
                else if (selectedEntry.CoreElement is GameFolder)
                {
                    var folder = selectedEntry.CoreElement as GameFolder;

                    if (folder.Equals(folder.ContainingFolder.Folders[0]))
                    {
                        //goto last file if exists or last folder
                        if (selectedEntry.CoreElement.ContainingFolder.Files.Count > 0)
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Files.Last()
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Folders.Last()
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetOlderSibling(selectedEntry.CoreElement as GameFolder)
                        ));
                        GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                        return;
                    }
                }
            }
            else if (dir.y < 0) //down
            {
                //opposite logic as above

                if (selectedEntry.CoreElement is GameFile)
                {
                    //if file is last in parent file list, go to first in parent folder list
                    //else go to younger file sibling

                    var file = selectedEntry.CoreElement as GameFile;

                    if (file.Equals(file.ContainingFolder.Files.Last()))
                    {
                        //go to first folder if exists
                        //else go to first file
                        if (selectedEntry.CoreElement.ContainingFolder.Folders.Count > 0)
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Folders[0]
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Files[0]
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetYoungerSibling(selectedEntry.CoreElement as GameFile)
                        ));
                        GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                        return;
                    }

                }
                else if (selectedEntry.CoreElement is GameFolder)
                {
                    //if folder is last in parent folder list, go to first in parent file list
                    //goto first file if exists or first folder
                    //else go to younger folder sibling

                    var folder = selectedEntry.CoreElement as GameFolder;

                    if (folder.Equals(folder.ContainingFolder.Folders.Last()))
                    {
                        if (selectedEntry.CoreElement.ContainingFolder.Files.Count > 0)
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Files[0]
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Folders[0]
                            ));
                            GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetYoungerSibling(selectedEntry.CoreElement as GameFolder)
                        ));
                        GameManager.UIManager.SetFileSystemScroll(selectedEntry.GetComponent<RectTransform>());
                        return;
                    }
                }
            }

        }

        private void NavigateMenuPopup(Vector2 dir)
        {
            if (dir.y == 0)
            {
                return;
            }

            if (dir.y > 0)
            {
                if (selectedMenuEntry.CoreElement.Equals(menuOptions[0]))
                {
                    if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]));
                    }
                    else if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]));
                    }
                }
                else if (selectedMenuEntry.CoreElement.Equals(menuOptions[1]))
                {
                    if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]));
                    }
                    else if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]));
                    }
                }
                else if (selectedMenuEntry.CoreElement.Equals(menuOptions[2]))
                {
                    if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]));
                    }
                    else if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]));
                    }
                }

            }
            else if (dir.y < 0)
            {
                if (selectedMenuEntry.CoreElement.Equals(menuOptions[0]))
                {
                    if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]));
                    }
                    else if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]));
                    }
                }
                else if (selectedMenuEntry.CoreElement.Equals(menuOptions[1]))
                {
                    if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[2]));
                    }
                    else if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]));
                    }
                }
                else if (selectedMenuEntry.CoreElement.Equals(menuOptions[2]))
                {
                    if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[0]));
                    }
                    else if (GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]).GetComponentInChildren<TextMeshProUGUI>().color != Color.gray)
                    {
                        selectedMenuEntry.SetMenuHighlight(false);
                        SetSelectedMenuEntry(GameManager.UIManager.GetMenuCoreComponent(menuOptions[1]));
                    }
                }
            }
        }

        void OnAction(InputValue value)
        {
            if (GameManager.State != GameState.Playing || NavState == PlayerNavigationState.MenuPopup || (delayElapsed < delay))
            {
                return;
            }

            NavState = PlayerNavigationState.MenuPopup;

            GameManager.UIManager.ShowPopup
            (
                selectedEntry.transform.position,
                menuOptions,
                (selectedEntry.CoreElement.InfectionState == InfectionState.Infected)
            );
        }

        void Scan()
        {
            //different CONSEQUENCES
            //get license
            //find password letter
            //joke - 9 jokes to come up with
            if (selectedEntry.CoreElement.Result == ScanResult.License)
            {
                if (!Licenses.HasFlag(Licenses.BetterQuarantine))
                {
                    Licenses |= Licenses.BetterQuarantine;
                    QuarantineTime += 10f;
                    GameManager.UIManager.BetterQuarantine.SetActive(true);
                }
                else if (!Licenses.HasFlag(Licenses.CleanInfected))
                {
                    Licenses |= Licenses.CleanInfected;
                    GameManager.UIManager.CleanInfected.SetActive(true);
                }
                else if (!Licenses.HasFlag(Licenses.QuarantineVirus))
                {
                    Licenses |= Licenses.QuarantineVirus;
                    GameManager.UIManager.QuarantineVirus.SetActive(true);
                }

                GameManager.ComputerManager.AddToEventLog("Found license upgrade!", new Color(0f, .8f, 0f));
            }
            else if (selectedEntry.CoreElement.Result == ScanResult.PasswordLetter)
            {
                if (usedLetters.Any((comp) => comp.CoreElement.Name == selectedEntry.CoreElement.Name))
                {
                    return;
                }

                Password.AddLetter();

                GameManager.ComputerManager.AddToEventLog("Found password letter!", new Color(0f, .8f, 0f));

                usedLetters.Add(selectedEntry);

                if (Password.CompletedPassword())
                {
                    //win
                    GameManager.State = GameState.Win;
                }
            }
            else if (selectedEntry.CoreElement.Result == ScanResult.Joke)
            {
                if (selectedEntry.CoreElement.Name == "Root")
                {
                    GameManager.ComputerManager.AddToEventLog("If this gets infected it's GAME OVER", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "Desktop")
                {
                    GameManager.ComputerManager.AddToEventLog("Better not look at the secret documents...", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "SECRET DOCUMENTS")
                {
                    GameManager.ComputerManager.AddToEventLog("TOP SECRET DO NOT LOOK", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "virus")
                {
                    GameManager.ComputerManager.AddToEventLog("It's a virus alright", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "Documents")
                {
                    GameManager.ComputerManager.AddToEventLog("Just documents", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "password")
                {
                    GameManager.ComputerManager.AddToEventLog("Did you think it'd be that easy?", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "doc9")
                {
                    GameManager.ComputerManager.AddToEventLog("Not quite :)", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "q0rn")
                {
                    GameManager.ComputerManager.AddToEventLog("I can't believe it's not meat!", Color.blue);
                }
                else if (selectedEntry.CoreElement.Name == "hill")
                {
                    GameManager.ComputerManager.AddToEventLog("DO I LOOK LIKE I KNOW WHAT A JPEG IS", Color.blue);
                }
            }
            else
            {
                GameManager.ComputerManager.AddToEventLog("Scanned result: null", Color.black);
            }
        }

        void Quarantine()
        {
            if (selectedEntry.CoreElement is GameFile &&
            (selectedEntry.CoreElement as GameFile).IsVirus)
            {
                //WON
                selectedEntry.Quarantine(float.MaxValue, GameManager);
                GameManager.ComputerManager.AddToEventLog($"Quarantined {selectedEntry.CoreElement.Name}", Color.cyan);
                GameManager.State = GameState.Win;
            }
            else
            {
                selectedEntry.Quarantine(QuarantineTime, GameManager);
                GameManager.ComputerManager.AddToEventLog($"Quarantined {selectedEntry.CoreElement.Name}", Color.cyan);
            }
        }

        void Clean()
        {
            selectedEntry.CoreElement.InfectionState = InfectionState.Clean;
            selectedEntry.GetComponent<TextMeshProUGUI>().color = Color.white;
            GameManager.ComputerManager.AddToEventLog($"Cleaned {selectedEntry.CoreElement.Name}!", Color.white);
        }

        void OnSubmit(InputValue value)
        {
            if (GameManager.State != GameState.Playing || (delayElapsed < delay))
            {
                return;
            }

            if (NavState == PlayerNavigationState.MenuPopup)
            {
                (selectedMenuEntry.CoreElement as MenuAction).SelectionAction();

                GameManager.UIManager.ClosePopup();

                NavState = PlayerNavigationState.FileSystem;
            }
        }

        void OnCancel(InputValue value)
        {
            if (GameManager.State != GameState.Playing || (delayElapsed < delay))
            {
                return;
            }

            GameManager.UIManager.ClosePopup();

            NavState = PlayerNavigationState.FileSystem;
        }

        public void Delay()
        {
            delayElapsed = 0f;
        }

        void Update()
        {
            if (delayElapsed < delay) //if not finish delay
            {
                delayElapsed += Time.deltaTime;
            }
        }

        public void Restart()
        {
            DeselectMenuEntry();
            NavState = PlayerNavigationState.FileSystem;
            delayElapsed = 10f;
            Password.Restart();
            QuarantineTime = initQTime;

            if (Licenses.HasFlag(Licenses.BetterQuarantine))
            {
                Licenses &= ~Licenses.BetterQuarantine;
            }
            if (Licenses.HasFlag(Licenses.QuarantineVirus))
            {
                Licenses &= ~Licenses.QuarantineVirus;
            }
            if (Licenses.HasFlag(Licenses.CleanInfected))
            {
                Licenses &= ~Licenses.CleanInfected;
            }

            Licenses = Licenses.None;
            usedLetters = null;
            usedLetters = new List<CoreComponent>();
        }

    }
}
