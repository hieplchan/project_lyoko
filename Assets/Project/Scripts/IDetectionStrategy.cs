using StartledSeal.Utils;
using UnityEngine;

namespace StartledSeal
{
    interface IDetectionStrategy
    {
        bool Execute(Transform player, Transform detector, CooldownTimer timer);
    }
}