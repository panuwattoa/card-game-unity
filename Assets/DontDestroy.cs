using System.Collections;
using System.Collections.Generic;
using Scripts.Utils;
using UnityEngine;

public class DontDestroy : Singleton<DontDestroy>
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

}
