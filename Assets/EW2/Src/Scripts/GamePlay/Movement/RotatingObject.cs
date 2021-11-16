using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace EW2
{
    public class RotatingObject : MonoBehaviour
    {
        public bool IsRotate { get; set; } 
    
        [SerializeField] private float speed;
    

        // Update is called once per frame
        private void Update()
        {
            Rotate();
        }
        
        public void ResetAndStopRotate()
        {
            transform.localRotation = Quaternion.identity;;
            IsRotate = false;
        }

        private void Rotate()
        {
            if (IsRotate)
            {
                transform.Rotate(0, 0, speed * Time.deltaTime);
            }
        }

       
    }
}


