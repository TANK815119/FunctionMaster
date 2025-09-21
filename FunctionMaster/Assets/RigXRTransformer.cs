using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Rendering;

namespace Rekabsen
{
    public class RigXRTransformer : MonoBehaviour
    {
        [SerializeField] private Transform headset;
        [SerializeField] private Transform leftController;
        [SerializeField] private Transform rightController;
        [SerializeField] private float turnSpeed = 120f;

        public float Scale { get; set; }

        private InputData inputData;
        private float turn;


        // Start is called before the first frame update
        void OnEnable()
        {
            inputData = GetComponent<InputData>();
            RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;

            Scale = 1f;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
        }

        // Update is called once per frame
        void Update()
        {
            //TransformDevices();
        }
        void OnBeginCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            TransformDevices();
        }

        void TransformDevices()
        {
            //headset
            if (inputData.headset.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 headsetPosition))
            {
                headset.localPosition = Quaternion.Euler(0f, turn, 0f) * headsetPosition * (1f / Scale);

            }
            if (inputData.headset.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion headsetRotation))
            {
                bool angleLimited = false;
                float angle = headsetRotation.eulerAngles.x;
                if (headsetRotation.eulerAngles.x > 45f && headsetRotation.eulerAngles.x <= 90f)
                {
                    headsetRotation = Quaternion.Euler(0f, headsetRotation.eulerAngles.y, headsetRotation.eulerAngles.z);
                    angleLimited = true;
                }

                headsetRotation = Quaternion.Euler(headsetRotation.eulerAngles.x, headsetRotation.eulerAngles.y, 0f);

                if (angleLimited || true) //true to avoid snapping
                {
                    headset.localRotation = Quaternion.Euler(0f, turn, 0f) * Quaternion.Slerp(headset.transform.localRotation, headsetRotation, Mathf.Pow(2f * ((90f - angle) / 45f), 2f) * Time.deltaTime);
                }
                else
                {
                    //Debug.Log("problem 2");
                    headset.localRotation = Quaternion.Euler(0f, turn, 0f) * headsetRotation;
                }
            }

            //left controller
            if (inputData.leftController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 leftPosition))
            {
                leftController.localPosition = Quaternion.Euler(0f, turn, 0f) * leftPosition * (1f / Scale);
            }
            if (inputData.leftController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion leftRotation))
            {
                leftController.localRotation = Quaternion.Euler(0f, turn, 0f) * leftRotation;
            }

            //rightController
            if (inputData.rightController.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 rightPosition))
            {
                rightController.localPosition = Quaternion.Euler(0f, turn, 0f) * rightPosition * (1f / Scale);
            }
            if (inputData.rightController.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rightRotation))
            {
                rightController.localRotation = Quaternion.Euler(0f, turn, 0f) * rightRotation;
            }

            if (inputData.rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 thumbstick))
            {
                if (Mathf.Abs(thumbstick.x) > 0.5f)
                {
                    turn += turnSpeed * thumbstick.x * Time.deltaTime;  //delta tiem ought to work, even here

                }
            }
        }
    }
}