using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BouleTurnier.Source
{
    [Serializable()]
    public class PlayerManager : INotifyPropertyChanged
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

        private MainWindow mainWindow;

        public SObservableCollection<Player> Players { get; set; }

        public PlayerManager()
        {
            Players = new SObservableCollection<Player>();
        }

        public PlayerManager(MainWindow newOwner)
        {
            mainWindow = newOwner;
        }

        public Player FindPlayer(int Number, string Name)
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].EqualsPlayer(Number, Name))
                    return Players[i];
            }
            return null;
        }

        public Player FindPlayer(Player player)
        {
            return FindPlayer(player.Number, player.Name);
        }

        public Player AddPlayer()
        {
            Player newPlayer = new Player();
            newPlayer.Name = "PlayerName";
            AddPlayer(newPlayer);

            return newPlayer;
        }
        public void AddPlayer(Player player)
        {
            if (player != null && !Players.Contains(player))
            {
                player.Number = Players.Count + 1;
                Players.Add(player);

                mainWindow.tournament.teamManager.AddAvaiblePlayer(player);
            }
        }
        public void AddPlayers(params Player[] player)
        {
            for (int i = 0; i < player.Length; i++)
            {
                AddPlayer(player[i]);
            }
        }
        public void RemovePlayer(Player player, bool remakeNumbers = false)
        {
            if (remakeNumbers)
            {
                for (int i = 0; i < Players.Count; i++)
                {
                    if (Players[i].Number > player.Number)
                    {
                        Players[i].Number--;
                    }
                }
            }
            Players.Remove(player);
            mainWindow.tournament.teamManager.RemoveAvaiblePlayer(player);
            
        }

        public void SortByNumber(bool ascend = true)
        {
            if (ascend)
            {
                Players.Sort((x, y) => x.Number.CompareTo(y.Number));
            }
            else
            {
                Players.DescendingSort((x, y) => x.Number.CompareTo(y.Number));
            }
        }

        public void SortByName(bool ascend = true)
        {
            if (ascend)
            {
                Players.Sort((x, y) => x.Name.CompareTo(y.Name));
            }
            else
            {
                Players.DescendingSort((x, y) => x.Name.CompareTo(y.Name));
            }
        }

        public void SortByPoints(bool ascend = true)
        {
            if (ascend)
            {
                Players.Sort((x, y) => x.Points.CompareTo(y.Points));
            }
            else
            {
                Players.DescendingSort((x, y) => x.Points.CompareTo(y.Points));
            }
        }

        public void ReplacePlayerList(Player[] newPlayers)
        {
            Players.Clear();
            foreach (var item in newPlayers)
                Players.Add(item);
        }

        public void SetWindowManager(MainWindow newOwner)
        {
            mainWindow = newOwner;
        }

        public void Save(string FileName)
        {
            using (var writer = new System.IO.StreamWriter(FileName))
            {
                var serializer = new XmlSerializer(this.GetType());
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static PlayerManager Load(string FileName)
        {
            using (var stream = System.IO.File.OpenRead(FileName))
            {
                var serializer = new XmlSerializer(typeof(PlayerManager));
                return serializer.Deserialize(stream) as PlayerManager;
            }
        }



        public void RecreatePlayerList()
        {
            Players = new SObservableCollection<Player>();
        }
    }
}
