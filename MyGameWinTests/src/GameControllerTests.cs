using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass()]
    public class GameControllerTests
    {
        private static BattleShipsGame _theGame;
        private static AIPlayer _ai;

        [TestMethod()]
        public void CheckAIDifficultyMedium()
        {
            _ai = new AIMediumPlayer(_theGame);
            Assert.AreEqual(_ai.ToString(), "AIMediumPlayer");

        }

        [TestMethod()]
        public void CheckAIDifficultyHard()
        {
            _ai = new AIHardPlayer(_theGame);
            Assert.AreEqual(_ai.ToString(), "AIHardPlayer");
        }


    }
}