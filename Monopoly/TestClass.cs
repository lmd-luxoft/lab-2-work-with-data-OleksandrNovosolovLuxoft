// NUnit 3 tests
// See documentation : https://github.com/nunit/docs/wiki/NUnit-Documentation
using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace Monopoly
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void GetPlayersListReturnCorrectList()
        {
            string[] players = new string[]{ "Peter","Ekaterina","Alexander" };
            Player[] expectedPlayers = new Player[]
            {
                new Player("Peter",6000),
                new Player("Ekaterina",6000),
                new Player("Alexander",6000)
            };
            Monopoly monopoly = new Monopoly(players);
            Player[] actualPlayers = monopoly.GetPlayersList().ToArray();

            CollectionAssert.AreEquivalent(expectedPlayers, actualPlayers);
        }
        [Test]
        public void GetFieldsListReturnCorrectList()
        {
            Field[] expectedCompanies = 
                new Field[]{
                new Field("Ford",Monopoly.MonopolyType.AUTO,0),
                new Field("MCDonald", Monopoly.MonopolyType.FOOD, 0),
                new Field("Lamoda", Monopoly.MonopolyType.CLOTHER, 0),
                new Field("Air Baltic",Monopoly.MonopolyType.TRAVEL,0),
                new Field("Nordavia",Monopoly.MonopolyType.TRAVEL,0),
                new Field("Prison",Monopoly.MonopolyType.PRISON,0),
                new Field("MCDonald",Monopoly.MonopolyType.FOOD,0),
                new Field("TESLA",Monopoly.MonopolyType.AUTO,0)
            };
            string[] players = new string[] { "Peter", "Ekaterina", "Alexander" };
            Monopoly monopoly = new Monopoly(players);
            Field[] actualCompanies = monopoly.GetFieldsList().ToArray();
            CollectionAssert.AreEquivalent(expectedCompanies, actualCompanies);
        }
        [Test]
        public void PlayerBuyNoOwnedCompanies()
        {
            string[] players = new string[] { "Peter", "Ekaterina", "Alexander" };
            Monopoly monopoly = new Monopoly(players);
            Field x = monopoly.GetFieldByName("Ford");
            monopoly.Buy(1, x);
            Player actualPlayer = monopoly.GetPlayerInfo(1);
            Player expectedPlayer = new Player("Peter", 5500);
            Assert.AreEqual(expectedPlayer, actualPlayer);
            Field actualField = monopoly.GetFieldByName("Ford");
            Assert.AreEqual(1, actualField.OwnerId);
        }
        [Test]
        public void RentaShouldBeCorrectTransferMoney()
        {
            string[] players = new string[] { "Peter", "Ekaterina", "Alexander" };
            Monopoly monopoly = new Monopoly(players);
            Field  x = monopoly.GetFieldByName("Ford");
            monopoly.Buy(1, x);
            x = monopoly.GetFieldByName("Ford");
            monopoly.Renta(2, x);
            Player player1 = monopoly.GetPlayerInfo(1);
            Assert.AreEqual(5750, player1.Amount);
            Player player2 = monopoly.GetPlayerInfo(2);
            Assert.AreEqual(5750, player2.Amount);
        }
    }
}
