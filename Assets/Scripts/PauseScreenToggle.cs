using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreenToggle : MonoBehaviour
{
    bool enabled = false;
    public CanvasGroup group;
    private void Start()
    {
        DisableUI();
Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {

            enabled = !enabled; //If pause screen is enabled, disable it. Otherwise, enable it

            if (!enabled)
            {
                Time.timeScale = 1;
                DisableUI();
            } else
            {
                Time.timeScale = 0;
                EnableUI();
            }
        }
    }
    void DisableUI()
    {
        group.alpha = 0f; //Makes UI transparent
        group.blocksRaycasts = false; //Prevents UI from receiving input events
    }
    void EnableUI()
    {
        group.alpha = 1f;
        group.blocksRaycasts = true;
    }
}
