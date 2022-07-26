using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Hero : NetworkBehaviour
{
    [SerializeField] private data _data;
    [SerializeField] private SkinnedMeshRenderer _heroMesh;
    [SerializeField] private int _score = 0;

    [SyncVar]
    private float _speed;

    private float _injureDuratuion;
    private int _pointsToWin;
    private float _restartCooldown;
    private Color _defaultColor;
    private Coroutine InjureCooldown;

    public bool _isDashing = false;
    private Coroutine DashAction;
    private Coroutine DashCooldown;
    private float _dashCooldown;
    private float _dashDistance;

    private float _turnSmoothTime;
    private float _turnSmoothVelocity;
    private Rigidbody _heroRB;

    private Camera _cam;
    private GameObject _pivot;
    private float _followSpeed;
    private Vector3 _followVelocity = new Vector3();
    private float _verticalAngle;
    private float _horizontalAngle;
    private float _sensitivity;

    PlayersScore playersScore;
    private int _id;

    [SerializeField] private Animator _heroAnimator;

    #region Initialize
    private void Start()
    {
        playersScore = FindObjectOfType<PlayersScore>();
        _id = playersScore.AddHero(this.gameObject,_score);
        _pointsToWin = _data.PointsToWin;
        _restartCooldown = _data.RestartCooldown;
        _injureDuratuion = _data.InjureDuration;
        _dashDistance = _data.DashDistance;
        _dashCooldown = _data.DashCooldown;
        _sensitivity = _data.Sensitivity;
        _followSpeed = _data.FollowSpeed;
        _turnSmoothTime = _data.SmoothTurnTime;
        _heroRB = this.GetComponent<Rigidbody>();
        _defaultColor = _heroMesh.material.color;
        if (isClient && isLocalPlayer)
        {
            InitialazeInput();         
        }
        if (isServer)
        {
            _speed = _data.Speed;           
        }        
    }
    private void InitialazeInput()
    {
        InputManager.Instance.SetHero(this);
        _cam = FindObjectOfType<Camera>();
        _pivot = _cam.transform.parent.gameObject;
    }
    #endregion
    #region CompetitiveLogic
    [Command]
    public void CmdHitedByDash(Hero hero)
    {
        hero.tag = "InjuredPlayer";
        hero._heroMesh.material.color = Color.red;
        RpcSuccsessfulDash();
        RpcHitedByDash(hero);
    }
    [ClientRpc]
    public void RpcHitedByDash(Hero hero)
    {
        hero.tag = "InjuredPlayer";
        hero._heroMesh.material.color = Color.red;
        hero.CmdStartInjure();
    }
    [ClientRpc]
    private void RpcSuccsessfulDash()
    {
        playersScore.AddPoint(_id);
    }
    [Command]
    private void CmdStartInjure()
    {
        InjureCooldown = StartCoroutine(injureCooldown());
    }
    [ClientRpc]
    private void RpcAfterInjure()
    {
        this.tag = "Player";
        _heroMesh.material.color = _defaultColor;
    }
    IEnumerator injureCooldown()
    {
        yield return new WaitForSeconds(_injureDuratuion);
        RpcAfterInjure();
    }
    #endregion
    #region Dash
    [Command]
    public void CmdDash(Vector3 targetPosition)
    {
        DashAction = StartCoroutine(dashAction(targetPosition));
        
    }
    public void Dash()
    {
        if (!_isDashing)
        {
            Vector3 targetPosition = this.transform.position + this.transform.forward*_dashDistance;
            DashAction = StartCoroutine(dashAction(targetPosition));
            DashCooldown = StartCoroutine(dashCooldown());
            CmdDash(targetPosition);
        }        
    }  
    IEnumerator dashAction(Vector3 target)
    {
        _isDashing = true;
        Vector3 startPosition = this.transform.position;
        float dashDuration = _data.DashDuration;
        float elapsedTime =0f;
        float percentOfComplete = 0f;
        bool gotTarget = false;
        while (!gotTarget)
        {
            elapsedTime += Time.deltaTime;
            percentOfComplete = elapsedTime / dashDuration;
            if (percentOfComplete > 0.9)
            {
                transform.position = target;               
                gotTarget = true;
            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, target, percentOfComplete);
            }           
            yield return new WaitForFixedUpdate();
        }
    }
    IEnumerator dashCooldown()
    {
        yield return new WaitForSeconds(_dashCooldown);
        _isDashing = false;
    }
    #endregion
    #region Movement
    [Command]
    public void CmdMoveHero(Vector3 move, float angle)
    {
        _heroRB.velocity = move.normalized * _speed;
        this.transform.rotation = Quaternion.Euler(0f, angle, 0f);
        _heroAnimator.Play("RUN");
    }
    [Command]
    private void StopRun()
    {
        _heroAnimator.Play("IDLE");
    }
    public void MoveHero(Vector3 move)
    {
        if(move.x==0&&move.z==0)
        {
            StopRun();
            return;
        }
        float targetAngle = Mathf.Atan2(move.x, move.z) * Mathf.Rad2Deg + _cam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
        this.transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        _heroRB.velocity = moveDir.normalized * _speed;
        CmdMoveHero(moveDir, targetAngle);
    }
    #endregion
    #region Camera
    public void CameraFollow()
    {
        Vector3 target = Vector3.SmoothDamp(_pivot.transform.position, this.transform.position, ref _followVelocity, _followSpeed);
        _pivot.transform.position = target;
    }
    public void CameraRotate(Vector3 look)
    {
        /*_horizontalAngle += (look.x * _sensativity);
        _verticalAngle += -(look.y * _sensativity);
        _verticalAngle = Mathf.Clamp(_verticalAngle, -15f, 35f);
        Vector3 rotation = Vector3.zero;
        rotation.y = _horizontalAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        _pivot.transform.rotation = targetRotation;
        rotation = _cam.transform.rotation.eulerAngles;
        rotation.x = _verticalAngle;
        targetRotation = Quaternion.Euler(rotation);
        _cam.transform.rotation = targetRotation;
        rotation = Vector3.zero;*/

        /*two variation of camera movement*/

        _horizontalAngle += (look.x * _sensitivity);
        _verticalAngle += -(look.y * _sensitivity);
        _verticalAngle = Mathf.Clamp(_verticalAngle, -15f, 35f);
        Vector3 rotation = Vector3.zero;
        rotation.y = _horizontalAngle;
        rotation.x = _verticalAngle;
        Quaternion targetRotation = Quaternion.Euler(rotation);
        _pivot.transform.rotation = targetRotation;
    }
    #endregion
    private void OnCollisionEnter(Collision collision)
    {
        if(_isDashing&&collision.gameObject.tag == "Player")
        {
            Hero hero = collision.gameObject.GetComponent<Hero>();
            CmdHitedByDash(hero);
        }
    }
}
