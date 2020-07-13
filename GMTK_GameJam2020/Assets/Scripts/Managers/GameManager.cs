using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public enum GameState { Playing, Win, Lose }
    public class GameManager : MonoBehaviour
    {
        private GameState state;
        public GameState State { 
            get => state; 
            set
            {
                state = value;

                if (state == GameState.Win)
                {
                    Win.SetActive(true);

                    if (PlayerManager.Password.CompletedPassword())
                    {
                        Win.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"You Won!\nYou found the password to deactivate the virus\nTime: {TimeElapsed.TimeElapsed.ToString("#0.00")}s";
                    }
                    else if (PlayerManager.Licenses.HasFlag(Licenses.QuarantineVirus))
                    {
                        Win.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = $"You Won!\nYou quarantined the virus!\nTime: {TimeElapsed.TimeElapsed.ToString("#0.00")}s";
                    }
                }
                else if (state == GameState.Lose)
                {
                    Lose.SetActive(true);
                }
            }
        }
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
        public TimeElapsedComponent TimeElapsed;

        public GameObject Win;
        public GameObject Lose;

        void Awake()
        {
            State = GameState.Playing;

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
