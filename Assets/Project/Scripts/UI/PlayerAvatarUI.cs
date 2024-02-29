using SuperMaxim.Messaging;
using UnityEngine;

namespace StartledSeal
{
    public class PlayerAvatarUI : MonoBehaviour
    {
        [SerializeField] private UIOutline _outline;

        private void Awake()
        {
            Messenger.Default.Subscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);
        }

        private void OnDestroy()
        {
            Messenger.Default.Unsubscribe<PlayerDeadEventPayload>(HandlePlayerDeadEvent);
        }
        
        private void HandlePlayerDeadEvent(PlayerDeadEventPayload payload)
        {
            _outline.color = Color.red;
        }
    }
}