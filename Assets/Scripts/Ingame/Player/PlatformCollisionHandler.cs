using System;
using StartledSeal.Utils;
using UnityEngine;

namespace StartledSeal.Mechanics
{
    public class PlatformCollisionHandler : MonoBehaviour
    {
        private Transform _platform;

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.CompareTag("MovingPlatform"))
            {
                // If contact normal is pointing up, we've contact with top of platform
                ContactPoint contact = other.GetContact(0);
                if (contact.normal.y < 0.5f) return;
                
                _platform = other.transform;
                transform.SetParent(_platform);
            }
        }

        private void OnCollisionExit(Collision other)
        {
            if (other.gameObject.CompareTag(Const.MovingPlatform))
            {
                transform.SetParent(null);
                _platform = null;
            }        
        }
    }
}