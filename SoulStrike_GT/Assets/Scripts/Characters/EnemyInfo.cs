using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo
{
    public int _hp;
    public int _sp;
    
    public int EnemyHP { get { return _hp; } }
    public int EnemySP { get { return _sp; } }
    
    public void AddHp(int value)
    {
        _hp += value;
    }

    public void AddSp(int value)
    {
        _sp += value;
    }
}
