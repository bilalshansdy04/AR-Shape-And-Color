using UnityEngine;
using UnityEngine.EventSystems;

public class DropZone : MonoBehaviour, IDropHandler
{
    // Jadikan public agar bisa di-assign dari Inspector Unity
    public DragAndDropQuizManager quizManager;

    public void SetQuizManager(DragAndDropQuizManager manager)
    {
        quizManager = manager;
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragItem droppedItem = eventData.pointerDrag.GetComponent<DragItem>();

        // Pastikan quizManager sudah terhubung dari Inspector
        if (droppedItem != null && quizManager != null)
        {
            quizManager.CekJawaban(droppedItem);

            droppedItem.transform.SetParent(this.transform);
            droppedItem.transform.position = this.transform.position;
        }
        else
        {
            Debug.LogWarning("Quiz Manager belum dihubungkan ke DropZone di Inspector!");
        }
    }
}
