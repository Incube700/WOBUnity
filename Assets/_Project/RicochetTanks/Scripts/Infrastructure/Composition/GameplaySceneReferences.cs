using RicochetTanks.Gameplay.Tanks;
using RicochetTanks.UI.Sandbox;
using UnityEngine;

namespace RicochetTanks.Infrastructure.Composition
{
    [System.Serializable]
    public sealed class GameplaySceneReferences
    {
        public GameplaySceneReferences(
            Transform arenaRoot,
            Transform playerSpawnPoint,
            Transform enemySpawnPoint,
            TankFacade playerTank,
            TankFacade enemyDummyTank,
            Transform cameraRig,
            Camera camera,
            Canvas gameplayCanvas,
            SandboxHudView hudView,
            Transform combatFeedbackRoot)
        {
            ArenaRoot = arenaRoot;
            PlayerSpawnPoint = playerSpawnPoint;
            EnemySpawnPoint = enemySpawnPoint;
            PlayerTank = playerTank;
            EnemyDummyTank = enemyDummyTank;
            CameraRig = cameraRig;
            Camera = camera;
            GameplayCanvas = gameplayCanvas;
            HudView = hudView;
            CombatFeedbackRoot = combatFeedbackRoot;
        }

        public Transform ArenaRoot { get; private set; }
        public Transform PlayerSpawnPoint { get; private set; }
        public Transform EnemySpawnPoint { get; private set; }
        public TankFacade PlayerTank { get; private set; }
        public TankFacade EnemyDummyTank { get; private set; }
        public Transform CameraRig { get; private set; }
        public Camera Camera { get; private set; }
        public Canvas GameplayCanvas { get; private set; }
        public SandboxHudView HudView { get; set; }
        public Transform CombatFeedbackRoot { get; private set; }

        public void ResolveMissing(Transform root)
        {
            ArenaRoot = ArenaRoot != null ? ArenaRoot : root.Find("ArenaRoot");
            PlayerSpawnPoint = PlayerSpawnPoint != null ? PlayerSpawnPoint : FindDescendant(root, "PlayerSpawnPoint");
            EnemySpawnPoint = EnemySpawnPoint != null ? EnemySpawnPoint : FindDescendant(root, "EnemySpawnPoint");
            PlayerTank = PlayerTank != null ? PlayerTank : ResolveTankReference(root, "PlayerTank");
            EnemyDummyTank = EnemyDummyTank != null ? EnemyDummyTank : ResolveTankReference(root, "EnemyDummyTank");
            CameraRig = CameraRig != null ? CameraRig : FindDescendant(root, "CameraRig");
            Camera = Camera != null ? Camera : root.GetComponentInChildren<Camera>(true);
            GameplayCanvas = GameplayCanvas != null ? GameplayCanvas : root.GetComponentInChildren<Canvas>(true);
            HudView = HudView != null ? HudView : root.GetComponentInChildren<SandboxHudView>(true);
            CombatFeedbackRoot = CombatFeedbackRoot != null ? CombatFeedbackRoot : FindDescendant(root, "CombatFeedbackRoot");
        }

        public void SetGameplayCanvas(Canvas gameplayCanvas)
        {
            GameplayCanvas = gameplayCanvas;
        }

        public void SetCombatFeedbackRoot(Transform combatFeedbackRoot)
        {
            CombatFeedbackRoot = combatFeedbackRoot;
        }

        public void EnsureCombatFeedbackRoot(Transform parent)
        {
            if (CombatFeedbackRoot != null)
            {
                return;
            }

            var root = new GameObject("CombatFeedbackRoot").transform;
            root.SetParent(parent, false);
            root.localPosition = Vector3.zero;
            CombatFeedbackRoot = root;
        }

        private static TankFacade ResolveTankReference(Transform root, string objectName)
        {
            var child = FindDescendant(root, objectName);
            if (child == null)
            {
                return null;
            }

            if (child.TryGetComponent<TankFacade>(out var tank))
            {
                return tank;
            }

            return child.gameObject.AddComponent<TankFacade>();
        }

        private static Transform FindDescendant(Transform root, string objectName)
        {
            if (root == null)
            {
                return null;
            }

            if (root.name == objectName)
            {
                return root;
            }

            for (var index = 0; index < root.childCount; index++)
            {
                var result = FindDescendant(root.GetChild(index), objectName);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
