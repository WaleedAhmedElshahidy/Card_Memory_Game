using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Sprite bgBtn;

    public List<Button> btns = new List<Button>();
    public Sprite[] cardsSprites;
    public List<Sprite> cards = new List<Sprite>();

    private bool firstGuess, secondGuess;
    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;

    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private void Awake()
    {
        cardsSprites = Resources.LoadAll<Sprite>("YoGiOhCards");
    }
    private void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzles();
        gameGuesses = cards.Count / 2;
    }

    void GetButtons()
    {
        GameObject[] BtnsObjects = GameObject.FindGameObjectsWithTag("PuzzleBtn");

        for (int i = 0; i < BtnsObjects.Length; i++)
        {
            btns.Add(BtnsObjects[i].GetComponent<Button>());
            btns[i].image.sprite = bgBtn;
            BtnsObjects[i].name = i.ToString();
        }
    }

    void AddGamePuzzles()
    {
        int looper = btns.Count;
        int index = 0;
        int pairsNeeded = looper / 2;

        for (int i = 0; i < looper; i++)
        {
            if (index == pairsNeeded) index = 0;
            cards.Add(cardsSprites[index]);
            index++;
        }

        // Shuffle the cards list
        for (int i = 0; i < cards.Count; i++)
        {
            Sprite temp = cards[i];
            int randomIndex = Random.Range(i, cards.Count);
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }
    void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickAPuzzle());
        }
    }

    public void PickAPuzzle()
    {
        if (!firstGuess)
        {
            firstGuess = true;

            firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

            firstGuessPuzzle = cards[firstGuessIndex].name;

            btns[firstGuessIndex].image.sprite = cards[firstGuessIndex];



        }
        else if (!secondGuess)
        {
            secondGuess = true;

            secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

            secondGuessPuzzle = cards[secondGuessIndex].name;



            btns[secondGuessIndex].image.sprite = cards[secondGuessIndex];
            countGuesses++;
            StartCoroutine(CheckIfMatch());

        }
    }

    IEnumerator CheckIfMatch()
    {
        yield return new WaitForSeconds(1);

        if (firstGuessPuzzle == secondGuessPuzzle)
        {
            yield return new WaitForSeconds(.5f);
            btns[firstGuessIndex].image.color = new Color(0,0,0,0);
            btns[secondGuessIndex].image.color = new Color(0,0,0,0);




            CheckIfGameFinished();
        }else
        {
            yield return new WaitForSeconds(.5f);


            btns[firstGuessIndex].image.sprite = bgBtn;
            btns[secondGuessIndex].image.sprite = bgBtn;
        }

        yield return new WaitForSeconds(.5f);

        firstGuess = secondGuess = false;
    }


    private void CheckIfGameFinished()
    {
        countCorrectGuesses++;
        if (countCorrectGuesses == gameGuesses)
        {
            // game finished
        }
    }
}
