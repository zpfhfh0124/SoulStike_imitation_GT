using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    public int _hp;
    public int _sp;

    public int PlayerHP { get { return _hp; } }
    public int PlayerSP { get { return _sp; } }
    
    public void AddHp(int value)
    {
        _hp += value;
    }

    public void AddSp(int value)
    {
        _sp += value;
    }
}
