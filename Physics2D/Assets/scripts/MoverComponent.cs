﻿using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

public class MoverComponent : MonoBehaviour
{

    [SerializeField] private float _speed = 0;
    private PhysicsInfo _info;
    void Start()
    {
      
    }

    public void Init()
    {
        int randomInt = UnityEngine.Random.Range(0, 360);
        Vector2 dir = Vector2.up;
        dir = Vector2DFunctions.RotateVec(dir, randomInt);
        Debug.Log("random DIR:" + dir);
        _info = GetComponent<PhysicsInfo>();
        _info.Speed = _speed;
        _info.Direction = dir.normalized;
        _info.OldPosition = Vector2DFunctions.GetTransform2D(this);

    }
    public void Step()
    {
        if (_info.Speed != 0)
        {
            _info.OldPosition = _info.NewPosition;
            Vector2 currentPos = _info.OldPosition;
            _info.Velocity = _info.Direction * _info.Speed * Time.deltaTime;
            currentPos += _info.Velocity;
            //  Vector2DFunctions.Update2DTransform(currentPos, this);
            _info.NewPosition = currentPos;
        }
        else
        {
          //  Vector2 currentPos = Vector2DFunctions.GetTransform2D(this);
            _info.NewPosition = _info.OldPosition;
        }

    }

    public float GetSpeed()
    {
        return _speed;
    }
    public void SetSpeed(float newSpeed)
    {
        _speed = newSpeed;
    }
}
