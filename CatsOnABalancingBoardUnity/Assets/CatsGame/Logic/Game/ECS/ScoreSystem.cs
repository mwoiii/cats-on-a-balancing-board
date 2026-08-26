using OMC.UI;
using Unity.Entities;

namespace OMC.ECS {
    public partial struct ScoreSystem : ISystem {
        public void OnUpdate(ref SystemState state) {
            if (!GameLogicScript.gameRunning || !HUDController.instance) {
                return;
            }

            double deltaTime = SystemAPI.Time.DeltaTime;
            GameLogicScript.instance.AddToScore(GameLogicScript.instance.catCount * deltaTime * GameLogicScript.scoreScaleFactor);
        }
    }
}
