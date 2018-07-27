#region

using UnityEngine;
using UnityEngine.UI;

#endregion

namespace Gamekit2D
{
    public class MenuActivityController : MonoBehaviour
    {
        private Canvas[] m_Canvases = new Canvas[0];
        private GraphicRaycaster[] m_Raycasters = new GraphicRaycaster[0];

        private void Awake()
        {
            m_Canvases = GetComponentsInChildren<Canvas>(true);
            m_Raycasters = GetComponentsInChildren<GraphicRaycaster>(true);
        }

        public void SetActive(bool active)
        {
            for (var i = 0; i < m_Canvases.Length; i++) m_Canvases[i].enabled = active;

            for (var i = 0; i < m_Raycasters.Length; i++) m_Raycasters[i].enabled = active;
        }
    }
}