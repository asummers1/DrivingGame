using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MenuManager : MonoBehaviour
{
    [SerializeField] List<Menu> menus = new List<Menu>();
    static public bool startFromTimeTrial;
    void Start()
    {
        if (!startFromTimeTrial)
        {
            ShowMenu(menus[0]);
        } else
        {
            ShowMenu(menus[2]);
            startFromTimeTrial = false;
        }
    }

    public void ShowMenu(Menu menuToShow)
    {
        if (menus.Contains(menuToShow) == false)
        {
            Debug.LogErrorFormat($"{menuToShow.name} is not in the list of menus");
        }

        foreach (var otherMenu in menus)
        {
            if (otherMenu == menuToShow)
            {
                otherMenu.gameObject.SetActive(true);
                otherMenu.menuDidAppear.Invoke();
            } 
            else
            {
                if (otherMenu.gameObject.activeInHierarchy)
                {
                    otherMenu.menuWillDisappear.Invoke();
                }
                otherMenu.gameObject.SetActive(false);
            }
        }
    }
}
