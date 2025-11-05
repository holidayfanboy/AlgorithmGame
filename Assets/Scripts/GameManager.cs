using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;
    [SerializeField] private Transform cardParent;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private SkillLayout skillLayout;

    public int startingCardCount = 4;
    public bool isActivatingSkills = false;

    public List<int> numbers = new List<int>();
    public List<Card> spawnedCards = new List<Card>();
    [SerializeField] private int firstCardIndex;
    [SerializeField] private int secondCardIndex;
    [SerializeField] private float cardSwapDuration = 0.05f; 
    [SerializeField] private int Stage;
    [SerializeField] private int MaxStage = 5;
    public bool isSwapping = false;
    public int timePlayed;
    public TMP_Text goldText;
    public TMP_Text playerHealthText;


    void Awake()
    {
        // start first round
        UpdateGold(0);
        UpdateHealth(0);
        firstCardIndex = -1;
        secondCardIndex = -1;
        ShuffleAndSpawn(startingCardCount);
        if (playerData == null)
        {
            Debug.LogError("PlayerData ScriptableObject not assigned to GameManager!");
        }
    }

    public void UpdateGold(int amount)
    {
        playerData.IncreaseGoldAmount(amount);
        goldText.text = playerData.getGoldAmount().ToString();
    }

    public void UpdateHealth(int amount)
    {
        playerData.IncreasePlayerHealth(amount);
        playerHealthText.text = playerData.getPlayerHealth().ToString();
    }
    public void ShuffleAndSpawn(int count)
    {
        ClearCards();
        GenerateNumbers(count);
        SpawnCards();
    }

    public void ClearCards()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
                Destroy(spawnedCards[i].gameObject);
        }

        spawnedCards.Clear();
        numbers.Clear();
    }

    void GenerateNumbers(int count)
    {
        numbers = Enumerable.Range(1, Mathf.Max(1, count)).ToList();
        Shuffle(numbers);
    }

    // Fisher-Yates shuffle
    void Shuffle<T>(List<T> list)
    {
        System.Random rng = new System.Random();
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }
    }

    void SpawnCards()
    {
        for (int i = 0; i < numbers.Count; i++)
        {
            if (cardPrefab == null || cardParent == null)
            {
                Debug.LogError("GameManager: cardPrefab or cardParent is not assigned.");
                return;
            }

            GameObject go = Instantiate(cardPrefab, cardParent);
            Card card = go.GetComponent<Card>();
            if (card == null)
            {
                Debug.LogError("Card prefab is missing the Card component.");
                Destroy(go);
                continue;
            }

            card.SetValue(numbers[i]);
            spawnedCards.Add(card);
        }

        // After spawning cards, start skill activation sequence
        if (skillLayout != null)
        {
            Debug.Log("Activating Skill after spawning cards");
            isActivatingSkills = true;
            skillLayout.ActivateSkill();
            isActivatingSkills = false;
        }
        else
        {
            Debug.LogWarning("SkillLayout not assigned to GameManager");
        }
    }

    public void OnItemClicked(GameObject selectedCardObject)
    {
        if (isSwapping || isActivatingSkills) // ignore clicks while swapping or activating skills
        {
            Debug.Log(isSwapping ? "Swap in progress - input ignored" : "Skills activating - input ignored");
            return;
        }
        Card selectedCard = selectedCardObject.GetComponent<Card>();
        if (selectedCard == null)
        {
            Debug.LogError("Selected object does not have a Card component");
            return;
        }

        int index = spawnedCards.IndexOf(selectedCard);
        if (index != -1)
        {
            Debug.Log("Clicked item at index: " + index);

            // If no first card selected -> make this the first and highlight range
            if (firstCardIndex == -1)
            {
                // Move the card up immediately when selected
                Vector3 pos = selectedCard.transform.localPosition;
                pos.y += 35f; // changed from 20f
                selectedCard.transform.localPosition = pos;

                firstCardIndex = index;
                Debug.Log("First Card Selected at index: " + firstCardIndex);

                // Highlight selectable range based on PlayerHandRange
                UpdateSelectableRange(firstCardIndex);
            }
            else
            {
                // If the player clicked the same card again, ignore
                if (index == firstCardIndex)
                    return;

                secondCardIndex = index;
                Debug.Log("Second Card Selected at index: " + secondCardIndex);

                // Move second card up immediately when selected
                Vector3 pos = selectedCard.transform.localPosition;
                pos.y += 35f; // changed from 20f
                selectedCard.transform.localPosition = pos;

                int range = playerData.getPlayerHand();
                if (range >= Math.Abs(firstCardIndex - secondCardIndex))
                {
                    Debug.Log(range);
                    Debug.Log(Math.Abs(firstCardIndex - secondCardIndex));
                    SwapCard(firstCardIndex, secondCardIndex);
                }
                else
                {
                    // invalid selection -> reset previous first card position & color
                    Card firstCard = spawnedCards[firstCardIndex];
                    Vector3 firstPos = firstCard.transform.localPosition;
                    firstPos.y -= 35f; // changed from 20f
                    firstCard.transform.localPosition = firstPos;
                    firstCard.ChangeColor(Color.red);

                    // Make the clicked card the new first card and re-highlight range
                    firstCardIndex = secondCardIndex;
                    secondCardIndex = -1;
                    UpdateSelectableRange(firstCardIndex);
                }
            }
        }
        else
        {
            Debug.LogWarning("Clicked card not found in the list!");
        }
    }


    public void SwapCard(int first, int second)
    {
        if (first < 0 || second < 0 || first >= spawnedCards.Count || second >= spawnedCards.Count)
        {
            Debug.LogError("Invalid card indices for swap");
            return;
        }

        if (isSwapping) return;
        isSwapping = true;

        spawnedCards[first].ChangeColor(Color.red);
        spawnedCards[second].ChangeColor(Color.red);

        StartCoroutine(AnimateCardSwap(first, second));
    }

    // Highlight cards that are within player hand range from the given first index.
    private void UpdateSelectableRange(int firstIndex)
    {
        if (firstIndex < 0 || firstIndex >= spawnedCards.Count) return;
        int range = playerData != null ? playerData.getPlayerHand() : 0;

        for (int i = 0; i < spawnedCards.Count; i++)
        {
            Card c = spawnedCards[i];
            if (c == null) continue;

            if (i == firstIndex)
            {
                c.ChangeColor(Color.red);
            }
            else if (Math.Abs(firstIndex - i) <= range)
            {
                // selectable cards -> blue
                c.ChangeColor(Color.blue);
            }
            else
            {
                // not selectable -> red
                c.ChangeColor(Color.red);
            }
        }
    }

    // Reset colors to red when clearing selection (call after swap)
    private void ResetAllCardColorsToRed()
    {
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
                spawnedCards[i].ChangeColor(Color.red);
        }
    }

    private IEnumerator AnimateCardSwap(int firstIndex, int secondIndex)
    {
        Transform firstTransform = spawnedCards[firstIndex].transform;
        Transform secondTransform = spawnedCards[secondIndex].transform;

        Vector3 firstStartPos = firstTransform.localPosition;
        Vector3 secondStartPos = secondTransform.localPosition;
        int firstSiblingIndex = firstTransform.GetSiblingIndex();
        int secondSiblingIndex = secondTransform.GetSiblingIndex();

        float elapsedTime = 0;
        while (elapsedTime < cardSwapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / cardSwapDuration);

            float smoothT = t * t * (3f - 2f * t);

            firstTransform.localPosition = Vector3.Lerp(firstStartPos, secondStartPos, smoothT);
            secondTransform.localPosition = Vector3.Lerp(secondStartPos, firstStartPos, smoothT);

            yield return null;
        }

        firstTransform.localPosition = secondStartPos;
        secondTransform.localPosition = firstStartPos;

        firstTransform.SetSiblingIndex(secondSiblingIndex);
        secondTransform.SetSiblingIndex(firstSiblingIndex);

        // swap the card references after animation
        Card temp = spawnedCards[firstIndex];
        spawnedCards[firstIndex] = spawnedCards[secondIndex];
        spawnedCards[secondIndex] = temp;

        // mark swap finished so input is allowed again
        isSwapping = false;

        firstCardIndex = -1;
        secondCardIndex = -1;

        // after swap, clear selectable highlights
        ResetAllCardColorsToRed();

        CheckForWin();
    }

    private void CheckForWin()
    {
        bool isSorted = true;
        for (int i = 0; i < spawnedCards.Count - 1; i++)
        {
            if (spawnedCards[i].Value > spawnedCards[i + 1].Value)
            {
                isSorted = false;
                break;
            }
        }

        if (isSorted)
        {
            Debug.Log("You Win!");
            if (Stage >= MaxStage)
            {
                UpdateGold(30);
                Debug.Log("Game Completed! Maximum Stage Reached.");
                return;
            }
            else
            {
                UpdateGold(10);
                Stage++;
                Debug.Log("Advancing to Stage: " + Stage);
                ShuffleAndSpawn(startingCardCount + Stage - 1);
            }
        }
    }
}
