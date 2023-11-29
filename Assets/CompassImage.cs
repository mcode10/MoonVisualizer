using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
public class CompassImage : MonoBehaviour
{
   public RectTransform compassBarTransform;

   public RectTransform northMarkerTransform;
   public RectTransform southMarkerTransform;
   public RectTransform eastMarkerTransform;
   public RectTransform westMarkerTransform;

   public Transform cameraObjectTransform;

void Update ()
{
    SetMarkerPosition(northMarkerTransform, Vector3.forward * 1000);
    SetMarkerPosition(southMarkerTransform, Vector3.back * 1000);
    SetMarkerPosition(eastMarkerTransform, Vector3.right * 1000);
    SetMarkerPosition(westMarkerTransform, Vector3.left * 1000);
}
   private void SetMarkerPosition(RectTransform markerTransform, Vector3 worldPosition)
   {
    Vector3 directionToTarget = worldPosition - cameraObjectTransform.position;
    float angle = Vector2.SignedAngle(new Vector2(directionToTarget.x, directionToTarget.z), new Vector2(cameraObjectTransform.transform.forward.x, cameraObjectTransform.transform.forward.z));
    float compassPositionX = Mathf.Clamp(2 * angle / Camera.main.fieldOfView, -1, 1);
    markerTransform.anchoredPosition = new Vector2(compassBarTransform.rect.width/2*compassPositionX, 0);
   }
}
 
 