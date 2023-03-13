using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
  RandomStream r;
  public int v;
  float f;
  void Start()
  {
    r = new RandomStream(2);
    f = v = 1000;
  }

  // Update is called once per frame
  void Update()
  {
    if (f == 0 || f == 1) return;
    f = r.nextFloat(0, 1, 4);
    v = (int)(10f*f);
    print(f);
  }
}
