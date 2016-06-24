using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace BouleTurnier.Source
{
    [Serializable]
    public class TeamManager
    {
        private MainWindow mainWindow;
        public SObservableCollection<Team> Teams = new SObservableCollection<Team>();

        public SObservableCollection<Player> AvaiblePlayers = new SObservableCollection<Player>();

        public TeamManager(MainWindow newOwner)
        {
            mainWindow = newOwner;
        }

        public void SetWindowManager(MainWindow newOwner)
        {
            mainWindow = newOwner;
        }

        public Team AddNewTeam(params Player[] spieler)
        {
            Team newTeam = new Team();
            newTeam.Number = Teams.Count + 1;
            ReplaceTeamPlayers(newTeam, spieler);

            Teams.Add(newTeam);
            return newTeam;
        }

        public void ReplaceTeamPlayers(Team team, params Player[] players)
        {
            team.Players.Clear();
            for (int i = 0; i < players.Length; i++)
            {
                AddPlayerToTeam(team, players[i]);
            }
        }
        public void AddPlayerToTeam(Team team, Player player)
        {
            AvaiblePlayers.Remove(player);
            team.AddPlayer(player);
        }
        public void RemovePlayerFromTeam(Team team, Player player)
        {
            AvaiblePlayers.Add(player);
            team.RemovePlayer(player);
        }

        public void SortByNumber(bool ascend = true)
        {
            if (ascend)
            {
                Teams.Sort((x, y) => x.Number.CompareTo(y.Number));
            }
            else
            {
                Teams.DescendingSort((x, y) => x.Number.CompareTo(y.Number));
            }
        }

        public void SortByPoints(bool ascend = true)
        {
            if (ascend)
            {
                Teams.Sort((x, y) => x.Points.CompareTo(y.Points));
            }
            else
            {
                Teams.DescendingSort((x, y) => x.Points.CompareTo(y.Points));
            }
        }

        public void ReplaceTeamList(Team[] newTeams)
        {
            Teams.Clear();
            foreach (var item in newTeams)
                Teams.Add(item);
        }

        public void AddAvaiblePlayer(Player player)
        {
            AvaiblePlayers.Add(player);
        }

        public void RemoveAvaiblePlayer(Player player)
        {
            if (AvaiblePlayers.Contains(player))
                AvaiblePlayers.Remove(player);
        }

        public TeamManager()
        {

        }

        //Only use this if you have global Teams
        //Don't use with supermile
        public void UpdateTeamConnections()
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                for (int a = 0; a < Teams[i].Players.Count; a++)
                {
                    Teams[i].Players[a].CurrentTeam = Teams[i];
                }
            }
        }

        public void UpdateAfterLoad()
        {
            UpdatePlayerConnection();
            UpdateTeamDescription();
        }

        private void UpdatePlayerConnection()
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                Teams[i].UpdatePlayerConnections(mainWindow);
            }

            Player[] currentPlayers = AvaiblePlayers.ToArray();
            AvaiblePlayers.Clear();

            Player newPlayer = null;
            for (int i = 0; i < currentPlayers.Length; i++)
            {
                newPlayer = mainWindow.playerManager.FindPlayer(currentPlayers[i]);
                if (newPlayer != null)
                {
                    AvaiblePlayers.Add(newPlayer);
                }

            }
        }

        private void UpdateTeamDescription()
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                Teams[i].UpdateDescription();
            }
        }

        public Team FindTeam(Team team)
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i].EqualsTeam(team))
                    return Teams[i];
            }
            return null;
        }

        public bool ContainsTeamWithPlayers(Player[] players)
        {
            for (int i = 0; i < Teams.Count; i++)
            {
                if (Teams[i].PlayersEqual(players))
                    return true;
            }
            return false;
        }

        public void RemoveTeam(Team team)
        {
            if (Teams.Contains(team))
                Teams.Remove(team);
        }
    }
}
