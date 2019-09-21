using Xunit;
using GameController;

[TestClass]
public class GameControllerTests
{
    private GameController gameController;

    public PrimeService_IsPrimeShould()
    {
        gameController = new GameController();
    }

    [TestMethod]
    [Fact]
    public void isGamestartInDeploying()
    {
        gameController.StartGame();
        var result = gameController.CurrentState

        Assert.AreEqual("Deploying", result, false, "Gamestate is not in deployment phase at bootup");
    }
}
