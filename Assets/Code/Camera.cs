using UnityEngine;
using System.Collections;

public class Camera : MonoBehaviour
{
    float _worldScreenHeight;
    float _worldScreenWidth;

    void Start()
    {
        //Camera.main.orthographicSize = Screen.height / 64.0f / 2f;
        _worldScreenHeight = UnityEngine.Camera.main.orthographicSize * 2 * UnityEngine.Camera.main.transform.localScale.y;
        _worldScreenWidth = UnityEngine.Camera.main.orthographicSize * 2 * UnityEngine.Camera.main.aspect * UnityEngine.Camera.main.transform.localScale.x;
    }

    void Update()
    {
        transform.localPosition = new Vector3(
            _worldScreenWidth * 0.4f - 0.64f + Player.position.x,
            _worldScreenHeight * 0.4f - 0.64f,
            transform.localPosition.z);

        if (Input.GetButtonDown("Cancel"))
            Application.LoadLevel("Menu");
    }
}