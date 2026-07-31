using UnityEngine;

public class PlayerGeneral : Singleton<PlayerGeneral> //TO DO: rename? Change architecture? Integrate to PauseMenu?
{
    public GameObject hud;
    public void BackToCheckpoint() //TO DO: move this somewhere else?
    {
        Time.timeScale = 1f;
        SaveManager.Instance.LoadGame();
    }
}
