using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AnttiStarterKit.Animations
{
    public class Shaker : MonoBehaviour
    {
        [SerializeField] private float amount = 0.1f;
        [SerializeField] private float rotationAmount = 1f;
        [SerializeField] private float duration = 0.1f;
        [SerializeField] private bool decreasing;
        [SerializeField] private bool useLocalPosition;

        private Vector3 _startPos;
        private float _durationLeft;
        private float _startAngle;
        private Transform _t;

        public void Shake()
        {
            _t = transform;
            _durationLeft = duration;
            _startPos = useLocalPosition ? _t.localPosition : _t.position;
            _startAngle = _t.rotation.eulerAngles.z;
            StartCoroutine(ShakeRoutine());
        }

        public void ShakeForever()
        {
            duration = 9999f;
            Shake();
        }

        private void PositionTo(Vector3 pos)
        {
            if (useLocalPosition)
            {
                _t.localPosition = pos;
                return;
            }

            _t.position = pos;
        }

        private IEnumerator ShakeRoutine()
        {
            while (_durationLeft > 0)
            {
                _durationLeft -= Time.deltaTime;
                PositionTo(_durationLeft > 0 ? _startPos + GetOffset(AdjustedAmount()) : _startPos);
                var angle = _durationLeft > 0 ? _startAngle + AdjustedAngleAmount() : _startAngle;
                transform.localRotation = Quaternion.Euler(0, 0, angle);
                yield return null;
            }
        }

        private float AdjustedAmount()
        {
            return decreasing ? Mathf.Lerp(0, amount, _durationLeft / duration) : amount;
        }
    
        private float AdjustedAngleAmount()
        {
            return decreasing ? Mathf.Lerp(0, rotationAmount, _durationLeft / duration) : rotationAmount;
        }

        private static Vector3 GetOffset(float max)
        {
            return new Vector3(Random.Range(-max, max), Random.Range(-max, max), 0);
        }

        public void StopShaking()
        {
            if (_durationLeft <= 0) return;
            _durationLeft = 0f;
            PositionTo(_startPos);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
