using UnityEngine;

namespace Portakalcik
{
    public class Door : MonoBehaviour
    {
        public bool isOpen = false;
        public float openSpeed = 3.0f;
        public float sinkDistance = 3.2f;

        private Vector3 closedPos;
        private Vector3 targetPos;

        private void Start()
        {
            closedPos = transform.position;
            targetPos = closedPos;
        }

        private void Update()
        {
            if (transform.position != targetPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPos, openSpeed * Time.deltaTime);
            }
        }

        public void OpenDoor()
        {
            if (isOpen) return;
            isOpen = true;
            targetPos = closedPos - new Vector3(0, sinkDistance, 0);

            // Kapı üzerindeki collider'ı açılınca devre dışı bırak
            Collider col = GetComponent<Collider>();
            if (col != null)
            {
                col.enabled = false;
            }

            Debug.Log($"[Portakalcik] Kısayol Kapısı ({name}) açıldı!");
        }
    }
}
