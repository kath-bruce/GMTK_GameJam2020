using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public ComputerManager ComputerManager { get; set; }
        public PlayerManager PlayerManager { get; set; }
        public UIManager UIManager { get; set; }
        public UserManager UserManager { get; set; }
        public VirusManager VirusManager { get; set; }

        public GameObject CompManGameObj;
        public GameObject PlayManGameObj;
        public GameObject UIManGameObj;
        public GameObject UserManGameObj;
        public GameObject VirusManGameObj;

        void Awake()
        {
            ComputerManager = CompManGameObj.GetComponent<ComputerManager>();
            PlayerManager = PlayManGameObj.GetComponent<PlayerManager>();
            UIManager = UIManGameObj.GetComponent<UIManager>();
            UserManager = UserManGameObj.GetComponent<UserManager>();
            VirusManager = VirusManGameObj.GetComponent<VirusManager>();

            ComputerManager.GameManager = this;
            PlayerManager.GameManager = this;
            UIManager.GameManager = this;
            UserManager.GameManager = this;
            VirusManager.GameManager = this;
        }
    }
}
