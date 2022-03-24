using System;
using ntw.CurvedTextMeshPro;
using TMPro;
using UnityEngine;

namespace EL.UI.Components
{
    [ExecuteInEditMode]
    public class TextMeshRenderByCurve : TextProOnACurve
    {
        [SerializeField] private AnimationCurve curve = new AnimationCurve();
        [SerializeField] private float curveMul = 12f;
        [SerializeField] private float maxCharsCount = 14;

        private AnimationCurve _oldCurve;
        private float _oldMul;
        private float _oldRotation;

        protected override bool ParametersHaveChanged()
        {
            var currentRotation = transform.rotation.eulerAngles.y;
            var ret =
                !curve.Equals(_oldCurve) ||
                Math.Abs(curveMul - _oldMul) > .01f ||
                Math.Abs(currentRotation - _oldRotation) > .01f;
            _oldCurve = curve;
            _oldMul = curveMul;
            _oldRotation = currentRotation;
            return ret;
        }

        protected override Matrix4x4 ComputeTransformationMatrix(
            Vector3 charMidBaselinePos,
            float zeroToOnePos,
            TMP_TextInfo textInfo,
            int charIdx)
        {
            var charsDiff = Mathf.Max(0f, maxCharsCount - textInfo.characterCount);
            var deltaChars = charsDiff / 2f;
            var k = deltaChars + charIdx;
            var yMod = curve.Evaluate(k / maxCharsCount) * curveMul;

            var x0 = charMidBaselinePos.x;
            var y0 = charMidBaselinePos.y + yMod;
            var newMidBaselinePos = new Vector2(x0,
                y0 - textInfo.lineInfo[0].lineExtents.max.y * textInfo.characterInfo[charIdx].lineNumber);

            return Matrix4x4.TRS(
                new Vector3(newMidBaselinePos.x, newMidBaselinePos.y, 0),
                Quaternion.AngleAxis(0, Vector3.forward), 
                Vector3.one);
        }
    }
}