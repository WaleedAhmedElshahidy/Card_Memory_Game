using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    // ---- Events ----
    public static event Action<int> OnScoreChanged;
    public static event Action<int> OnGuessChanged;
    public static event Action OnGameFinished;

    public UnityEvent OnGameStarted;

    // ---- Serialized Fields ----
    [SerializeField] private Card cardPrefab;
    [SerializeField] private RectTransform gridTransform;
    [SerializeField] private GridLayoutConfig gridConfigCS;
    [SerializeField] private int totalCards = 16;

    // ---- Private Fields ----
    private Sprite[] cardsSprites;
    private List<Sprite> spritePairs;
    private List<Card> spawnedCards = new List<Card>();
    private List<int> spriteIndices = new List<int>();

    private Card firstSelected;
    private Card secondSelected;

    private int score = 0;
    private int matchedPairs = 0;
    private int totalPairs;
    private int countGuesses = 0;

    // ---- Unity Methods ----
    private void Awake()
    {
        cardsSprites = Resources.LoadAll<Sprite>("YoGiOhCards");

        if (cardsSprites == null || cardsSprites.Length == 0)
            Debug.LogError("No sprites loaded! Check: Resources/YoGiOhCards");
    }

    //private void Start()
    //{
    //    if (SaveSystem.HasSave())
    //        LoadGame();
    //    else
    //        StartNewGame();
    //}

    // ---- New Game ----
    public void StartNewGame()
    {
        score = 0;
        matchedPairs = 0;
        countGuesses = 0;
        totalPairs = totalCards / 2;

        PrepareSprites();

        // force canvas update so container has correct size
        Canvas.ForceUpdateCanvases();

        gridConfigCS.SetupGrid(totalCards, gridTransform);
        CreateCards();
    }

    public void StartNewGame(int selectedCards)
    {
        totalCards = selectedCards;
        StartNewGame(); 
    }

    private void PrepareSprites()
    {
        spritePairs = new List<Sprite>();
        spriteIndices = new List<int>();

        for (int i = 0; i < totalPairs; i++)
        {
            spritePairs.Add(cardsSprites[i]);
            spritePairs.Add(cardsSprites[i]);
            spriteIndices.Add(i);
            spriteIndices.Add(i);
        }

        ShuffleSprites();
    }

    private void ShuffleSprites()
    {
        for (int i = spritePairs.Count - 1; i > 0; i--)
        {
            int randomIndex = UnityEngine.Random.Range(0, i + 1);

            Sprite tempSprite = spritePairs[i];
            spritePairs[i] = spritePairs[randomIndex];
            spritePairs[randomIndex] = tempSprite;

            int tempIndex = spriteIndices[i];
            spriteIndices[i] = spriteIndices[randomIndex];
            spriteIndices[randomIndex] = tempIndex;
        }
    }

    private void CreateCards()
    {
        foreach (Transform child in gridTransform)
            Destroy(child.gameObject);

        spawnedCards.Clear();

        for (int i = 0; i < spritePairs.Count; i++)
        {
            Card card = Instantiate(cardPrefab, gridTransform);
            card.name = i.ToString();
            card.SetIconSprite(spritePairs[i]);
            card.controller = this;
            spawnedCards.Add(card);
        }
        OnGameStarted?.Invoke();
    }

    public void SetSelected(Card card)
    {
        if (card.isSelected) return;
        if (card.isMatched) return;

        // if two cards already pending hide them immediately before continuing
        if (firstSelected != null && secondSelected != null)
        {
            StopAllCoroutines();

            if (!firstSelected.isMatched) firstSelected.Hide();
            if (!secondSelected.isMatched) secondSelected.Hide();

            firstSelected = null;
            secondSelected = null;
        }

        AudioManager.Instance.PlayFlip();
        card.Show();

        if (firstSelected == null)
        {
            firstSelected = card;
            return;
        }

        secondSelected = card;
        countGuesses++;

        OnGuessChanged?.Invoke(countGuesses);
        StartCoroutine(CheckMatching(firstSelected, secondSelected));
    }

    private IEnumerator CheckMatching(Card a, Card b)
    {
        yield return new WaitForSeconds(0.5f);

        if (a.iconSprite == b.iconSprite)
        {
            a.isMatched = true;
            b.isMatched = true;

            matchedPairs++;
            score += 10;

            AudioManager.Instance.PlayMatch();
            OnScoreChanged?.Invoke(score);

            SaveGame();

            if (matchedPairs >= totalPairs)
            {
                SaveSystem.DeleteSave();
                AudioManager.Instance.PlayGameOver();
                OnGameFinished?.Invoke();
            }
        }
        else
        {
            AudioManager.Instance.PlayMismatch();

            yield return new WaitForSeconds(0.3f);

            // only hide if they haven't been clicked again
            if (!a.isMatched) a.Hide();
            if (!b.isMatched) b.Hide();
        }

        firstSelected = null;
        secondSelected = null;
    }

    // ---- Save / Load ----
    private void SaveGame()
    {
        GameSaveData data = new GameSaveData
        {
            score = this.score,
            matchedPairs = this.matchedPairs,
            countGuesses = this.countGuesses,
            totalCards = this.totalCards,
            spriteIndices = spriteIndices.ToArray(),
            matchedCards = new bool[spawnedCards.Count]
        };

        for (int i = 0; i < spawnedCards.Count; i++)
            data.matchedCards[i] = spawnedCards[i].isMatched;

        SaveSystem.Save(data);
    }

    public void LoadGame()
    {
        GameSaveData data = SaveSystem.Load();

        if (data == null)
        {
            Debug.LogWarning("No save data found!");
            return; 
        }

        score = data.score;
        matchedPairs = data.matchedPairs;
        countGuesses = data.countGuesses;
        totalCards = data.totalCards;
        totalPairs = totalCards / 2;

        // rebuild sprite pairs from saved indices
        spritePairs = new List<Sprite>();
        spriteIndices = new List<int>(data.spriteIndices);

        foreach (int idx in spriteIndices)
            spritePairs.Add(cardsSprites[idx]);

        Canvas.ForceUpdateCanvases();
        gridConfigCS.SetupGrid(totalCards, gridTransform);
        CreateCards();

        // restore matched state
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (data.matchedCards[i])
            {
                spawnedCards[i].isMatched = true;
                spawnedCards[i].isSelected = true;
                spawnedCards[i].Show();
            }
        }

        OnScoreChanged?.Invoke(score);
        OnGuessChanged?.Invoke(countGuesses);
    }

    // ---- Public Helpers ----
    public void SetTotalCards(int count)
    {
        totalCards = count;
    }

    public void RestartGame()
    {
        SaveSystem.DeleteSave();
        StartNewGame();
        UIManager.Instance.scoreText.text = "Score: 0";
        UIManager.Instance.guessesText.text = "Guesses: 0";
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}