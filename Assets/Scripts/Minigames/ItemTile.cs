using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StormWaits
{
    public class ItemTile : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private Image background;
        [SerializeField] private Image icon;
        [SerializeField] private TMP_Text label;
        [SerializeField] private CanvasGroup canvasGroup;

        public event Action<ItemTile, Vector2> Dropped;

        public SelectChoice Choice { get; private set; }

        private Transform dragLayer;     
        private Transform homeParent;   
        private int homeIndex;
        private bool packed;
        private bool dragging;
        private Coroutine flashRoutine;

        public void Setup(SelectChoice choice, Transform layerForDragging)
        {
            Choice = choice;
            dragLayer = layerForDragging;
            packed = false;
            dragging = false;

            label.text = choice.label;

            if (choice.icon != null)
            {
                icon.enabled = true;
                icon.sprite = choice.icon;
                icon.color = Color.white;
                label.alignment = TextAlignmentOptions.Bottom;
            }
            else
            {
                icon.enabled = false;
                label.alignment = TextAlignmentOptions.Center;
            }

            background.color = GameSettings.Current.tileColor;
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }

        private void Update()
        {
            if (dragging && !MinigameManager.CursorFree)
            {
                dragging = false;
                ReturnHome();
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (packed || !MinigameManager.CursorFree) return;

            dragging = true;
            homeParent = transform.parent;
            homeIndex = transform.GetSiblingIndex();

            transform.SetParent(dragLayer, true);
            transform.SetAsLastSibling();

            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = GameSettings.Current.dragAlpha;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!dragging) return;

            transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!dragging) return;
            dragging = false;

            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;

            if (Dropped != null) Dropped(this, eventData.position);

            if (transform.parent == dragLayer) ReturnHome();
        }

        public void ReturnHome()
        {
            if (homeParent == null) return;

            transform.SetParent(homeParent, false);
            transform.SetSiblingIndex(homeIndex);
            canvasGroup.blocksRaycasts = true;
            canvasGroup.alpha = 1f;
        }

        public void PackInto(Transform bagContent)
        {
            packed = true;
            transform.SetParent(bagContent, false);
            background.color = GameSettings.Current.packedTileColor;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.alpha = 1f;
        }

        
        public void FlashWrong()
        {
            if (flashRoutine != null) StopCoroutine(flashRoutine);
            flashRoutine = StartCoroutine(Flash());
        }

        private IEnumerator Flash()
        {
            GameSettings settings = GameSettings.Current;

            background.color = settings.wrongDropColor;
            yield return new WaitForSecondsRealtime(settings.wrongFlashSeconds);
            background.color = settings.tileColor;

            flashRoutine = null;
        }
    }
}
