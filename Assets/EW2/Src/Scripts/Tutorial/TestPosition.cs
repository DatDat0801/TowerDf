using System;
using System.Collections;
using UnityEngine;

namespace EW2.Src.Scripts.Tutorial
{
    public class TestPosition : MonoBehaviour
    {

        [SerializeField] private CameraController _cameraController;
        [SerializeField] private float sizeTarget ;
        [SerializeField] private Transform position ;
        [SerializeField] private float speed ;
        
        private void Start()
        {
            _cameraController.ExecuteFocusPoint(sizeTarget, position.position, speed);
        }

        IEnumerator Test()
        {
            yield return new WaitForSeconds(1);
            Debug.Log("XXX");
        }

        private void FixedUpdate()
        {
           
        }

        private void Update()
        {
//            Debug.Log(transform.position);
        }
    }
}