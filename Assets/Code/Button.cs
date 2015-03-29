using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Button : MonoBehaviour
{
    public Color colorTo;
    private Color _color;
    private Text _text;

    public void Start()
    {
        //var button = GameObject.Find("NewGameButton");
        _text = gameObject.GetComponentInChildren<Text>();
    }

    public void OnMouseOver()
    {
        _color = _text.color;
        _text.color = colorTo;
    }

    public void OnMouseExit()
    {
        _text.color = _color;
    }
}