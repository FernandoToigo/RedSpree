using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum MessageState
{
    Hidden,
    FadingIn,
    Shown,
    FadingOut
}

public class Message : MonoBehaviour
{
    private MessageState state;
    private float fadeTimer;
    private float fadeCooldown = 0.5f;

    private float showTimer;
    private float showCooldown = 3.0f;

    private Text text;

    void Start()
    {
        state = MessageState.Hidden;
        text = GetComponent<Text>();
        fadeTimer = fadeCooldown;
        showTimer = showCooldown;
    }

    void Update()
    {
        switch (state)
        {
            case MessageState.FadingIn:
                fadeTimer = Mathf.Max(0.0f, fadeTimer - Time.deltaTime);
                text.color = new Color(text.color.r, text.color.g, text.color.b, ((fadeCooldown - fadeTimer) / fadeCooldown));
                if (fadeTimer <= 0)
                    state = MessageState.Shown;
                break;
            case MessageState.Shown:
                showTimer = Mathf.Max(0.0f, showTimer - Time.deltaTime);

                if (showTimer <= 0)
                {
                    state = MessageState.FadingOut;
                    fadeTimer = fadeCooldown;
                }

                break;
            case MessageState.FadingOut:
                fadeTimer = Mathf.Max(0.0f, fadeTimer - Time.deltaTime);
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - (((fadeCooldown - fadeTimer) / fadeCooldown)));
                break;
        }
    }

    public void ShowMessage(string message, float time)
    {
        text.text = message;
        showCooldown = time;
        fadeTimer = fadeCooldown;
        showTimer = showCooldown;
        state = MessageState.FadingIn;
    }
}