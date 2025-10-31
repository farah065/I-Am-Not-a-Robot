using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace PixagonalGames.BillboardAndScrollingShaderGraphs
{
    /// <summary>
    /// This script can be applied to an object to make it always face a specific target object.
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        [Tooltip("This lets you change the billboarding type.")] [SerializeField]
        private BillboardingTypes _billboardingType;

        [Tooltip("This specifies which object this one should always face.")] [SerializeField]
        private Transform _target;

        [Tooltip(
            "This lets you apply an offset to the final horizontal and vertical angles of this object (in degrees). This gets applied every time the object's rotation is recalculated (aka every frame).")]
        [SerializeField]
        private Vector2 _offset;

        private BillboardRenderer r;

        public BillboardingTypes BillboardingType
        {
            get => _billboardingType;
            set { _billboardingType = value; }
        }

        public Vector2 Offset
        {
            get => _offset;
            set { _offset = value; }
        }

        public Transform Target
        {
            get => _target;
            set { _target = value; }
        }


        public enum BillboardingTypes
        {
            Both,
            HorizontalOnly,
            VerticalOnly
        }



        private Vector3 _direction;
        private Vector3 _direction_HorizontalOnly; // This is the same as _direction, except the y-coordinate has been set to 0 so there is no verticality to the vector.



        // Update is called once per frame
        void Update()
        {
            if (_target != null)
                DoBillboarding();
        }

        private void DoBillboarding()
        {
            // Calculate the vector from the target to the object this script is on.
            Vector3 targetPos = _target.transform.position;
            _direction = targetPos - transform.position;

            // Create a horizontal-only version of the direction vector by clearing the Y component.
            _direction_HorizontalOnly = new Vector3(_direction.x, 0, _direction.z);

            // Calculate the horizontal and vertical angles we need to rotate by to make this object face the target.
            // NOTE: The side of the object facing down the positive Z axis will be the one that faces the target by default.
            //       This can be adjusted in the inspector via the Offset option.
            float finalAngleH = CalculateHorizontalRotation();
            float finalAngleV = CalculateVerticalRotation();

            //Debug.Log($"FinalAngleH: {finalAngleH}    FinalAngleV: {finalAngleV}");

            Quaternion q = transform.rotation;
            q.eulerAngles = new Vector3(
                finalAngleV,
                finalAngleH,
                0);

            transform.rotation = q;


        }

        private float CalculateVerticalRotation()
        {
            // We need to calculate two dot products. The first tells us how the direction is relative to forward (the Z axis),
            // and the second tells us how the direction is relative to right (the X axis).
            float vDot  = Vector3.Dot(_direction.normalized, _direction_HorizontalOnly.normalized);
            float vDot2 = Vector3.Dot(_direction.normalized, Vector3.up.normalized);

            // Now we can calculate the angle between the horizontalOnly vector and Vector3.forward.
            float vAngleInDegrees = Mathf.Acos(vDot) * Mathf.Rad2Deg;

            // We need to adjust the angle if vDot2 has gone negative by inverting it.
            // This is necessary, because otherwise the object will not face the target correctly when vDot2 is negative.
            // For example, in the test scene when the camera goes below the billboarded object, you'd see it start to tilt back up
            // rather than continuing to tilt downwards more as the camera descends.
            float finalAngleV = vDot2 < 0 ? vAngleInDegrees : 360f - vAngleInDegrees;
            if (_billboardingType == BillboardingTypes.HorizontalOnly)
                finalAngleV = 0f;

            // Finally, apply the vertical offset value.
            finalAngleV += _offset.y;

            return finalAngleV;
        }

        private float CalculateHorizontalRotation()
        {
            // We need to calculate two dot products. The first tells us how the direction is relative to forward (the Z axis),
            // and the second tells us how the direction is relative to right (the X axis).
            float hDot  = Vector3.Dot(_direction_HorizontalOnly.normalized, Vector3.forward.normalized);
            float hDot2 = Vector3.Dot(_direction_HorizontalOnly.normalized, Vector3.right.normalized);

            // Now we can calculate the angle between the horizontalOnly vector and Vector3.forward.
            float hAngleInDegrees = Mathf.Acos(hDot) * Mathf.Rad2Deg;

            // We need to adjust the angle if hDot2 has gone negative by inverting it.
            // This is necessary, because otherwise the object will not face the target correctly when hDot2 is negative.
            // For example, in the test scene when the camera goes below the billboarded object, you'd see it start to tilt back up
            // rather than continuing to tilt downwards more as the camera descends.
            float finalAngleH = hDot2 >= 0 ? hAngleInDegrees : 360f - hAngleInDegrees;
            if (_billboardingType == BillboardingTypes.VerticalOnly)
                finalAngleH = 0f;

            // Finally, apply the horizontal offset value.
            finalAngleH += _offset.x;

            return finalAngleH;
        }

    }
}