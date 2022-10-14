using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnter : MonoBehaviour
{
    Vector2 scrollPos = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        Singleton<GameManager>.Inst.Init();
    }

    // Update is called once per frame
    void Update()
    {
        Singleton<GameManager>.Inst.Update();
    }

    /// <summary>
    /// 绘制GUI
    /// </summary>
    private void OnGUI()
    {
        var gameLog = Singleton<LogManager>.Inst.GetGameLog();
        GUILayout.BeginHorizontal();
        scrollPos = GUILayout.BeginScrollView(scrollPos);
        foreach (var log in gameLog)
        {
            GUILayout.Label(log);
        }
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
    }
}
