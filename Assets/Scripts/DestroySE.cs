using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySE : MonoBehaviour
{
    // AudioSourceコンポーネント
    AudioSource audioSource;

    void Start()
    {
        // AudioSourceコンポーネントを取得する
        audioSource = GetComponent<AudioSource>();

        // 効果音再生した後にDestroyする 音の再生はPlay On Awake
        Destroy(gameObject, audioSource.clip.length);
    }
}
