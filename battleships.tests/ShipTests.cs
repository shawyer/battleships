using Ship;
using ShipName;

[TestClass]
public class ShipTests
{
    private Ship ship;


    [TestMethod]
    public void isShipNameCorrect()
    {
        shipName = new ShipName shipName;
        ship = new Ship(shipName.Submarine);
       
        var result = ship._shipName;

        Assert.AreEqual("Submarine", result, false, "ShipName is not Submarine");
    }

    [TestMethod]
    public void addHit()
    {
        shipName = new ShipName shipName;
        ship = new Ship(shipName.Submarine);
        ship.Hit();
        
        var result = ship.Hits;

        Assert.AreEqual(1, result);
    }

    [TestMethod]
    public void checkSubmarineSize()
    {
        shipName = new ShipName shipName;
        ship = new Ship(shipName.Submarine);
       
        var result = ship.Size;

        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public void checkDestroyerSize()
    {
        shipName = new ShipName shipName;
        ship = new Ship(shipName.Destroyer);
       
        var result = ship.Size;

        Assert.AreEqual(3, result);
    }

}
