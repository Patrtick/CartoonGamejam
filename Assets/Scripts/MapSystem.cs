using UnityEngine;

public class MapSystem : MonoBehaviour
{
    [SerializeField] private GameObject[] maps;

    private void Start()
    {
        ActivateSelectedMap();
    }

    private void ActivateSelectedMap()
    {
        for (int i = 0; i < maps.Length; i++)
        {
            maps[i].SetActive(
                i == GameSettings.SelectedMap
            );
        }
    }
}