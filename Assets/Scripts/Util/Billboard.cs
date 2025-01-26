using UnityEngine;

namespace Scenes.Alex.Scripts
{
    public class Billboard : MonoBehaviour
    {
        public Transform cam;

        private void Awake()
        {
            cam = GameObject.FindGameObjectWithTag("MainCamera").transform;
        }

        private void LateUpdate()
        {
            transform.LookAt(transform.position + cam.forward);
        }
    }
}
