using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : InterfaceMethod.TableData
{ 
    int _index;
    string _name;
    int _characterCost;

    public int INDEX
    {
        get { return _index; }
        set { _index = value; }
    }

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public int CharacterCost
    {
        get { return _characterCost; }
        set { _characterCost = value; }
    }
}
