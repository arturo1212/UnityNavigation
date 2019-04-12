using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Side {
    public Vector3 start;
    public Vector3 end;
    public Vector3 mid
    {
        get { return (end + start) / 2; }
        set { }
    }
    
}
