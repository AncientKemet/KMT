using UnityEngine;

public class DetectHits
{

  bool DetectHitInCircle(float maxradius, float minradius, Vector2 xztarget, Vector2 xzattack)
  {
    float distance = Mathf.Sqrt(Mathf.Pow(xztarget.x - xzattack.x, 2) + Mathf.Pow(xztarget.y - xzattack.y, 2));
    if (distance < maxradius && distance > minradius)
      return(true);
    else
      return(false);
  }

}
