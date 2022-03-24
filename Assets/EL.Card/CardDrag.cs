using System;
using DG.Tweening;
using EL.Common;
using EL.Common.Exts;
using UnityEngine;
using UnityEngine.EventSystems;

namespace EL.Card
{
    public class CardDrag : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        private bool _inDrag;
        private Vector3 _mousePosition;
        private RaycastHit[] _raycastHits;
        private Tweener _rotateAnimation;
        public Camera Cam { get; set; }
        public IDraggable Self { get; set; }
        public bool AllowDrag { get; set; }

        private void Awake()
        {
            _raycastHits = new RaycastHit[32];
        }

        private void Update()
        {
            StopAnimation();
            if (!_inDrag)
                return;
            var currentMousePosition = CursorWorldPosition();
            var delta = currentMousePosition - _mousePosition;
            _mousePosition = currentMousePosition;
            var currentPos = transform.position;
            currentPos.x += delta.x;
            currentPos.z += delta.z;
            transform.position = currentPos;
            var isForward = delta.z > 0f;
            var sign = isForward ? 1f : -1f;
            if (Mathf.Approximately(delta.z, 0f)) sign = 0f;
            _rotateAnimation = transform.DOLocalRotate(new Vector3(sign * 10f, 0f, 0f), .2f);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!AllowDrag)
                return;
            _mousePosition = CursorWorldPosition();
            _inDrag = true;
            OnDragStart?.Invoke();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (!_inDrag || !AllowDrag)
                return;
            _inDrag = false;

            var targetFound = false;
            var count = Physics.RaycastNonAlloc(new Ray(_mousePosition.WithY(100), Vector3.down), _raycastHits, 100f);
            for (var i = 0; i < count; i++)
            {
                var col = _raycastHits[i].collider;
                var dragSupport = col.GetComponent<IDraggableTarget>();
                if (dragSupport != null && dragSupport.IsAllowDrop(Self))
                {
                    targetFound = true;
                    OnDragComplete(dragSupport);
                    break;
                }
            }

            if (!targetFound)
                OnDragCancel?.Invoke();
        }

        public event Action OnDragStart;
        public event Action OnDrop;
        public event Action OnDragCancel;

        public void OnDragComplete(IDraggableTarget target)
        {
            OnDrop?.Invoke();
            target.OnDrop(Self);
            _inDrag = false;
        }

        private void StopAnimation()
        {
            if (_rotateAnimation != null && _rotateAnimation.active)
            {
                DOTween.Kill(_rotateAnimation);
                _rotateAnimation = null;
            }
        }

        private Vector3 CursorWorldPosition()
        {
            return Cam.ScreenToWorldPoint(Input.mousePosition.WithZ(Cam.transform.position.y - transform.position.y));
        }
    }
}