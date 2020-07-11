using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public ComputerManager ComputerManager { get; set; }
        public PlayerManager PlayerManager { get; set; }
        public TurnManager TurnManager { get; set; }
        public UIManager UIManager { get; set; }
        public UserManager UserManager { get; set; }
        public VirusManager VirusManager { get; set; }

        public GameObject CompManGameObj;
        public GameObject PlayManGameObj;
        public GameObject TurnManGameObj;
        public GameObject UIManGameObj;
        public GameObject UserManGameObj;
        public GameObject VirusManGameObj;

        void Awake()
        {
            ComputerManager = CompManGameObj.GetComponent<ComputerManager>();
            PlayerManager = PlayManGameObj.GetComponent<PlayerManager>();
            TurnManager = TurnManGameObj.GetComponent<TurnManager>();
            UIManager = UIManGameObj.GetComponent<UIManager>();
            UserManager = UserManGameObj.GetComponent<UserManager>();
            VirusManager = VirusManGameObj.GetComponent<VirusManager>();

            ComputerManager.GameManager = this;
            PlayerManager.GameManager = this;
            TurnManager.GameManager = this;
            UIManager.GameManager = this;
            UserManager.GameManager = this;
            VirusManager.GameManager = this;
        }
    }
}
