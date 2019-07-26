using UnityEngine;

namespace Oxide.Plugins
{
    [Info("CH47NoScientistsBugFix", "Ultra", "1.0.1")]
    [Description("CH47 respawn if spawned out of livable map")]

    class CH47NoScientistsBugFix : RustPlugin
    {
        private uint overBorderTolerance = 100;

        private void OnEntitySpawned(BaseEntity Entity)
        {
            if (Entity == null) return;

            if (Entity is CH47Helicopter)
            {
                CH47Helicopter ch47 = (CH47Helicopter)Entity;
                Vector3 newPosition = GetFixedPosition(ch47.transform.position);

                if (ch47.transform.position != newPosition)
                {

                    timer.Once(1f, () => { ch47.Kill(); });
                    timer.Once(2f, () => { SpawnCH47Helicopter(newPosition); });
                    Puts("CH47Helicopter removed as it's out of map");
                }
            }
        }

        private Vector3 GetFixedPosition(Vector3 originalPosition)
        {
            int mapLimit = ConVar.Server.worldsize / 2;
            Vector3 newPosition = originalPosition;

            if (originalPosition.x < -(mapLimit) - overBorderTolerance) newPosition.x = -(mapLimit) - overBorderTolerance;
            if (originalPosition.x > mapLimit + overBorderTolerance) newPosition.x = mapLimit + overBorderTolerance;
            if (originalPosition.z < -(mapLimit) - overBorderTolerance) newPosition.z = -(mapLimit) - overBorderTolerance;
            if (originalPosition.z > mapLimit + overBorderTolerance) newPosition.z = mapLimit + overBorderTolerance;

            if (originalPosition != newPosition)
            {
                Puts(string.Format("CH47 out of map position detected: {0} | {1} | {2}", originalPosition.x, originalPosition.y, originalPosition.z));
            }

            return newPosition;
        }

        private void SpawnCH47Helicopter(Vector3 position)
        {
            Unsubscribe(nameof(OnEntitySpawned));
            var ch47 = (CH47HelicopterAIController)GameManager.server.CreateEntity("assets/prefabs/npc/ch47/ch47scientists.entity.prefab", position);
            if (ch47 == null) return;
            ch47.Spawn();
            Subscribe(nameof(OnEntitySpawned));
            Puts(string.Format("New CH47Helicopter spawned: {0} | {1} | {2}", ch47.transform.position.x, ch47.transform.position.y, ch47.transform.position.z));
        }
    }
}
