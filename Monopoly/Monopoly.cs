using System;
using System.Collections.Generic;
using System.Linq;

namespace Monopoly
{
    class Player : IEquatable<Player>
    {
        public Player(string name, int amount)
        {
            Name = name;
            Amount = amount;
        }

        public string Name { get; }
        public int Amount { get; }

        public bool Equals(Player other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name
                && Amount == other.Amount;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            return Equals(obj as Player);
        }

        public override int GetHashCode()
        {
            return Amount.GetHashCode() + 17 * Name?.GetHashCode() ?? 0;
        }
    }

    class Field : IEquatable<Field>
    {
        public Field(string name, Monopoly.MonopolyType monopolyType, int ownerId)
        {
            Name = name;
            MonopolyType = monopolyType;
            OwnerId = ownerId;
        }

        public string Name { get; }
        public Monopoly.MonopolyType MonopolyType { get; }
        public int OwnerId { get; }

        public bool TryBuy(Player buyer, out Player owner)
        {
            return MonopolyType.TryBuy(buyer, this, out owner);
        }

        public bool TryRent(Player playerToRent, Player owner, out (Player Owner, Player RentedPlayer) parties)
        {
            return MonopolyType.TryRent(playerToRent, owner, this, out parties);
        }

        public bool Equals(Field other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name
                && MonopolyType == other.MonopolyType
                && OwnerId == other.OwnerId;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }

            return Equals(obj as Player);
        }

        public override int GetHashCode()
        {
            return OwnerId.GetHashCode() + 17 * Name?.GetHashCode() ?? 0 + 256 * MonopolyType?.GetHashCode() ?? 0;
        }
    }

    class Monopoly
    {
        private readonly List<Player> _players = new List<Player>();
        private readonly List<Field> _fields = new List<Field>();
        public Monopoly(string[] names)
        {
            for (int i = 0; i < names.Length; i++)
            {
                _players.Add(new Player(names[i], 6000));     
            }

            _fields.Add(new Field("Ford", Monopoly.MonopolyType.AUTO, 0));
            _fields.Add(new Field("MCDonald", Monopoly.MonopolyType.FOOD, 0));
            _fields.Add(new Field("Lamoda", Monopoly.MonopolyType.CLOTHER, 0));
            _fields.Add(new Field("Air Baltic", Monopoly.MonopolyType.TRAVEL, 0));
            _fields.Add(new Field("Nordavia", Monopoly.MonopolyType.TRAVEL, 0));
            _fields.Add(new Field("Prison", Monopoly.MonopolyType.PRISON, 0));
            _fields.Add(new Field("MCDonald", Monopoly.MonopolyType.FOOD, 0));
            _fields.Add(new Field("TESLA", Monopoly.MonopolyType.AUTO, 0));
        }

        internal List<Player> GetPlayersList()
        {
            return _players;
        }

        internal class MonopolyType
        {
            private readonly int? _price;
            private readonly int? _rentaIncome;
            private readonly int? _rentaPrice;
            private MonopolyType(int? price, int? rentaIncome, int? rentaPrice)
            {
                _price = price;
                _rentaIncome = rentaIncome;
                _rentaPrice = rentaPrice;
            }
            public static readonly MonopolyType AUTO = new MonopolyType(500, 250, 250);
            public static readonly MonopolyType FOOD = new MonopolyType(250, 250, 250);
            public static readonly MonopolyType CLOTHER = new MonopolyType(100, 1000, 100);
            public static readonly MonopolyType TRAVEL = new MonopolyType(700, 300, 300);
            public static readonly MonopolyType PRISON = new MonopolyType(null, 0, 1000);
            public static readonly MonopolyType BANK = new MonopolyType(null, 0, 700);

            public bool TryBuy(Player buyer, Field targetField, out Player owner)
            {
                owner = null;
                if (targetField.OwnerId > 0 || !_price.HasValue)
                {
                    return false;
                }

                owner = new Player(buyer.Name, buyer.Amount - _price.Value);
                return true;
            }

            public bool TryRent(Player playerToRent, Player owner, Field targetField, out (Player Owner, Player RentedPlayer) parties)
            {
                parties = default;
                if (targetField.OwnerId == 0)
                {
                    return false;
                }

                if (_rentaIncome > 0)
                {
                    owner = new Player(owner.Name, owner.Amount + _rentaIncome.Value);
                }

                if (_rentaPrice > 0)
                {
                    playerToRent = new Player(playerToRent.Name, playerToRent.Amount - _rentaPrice.Value);
                }

                parties = (owner, playerToRent);
                return true;
            }
        }

        internal List<Field> GetFieldsList()
        {
            return _fields;
        }

        internal Field GetFieldByName(string v)
        {
            return _fields.FirstOrDefault(p => p.Name == v);
        }

        internal bool Buy(int buyerId, Field targetField)
        {
            var buyer = GetPlayerInfo(buyerId);
            int fieldIndex = _fields.IndexOf(targetField);
            if (fieldIndex < 0)
            {
                return false;
            }    

            if (!targetField.TryBuy(buyer, out var owner))
            {
                return false;
            }

            SetPlayerInfo(buyerId, owner);
            _fields[fieldIndex] = new Field(targetField.Name, targetField.MonopolyType, buyerId);
             return true;
        }

        internal Player GetPlayerInfo(int playerId)
        {
            return _players[playerId - 1];
        }

        internal void SetPlayerInfo(int playerId, Player player)
        {
            _players[playerId - 1] = player;
        }

        internal bool Renta(int playerToRentId, Field targetField)
        {
            var playerToRent = GetPlayerInfo(playerToRentId);
            var owner = GetPlayerInfo(targetField.OwnerId);

            int fieldIndex = _fields.IndexOf(targetField);
            if (fieldIndex < 0)
            {
                return false;
            }

            if (!targetField.TryRent(playerToRent, owner, out var parties))
            {
                return false;
            }

            SetPlayerInfo(playerToRentId, parties.RentedPlayer);
            SetPlayerInfo(targetField.OwnerId, parties.Owner);
            return true;
        }
    }
}
