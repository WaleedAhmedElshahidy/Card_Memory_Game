using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Buttons")]
    [SerializeField] private Button continueButton;

    [Header("Text")]
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI guessesText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;

    private GameManager gameManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        gameManager = GetComponent<GameManager>();

        // continue button only active if save exists
        continueButton.interactable = SaveSystem.HasSave();

        // button listeners
        continueButton.onClick.AddListener(OnContinueClicked);
    }

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnGuessChanged += UpdateGuesses;
        GameManager.OnGameFinished += ShowGameOver;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnGuessChanged -= UpdateGuesses;
        GameManager.OnGameFinished -= ShowGameOver;
    }

    // ---- Button Handlers ----
    private void OnContinueClicked()
    {
        gameManager.LoadGame();
    }

    

    // ---- Event Handlers ----
    private void UpdateScore(int newScore)
    {
        scoreText.text = "Score: " + newScore;
    }

    private void UpdateGuesses(int newGuesses)
    {
        guessesText.text = "Guesses: " + newGuesses;
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}