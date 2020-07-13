using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public enum Request {}

    public class UserManager : MonoBehaviour
    {
        public GameManager GameManager { get; set; }

        void Update()
        {
            //every 10 - 15s, do something weird
            //reboot - 50/50 either you or the virus are delayed
            //restore - virus is weaker - remove mutation
            //opened quarantined file - infection spreads quicker now
        }

        /*public void AskUser(Request request)
        {
            //do something based on request
            //can't do anything else
            //ask for permission - unlikely to succeed - takes times
            //ask to upgrade license - likely to succeed but has chance to take a while if user forgets login
                //becomes available on finding license
            //attempt password entry to win - depends on how many letters so far 
        }*/
    }
}
