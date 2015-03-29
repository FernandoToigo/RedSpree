using UnityEngine;
using System.Collections;

public class BulletsLootText : MonoBehaviour
{
    void Update()
    {
        transform.localPosition += new Vector3(0.0f, Time.deltaTime * 0.3f, 0.0f);
    }
}