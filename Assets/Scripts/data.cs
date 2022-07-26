using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Data", menuName = "Data", order = 51)]
public class data : ScriptableObject
{
    [Header("Hero")]
    [SerializeField] private float _speed;
    [SerializeField] private float _smoothTurnTime;
    [Header("Camera")]
    [SerializeField] private float _followSpeed;
    [SerializeField] private float _sensitivity;
    [Header("Dash")]
    [SerializeField] private float _dashCooldown;
    [SerializeField] private float _dashDistance;
    [SerializeField] private float _dashDuration;
    [SerializeField] private float _injureDuration;
    [Header("Competitive")]
    [SerializeField] private int _pointsToWin;
    [SerializeField] private float _restartCooldown;


    public float Speed
    {
        get
        {
            return _speed;
        }
    }
    public float SmoothTurnTime
    {
        get
        {
            return _smoothTurnTime;
        }
    }
    public float FollowSpeed
    {
        get
        {
            return _followSpeed;
        }
    }
    public float Sensitivity
    {
        get
        {
            return _sensitivity;
        }
    }
    public float DashCooldown
    {
        get
        {
            return _dashCooldown;
        }
    }
    public float DashDistance
    {
        get
        {
            return _dashDistance;
        }
    }
    public float DashDuration
    {
        get
        {
            return _dashDuration;
        }
    }
    public float InjureDuration
    {
        get
        {
            return _injureDuration;
        }
    }
    public int PointsToWin
    {
        get
        {
            return _pointsToWin;
        }
    }
    public float RestartCooldown
    {
        get
        {
            return _restartCooldown;
        }
    }

}
