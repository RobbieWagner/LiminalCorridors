using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RND : MonoBehaviour
{
    public System.Random rnd;

    // Start is called before the first frame update
    void Start()
    {
        rnd = new System.Random();
    }
}
