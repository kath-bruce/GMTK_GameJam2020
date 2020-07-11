﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Core;
using System;
using System.Linq;

namespace Managers
{
    public class PlayerManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }

        private CoreComponent selectedEntry;

        public void SetSelectedEntry(CoreComponent coreComp)
        {
            if (coreComp == null)
            {
                throw new InvalidOperationException("Attempting to set selected entry to null");
            }

            selectedEntry = coreComp;
            selectedEntry.SetHighlight(true);
        }

        void OnNavigate(InputValue value)
        {
            var dir = value.Get<Vector2>();
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
                        return;
                    }
                    else if ((selectedEntry.CoreElement as GameFolder).Folders.Count > 0)
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent((selectedEntry.CoreElement as GameFolder).Folders[0]));
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
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Files.Last()
                            ));
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetOlderSibling(selectedEntry.CoreElement as GameFile)
                        ));
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
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Folders.Last()
                            ));
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetOlderSibling(selectedEntry.CoreElement as GameFolder)
                        ));
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
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Files[0]
                            ));
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetYoungerSibling(selectedEntry.CoreElement as GameFile)
                        ));
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
                            return;
                        }
                        else
                        {
                            selectedEntry.SetHighlight(false);
                            SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                                selectedEntry.CoreElement.ContainingFolder.Folders[0]
                            ));
                            return;
                        }
                    }
                    else
                    {
                        selectedEntry.SetHighlight(false);
                        SetSelectedEntry(GameManager.UIManager.GetCoreComponent(
                            selectedEntry.CoreElement.ContainingFolder.GetYoungerSibling(selectedEntry.CoreElement as GameFolder)
                        ));
                        return;
                    }
                }
            }
        }

        void OnAction(InputValue value)
        {

            Debug.Log($"action : {value.Get<float>()}");
        }

        void OnSubmit(InputValue value)
        {

            Debug.Log($"submit : {value.Get<float>()}");
        }

        void OnCancel(InputValue value)
        {

            Debug.Log($"cancel : {value.Get<float>()}");
        }

    }
}
