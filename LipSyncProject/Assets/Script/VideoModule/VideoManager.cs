using libx;
using RenderHeads.Media.AVProMovieCapture;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoManager:MonoBehaviour
{    
    private VideoManager()
    {

    }

    private static VideoManager instance = null;

    public static VideoManager Instance
    {
        get { return instance; }
    }

    private VideoPlayer videoPlayer;

    private RenderTexture renderTexture;

    [SerializeField]
    private RawImage rawImage;

    [SerializeField]
    private CanvasScaler canvasScaler;

    private AudioSource audioSource;

    private Vector2 size = Vector2.zero;

    private GameObject target;

    private AssetRequest asset;

    private CaptureFromCamera capture;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        instance = this;
        capture = transform.GetComponent<CaptureFromCamera>();
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += loopPointReached;
    }

    private IEnumerator Test()
    {
        Assets.Initialize();
        yield return new WaitForSeconds(1);
        StepSetting.StepData stepData = new StepSetting.StepData();
        stepData.audioInput = 1;
        stepData.videoFilePath = "D:/GitProject/Sync/LipSyncProject/LipSyncProject/Assets/Asset/MyTestVideo.mp4";

        SetReady(stepData);
    }

    private void OnDestroy()
    {
        if (target != null)
        {
            Destroy(target);
            Assets.UnloadAsset(asset);
        }
        videoPlayer.loopPointReached -= loopPointReached;        
    }
    StepSetting.StepData stepData;
    public void SetReady(StepSetting.StepData stepData)
    {
        this.stepData = stepData;
    }

    public void Play()
    {
        if (canvasScaler == null)
        {
            var obj = GameObject.Find("Canvas");
            canvasScaler = obj.GetComponent<CanvasScaler>();
            rawImage = obj.transform.Find("RawImage").GetComponent<RawImage>();
            var camera = obj.transform.Find("UICamera").GetComponent<Camera>();
            capture.SetCamera(camera);
        }

        StartCoroutine(DelayPlayVideo(stepData));
    }

    private void aliveModle(int modelIndex,Vector3 pos,Vector3 scale)
    {
        string targetPath = modelIndex == 0 ? "Assets/Asset/Prefabs/Modle1.prefab" : "Assets/Asset/Prefabs/LipSyncTextureFlipTarget_Robot.prefab";
        asset = Assets.LoadAsset(targetPath, typeof(GameObject));        
        target = GameObject.Instantiate(asset.asset) as GameObject;

        target.name = "modle";
        var root = GameObject.Find("Canvas");
        target.transform.SetParent(root.transform);
        target.transform.localPosition = new Vector3(pos.x,pos.y,-500);
        target.transform.localScale = scale;
    }

    private void aliveAudio(int audioInput)
    {
        if (audioInput == 0)
        {
            audioSource = target.transform.GetChild(0).GetComponent<AudioSource>();
        }
        else
        {
            //音频播放放到当前节点
            audioSource = gameObject.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }

            //激活麦克风输入的组件
            var micinput = target.transform.GetChild(0).GetComponent<OVRLipSyncMicInput>();
            micinput.enabled = true;
        }
    }

    private void aliveVideo(string videoPath)
    {
        videoPlayer.source = VideoSource.Url;
        videoPlayer.url = videoPath;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.Prepare();
    }

    private void loopPointReached(VideoPlayer source)
    {
        source.Stop();
        capture.StopCapture();
    }

    IEnumerator DelayPlayVideo(StepSetting.StepData stepData)
    {

        yield return null;

        aliveModle(stepData.modelIndex, stepData.modelPos, stepData.modelScale);

        aliveAudio(stepData.audioInput);

        aliveVideo(stepData.videoFilePath);


        if (!videoPlayer.isPrepared)
        {            
            yield return new WaitUntil(() => videoPlayer.isPrepared);
        }
        //根据视频分辨率构建RT
        size = new Vector2(videoPlayer.width, videoPlayer.height);
        canvasScaler.referenceResolution = size;
        renderTexture = new RenderTexture((int)size.x, (int)size.y, 0);
        videoPlayer.targetTexture = renderTexture;
        rawImage.texture = renderTexture;
        
        yield return null;

        //开始播放
        videoPlayer.Play();
        capture.StartCapture();
    }
}
