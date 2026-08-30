using UnityEngine;

/// <summary>
/// Manages the overall game state and timing.
/// Holds variables that don't fit anywhere else.
/// Remember to not directly reference GameManager GameObject in scenes, but instead use GameManager.Instance to access the singleton instance.
/// </summary>
public class GameManager : Singleton<GameManager>
{
    public float playTime; // Total playtime in seconds
    public float lastSaved;
    public int difficulty; // current difficulty level
    public int saveSlot; // As in: current save slot
    // Certain options are stored here because:
    // For example FOV must be applied to player camera, but it does not exist in main menu, so the player needs to get it when he gets created.
    [HideInInspector] public float fov;
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        playTime += Time.deltaTime;
        lastSaved += Time.deltaTime; // SaveManager.Save() and SaveManager.LoadGame() resets this when it saves, so we can use this to determine when to save next
    }
}
