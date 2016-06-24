using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BouleTurnier.Source
{
    
    [Serializable]
    public class Team : INotifyPropertyChanged
    {

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanges(string Property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(Property));
            }
        }

        public Team()
        {

        }

        //Für die normale Logik nötig
        private int number = 0;
        public int Number
        {
            get
            {
                return number;
            }
            set
            {
                number = value;
                TriggerPropertyChanges("Number");
            }
        }

        private int points = 0;
        public int Points
        {
            get
            {
                return points;
            }
            set
            {
                points = value;
                TriggerPropertyChanges("Points");
            }
        }

        private int wins = 0;
        public int Wins
        {
            get
            {
                return wins;
            }
            set
            {
                wins = value;
                TriggerPropertyChanges("Wins");
            }
        }

        private int smallPoints = 0;
        public int SmallPoints
        {
            get
            {
                return smallPoints;
            }
            set
            {
                smallPoints = value;
                TriggerPropertyChanges("SmallPoints");
            }
        }

        private int positivePoints = 0;
        public int PositivePoints
        {
            get
            {
                return positivePoints;
            }
            set
            {
                positivePoints = value;
                TriggerPropertyChanges("PositivePoints");
            }
        }

        
        public SObservableCollection<Player> Players = new SObservableCollection<Player>();

        public bool descriptionSetFromOutside = false;

        private string description = "Teamname/Description";
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                if (description != value)
                {
                    description = value;
                    if (description != GetAutomaticDescription())
                        descriptionSetFromOutside = true;
                    else
                    {
                        descriptionSetFromOutside = false;
                    }
                    TriggerPropertyChanges("Description");
                }
            }
        }

        public void UpdateDescription()
        {
            if (!descriptionSetFromOutside)
            {
                description = GetAutomaticDescription();
                TriggerPropertyChanges("Description");
            }
            else
            {
                if (description == GetAutomaticDescription())
                {
                    Description = GetAutomaticDescription();
                }
            }
        }

        public string GetAutomaticDescription()
        {
            string newDesc = "Teamname/Description";
            if (Players.Count > 0)
            {
                newDesc = Players[0].Name;
                for (int i = 1; i < Players.Count; i++)
                {
                    newDesc = String.Format("{0},{1}", newDesc, Players[i].Name);
                }
            }
            return newDesc;
        }

        public int PlayerCount
        {
            get
            {
                return Players.Count;
            }
        }
        public Player GetPlayer(int index)
        {
            if (index < 0 || index >= Players.Count)
                throw new IndexOutOfRangeException();

            return Players[index];
        }
        public void ReplacePlayers(params Player[] newPlayers)
        {
            Players.Clear();
            for (int i = 0; i < newPlayers.Length; i++)
            {
                if(newPlayers[i] != null)
                    Players.Add(newPlayers[i]);
            }
            UpdateDescription();
        }
        public Player AddPlayer(Player player = null)
        {
            if (player == null)
                player = new Player() { Name = "PlayerName" };
            Players.Add(player);

            UpdateDescription();
            return player;
        }
        public void RemovePlayer(Player player)
        {
            Players.Remove(player);
            UpdateDescription();
        }

        public void AddPoints(int value)
        {
            Points += value;
            for (int i = 0; i < Players.Count; i++)
            {
                Players[i].Points += value;
            }

            if (value > 0)
                PositivePoints += value;
        }

        public bool EqualsTeam(Team otherTeam)
        {
            if (otherTeam.Players.Count == 0 && Players.Count == 0)
            {
                bool result = true;

                result = (Number == otherTeam.Number);
                if (!result)
                    return false;

                result = (Description.Equals(otherTeam.Description, StringComparison.OrdinalIgnoreCase));
                return result;
            }
            return PlayersEqual(otherTeam.Players.ToArray());
        }
        public bool PlayersEqual(Player[] otherPlayers)
        {
            if (Players.Count == 0)
                return false;
            if (otherPlayers.Length == 0)
                return false;

            int EqualPlayerCount = 0;

            if (Players.Count != otherPlayers.Length)
                return false;

            for (int i = 0; i < otherPlayers.Length; i++)
            {
                for (int a = 0; a < Players.Count; a++)
                {
                    if (otherPlayers[i].EqualsPlayer(Players[a]))
                    {
                        //Debug.WriteLine("{0} same {1}", Players[a].ToString(), otherPlayers[i].ToString());
                        EqualPlayerCount++;
                    }
                }
            }

            if (EqualPlayerCount == Players.Count)
            {
                return true;
            }
            return false;
        }

        public string ExportInfo()
        {
            return String.Format("{0};{1};{2};{3}", Number, Description, Wins, SmallPoints);
        }

        public void UpdatePlayerConnections(MainWindow mainWindow)
        {
            Player[] currentPlayers = Players.ToArray();
            Players.Clear();

            Player newPlayer = null;
            for (int i = 0; i < currentPlayers.Length; i++)
            {
                newPlayer = mainWindow.playerManager.FindPlayer(currentPlayers[i]);
                if (newPlayer != null)
                {
                    Players.Add(newPlayer);
                }
                    
            }
        }
    }
}
