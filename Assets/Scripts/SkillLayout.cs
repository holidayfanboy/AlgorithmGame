using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

public class SkillLayout : MonoBehaviour
{
	[Header("Skill Objects (max 5)")]
	[Tooltip("Assign up to 5 child GameObjects that each contain a skill script (e.g., FindMinimum). If left empty, first 5 children will be used.")]
	[SerializeField] private List<GameObject> skillObjects = new List<GameObject>();

	// Collected scripts that expose a public bool isActive (property or field)
	[SerializeField] private List<MonoBehaviour> scriptList = new List<MonoBehaviour>();

	// Tracks which script to activate next
	private int currentScriptIndex = 0;

	// Flag to prevent re-initialization from resetting activated skills
	private bool isInitialized = false;
	[SerializeField] private PlayerData playerData;
    [SerializeField] private GameManager gameManager; // Add reference to GameManager
    private bool isActivatingSkills = false;

    private void Awake()
	{
		// Only build lists once in Awake to prevent resetting active skills
		if (!isInitialized)
		{
			BuildLists();
			isInitialized = true;
		}
		
		// Find GameManager if not assigned
		if (gameManager == null)
		{
			gameManager = FindObjectOfType<GameManager>();
		}
	}

	// Build the skill object list (if empty, use up to first 5 children) and collect scripts (searching recursively)
	private void BuildLists()
	{
		// If list not assigned in Inspector, use first 5 children
		if (skillObjects == null)
		{
			skillObjects = new List<GameObject>();
		}

		// First priority: Load owned skills from PlayerData
		if (playerData != null)
		{
			List<GameObject> ownedSkills = playerData.GetOwnedSkills();
			if (ownedSkills != null && ownedSkills.Count > 0)
			{
				skillObjects.Clear(); // Clear existing to use PlayerData skills
				
				// Add up to 5 owned skills
				int skillsToAdd = Mathf.Min(ownedSkills.Count, 5);
				for (int i = 0; i < skillsToAdd; i++)
				{
					if (ownedSkills[i] != null)
					{
						// Instantiate the skill prefab as a child of this transform
						GameObject skillInstance = Instantiate(ownedSkills[i], transform);
						skillObjects.Add(skillInstance);
					}
				}
				
				Debug.Log($"SkillLayout: Loaded {skillObjects.Count} skills from PlayerData.");
			}
		}

		// Fallback: If still empty, use first 5 children
		if (skillObjects.Count == 0)
		{
			int childCount = Mathf.Min(transform.childCount, 5);
			for (int i = 0; i < childCount; i++)
			{
				var child = transform.GetChild(i);
				if (child != null)
				{
					skillObjects.Add(child.gameObject);
				}
			}
		}

		// Enforce maximum of 5
		if (skillObjects.Count > 5)
		{
			skillObjects.RemoveRange(5, skillObjects.Count - 5);
		}

		// Collect scripts exposing isActive from each skill object and ALL of its descendants
		scriptList.Clear();
		var seen = new HashSet<object>();
		foreach (var go in skillObjects)
		{
			if (go == null) continue;

			// Include inactive children too; skills might be disabled until activated
			var behaviours = go.GetComponentsInChildren<MonoBehaviour>(true);
			foreach (var b in behaviours)
			{
				if (b == null) continue;
				if (seen.Contains(b)) continue; // avoid duplicates
				if (HasIsActiveMember(b))
				{
					scriptList.Add(b);
					seen.Add(b);
					// Ensure default inactive
					SetIsActive(b, false);
				}
			}
		}

		currentScriptIndex = 0;

		Debug.Log($"SkillLayout initialized: {skillObjects.Count} objects, {scriptList.Count} skill scripts collected.");
	}

	// Public API: activates ALL collected skill scripts' isActive = true with 0.5s delay between each
    public void ActivateSkill()
    {
        // Ensure initialization before first use
        if (!isInitialized)
        {
            BuildLists();
            isInitialized = true;
        }

        // If nothing collected, warn and return
        if (scriptList.Count == 0)
        {
            Debug.LogWarning("SkillLayout: No skill scripts found to activate.");
            
            // Reset isActivatingSkills flag
            isActivatingSkills = false;
            if (gameManager != null)
            {
                gameManager.isActivatingSkills = false;
                Debug.Log("SkillLayout: Input re-enabled (no skills to activate).");
            }
            
            return;
        }

        // Start coroutine to activate scripts with delay
        StartCoroutine(ActivateSkillsWithDelay());
    }

    // Coroutine to activate all scripts with 0.5 second delay between each
    private IEnumerator ActivateSkillsWithDelay()
    {
        // Disable input during skill activation
        isActivatingSkills = true;
        if (gameManager != null)
        {
            gameManager.isActivatingSkills = true;
            Debug.Log("SkillLayout: Input disabled during skill activation.");
        }

        int activatedCount = 0;
        for (int i = 0; i < scriptList.Count; i++)
        {
            var script = scriptList[i];
            if (script == null)
            {
                Debug.LogWarning($"SkillLayout: Script at index {i} is null. Skipping.");
                continue;
            }

            SetIsActive(script, true);
            Debug.Log($"SkillLayout: Activated skill {i + 1}/{scriptList.Count} - {script.GetType().Name}");
            activatedCount++;

            // Wait 1.5 seconds before activating the next script
            yield return new WaitForSeconds(2f);
        }

        Debug.Log($"SkillLayout: Activated {activatedCount} total skills.");
        currentScriptIndex = scriptList.Count;
        
        // Re-enable input after all skills are activated
        isActivatingSkills = false;
        if (gameManager != null)
        {
            gameManager.isActivatingSkills = false;
            Debug.Log("SkillLayout: Input re-enabled after skill activation.");
            
            // Call CheckForWin after all skills are activated
            gameManager.CheckForWin();
            Debug.Log("SkillLayout: Called CheckForWin after skill activation.");
        }
    }

	// Optional: allow manual refresh if children are spawned later at runtime
	public void RefreshSkills()
	{
		isInitialized = false; 
		BuildLists();
		isInitialized = true;
	}

	// Helper: does the component expose a public bool isActive (property or field)?
	private static bool HasIsActiveMember(MonoBehaviour component)
	{
		var type = component.GetType();

		// Prefer property (like in FindMinimum)
		var prop = type.GetProperty("isActive", BindingFlags.Public | BindingFlags.Instance);
		if (prop != null && prop.CanWrite && prop.PropertyType == typeof(bool))
		{
			return true;
		}

		// Fallback to public field
		var field = type.GetField("isActive", BindingFlags.Public | BindingFlags.Instance);
		if (field != null && field.FieldType == typeof(bool))
		{
			return true;
		}

		return false;
	}

	// Helper: set isActive=true/false via property or field
	private static void SetIsActive(MonoBehaviour component, bool value)
	{
		var type = component.GetType();

		var prop = type.GetProperty("isActive", BindingFlags.Public | BindingFlags.Instance);
		if (prop != null && prop.CanWrite && prop.PropertyType == typeof(bool))
		{
			prop.SetValue(component, value, null);
			return;
		}

		var field = type.GetField("isActive", BindingFlags.Public | BindingFlags.Instance);
		if (field != null && field.FieldType == typeof(bool))
		{
            field.SetValue(component, value);
            Debug.Log($"SkillLayout: Set isActive={value} on {type.Name} via field.");
			return;
		}

		Debug.LogWarning($"SkillLayout: Component {type.Name} has no public bool isActive to set.");
	}
}
