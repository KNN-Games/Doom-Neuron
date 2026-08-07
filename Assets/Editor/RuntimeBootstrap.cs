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
        if (GameManager.Instance == null)
        {
            GameObject gameManager = Object.Instantiate(gameManagerPrefab);
            gameManager.name = "GameManager";
            Debug.LogWarning("GAME MANAGER NOT FOUND. YOU MOST LIKELY DID NOT LOAD THE GAME FROM THE MAIN MENU.\nSAVING WILL NOT WORK! CHANGING SCENES WILL NOT WORK! LOADED DEFAULT DIFFICULTY: MEDIUM");
        }

        GameObject playerPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(PlayerPrefabPath);
        if (playerPrefab == null)
        {
            Debug.LogError($"Player prefab not found:\n{PlayerPrefabPath}");
            return;
        }
        if(PlayerController.Instance == null)
        {
            GameObject player = Object.Instantiate(playerPrefab);
            player.name = "Player";
            Debug.LogWarning("PLAYER NOT FOUND. YOU MOST LIKELY DID NOT LOAD THE GAME FROM THE MAIN MENU. \nLOADING DEFAULT PLAYER STATE");
        }

    }
}

#endif