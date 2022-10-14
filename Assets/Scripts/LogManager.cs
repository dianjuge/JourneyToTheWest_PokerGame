using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogManager
{
    static List<string> gameLog = new List<string>();

    /// <summary>
    /// 打印日志
    /// </summary>
    public static void PrintLog(string log)
    {
        var logs = log.Split('\n');
        Debug.Log(log);
        gameLog.Add(logs[0]);
    }

    /// <summary>
    /// 获取游戏日志
    /// </summary>
    /// <returns></returns>
    public List<string> GetGameLog()
    {
        return gameLog;
    }
}
