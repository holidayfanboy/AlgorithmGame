using System.Collections.Generic;
using UnityEngine;
using System.Linq; 

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject cardPrefab;   // assign your Card prefab (must have Card.cs)
    [SerializeField] private Transform cardParent;    // UI container / parent for cards
    [SerializeField] GameObject player;
    public int startingCardCount = 4;

    public List<int> numbers = new List<int>();      // values for current round (shuffled)
    public List<Card> spawnedCards = new List<Card>(); // spawned Card instances

    void Start()
    {
        // start first round
        ShuffleAndSpawn(startingCardCount);
    }

    // Public: clear existing cards, generate new numbers, spawn them
    public void ShuffleAndSpawn(int count)
    {
        ClearCards();
        GenerateNumbers(count);
        SpawnCards();
    }

    // Destroy current card gameobjects and clear lists
    public void ClearCards()
    {
        // destroy all spawned card GameObjects
        for (int i = 0; i < spawnedCards.Count; i++)
        {
            if (spawnedCards[i] != null)
                Destroy(spawnedCards[i].gameObject);
        }

        spawnedCards.Clear();
        numbers.Clear();
    }

    // Create a list of unique numbers and shuffle them (1..count)
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

    // Instantiate card prefabs and set their values, then add to spawnedCards
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
        }
        else
        {
            Debug.LogWarning("Clicked card not found in the list!");
        }
    }
}
