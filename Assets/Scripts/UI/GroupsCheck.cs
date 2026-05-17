using UnityEngine;
using UnityEngine.UI;

public class GroupsCheck : MonoBehaviour
{
    [SerializeField] private Button targetButton;

    private bool group1Ready;
    private bool group2Ready;

    public void PressGroup1Button()
    {
        group1Ready = true;
        Check();
    }

    public void PressGroup2Button()
    {
        group2Ready = true;
        Check();
    }

    private void Check()
    {
        targetButton.interactable = group1Ready && group2Ready;
    }

    public void ResetGroups()
    {
        group1Ready = false;
        group2Ready = false;
        targetButton.interactable = false;
    }
}