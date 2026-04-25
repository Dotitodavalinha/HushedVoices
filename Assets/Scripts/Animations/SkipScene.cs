using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SkipScene : MonoBehaviour
{
    public VideoPlayer introScene;

    void Start()
    {
        introScene.loopPointReached += AtVideoEnd;
    }

    void AtVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene("RoomInicio");
    }

}
