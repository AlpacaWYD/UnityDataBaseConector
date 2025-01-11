using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel_UI : MonoBehaviour
{
    //主界面音频
    public AudioClip MainAudioClip;

    public AudioSource BgmSource;

    //主界面
    private Transform MainPanel;
    private Button StartBtn;
    private Button SettingBtn;
    private Button ExitBtn;
    private Text CurrentScore;

    //设置界面
    private Transform SettingPanel;
    private Slider BGMValue;
    private Button BackBtn;


    //登录界面
    private Transform loginPanel;

    void Awake()
    {
        loginPanel = transform.Find("LoginPanel");

        // 查找Main界面的各个UI组件
        MainPanel = transform.Find("MainPanel");
        StartBtn = MainPanel.Find("StartBtn").GetComponent<Button>();
        SettingBtn = MainPanel.Find("SettingBtn").GetComponent<Button>();
        ExitBtn = MainPanel.Find("ExitBtn").GetComponent<Button>();
        CurrentScore = MainPanel.Find("CurrentScore").GetComponent<Text>();

        // 为注册按钮和切换到登录界面按钮添加点击事件
        StartBtn.onClick.AddListener(StartGame);
        SettingBtn.onClick.AddListener(SettingOpen);
        ExitBtn.onClick.AddListener(Exit);


        //查找设置界面
        SettingPanel = transform.Find("SettingPanel");
        BackBtn = SettingPanel.Find("BackBtn").GetComponent<Button>();
        BGMValue = SettingPanel.Find("BGMValue").GetComponent<Slider>();

        BackBtn.onClick.AddListener(Back);
        // 注册 Slider 的事件
        BGMValue.onValueChanged.AddListener(OnBGMValueChanged);
    }

    private void Start()
    {
        BGMValue.value = 1f;
        MainPanel.gameObject.SetActive(true);
        loginPanel.gameObject.SetActive(false);
        SettingPanel.gameObject.SetActive(false);

        GetCurrentScoreFromDataBase();//从数据库查询当前的分数
    }

    private void GetCurrentScoreFromDataBase()
    {
        int score = DatabaseManager.GetTotalScoreFromDatabase();
        CurrentScore.text = "当前积分： " + score.ToString();
    }

    void StartGame()
    {
        loginPanel.gameObject.SetActive(true);
    }

    void SettingOpen()
    {
        BGMValue.value = BgmSource.volume;
        SettingPanel.gameObject.SetActive(true);
    }

    void Exit()
    {
        DatabaseManager.ClearCurrentUserScore();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
        
#endif
    }

    void Back()
    {
        SettingPanel.gameObject.SetActive(false);
        MainPanel.gameObject.SetActive(true);
    }

    // Slider 值改变时调用
    void OnBGMValueChanged(float value)
    {
        BgmSource.volume = value;
    }
}
