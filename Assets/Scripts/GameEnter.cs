using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnter : MonoBehaviour
{
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
}
