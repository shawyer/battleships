using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class ShipTests
    {
        private Ship ship;
        private ShipName shipName;
        private int _sizeofship;
        private string _shipname;
        private int _hitcount;

        [TestMethod()]
        public void IsShipNameCorrect()
        {
            shipName = ShipName.AircraftCarrier;
            ship = new Ship(shipName);
            _shipname = ship.Name;
            
            Assert.AreEqual(_shipname, "Aircraft Carrier");
            
        }

        [TestMethod()]
        public void checkSubmarineSize()
        {
            shipName = ShipName.Submarine;
            ship = new Ship(shipName);
            _sizeofship = ship.Size;

            Assert.AreEqual(_sizeofship, 2);
        }

        [TestMethod()]
        public void checkDestroyerSize()
        {
            shipName = ShipName.Destroyer;
            ship = new Ship(shipName);
            _sizeofship = ship.Size;

            Assert.AreEqual(_sizeofship, 3);
        }

        [TestMethod()]
        public void addHit()
        {
            shipName = ShipName.Submarine;
            ship = new Ship(shipName);
            ship.Hit();
            _hitcount = ship.Hits;

            Assert.AreEqual(_hitcount, 1);
        }
    }
}