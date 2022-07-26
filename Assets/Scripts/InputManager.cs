using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager _instance;
    private Vector3 _move = new Vector3();
    private Vector3 _look = new Vector3();
    private Hero _hero;
    private void Update()
    {
        if (_hero)
        {
            GetInput();
            SendInput();
        }
    }
    #region Initialize
    public static InputManager Instance
    {
        get
        {
            return _instance;
        }
    }
    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        _instance = this;
    }
    public void SetHero(Hero hero)
    {
        _hero = hero;
    }
    #endregion
    #region Controls
    private void GetInput()
    {
        _move.x = Input.GetAxis("Horizontal");
        _move.z = Input.GetAxis("Vertical");
        _look.x = Input.GetAxis("Mouse X");
        _look.y = Input.GetAxis("Mouse Y");
    }
    private void SendInput()
    {
        _hero.MoveHero(_move);
        _hero.CameraRotate(_look);
        _hero.CameraFollow();
        Dash();
    }
    private void Dash()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _hero.Dash();
        }       
    }
    #endregion
}

