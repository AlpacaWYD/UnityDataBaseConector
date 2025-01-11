using System;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [Tooltip("���ݿ���")]
    public static string dbName = "unity";
    [Tooltip("�û���")]
    public static string userName = "root";
    [Tooltip("����")]
    public static string password = "123456";
    [Tooltip("��������ַ")]
    public static string serverName = "localhost";
    [Tooltip("�������˿ں�")]
    public static string port = "3306";

    private static MySqlConnection connection;

    public static string currentUserName; // ��ǰ�û�
    public static int TotalScore = 0;     // ��ǰ�ܻ���

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // ��ʼ�����ݿ�����
    private static void InitializeDatabaseConnection()
    {
        string connectionString = $"Server={serverName};Database={dbName};Uid={userName};Pwd={password};Port={port};";
        connection = new MySqlConnection(connectionString);

        try
        {
            connection.Open();
            Debug.Log("���ݿ����ӳɹ�");
        }
        catch (Exception ex)
        {
            Debug.LogError("���ݿ�����ʧ��: " + ex.Message);
        }
    }

    // ��ȡ���ݿ�����
    public static MySqlConnection GetConnection()
    {
        if (connection == null || connection.State == System.Data.ConnectionState.Closed)
        {
            InitializeDatabaseConnection();
        }
        return connection;
    }

    // ���ӻ���
    public static void AddScore(int points)
    {
        TotalScore += points;
    }

    // ���û���
    public static void ResetScore()
    {
        TotalScore = 0;
    }

    // ������ֵ����ݿ�
    public static void SaveScoreToDatabase()
    {
        string query = "UPDATE users SET score = score + @score WHERE username = @username";
        MySqlCommand cmd = new MySqlCommand(query, GetConnection());
        cmd.Parameters.AddWithValue("@score", TotalScore);
        cmd.Parameters.AddWithValue("@username", currentUserName);
        int rowsAffected = cmd.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            Debug.Log("���ֱ���ɹ���");
            ResetScore();
        }
        else
        {
            Debug.Log("�������ʧ�ܣ�");
        }
    }

    // ��յ�ǰ�û��Ļ���
    public static void ClearCurrentUserScore()
    {
        string query = "UPDATE users SET score = 0 WHERE username = @username";
        MySqlCommand cmd = new MySqlCommand(query, GetConnection());
        cmd.Parameters.AddWithValue("@username", currentUserName);

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
            Debug.Log($"�û� {currentUserName} �Ļ�������գ�");
        }
        else
        {
            Debug.Log($"δ�ҵ��û� {currentUserName} �����������գ�");
        }
    }

    // �����ݿ��ȡ�ܻ���
    public static int GetTotalScoreFromDatabase()
    {
        try
        {
            string query = "SELECT score FROM users WHERE username = @username";
            MySqlCommand cmd = new MySqlCommand(query, GetConnection());
            cmd.Parameters.AddWithValue("@username", currentUserName);

            object result = cmd.ExecuteScalar();

            if (result != null && int.TryParse(result.ToString(), out int totalScore))
            {
                return totalScore;
            }
            else
            {
                Debug.Log("δ�ҵ��û������������Ч��");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("��ȡ�ܻ���ʱ����" + ex.Message);
            return 0;
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    // �ر����ݿ�����
    public static void CloseConnection()
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
            Debug.Log("���ݿ������ѹر�");
        }
    }
}