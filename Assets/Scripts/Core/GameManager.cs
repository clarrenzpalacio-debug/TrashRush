using System;
using UnityEngine;
using TrashRush.Core;
using TrashRush.Trash;

namespace TrashRush.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        [SerializeField] private GameState currentState = GameState.Playing;
        [SerializeField] private float gameTime = 180f;
        private float timeRemaining;
        private int currentScore = 0;
        private int totalTrashCount = 0;
        private int sortedTrashCount = 0;

        public GameState CurrentState => currentState;
        public float TimeRemaining => timeRemaining;
        public int CurrentScore => currentScore;
        public int TotalTrashCount => totalTrashCount;
        public int SortedTrashCount => sortedTrashCount;

        public event Action<int> OnScoreChanged;
        public event Action<float> OnTimerTick;
        public event Action<GameState> OnStateChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            timeRemaining = gameTime;
        }

        private void Start()
        {
            // Count total initial trash
            TrashItem[] allTrash = FindObjectsOfType<TrashItem>();
            totalTrashCount = allTrash.Length;
            SetState(GameState.Playing);
        }

        private void Update()
        {
            if (currentState == GameState.Playing)
            {
                timeRemaining -= Time.deltaTime;
                OnTimerTick?.Invoke(timeRemaining);

                if (timeRemaining <= 0)
                {
                    timeRemaining = 0;
                    GameOver(false);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (currentState == GameState.Playing)
                    PauseGame();
                else if (currentState == GameState.Paused)
                    ResumeGame();
            }
        }

        public void SetState(GameState newState)
        {
            currentState = newState;
            OnStateChanged?.Invoke(currentState);
        }

        public void ProcessRecycle(bool isCorrect, TrashItem item, TrashCategory binCategory)
        {
            if (currentState != GameState.Playing) return;

            sortedTrashCount++;
            if (isCorrect)
            {
                currentScore += 10;
            }
            else
            {
                currentScore = Mathf.Max(0, currentScore - 5);
            }

            OnScoreChanged?.Invoke(currentScore);

            if (sortedTrashCount >= totalTrashCount)
            {
                GameOver(true);
            }
        }

        public void PauseGame()
        {
            SetState(GameState.Paused);
            Time.timeScale = 0f;
        }

        public void ResumeGame()
        {
            SetState(GameState.Playing);
            Time.timeScale = 1f;
        }

        public void GameOver(bool victory)
        {
            Time.timeScale = 0f;
            SetState(victory ? GameState.Won : GameState.Lost);
            if (Audio.AudioManager.Instance != null)
            {
                Audio.AudioManager.Instance.PlaySFX(victory ? Audio.AudioManager.SFXType.Victory : Audio.AudioManager.SFXType.GameOver);
            }
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }
    }
}
