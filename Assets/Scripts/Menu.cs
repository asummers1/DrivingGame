using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Menu : MonoBehaviour
{
    //Adapted from "Unity Game Development Cookbook", published by O'Reilly
    public UnityEvent menuDidAppear = new UnityEvent();

    public UnityEvent menuWillDisappear = new UnityEvent();
}
