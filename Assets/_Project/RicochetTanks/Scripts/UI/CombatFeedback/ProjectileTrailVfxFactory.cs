using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Projectiles;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    internal sealed class ProjectileTrailVfxFactory
    {
        private readonly CombatVfxConfig _config;

        public ProjectileTrailVfxFactory(CombatVfxConfig config)
        {
            _config = config;
        }

        public void ConfigureProjectileTrail(Projectile projectile)
        {
            if (projectile == null || _config == null)
            {
                return;
            }

            if (!projectile.TryGetComponent<TrailRenderer>(out var trail))
            {
                trail = projectile.gameObject.AddComponent<TrailRenderer>();
            }

            trail.time = _config.ProjectileTrailLifetime;
            trail.startWidth = _config.ProjectileTrailWidth;
            trail.endWidth = 0f;
            trail.startColor = _config.ProjectileTrailStartColor;
            trail.endColor = _config.ProjectileTrailEndColor;
            trail.emitting = true;

            if (trail.material == null)
            {
                var trailMaterial = CombatVfxUtility.CreateMaterial(_config.ProjectileTrailStartColor);
                if (trailMaterial != null)
                {
                    trail.material = trailMaterial;
                }
            }
        }
    }
}
