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
        }

        public void AskUser(Request request)
        {
            //do something based on request
        }
    }
}
