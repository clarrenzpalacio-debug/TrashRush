using UnityEngine;
using UnityEngine.UI;
using TrashRush.Core;
using TrashRush.Player;
using TrashRush.Inventory;

namespace TrashRush.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("HUD")]
        [SerializeField] private Text scoreText;
        [SerializeField] private Text timerText;
        [SerializeField] private Text trashCountText;
        [SerializeField] private Text promptText;
        [SerializeField] private Text carriedItemText;
        [SerializeField] private Text feedbackText;
        [SerializeField] private Text inventoryCountText;

        [Header("Stamina")]
        [SerializeField] private Image staminaBarFill;

        [Header("Screens")]
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject victoryPanel;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private Text victoryScoreText;
        [SerializeField] private Text gameOverScoreText;

        private PlayerInteraction playerInteraction;
        private TrashBagInventory trashBag;
        private PlayerStamina playerStamina;

        private void Start()
        {
            playerInteraction = FindObjectOfType<PlayerInteraction>();
            if (playerInteraction != null)
            {
                trashBag = playerInteraction.TrashBag;
            }

            // Find stamina via PlayerController
            PlayerController pc = FindObjectOfType<PlayerController>();
            if (pc != null)
            {
                playerStamina = pc.Stamina;
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.OnScoreChanged += UpdateScore;
                GameManager.Instance.OnTimerTick += UpdateTimer;
                GameManager.Instance.OnStateChanged += UpdateStateScreens;
            }

            UpdateScore(0);
            UpdateTimer(180f);
        }

        private void Update()
        {
            if (playerInteraction == null)
            {
                playerInteraction = FindObjectOfType<PlayerInteraction>();
                if (playerInteraction != null)
                {
                    trashBag = playerInteraction.TrashBag;
                }
            }

            if (playerInteraction != null)
            {
                // Dynamic contextual prompt
                if (promptText != null)
                {
                    promptText.text = playerInteraction.GetCurrentPrompt();
                }

                // ── Inventory / Bag display ─────────────────────────────
                if (carriedItemText != null)
                {
                    if (trashBag != null && trashBag.ItemCount > 0)
                    {
                        string summary = trashBag.GetContentsSummary();
                        carriedItemText.text = $"BAG: {summary}";
                    }
                    else
                    {
                        carriedItemText.text = "BAG: Empty";
                    }
                }

                // Inventory count (e.g. "3/10")
                if (inventoryCountText != null && trashBag != null)
                {
                    inventoryCountText.text = $"{trashBag.ItemCount}/{trashBag.Capacity}";

                    // Color the text red when full
                    if (trashBag.IsFull)
                        inventoryCountText.color = new Color(1f, 0.27f, 0.27f); // red
                    else
                        inventoryCountText.color = Color.white;
                }

                // Toast Feedback notification
                if (feedbackText != null)
                {
                    string fb = playerInteraction.FeedbackMessage;
                    feedbackText.text = fb;
                    feedbackText.gameObject.SetActive(!string.IsNullOrEmpty(fb));
                }
            }

            if (GameManager.Instance != null && trashCountText != null)
            {
                trashCountText.text = $"CLEANED: {GameManager.Instance.SortedTrashCount} / {GameManager.Instance.TotalTrashCount}";
            }

            // ── Stamina bar ─────────────────────────────────────────────
            UpdateStaminaBar();
        }

        private void UpdateStaminaBar()
        {
            if (staminaBarFill == null) return;

            // Find stamina if not cached yet
            if (playerStamina == null)
            {
                PlayerController pc = FindObjectOfType<PlayerController>();
                if (pc != null) playerStamina = pc.Stamina;
                if (playerStamina == null) return;
            }

            float ratio = playerStamina.StaminaRatio;
            staminaBarFill.fillAmount = ratio;

            // Color: green > 50%, yellow 25-50%, red < 25%
            if (ratio > 0.5f)
                staminaBarFill.color = new Color(0.30f, 0.85f, 0.35f); // green
            else if (ratio > 0.25f)
                staminaBarFill.color = new Color(0.95f, 0.80f, 0.20f); // yellow
            else
                staminaBarFill.color = new Color(0.90f, 0.25f, 0.25f); // red
        }

        private string GetCategoryHexColor(TrashCategory cat)
        {
            switch (cat)
            {
                case TrashCategory.Paper:   return "#1E88E5"; // Blue
                case TrashCategory.Plastic: return "#FDD835"; // Yellow
                case TrashCategory.Metal:   return "#E53935"; // Red
                case TrashCategory.Organic: return "#43A047"; // Green
                default: return "#FFFFFF";
            }
        }

        public void BindUI(Text score, Text timer, Text trashCount, Text prompt, GameObject pause, GameObject victory, GameObject gameOver, Text vScore, Text goScore)
        {
            BindUI(score, timer, trashCount, prompt, null, null, pause, victory, gameOver, vScore, goScore);
        }

        public void BindUI(Text score, Text timer, Text trashCount, Text prompt, Text carriedItem, Text feedback, GameObject pause, GameObject victory, GameObject gameOver, Text vScore, Text goScore)
        {
            scoreText = score;
            timerText = timer;
            trashCountText = trashCount;
            promptText = prompt;
            carriedItemText = carriedItem;
            feedbackText = feedback;
            pausePanel = pause;
            victoryPanel = victory;
            gameOverPanel = gameOver;
            victoryScoreText = vScore;
            gameOverScoreText = goScore;
        }

        private void UpdateScore(int score)
        {
            if (scoreText != null)
                scoreText.text = $"SCORE: {score:D4}";
        }

        private void UpdateTimer(float time)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(time / 60F);
                int seconds = Mathf.FloorToInt(time % 60F);
                timerText.text = $"TIME: {minutes:00}:{seconds:00}";
            }
        }

        private void UpdateStateScreens(GameState state)
        {
            if (pausePanel != null) pausePanel.SetActive(state == GameState.Paused);
            if (victoryPanel != null)
            {
                victoryPanel.SetActive(state == GameState.Won);
                if (state == GameState.Won && victoryScoreText != null && GameManager.Instance != null)
                {
                    victoryScoreText.text = $"Final Score: {GameManager.Instance.CurrentScore}";
                }
            }
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(state == GameState.Lost);
                if (state == GameState.Lost && gameOverScoreText != null && GameManager.Instance != null)
                {
                    gameOverScoreText.text = $"Final Score: {GameManager.Instance.CurrentScore}";
                }
            }
        }

        public void OnResumeClicked()
        {
            if (GameManager.Instance != null) GameManager.Instance.ResumeGame();
        }

        public void OnRestartClicked()
        {
            if (GameManager.Instance != null) GameManager.Instance.RestartGame();
        }
    }
}
