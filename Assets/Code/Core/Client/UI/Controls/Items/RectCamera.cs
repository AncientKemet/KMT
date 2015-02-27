using UnityEngine;

namespace Client.UI.Controls.Items
{
    [ExecuteInEditMode]
    public class RectCamera : MonoBehaviour
    {

        public Vector3 size;
        public Renderer Target;
        public Camera camera;

        public float Margin;

        private Vector3[] pts = new Vector3[8];

        private void Update()
        {
            if (Target == null || camera == null)
                return;

            Bounds b = Target.bounds;

            Camera cam = tk2dUIManager.Instance.UICamera;

            //The object is behind us
            if (cam.WorldToScreenPoint(b.center).z < 0) return;

            //All 8 vertices of the bounds
            pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
            pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
            pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
            pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
            pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
            pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
            pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
            pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));

            //Get them in GUI space
            for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;

            //Calculate the min and max positions
            Vector3 min = pts[0];
            Vector3 max = pts[0];
            for (int i = 1; i < pts.Length; i++)
            {
                min = Vector3.Min(min, pts[i]);
                max = Vector3.Max(max, pts[i]);
            }

            //Construct a rect of the min and max positions and apply some margin
            Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            r.xMin -= Margin;
            r.xMax += Margin;
            r.yMin -= Margin;
            r.yMax += Margin;


            /*r.width /= Screen.width;
            r.height /= Screen.height;
            r.x /= Screen.width;
            r.y /= Screen.height;
            */

            r.y = Screen.height - r.y;
            camera.pixelRect = r;
        }
    }
}
