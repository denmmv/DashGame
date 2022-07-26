using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasUI : MonoBehaviour
{
    public RectTransform Panel;  
    public static CanvasUI instance;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        instance = this;
    }
    
}
