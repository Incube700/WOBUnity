using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Components
{
    public readonly struct ProjectileTagComponent
    {
    }

    public struct PreviousPositionComponent
    {
        public PreviousPositionComponent(Vector3 value)
        {
            Value = value;
        }

        public Vector3 Value { get; set; }
    }

    public struct MoveDirectionComponent
    {
        public MoveDirectionComponent(Vector3 value)
        {
            Value = value;
        }

        public Vector3 Value { get; set; }
    }

    public struct MoveSpeedComponent
    {
        public MoveSpeedComponent(float value)
        {
            Value = value;
        }

        public float Value { get; set; }
    }

    public struct DamageComponent
    {
        public DamageComponent(float value, int penetration)
        {
            Value = value;
            Penetration = penetration;
        }

        public float Value { get; set; }
        public int Penetration { get; }
    }

    public struct LifetimeComponent
    {
        public LifetimeComponent(float spawnTime, float lifetime, float safeOwnerTime)
        {
            SpawnTime = spawnTime;
            Lifetime = lifetime;
            SafeOwnerTime = safeOwnerTime;
        }

        public float SpawnTime { get; }
        public float Lifetime { get; }
        public float SafeOwnerTime { get; }
    }

    public struct RicochetComponent
    {
        public RicochetComponent(
            int bouncesLeft,
            float castRadius,
            float positionOffset,
            float speedMultiplierPerBounce,
            float minSpeed,
            float damageMultiplierPerBounce)
        {
            BouncesLeft = bouncesLeft;
            RicochetCount = 0;
            CastRadius = castRadius;
            PositionOffset = positionOffset;
            SpeedMultiplierPerBounce = speedMultiplierPerBounce;
            MinSpeed = minSpeed;
            DamageMultiplierPerBounce = damageMultiplierPerBounce;
        }

        public int BouncesLeft { get; set; }
        public int RicochetCount { get; set; }
        public float CastRadius { get; }
        public float PositionOffset { get; }
        public float SpeedMultiplierPerBounce { get; }
        public float MinSpeed { get; }
        public float DamageMultiplierPerBounce { get; }
    }

    public struct RicochetRequestComponent
    {
        public bool IsActive { get; private set; }
        public Collider Collider { get; private set; }
        public Vector3 HitPoint { get; private set; }
        public Vector3 HitNormal { get; private set; }
        public Vector3 IncomingDirection { get; private set; }

        public void Set(Collider collider, Vector3 hitPoint, Vector3 hitNormal, Vector3 incomingDirection)
        {
            IsActive = true;
            Collider = collider;
            HitPoint = hitPoint;
            HitNormal = hitNormal;
            IncomingDirection = incomingDirection;
        }

        public void Clear()
        {
            IsActive = false;
            Collider = null;
            HitPoint = Vector3.zero;
            HitNormal = Vector3.zero;
            IncomingDirection = Vector3.zero;
        }
    }
}
