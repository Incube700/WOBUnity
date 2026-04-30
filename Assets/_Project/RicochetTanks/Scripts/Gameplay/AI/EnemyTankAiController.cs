using System;
using RicochetTanks.Gameplay.Events;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.AI
{
    [DisallowMultipleComponent]
    public sealed class EnemyTankAiController : MonoBehaviour
    {
        [SerializeField] private EnemyAiConfig _config;
        [SerializeField] private TankFacade _enemy;
        [SerializeField] private TankFacade _target;

        private EnemyTankBrain _brain;

        public void Configure(
            TankFacade enemy,
            TankFacade target,
            EnemyAiConfig config,
            SandboxGameplayEvents gameplayEvents)
        {
            DisposeBrain();

            _enemy = enemy;
            _target = target;
            _config = config != null ? config : ScriptableObject.CreateInstance<EnemyAiConfig>();

            if (_enemy == null || _target == null)
            {
                throw new InvalidOperationException("Enemy AI requires enemy and target tank references.");
            }

            _brain = new EnemyTankBrain(_enemy, _target, _config, gameplayEvents);
        }

        private void Update()
        {
            _brain?.Tick(Time.deltaTime);
        }

        private void OnDestroy()
        {
            DisposeBrain();
        }

        private void DisposeBrain()
        {
            _brain?.Dispose();
            _brain = null;
        }
    }
}
