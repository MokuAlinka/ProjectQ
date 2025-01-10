using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RPGM.Gameplay
{
    /// <summary>
    /// A simple camera follower class. It saves the offset from the
    ///  focus position when started, and preserves that offset when following the focus.
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        public Transform focus;
        public Transform NormalTarget;
        public float smoothTime = 2;

        Vector3 offset;

        public void SetFocus(Transform newFocus)
        {
            focus = newFocus;
        }

        void Awake()
        {
            if (focus != null)
            {
                offset = focus.position - transform.position;
            }
            else if (NormalTarget != null)
            {
                focus = NormalTarget;
            }
        }

        void Update()
        {
            if (focus != null)
            {
                transform.position = Vector3.Lerp(transform.position, focus.position - offset, Time.deltaTime * smoothTime);
            }
        }
    }
}