#if UNITY_EDITOR

using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class RuntimeBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void CreateGameManagerAndPlayer()
    {
        if (GameManager.Instance != null || SceneManager.GetActiveScene().name == "MainMenu") return;

        const string GameManagerPrefabPath = "Assets/Prefabs/GameManager.prefab";
        const string PlayerPrefabPath = "Assets/Prefabs/Player.prefab";

        GameObject gameManagerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(GameManagerPrefabPath);
        if (gameManagerPrefab == null)
        {
            Debug.LogError($"GameManager prefab not found:\n{GameManagerPrefabPath}");
            return;
        }
        Object.Instantiate(gameManagerPrefab);

        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        if (playerPrefab == null)
        {
            Debug.LogError($"Player prefab not found:\n{PlayerPrefabPath}");
            return;
        }
        Object.Instantiate(playerPrefab);

        Debug.LogWarning("GAME NOT LOADED FROM MAIN MENU. SAVING WILL NOT WORK! CHANGING SCENES WILL NOT WORK! LOADED DEFAULT DIFFICULTY: MEDIUM");
    }
}

#endif