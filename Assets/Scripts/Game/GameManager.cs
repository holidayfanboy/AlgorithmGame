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
    [SerializeField] private FirstStageData stageData;
    [SerializeField] private SkillLayout skillLayout;
    [SerializeField] private EnemyData enemyData;
    [SerializeField] private HorizontalLayoutNonUI horizontalLayout;
    [SerializeField] private TimerScript timerScript;
    [SerializeField] private PlayerScript playerScript;
    [SerializeField] private TMP_Text stageClearText;
    public int startingCardCount = 4;
    public bool isActivatingSkills = false;
    [SerializeField] private List<AudioClip> backgroundClip;
    [SerializeField] private List<AudioClip> cardClip;
    [SerializeField] private AudioClip stageclearClip;
    public List<int> numbers = new List<int>();
    public List<Card> spawnedCards = new List<Card>();
    [SerializeField] private int firstCardIndex;
    [SerializeField] private int secondCardIndex;
    [SerializeField] private float cardSwapDuration = 1f; 
    [SerializeField] private int Stage;
    public bool isSwapping = false;
    public int timePlayed;
    public TMP_Text goldText;
    public TMP_Text playerHealthText;

    public int stageClearGold = 10;
    public int gameClearGold = 30;
    
    private bool freeHandSwapActive = false;
    private int originalHandRange = 0;


    void Awake()
    {
        // start first round
        if (stageData != null)
            startingCardCount = stageData.GiveCardSize(); // ADDED: get from FirstStageData
        SoundData.PlaySoundFXClip(backgroundClip.ToArray(), transform.position, 0.13f);
        freeHandSwapActive = false;
        UpdateGold(0);
        UpdateHealth(0);
        firstCardIndex = -1;
        secondCardIndex = -1;
        ShuffleAndSpawn(startingCardCount); // unchanged call, now uses updated startingCardCount
        horizontalLayout.SpawnEnemy(startingCardCount);
        if (playerData == null)
        {
            Debug.LogError("PlayerData ScriptableObject not assigned to GameManager!");
        }
    }

    void Update()
    { 
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
        
        // Check if player health reached 0
        if (playerData.getPlayerHealth() <= 0)
        {
            GameOver();
        }
    }
    public void ShuffleAndSpawn(int count)
    {
        ClearCards();
        GenerateNumbers(count);
        SpawnCards();
        timerScript.StartTimer();
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
        numbers.Clear();

        int poolMax = Mathf.Max(1, count * 4);

        // If boss stage, allow duplicates for color sorting challenge
        if (stageData != null && stageData.isBoss)
        {
            Debug.Log("Boss stage: Generating numbers with possible duplicates");
            for (int i = 0; i < count; i++)
            {
                int value = UnityEngine.Random.Range(1, poolMax + 1);
                numbers.Add(value);
            }
        }
        else
        {
            // Normal stage: no duplicates
            HashSet<int> picked = new HashSet<int>();
            int safety = 0;
            while (picked.Count < count && safety < 100)
            {
                int candidate = UnityEngine.Random.Range(1, poolMax + 1);
                if (!picked.Contains(candidate))
                {
                    picked.Add(candidate);
                }
                else
                {
                    // already picked, try again
                }
                safety++;
            }

            numbers = picked.ToList();
        }

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
            
            // If boss stage, assign random color to each card
            if (stageData != null && stageData.isBoss)
            {
                int randomColor = UnityEngine.Random.Range(0, 3); // 0=Red, 1=Blue, 2=Green
                card.colorType = (Card.ColorType)randomColor;
                card.UpdateCardColor(); // Apply visual color change
                Debug.Log($"Boss stage: Card {i} assigned color {card.colorType}");
            }
            
            spawnedCards.Add(card);
        }

        // After spawning cards, start skill activation sequence
        if (skillLayout != null)
        {
            //Debug.Log("Activating Skill after spawning cards");
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
        if (isSwapping) // ignore clicks while swapping
        {
            //Debug.Log("Swap in progress - input ignored");
            return;
        }
        
        // Ignore clicks while skills are being activated
        if (isActivatingSkills)
        {
            Debug.Log("Skills activating - input ignored");
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
                //Debug.Log("First Card Selected at index: " + firstCardIndex);

                // Highlight selectable range based on PlayerHandRange
                UpdateSelectableRange(firstCardIndex);
            }
            else
            {
                // If the player clicked the same card again, ignore
                if (index == firstCardIndex)
                    return;

                secondCardIndex = index;
                //Debug.Log("Second Card Selected at index: " + secondCardIndex);

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
        horizontalLayout.EnemySwap(firstIndex, secondIndex);
        
        // Play card swap sound
        if (cardClip != null && cardClip.Count > 0)
        {
            SoundData.PlaySoundFXClip(cardClip.ToArray(), transform.position, 2.0f);
        }
        
        //Debug.Log($"[AnimateCardSwap] START - Swapping cards at indices {firstIndex} and {secondIndex}");
        //Debug.Log($"[AnimateCardSwap] Card {firstIndex} value: {spawnedCards[firstIndex].Value}, Card {secondIndex} value: {spawnedCards[secondIndex].Value}");
        
        Transform firstTransform = spawnedCards[firstIndex].transform;
        Transform secondTransform = spawnedCards[secondIndex].transform;

        Vector3 firstStartPos = firstTransform.localPosition;
        Vector3 secondStartPos = secondTransform.localPosition;
        //Debug.Log($"[AnimateCardSwap] First card start pos: {firstStartPos}, Second card start pos: {secondStartPos}");
        
        int firstSiblingIndex = firstTransform.GetSiblingIndex();
        int secondSiblingIndex = secondTransform.GetSiblingIndex();
        //Debug.Log($"[AnimateCardSwap] First sibling index: {firstSiblingIndex}, Second sibling index: {secondSiblingIndex}");

        //Debug.Log($"[AnimateCardSwap] Card swap duration: {cardSwapDuration} seconds");
        float elapsedTime = 0;
        int frameCount = 0;
        
        while (elapsedTime < cardSwapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / cardSwapDuration);

            float smoothT = t * t * (3f - 2f * t);

            firstTransform.localPosition = Vector3.Lerp(firstStartPos, secondStartPos, smoothT);
            secondTransform.localPosition = Vector3.Lerp(secondStartPos, firstStartPos, smoothT);

            frameCount++;
            yield return null;
        }
        
        //Debug.Log($"[AnimateCardSwap] Animation completed in {frameCount} frames over {elapsedTime} seconds");

        firstTransform.localPosition = secondStartPos;
        secondTransform.localPosition = firstStartPos;
        //Debug.Log($"[AnimateCardSwap] Final positions set - First: {firstTransform.localPosition}, Second: {secondTransform.localPosition}");

        firstTransform.SetSiblingIndex(secondSiblingIndex);
        secondTransform.SetSiblingIndex(firstSiblingIndex);
        //Debug.Log($"[AnimateCardSwap] Sibling indices swapped");

        // swap the card references after animation
        Card temp = spawnedCards[firstIndex];
        spawnedCards[firstIndex] = spawnedCards[secondIndex];
        spawnedCards[secondIndex] = temp;
        //Debug.Log($"[AnimateCardSwap] List references swapped - Index {firstIndex} now has value {spawnedCards[firstIndex].Value}, Index {secondIndex} now has value {spawnedCards[secondIndex].Value}");

        // mark swap finished so input is allowed again
        isSwapping = false;
        firstCardIndex = -1;
        secondCardIndex = -1;
        
        // If free hand swap was active, restore original hand range after this swap
        if (freeHandSwapActive)
        {
            playerData.SetPlayerHandRange(originalHandRange);
            freeHandSwapActive = false;
            //Debug.Log($"[AnimateCardSwap] Free hand swap used. Restored hand range to {originalHandRange}");
        }
        
        // after swap, clear selectable highlights
        ResetAllCardColorsToRed();

        //Debug.Log($"[AnimateCardSwap] END - Calling CheckForWin");
        CheckForWin();
    }
    
    // Activate free hand swap - temporarily sets hand range to 10 for one swap
    public void ActivateFreeHandSwap()
    {
        if (!freeHandSwapActive)
        {
            originalHandRange = playerData.getPlayerHand();
            playerData.SetPlayerHandRange(10);
            freeHandSwapActive = true;
            //Debug.Log($"[FreeHandSwap] Activated. Original range: {originalHandRange}, New range: 10");
        }
    }

    public void FailedtoComplete()
    {
        //Debug.Log("GameManager: Player failed to complete the stage in time.");
        if (Stage >= stageData.MaxStage)
        {
            Debug.Log("Round Completed! Maximum Stage Reached.");
            StartCoroutine(AttackThenContinue(true));
            return;
        }
        else
        {
            Stage++;
            if (stageData != null)
            startingCardCount = stageData.GiveCardSize();
            StartCoroutine(AttackThenContinue());
        }
       
    }

    public void CheckForWin()
    {
        bool isSorted = true;
        
        // Check number sorting
        for (int i = 0; i < spawnedCards.Count - 1; i++)
        {
            if (spawnedCards[i].Value > spawnedCards[i + 1].Value)
            {
                isSorted = false;
                break;
            }
        }
        
        // If boss stage, also check color sorting (Red -> Blue -> Green)
        if (isSorted && stageData != null && stageData.isBoss)
        {
            Debug.Log("Boss stage: Checking color sorting (Red -> Blue -> Green)");
            
            for (int i = 0; i < spawnedCards.Count - 1; i++)
            {
                Card currentCard = spawnedCards[i];
                Card nextCard = spawnedCards[i + 1];
                
                // If values are equal, check color order
                if (currentCard.Value == nextCard.Value)
                {
                    // Red < Blue < Green
                    if (currentCard.colorType > nextCard.colorType)
                    {
                        isSorted = false;
                        Debug.Log($"Color sorting failed at index {i}: {currentCard.colorType} should come before {nextCard.colorType}");
                        break;
                    }
                }
            }
        }

        if (isSorted)
        {
            //Debug.Log("You Win!");
            if (Stage >= stageData.MaxStage)
            {
                UpdateGold(gameClearGold);
                Debug.Log("Round Completed! Maximum Stage Reached.");
                SoundData.PlaySoundFXClip(stageclearClip, transform.position, 0.5f);
                StartCoroutine(AttackThenAdvance(true));
                return;
            }
            else
            {
                UpdateGold(stageClearGold);
                Stage++;
                //Debug.Log("Advancing to Stage: " + Stage);

                if (stageData != null)
                    startingCardCount = stageData.GiveCardSize();
                    
                StartCoroutine(AttackThenAdvance());
            }
        }
    }

    public void SkillSwapCard(int first, int second)
    {
        if (first < 0 || second < 0 || first >= spawnedCards.Count || second >= spawnedCards.Count)
        {
            Debug.LogError("Invalid card indices for swap");
            return;
        }

        if (isSwapping) return;
        isSwapping = true;

        StartCoroutine(SkillAnimateCardSwap(first, second));
    }

    public IEnumerator SkillAnimateCardSwap(int first, int second)
    {
        // Play card swap sound
        if (cardClip != null)
        {
            SoundData.PlaySoundFXClip(cardClip.ToArray(), transform.position, 2.0f);
        }
        
        //Debug.Log($"[SkillAnimateCardSwap] START - Swapping cards at indices {first} and {second}");
        
        // Wait 0.5 seconds before capturing positions to ensure layout has stabilized
        yield return new WaitForSeconds(0.5f);
        //Debug.Log("[SkillAnimateCardSwap] Waited 0.5 seconds for layout to stabilize");
        
        Transform firstTransform = spawnedCards[first].transform;
        Transform secondTransform = spawnedCards[second].transform;

        // Get layout group reference
        UnityEngine.UI.LayoutGroup layoutGroup = cardParent.GetComponent<UnityEngine.UI.LayoutGroup>();
        
        // CAPTURE POSITIONS BEFORE DISABLING LAYOUT
        Vector3 firstStartPos = firstTransform.localPosition;
        Vector3 secondStartPos = secondTransform.localPosition;
        
        //Debug.Log($"[SkillAnimateCardSwap] Positions captured BEFORE disabling layout - First: {firstStartPos}, Second: {secondStartPos}");

        // Now disable layout group
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
            Debug.Log("[SkillAnimateCardSwap] Layout group disabled");
        }

        // Wait one frame for layout to disable
        yield return null;

        // Restore the captured positions manually
        firstTransform.localPosition = firstStartPos;
        secondTransform.localPosition = secondStartPos;
        
        //Debug.Log($"[SkillAnimateCardSwap] Positions restored after layout disable - First: {firstTransform.localPosition}, Second: {secondTransform.localPosition}");
        
        int firstSiblingIndex = firstTransform.GetSiblingIndex();
        int secondSiblingIndex = secondTransform.GetSiblingIndex();

        float elapsedTime = 0;
        
        // Animate the swap
        while (elapsedTime < cardSwapDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / cardSwapDuration);
            float smoothT = t * t * (3f - 2f * t);

            firstTransform.localPosition = Vector3.Lerp(firstStartPos, secondStartPos, smoothT);
            secondTransform.localPosition = Vector3.Lerp(secondStartPos, firstStartPos, smoothT);

            yield return null;
        }

        // Ensure final positions are exact
        firstTransform.localPosition = secondStartPos;
        secondTransform.localPosition = firstStartPos;

        // Swap sibling indices
        firstTransform.SetSiblingIndex(secondSiblingIndex);
        secondTransform.SetSiblingIndex(firstSiblingIndex);

        // Swap references in the list
        Card temp = spawnedCards[first];
        spawnedCards[first] = spawnedCards[second];
        spawnedCards[second] = temp;
        
        //Debug.Log($"[SkillAnimateCardSwap] Swap complete - Index {first} now has value {spawnedCards[first].Value}, Index {second} now has value {spawnedCards[second].Value}");

        // Re-enable layout group
        if (layoutGroup != null)
        {
            layoutGroup.enabled = true;
            //Debug.Log("[SkillAnimateCardSwap] Layout group re-enabled");
        }

        // Mark swap finished
        isSwapping = false;
    }

    public void DeleteCard(int index)
    {
        if (index < 0 || index >= spawnedCards.Count)
        {
            Debug.LogError($"DeleteCard: Invalid index {index}. Must be between 0 and {spawnedCards.Count - 1}");
            return;
        }

        Card cardToDelete = spawnedCards[index];
        //Debug.Log($"DeleteCard: Deleting card at index {index} with value {cardToDelete.Value}");

        // Remove from the list
        spawnedCards.RemoveAt(index);
        numbers.RemoveAt(index);

        // Destroy the GameObject
        Destroy(cardToDelete.gameObject);

        //Debug.Log($"DeleteCard: Card deleted. Remaining cards: {spawnedCards.Count}");

        // Re-enable layout group to automatically reposition remaining cards
        UnityEngine.UI.LayoutGroup layoutGroup = cardParent.GetComponent<UnityEngine.UI.LayoutGroup>();
        if (layoutGroup != null)
        {
            layoutGroup.enabled = false;
            layoutGroup.enabled = true;
            //Debug.Log("DeleteCard: Layout group refreshed to fill gap from right");
        }
    }
    //When Succeeded
    private IEnumerator AttackThenAdvance(bool goToShopAtEnd = false)
    {
        // Disable player input during attack sequence
        isSwapping = true;
        isActivatingSkills = true;
        
        stageData.SetStageSuccess();
        timerScript.StopTimer();
        
            playerScript.StartAttack();
            yield return StartCoroutine(playerScript.WaitForAttackAnimation());

        

        enemyData.ActivateDeath();

        yield return new WaitForSeconds(2f);
        

            playerScript.Idle();

        enemyData.DeactivateDeath();
        
        // If this was the final stage/round, go to shop after finishing the attack/death sequence
        if (goToShopAtEnd)
        {
            // Enable stage clear text
            if (stageClearText != null)
            {
                stageClearText.gameObject.SetActive(true);
            }
            
            // Wait 2 seconds before transitioning to shop
            yield return new WaitForSeconds(2f);
            
            stageData.SetStateToShop();
            yield break;
        }

        horizontalLayout.SpawnEnemy(startingCardCount);
        playerData.ActivateMove();
        yield return new WaitForSeconds(4f);
        ShuffleAndSpawn(startingCardCount);

        // Activate skills after advancing to next stage
        if (skillLayout != null)
        {
            //Debug.Log("Activating skills for new stage");
            isActivatingSkills = true;
            skillLayout.ActivateSkill();
            isActivatingSkills = false;
        }
        
        // Re-enable player input after sequence completes
        isSwapping = false;
    }
    //When Failed
    private IEnumerator AttackThenContinue(bool goToShopAtEnd = false)
    {
        // Disable player input during attack sequence
        isSwapping = true;
        isActivatingSkills = true;

        stageData.SetStageFail();
        timerScript.StopTimer();
        

            playerScript.StartAttack();
            yield return StartCoroutine(playerScript.WaitForAttackAnimation());
        


        enemyData.ActivateDeath();

        yield return new WaitForSeconds(2f);
        
            playerScript.Idle();

        enemyData.DeactivateDeath();
        
        // If this was the final stage/round, apply fail penalty and go to shop after sequence
        if (goToShopAtEnd)
        {
            UpdateHealth(-1);
            
            // Enable stage clear text
            if (stageClearText != null)
            {
                stageClearText.gameObject.SetActive(true);
            }
            
            // Wait 2 seconds before transitioning to shop
            yield return new WaitForSeconds(2f);
            
            stageData.SetStateToShop();
            yield break;
        }
        horizontalLayout.SpawnEnemy(startingCardCount);
        playerData.ActivateMove(); 
        yield return new WaitForSeconds(2f);
        ShuffleAndSpawn(startingCardCount);
        UpdateHealth(-1);
        
        if (skillLayout != null)
        {
            Debug.Log("Activating skills for new stage");
            isActivatingSkills = true;
            skillLayout.ActivateSkill();
            isActivatingSkills = false;
        }
        
        // Re-enable player input after sequence completes
        isSwapping = false;
    }
    
    private void GameOver()
    {
        Debug.Log("GameManager: GAME OVER - Player health reached 0");
        
        // Stop all running coroutines first
        StopAllCoroutines();
        
        // Start the game over sequence
        StartCoroutine(GameOverSequence());
    }
    
    private IEnumerator GameOverSequence()
    {
        // Stop the timer
        if (timerScript != null)
            timerScript.StopTimer();
        
        // Trigger player death animation
            playerScript.PlayerDead();

        
        // Disable input
        isSwapping = true; // Prevent any card swapping
        isActivatingSkills = true; // Prevent skill activation
        
        Debug.Log("GameOver: All functions stopped. Waiting for death animation before scene transition...");
        
        // Wait for death animation to complete
        yield return new WaitForSeconds(3f);
        
        // Change to game over scene
        Debug.Log("GameOver: Loading GameOverScene");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameOverScene");
    }
    
    public void ResetGame()
    {
        Debug.Log("GameManager: Resetting game to initial state");
        
        // // Stop all coroutines
        // StopAllCoroutines();
        
        // // Reset player data
        // if (playerData != null)
        // {
        //     playerData.ResetToDefaults();
        // }
        
        // // Reset stage counter and stage data
        // if (stageData != null)
        // {
        //     stageData.Stage = 0;
        //     stageData.stageSuccess = false;
        // }
        
        // // Reset flags
        // isSwapping = false;
        // isActivatingSkills = false;
        // freeHandSwapActive = false;
        // firstCardIndex = -1;
        // secondCardIndex = -1;
        
        // // Clear cards
        // ClearCards();
        
        // // Reset UI
        // UpdateGold(0);
        // UpdateHealth(0);
        
        // // Hide stage clear text if visible
        // if (stageClearText != null)
        // {
        //     stageClearText.gameObject.SetActive(false);
        // }
        
        // playerScript.Idle();


        // if (timerScript != null)
        // {
        //     timerScript.StopTimer();
        // }
        
        // // Restart from first stage
        // if (stageData != null)
        // {
        //     startingCardCount = stageData.GiveCardSize();
        // }
        
        // ShuffleAndSpawn(startingCardCount);
        // horizontalLayout.SpawnEnemy(startingCardCount);
        
        // Debug.Log("GameManager: Game reset complete");
    }
    
    public void QuitGame()
    {
        Debug.Log("GameManager: Quitting game");
        
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }
}
  