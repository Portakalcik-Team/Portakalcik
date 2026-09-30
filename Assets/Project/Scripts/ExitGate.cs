using UnityEngine;

namespace Portakalcik
{
    public class ExitGate : MonoBehaviour
    {
        [Header("Çıkış Kapısı Gereksinimleri")]
        [Tooltip("Denge dokümanına göre çıkış için en az 1 Portakal gerekir")]
        public int requiredOranges = 1;
        public float triggerRadius = 3.0f;

        private bool levelFinished = false;

        private void Update()
        {
            if (levelFinished) return;

            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj == null) return;

            float dist = Vector3.Distance(transform.position, playerObj.transform.position);
            if (dist <= triggerRadius)
            {
                PlayerController pc = playerObj.GetComponent<PlayerController>();
                if (pc != null)
                {
                    if (pc.portakalCount >= requiredOranges)
                    {
                        levelFinished = true;
                        pc.CompleteLevel();
                    }
                    else
                    {
                        pc.ShowMessage("🔒 Çıkış Kilitli! Çıkış kapısını açmak için en az 1 PORTAKAL bulmalısın!", 2.0f);
                    }
                }
            }
        }
    }
}
