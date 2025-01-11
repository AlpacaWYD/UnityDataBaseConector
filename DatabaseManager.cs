using System;
using MySql.Data.MySqlClient;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{
    [Tooltip("数据库名")]
    public static string dbName = "unity";
    [Tooltip("用户名")]
    public static string userName = "root";
    [Tooltip("密码")]
    public static string password = "123456";
    [Tooltip("服务器地址")]
    public static string serverName = "localhost";
    [Tooltip("服务器端口号")]
    public static string port = "3306";

    private static MySqlConnection connection;

    public static string currentUserName; // 当前用户
    public static int TotalScore = 0;     // 当前总积分

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // 初始化数据库连接
    private static void InitializeDatabaseConnection()
    {
        string connectionString = $"Server={serverName};Database={dbName};Uid={userName};Pwd={password};Port={port};";
        connection = new MySqlConnection(connectionString);

        try
        {
            connection.Open();
            Debug.Log("数据库连接成功");
        }
        catch (Exception ex)
        {
            Debug.LogError("数据库连接失败: " + ex.Message);
        }
    }

    // 获取数据库连接
    public static MySqlConnection GetConnection()
    {
        if (connection == null || connection.State == System.Data.ConnectionState.Closed)
        {
            InitializeDatabaseConnection();
        }
        return connection;
    }

    // 增加积分
    public static void AddScore(int points)
    {
        TotalScore += points;
    }

    // 重置积分
    public static void ResetScore()
    {
        TotalScore = 0;
    }

    // 保存积分到数据库
    public static void SaveScoreToDatabase()
    {
        string query = "UPDATE users SET score = score + @score WHERE username = @username";
        MySqlCommand cmd = new MySqlCommand(query, GetConnection());
        cmd.Parameters.AddWithValue("@score", TotalScore);
        cmd.Parameters.AddWithValue("@username", currentUserName);
        int rowsAffected = cmd.ExecuteNonQuery();

        if (rowsAffected > 0)
        {
            Debug.Log("积分保存成功！");
            ResetScore();
        }
        else
        {
            Debug.Log("保存积分失败！");
        }
    }

    // 清空当前用户的积分
    public static void ClearCurrentUserScore()
    {
        string query = "UPDATE users SET score = 0 WHERE username = @username";
        MySqlCommand cmd = new MySqlCommand(query, GetConnection());
        cmd.Parameters.AddWithValue("@username", currentUserName);

        int rowsAffected = cmd.ExecuteNonQuery();
        if (rowsAffected > 0)
        {
            Debug.Log($"用户 {currentUserName} 的积分已清空！");
        }
        else
        {
            Debug.Log($"未找到用户 {currentUserName} 或积分无需清空！");
        }
    }

    // 从数据库读取总积分
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
                Debug.Log("未找到用户或积分数据无效。");
                return 0;
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("读取总积分时出错：" + ex.Message);
            return 0;
        }
    }

    private void OnApplicationQuit()
    {
        CloseConnection();
    }

    // 关闭数据库连接
    public static void CloseConnection()
    {
        if (connection != null && connection.State == System.Data.ConnectionState.Open)
        {
            connection.Close();
            Debug.Log("数据库连接已关闭");
        }
    }
}