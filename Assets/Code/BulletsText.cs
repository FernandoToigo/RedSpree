using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BulletsText : MonoBehaviour
{
    void Update()
    {
        var text = GetComponent<Text>();
        text.text = Player.Bullets.ToString();
    }
}