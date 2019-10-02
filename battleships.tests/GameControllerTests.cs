using GameController;

[TestClass]
public class GameControllerTests
{
    private GameController gameController;

    public GameControllerTests()
    {
        gameController = new GameController();
    }

    [TestMethod]
    public void isGamestartInDeploying()
    {
        gameController.StartGame();
        var result = gameController.CurrentState;

        Assert.AreEqual("Deploying", result, false, "Gamestate is not in deployment phase at bootup");
    }

    [TestMethod]
    public void isEndDeploymentInDiscovering()
    {
        gameController.EndDeployment();
        var result = gameController.CurrentState;

        Assert.AreEqual("Discovering", result, false, "Gamestate is not Discovering");
    }

    [TestMethod]
    public void isAddNewStateAddingState()
    {
        gameController.AddNewState(GameState.ViewingMainMenu);
        var result = gameController.CurrentState;

        Assert.AreEqual("ViewingMainMenu", result, false, "Gamestate is not ViewingMainMenu");
    }

    [TestMethod]
    public void isSwitchStateSwitchingState()
    {
        gameController.SwitchState(GameState.AlteringSettings);
        var result = gameController.CurrentState;

        Assert.AreEqual("AlteringSettings", result, false, "Gamestate is not AlteringSettings");
    }

    [TestMethod]
    public void isEndCurrentStateEndingState()
    {
        gameController.AddNewState(GameState.ViewingMainMenu);
        gameController.AddNewState(GameState.AlteringSettings);
        gameController.EndCurrentState();
        var result = gameController.CurrentState;

        Assert.AreEqual("ViewingMainMenu", result, false, "Gamestate is not ViewingMainMenu");
    }

    [TestMethod]
    public void canWeSetDifficulty()
    {
        gameController.SetDifficulty(AIOption.Hard);
        var result = gameController._aiSetting;

        Assert.AreEqual("Hard", result, false, "AIOption is not set to Hard");
    }

    [TestMethod]
    public void canWeSetDifficultyMedium()
    {
        gameController.SetDifficulty(AIOption.Medium);
        var result = gameController._aiSetting;

        Assert.AreEqual("Medium", result, false, "AIOption is not set to Medium");
    }

    [TestMethod]
    public void canWeSetDifficultyEasy()
    {
        gameController.SetDifficulty(AIOption.Easy);
        var result = gameController._aiSetting;

        Assert.AreEqual("Easy", result, false, "AIOption is not set to Easy");
    }

}
