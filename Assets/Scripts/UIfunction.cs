using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIfunction : MonoBehaviour
{

    public Image specification;
    [Serializable]
    public struct NamedImage
    {
        public string name;
        public Sprite image;
    }
    public NamedImage[] pictures;
    static public string prefabName;
    private void Awake()
    {
        Screen.orientation = ScreenOrientation.Portrait;
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in pictures)
        {
            Debug.Log(item.image);
        }
    }

    public void onChnage()
    {
        Screen.orientation = ScreenOrientation.Landscape;
    }

    public void dynamicUI(Button button)
    {
        foreach(var item in pictures)
        {
            if(item.name.ToString() == button.name)
            {
                specification.sprite = item.image;
                prefabName = item.name.ToString();
            }
        }
    }

    public string secondFunction()
    {
        return prefabName;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
