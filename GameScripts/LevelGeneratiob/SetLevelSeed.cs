using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetLevelSeed : MonoBehaviour
{
    public string stringSeed;
    public bool useStringSeed;

    public int seed;
    public bool randomizeSeed;



    // Start is called before the first frame update
    void Awake()
    {
        if (useStringSeed)
        {
            seed = stringSeed.GetHashCode();
        }

        if (randomizeSeed)
        {
            seed = Random.Range(0, 99999);
        }


    }


}
