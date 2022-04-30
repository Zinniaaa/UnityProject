using libx;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StepSetting : MonoBehaviour
{
    public enum StepType
    {
        None,

        //音频输入
        AudioInput,

        //音频导入
        VideoInput,

        //模型选择
        ModelInput,

        //模型设置
        ModelSetting,

        SettingDone,
    }

    public class StepData
    {
        //0音频输入 1麦克风输入
        public int audioInput = 0;

        //视频文件路径
        public string videoFilePath = string.Empty;

        //0妹子 1卡通
        public int modelIndex = 0;

        //模型尺寸
        public Vector3 modelScale = Vector3.one;

        //模型位置
        public Vector3 modelPos = Vector3.zero;
    }

    private StepType stepType = StepType.None;

    public StepData stepData = new StepData();

    private GameObject stepBtns;
    private GameObject stepModle;
    private Text txt_msg;
    private Button btn_next;

    private Slider sld_scale;

    private void Awake()
    {
        Assets.Initialize();

        stepBtns = transform.Find("Step/StepBtns").gameObject;
        stepModle = transform.Find("Step/StepModle").gameObject;
        txt_msg = transform.Find("Step/Msg").GetComponent<Text>();
        btn_next = transform.Find("Step/next").GetComponent<Button>();
        sld_scale = transform.Find("Step/StepModle/sld_scale").GetComponent<Slider>();
        btn_next.onClick.AddListener(onClickNext);
        sld_scale.onValueChanged.AddListener(onValueChanged);
    }

    void Start()
    {
        stepType = StepType.AudioInput;

        enterStep();
    }
    
    private void NextStep()
    {
        if (stepType < StepType.SettingDone)
        {
            stepType++;
        }
        else
        {
            stepSettingDone();
        }
    }

    private void enterStep()
    {
        switch (stepType)
        {
            case StepType.AudioInput:
                stepAudioInput();
                break;
            case StepType.VideoInput:
                stepVideoInput();
                break;
            case StepType.ModelInput:
                stepModelInput();
                break;
            case StepType.ModelSetting:
                stepModelSetting();
                break;
            case StepType.SettingDone:
                stepSettingDone();
                break;            
        }
    }

    private void stepAudioInput()
    {
        List<string> names = new List<string>() {
            "视频音源","麦克风音源"
        };
        List<UnityAction> actions = new List<UnityAction>();
        actions.Add(()=> {
            stepData.audioInput = 0;
            txt_msg.text = "视频音源";
        });
        actions.Add(() => {
            stepData.audioInput = 1;
            txt_msg.text = "麦克风音源";
        });
        setBtns(names, actions);
    }

    private void stepVideoInput()
    {
        List<string> names = new List<string>() {
            "选择视频文件"
        };
        List<UnityAction> actions = new List<UnityAction>();
        actions.Add(() => {
            var fileInfo = SelectVideoFile();
            txt_msg.text = fileInfo.fileTitle;
            stepData.videoFilePath = fileInfo.file.Replace("\\","/");
        });
        setBtns(names, actions);
    }

    private void stepModelInput()
    {
        List<string> names = new List<string>() {
            "角色1",//"角色2"
        };
        List<UnityAction> actions = new List<UnityAction>();
        actions.Add(() => {
            stepData.modelIndex = 0;
            txt_msg.text = "角色1";
        });
        //actions.Add(() => {
        //    stepData.modelIndex = 1;
        //    txt_msg.text = "角色2";
        //});
        setBtns(names, actions);
    }
    AssetRequest asset;
    GameObject target;

    private void stepModelSetting()
    {
        stepModle.SetActive(true);
        string targetPath = stepData.modelIndex == 0 ? "Assets/Asset/Prefabs/Modle1.prefab" : "Assets/Asset/Prefabs/LipSyncTextureFlipTarget_Robot.prefab";
        asset = Assets.LoadAsset(targetPath, typeof(GameObject));
        target = GameObject.Instantiate(asset.asset) as GameObject;
        target.name = "modle";
        target.transform.SetParent(transform);    
        target.transform.localPosition = Vector3.zero;
        target.transform.localScale = Vector3.one;
    }

    private void stepSettingDone()
    {
        stepBtns.SetActive(false);
        stepModle.SetActive(false);
        stepData.modelPos =new Vector3(target.transform.localPosition.x, target.transform.localPosition.y,-500);
        Destroy(target);
        Assets.UnloadAsset(asset);

        txt_msg.text = "点击下一步，开始录制！";
    }

    private void onValueChanged(float v)
    {
        if (target)
        {
            target.transform.localScale = Vector3.one * v;
            stepData.modelScale = Vector3.one * v;
        }
    }

    private void onClickNext()
    {
        if (stepType == StepType.SettingDone)
        {
            //加载场景 切换场景
            VideoManager.Instance.SetReady(stepData);
            Assets.Instance.Switch("Assets/Asset/Scenes/Record.unity",VideoManager.Instance.Play);
        }
        else
        {
            stepBtns.SetActive(false);
            stepModle.SetActive(false);
            txt_msg.text = "";
            NextStep();
            enterStep();
        }        
    }

    private void setBtns(List<string> btnNames,List<UnityAction> actions)
    {
        stepBtns.SetActive(true);
        for (int i = 0; i < stepBtns.transform.childCount; i++)
        {
            var child = stepBtns.transform.GetChild(i);
            if (i < btnNames.Count)
            {
                var name = btnNames[i];
                var txt = child.Find("Text").GetComponent<Text>();
                txt.text = btnNames[i];
                var btn = child.GetComponent<Button>();
                var action = actions[i];
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(action);
            }
            child.gameObject.SetActive(i < btnNames.Count);
        }
    }

    /// <summary>
    /// 选择视频文件
    /// </summary>
    /// <returns></returns>
    public OpenFileName SelectVideoFile()
    {
        OpenFileName openFileName = new OpenFileName();
        openFileName.structSize = Marshal.SizeOf(openFileName);
        //openFileName.filter = "视频文件(*.mp4)/0*.wmv";
        openFileName.file = new string(new char[256]);
        openFileName.maxFile = openFileName.file.Length;
        openFileName.fileTitle = new string(new char[64]);
        openFileName.maxFileTitle = openFileName.fileTitle.Length;
        openFileName.initialDir = Application.streamingAssetsPath.Replace('/', '\\');//默认路径    
        openFileName.title = "选择视频文件";
        openFileName.flags = 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008;
        if (LocalDialog.GetOpenFileName(openFileName))
        {
            Debug.Log(openFileName.file);
            Debug.Log(openFileName.fileTitle);
        }
        return openFileName;
    }
}
