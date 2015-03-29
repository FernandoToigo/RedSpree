using UnityEngine;
using System.Collections;

public class Ground : MonoBehaviour
{
    void Start()
    {
        float worldScreenHeight = UnityEngine.Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var width = worldScreenWidth;
        var height = worldScreenHeight * 0.12f;

        transform.localPosition = new Vector3(0.0f, -worldScreenHeight * 0.5f + height * 0.5f, 0.0f);
        transform.localScale = new Vector3(width, height, 1f);
    }
}