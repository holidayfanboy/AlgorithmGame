using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;  
    [SerializeField] private Transform cardParent;   
    [SerializeField] GameObject Player;
    private PlayerData playerDataScript;
    public int startingCardCount = 4;

    public List<int> numbers = new List<int>();      
    public List<Card> spawnedCards = new List<Card>();
    [SerializeField] private int firstCardIndex = -1;
    [SerializeField] private int secondCardIndex = -1;
    [SerializeField] private float cardSwapDuration = 0.5f; 


    void Awake()
    {
        // start first round
        ShuffleAndSpawn(startingCardCount);
        playerDataScript = Player.GetComponent<PlayerData>();
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
    }

    public void OnItemClicked(GameObject selectedCardObject)
    {
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
            if (firstCardIndex == -1)
            {
                firstCardIndex = index;
                Debug.Log("First Card Selected at index: " + firstCardIndex);
            }
            else
            {
                secondCardIndex = index;
                Debug.Log("Second Card Selected at index: " + secondCardIndex);
                int range = playerDataScript.getPlayerHand();
                if (range >= Math.Abs(firstCardIndex - secondCardIndex))
                {
                    Debug.Log(range);
                    Debug.Log(Math.Abs(firstCardIndex - secondCardIndex));
                    SwapCard(firstCardIndex, secondCardIndex);
                }
                else
                {
                    firstCardIndex = secondCardIndex;
                    secondCardIndex = -1;
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

        StartCoroutine(AnimateCardSwap(first, second));
        
        Card temp = spawnedCards[first];
        spawnedCards[first] = spawnedCards[second];
        spawnedCards[second] = temp;
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
            float t = elapsedTime / cardSwapDuration;
            
            float smoothT = t * t * (3f - 2f * t);
            
            firstTransform.localPosition = Vector3.Lerp(firstStartPos, secondStartPos, smoothT);
            secondTransform.localPosition = Vector3.Lerp(secondStartPos, firstStartPos, smoothT);
            
            yield return null;
        }

        firstTransform.localPosition = secondStartPos;
        secondTransform.localPosition = firstStartPos;
        
        firstTransform.SetSiblingIndex(secondSiblingIndex);
        secondTransform.SetSiblingIndex(firstSiblingIndex);

        firstCardIndex = -1;
        secondCardIndex = -1;
    }
}
