using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class MatchmakingLogic : MonoBehaviour
{
    [SerializeField] private data _data;
    [SerializeField] private GameObject _uiFinal;

    private float _restartCooldown;

    private void Start()
    {
        _restartCooldown = _data.RestartCooldown;
    }





    // static instance that can be referenced directly from Player script
    public static MatchmakingLogic   instance;

    void Awake()
    {
        instance = this;
    }

}
