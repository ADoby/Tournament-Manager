using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BouleTurnier.Source
{
    [Serializable()]
    public class Player : INotifyPropertyChanged
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

        private string name = "PlayerName";
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                TriggerPropertyChanges("Name");
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

        [field: NonSerialized]
        private Team currentTeam = null;
        public Team CurrentTeam
        {
            get
            {
                return currentTeam;
            }
            set
            {
                currentTeam = value;
                TriggerPropertyChanges("CurrentTeam");
            }
        }

        //Benötigt man vllt. später
        private string license = "";
        public string License
        {
            get
            {
                return license;
            }
            set
            {
                license = value;
                TriggerPropertyChanges("License");
            }
        }

        public Player()
        {

        }

        public bool EqualsPlayer(int otherNumber, string otherName)
        {
            return (Number == otherNumber && Name.Equals(otherName, StringComparison.OrdinalIgnoreCase));
        }
        public bool EqualsPlayer(Player player)
        {
            return EqualsPlayer(player.Number, player.Name);
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}", Number, Name);
        }

        public string ExportInfo()
        {
            return String.Format("{0};{1};{2};{3}", Number, Name, Points, License);
        }
        
        public static Player ParseFromString(string info)
        {
            string[] values = info.Split(';');
            if (values.Length != 4)
                return null;
            Player newPlayer = new Player();

            bool result;

            int newNumber;
            result = int.TryParse(values[0], out newNumber);
            if (!result)
                return null;
            newPlayer.Number = newNumber;

            newPlayer.Name = values[1];

            int newPoints;
            result = int.TryParse(values[2], out newPoints);
            if (!result)
                return null;
            newPlayer.Points = newPoints;

            newPlayer.License = values[3];

            return newPlayer;
        }
    }
}
