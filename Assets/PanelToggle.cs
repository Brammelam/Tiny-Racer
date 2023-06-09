using UnityEngine;

public class PanelToggle : MonoBehaviour
{
    public Animator[] panelAnimators;
    public bool defaultState = false;

    private bool isOpen;

    private void Start()
    {
        // Initialize the initial state of the panels
        isOpen = defaultState;
        SetPanelState();
    }

    public void TogglePanels()
    {
        // Toggle the state of the panels
        isOpen = !isOpen;

        // Play the appropriate animation for each panel
        foreach (Animator animator in panelAnimators)
        {
            if (isOpen)
            {
                animator.SetBool("Open", true);
                animator.Play("menuopen", -1, 0f);
            }
            else
            {
                animator.SetBool("Open", false);
                animator.Play("menuclose", -1, 0f);
            }
        }
    }

    private void SetPanelState()
    {
        // Set the initial state of the panels based on the isOpen flag
        foreach (Animator animator in panelAnimators)
        {
            animator.SetBool("Open", isOpen);
        }
    }
}
