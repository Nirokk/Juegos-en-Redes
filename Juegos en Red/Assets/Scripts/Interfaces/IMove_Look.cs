using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMove_Look
{
    void Move();
    void LookDir();
    void SetPosition(Vector3 pos);
}
