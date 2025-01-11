using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel_UI : MonoBehaviour
{
    //��������Ƶ
    public AudioClip MainAudioClip;

    public AudioSource BgmSource;

    //������
    private Transform MainPanel;
    private Button StartBtn;
    private Button SettingBtn;
    private Button ExitBtn;
    private Text CurrentScore;

    //���ý���
    private Transform SettingPanel;
    private Slider BGMValue;
    private Button BackBtn;


    //��¼����
    private Transform loginPanel;

    void Awake()
    {
        loginPanel = transform.Find("LoginPanel");

        // ����Main����ĸ���UI���
        MainPanel = transform.Find("MainPanel");
        StartBtn = MainPanel.Find("StartBtn").GetComponent<Button>();
        SettingBtn = MainPanel.Find("SettingBtn").GetComponent<Button>();
        ExitBtn = MainPanel.Find("ExitBtn").GetComponent<Button>();
        CurrentScore = MainPanel.Find("CurrentScore").GetComponent<Text>();

        // Ϊע�ᰴť���л�����¼���水ť��ӵ���¼�
        StartBtn.onClick.AddListener(StartGame);
        SettingBtn.onClick.AddListener(SettingOpen);
        ExitBtn.onClick.AddListener(Exit);


        //�������ý���
        SettingPanel = transform.Find("SettingPanel");
        BackBtn = SettingPanel.Find("BackBtn").GetComponent<Button>();
        BGMValue = SettingPanel.Find("BGMValue").GetComponent<Slider>();

        BackBtn.onClick.AddListener(Back);
        // ע�� Slider ���¼�
        BGMValue.onValueChanged.AddListener(OnBGMValueChanged);
    }

    private void Start()
    {
        BGMValue.value = 1f;
        MainPanel.gameObject.SetActive(true);
        loginPanel.gameObject.SetActive(false);
        SettingPanel.gameObject.SetActive(false);

        GetCurrentScoreFromDataBase();//�����ݿ��ѯ��ǰ�ķ���
    }

    private void GetCurrentScoreFromDataBase()
    {
        int score = DatabaseManager.GetTotalScoreFromDatabase();
        CurrentScore.text = "��ǰ���֣� " + score.ToString();
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

    // Slider ֵ�ı�ʱ����
    void OnBGMValueChanged(float value)
    {
        BgmSource.volume = value;
    }
}
