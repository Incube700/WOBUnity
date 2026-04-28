using RicochetTanks.Gameplay.Combat;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.Gameplay.Events
{
    public readonly struct CombatFeedbackEvent
    {
        public CombatFeedbackEvent(
            Vector3 worldPoint,
            Vector3 worldNormal,
            TankFacade source,
            TankFacade target,
            HitResult result,
            float damage,
            float currentHp,
            float maxHp,
            ArmorHitInfo armorHit)
        {
            WorldPoint = worldPoint;
            WorldNormal = worldNormal;
            Source = source;
            Target = target;
            Result = result;
            Damage = damage;
            CurrentHp = currentHp;
            MaxHp = maxHp;
            ArmorHit = armorHit;
        }

        public Vector3 WorldPoint { get; }
        public Vector3 WorldNormal { get; }
        public TankFacade Source { get; }
        public TankFacade Target { get; }
        public HitResult Result { get; }
        public float Damage { get; }
        public float CurrentHp { get; }
        public float MaxHp { get; }
        public ArmorHitInfo ArmorHit { get; }
    }
}
