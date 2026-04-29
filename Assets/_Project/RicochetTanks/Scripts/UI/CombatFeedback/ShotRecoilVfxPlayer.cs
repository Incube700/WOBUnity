using RicochetTanks.Configs;
using RicochetTanks.Gameplay.Tanks;
using UnityEngine;

namespace RicochetTanks.UI.CombatFeedback
{
    internal sealed class ShotRecoilVfxPlayer
    {
        private readonly CombatVfxConfig _config;

        public ShotRecoilVfxPlayer(CombatVfxConfig config)
        {
            _config = config;
        }

        public void PlayShotRecoil(TankFacade owner)
        {
            if (_config == null || owner == null)
            {
                return;
            }

            var target = CombatVfxUtility.FindDescendant(owner.transform, "Turret") ?? owner.transform;
            if (!target.TryGetComponent<ShotRecoilView>(out var recoil))
            {
                recoil = target.gameObject.AddComponent<ShotRecoilView>();
            }

            recoil.Play(_config.ShotRecoilDistance, _config.ShotRecoilDuration);
        }
    }
}
