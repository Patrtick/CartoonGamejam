using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void StartVsBot()
    {
        GameSettings.CurrentMode = GameMode.VsBot;
    }

    public void StartVsPlayer()
    {
        GameSettings.CurrentMode = GameMode.VsPlayer;
    }

    public void SelectPlayer1Character(int characterIndex)
    {
        GameSettings.Player1Character = characterIndex;
    }

    public void SelectPlayer2Character(int characterIndex)
    {
        GameSettings.Player2Character = characterIndex;
    }
    public void SelectMap(int mapIndex)
    {
        GameSettings.SelectedMap = mapIndex;
    }
}