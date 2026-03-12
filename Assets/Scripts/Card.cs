using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    public Sprite hiddenIconSprite;
    public Sprite iconSprite;

    public bool isSelected;
    public bool isMatched;

    public GameManager controller;


    private void Awake()
    {
        iconImage.sprite = hiddenIconSprite;
    }
    public void SetIconSprite(Sprite sp)
    {
        iconSprite = sp;
    }

    public void OnCardClick()
    {
        controller.SetSelected(this);
    }

    public void Show()
    {
        StartCoroutine(FlipTo(iconSprite, true));
    }

    public void Hide()
    {
        StartCoroutine(FlipTo(hiddenIconSprite, false));
    }

    private IEnumerator FlipTo(Sprite targetSprite, bool selected)
    {
        float duration = 0.1f;
        float elapsed = 0f;

        // scale down on X
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(1f, 0f, elapsed / duration);
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        // swap sprite at midpoint
        iconImage.sprite = targetSprite;
        isSelected = selected;

        elapsed = 0f;

        // scale back up on X
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float scale = Mathf.Lerp(0f, 1f, elapsed / duration);
            transform.localScale = new Vector3(scale, 1f, 1f);
            yield return null;
        }

        transform.localScale = Vector3.one;
    }
}