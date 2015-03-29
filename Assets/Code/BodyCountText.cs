using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BodyCountText : MonoBehaviour
{
    Text bodyCountText;
    Text bulletsText;
    Text timeText;

    void Start()
    {
        bodyCountText = transform.Find("BodyCountValue").GetComponent<Text>();
        bulletsText = transform.Find("BulletsValue").GetComponent<Text>();
        timeText = transform.Find("TimeValue").GetComponent<Text>();
    }

    void Update()
    {
        bodyCountText.text = Player.zombiesKilled.ToString();
        bulletsText.text = Player.Bullets.ToString();

        var seconds = (int)(Time.timeSinceLevelLoad % 60.0f);
        var minutes = (int)(Time.timeSinceLevelLoad / 60.0f);

        timeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
