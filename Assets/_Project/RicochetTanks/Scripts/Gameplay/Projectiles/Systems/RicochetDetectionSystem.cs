using UnityEngine;

namespace RicochetTanks.Gameplay.Projectiles.Systems
{
    public sealed class RicochetDetectionSystem : IProjectileFixedSystem
    {
        public void Tick(ProjectileEntity entity, float deltaTime)
        {
            if (entity.IsDestroyRequested || entity.RicochetRequest.IsActive)
            {
                return;
            }

            var previousPosition = entity.PreviousPosition.Value;
            var currentPosition = entity.Transform.position;
            var movement = currentPosition - previousPosition;
            var distance = movement.magnitude;

            if (distance < 0.001f)
            {
                return;
            }

            var castDirection = movement / distance;
            var hits = Physics.SphereCastAll(
                previousPosition,
                entity.Ricochet.CastRadius,
                castDirection,
                distance,
                entity.CollisionMask,
                QueryTriggerInteraction.Ignore);

            if (hits.Length == 0)
            {
                return;
            }

            SortHitsByDistance(hits);

            for (var index = 0; index < hits.Length; index++)
            {
                var hit = hits[index];

                if (entity.ShouldIgnoreHit(hit.collider))
                {
                    continue;
                }

                var request = entity.RicochetRequest;
                request.Set(hit.collider, hit.point, hit.normal, entity.MoveDirection.Value);
                entity.RicochetRequest = request;
                entity.GameplayEvents?.RaiseProjectileHit(entity.Projectile, hit.collider, hit.point, hit.normal, entity.MoveDirection.Value);
                return;
            }
        }

        private static void SortHitsByDistance(RaycastHit[] hits)
        {
            for (var index = 1; index < hits.Length; index++)
            {
                var current = hits[index];
                var previousIndex = index - 1;

                while (previousIndex >= 0 && hits[previousIndex].distance > current.distance)
                {
                    hits[previousIndex + 1] = hits[previousIndex];
                    previousIndex--;
                }

                hits[previousIndex + 1] = current;
            }
        }
    }
}
