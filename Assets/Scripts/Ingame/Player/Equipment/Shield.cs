using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartledSeal
{
    public class Shield : BaseEquipment
    {
        public override bool IsUsable() => true;

        public override async UniTask Use(Animator animatorComp)
        {
            animatorComp.CrossFade(_animHash, 0.001f);
            animatorComp.SetLayerWeight(1, 1);
        }

        public async UniTask DisableShield(Animator animatorComp)
        {
            animatorComp.SetLayerWeight(1, 0);
        }
    }
}