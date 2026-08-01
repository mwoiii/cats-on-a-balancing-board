using Unity.Entities;

public partial struct ScoreSystem : ISystem {
    public void OnUpdate(ref SystemState state) {
        if (!GameLogicScript.gameRunning || HUDController.instance == null) { return; }

        double deltaTime = SystemAPI.Time.DeltaTime;
        GameLogicScript.score += HUDController.instance.catCount * deltaTime * GameLogicScript.scoreScaleFactor;
    }
}
