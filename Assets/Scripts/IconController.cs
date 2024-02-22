using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconController : MonoBehaviour
{
    [SerializeField] private Image[] _icons;
    [SerializeField] private Color _usedColor;

    public void UseShot(int shotNum)
    {
        for(int i = 0; i < _icons.Length; i++){
            if(shotNum == i + 1)
            {
                _icons[i].color = _usedColor;
                return;
            }
        }
    }
}
