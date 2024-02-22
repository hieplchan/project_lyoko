using StartledSeal.Common;
using UnityEngine;
using UnityEngine.AI;

namespace StartledSeal
{
    public sealed class PetFollowPlayerRunState : PetBaseState
    {
        private readonly NavMeshAgent _agent;
        private Transform _player;
        private float _speed;
        private float _angularSpeed;
        
        public PetFollowPlayerRunState(Pet pet, Animator animator, NavMeshAgent agent, 
            Transform player, float speed, float angularSpeed) : base(pet, animator)
        {
            _agent = agent;
            _player = player;
            _speed = speed;
            _angularSpeed = angularSpeed;
        }
        
        public override void OnEnter()
        {
            MLog.Debug("PetFollowPlayerState", "OnEnter");
            _animator.CrossFade(RunHash, CrossDuration);
            _agent.speed = _speed;
            _agent.angularSpeed = _angularSpeed;
        }
     
        public override void Update()
        {
            _agent.SetDestination(_player.position);
        }
    }
}