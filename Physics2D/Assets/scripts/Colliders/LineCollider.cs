using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor;
using UnityEngine;

public class LineCollider : PhysicsComponent
{
    private static readonly bool _static = true;
    private Vector2 _lineStart;
    private Vector2 _lineCenter;

    private Vector2 _lineEnd;

    private Vector2 _lineDirVec;
    // Start is called before the first frame update
    void Start()
    {
        Info = GetComponent<PhysicsInfo>();

        Info.OldPosition = new Vector2(transform.position.x, transform.position.y);
        Info.NewPosition = new Vector2(transform.position.x, transform.position.y);

        _lineCenter = Info.OldPosition;
        float length = transform.localScale.y;
        Vector3 orientation = transform.eulerAngles;
        Vector2 dir = Vector2.up;
        dir = Vector2DFunctions.RotateVec(dir, orientation.z);
        
        _lineStart = -dir * length / 2 + _lineCenter;
        _lineEnd = dir * length / 2 + _lineCenter;
        _lineDirVec = dir;
        
    }

    public Vector2 GetLineStart()
    {
        return _lineStart;
    }
    public Vector2 GetLineEnd()
    {
        return _lineEnd;
    }

    public Vector2 GetLineDir()
    {
        return _lineDirVec;

    }
    public override bool IsStatic()
    {
        return _static;
    }
}
