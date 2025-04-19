using UnityEngine;

namespace _Scripts.Island
{
    public class ThreatIndicator : MonoBehaviour
    {
        [Header("Visual FX")]
        [SerializeField] private GameObject _vfxThreatGlow;
        [SerializeField] private GameObject _uiThreatMarker;

        private void Awake()
        {
            if (_vfxThreatGlow != null)
                _vfxThreatGlow.SetActive(false);

            if (_uiThreatMarker != null)
                _uiThreatMarker.SetActive(false);
        }

        public void SetThreatActive(bool active)
        {
            if (_vfxThreatGlow != null)
                _vfxThreatGlow.SetActive(active);

            if (_uiThreatMarker != null)
                _uiThreatMarker.SetActive(active);
        }
    }
}