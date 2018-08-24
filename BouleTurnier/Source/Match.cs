using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouleTurnier.Source
{
    public class Match
    {
        MainWindow mainWindow;
        private int _team1Points;
        private int _team2Points;

        public Match(MainWindow newOwner)
        {
            mainWindow = newOwner;
        }

        public Match()
        {

        }

        public void SetWindowManager(MainWindow newOwner)
        {
            mainWindow = newOwner;
        }

        public Team Team1 { get; set; }
        public int Team1Number
        {
            get
            {
                return Team1.Number;
            }
        }
        public string Team1Description
        {
            get
            {
                return Team1.Description;
            }
        }
        public int Team1Points
        {
            get => _team1Points;
            set
            {
                _team1Points = value;
                mainWindow?.SaveSelectedTournament();
            }
        }

        public Team Team2 { get; set; }
        public int Team2Number
        {
            get
            {
                return Team2.Number;
            }
        }
        public string Team2Description
        {
            get
            {
                return Team2.Description;
            }
        }

        public int Team2Points
        {
            get => _team2Points;
            set
            {
                _team2Points = value;
                mainWindow?.SaveSelectedTournament();
            }
        }

        public bool CompareMatch(Team team1, Team team2)
        {
            if (Team1.EqualsTeam(team1) && Team2.EqualsTeam(team2))
            {
                return true;
            }

            if (Team1.EqualsTeam(team2) && Team2.EqualsTeam(team1))
            {
                return true;
            }

            return false;
        }

        public Team TeamThatWon
        {
            get
            {
                if (Team1Points > Team2Points)
                    return Team1;
                else if (Team2Points > Team1Points)
                    return Team2;
                return null;
            }
        }

        public void CalculatePoints(int PointsPerWin)
        {
            Team1.AddPoints(Team1Points - Team2Points);
            Team2.AddPoints(Team2Points - Team1Points);
        }

        public void UpdatePlayerConnection()
        {
            Team1.UpdatePlayerConnections(mainWindow);
            Team2.UpdatePlayerConnections(mainWindow);
        }

        public void UpdateTeamConnections()
        {
            Team temp = mainWindow.teamManager.FindTeam(Team1);
            if (temp != null)
            {
                Team1 = temp;
            }
            else
            {
                if (Team1 != null)
                    Team1.UpdatePlayerConnections(mainWindow);
            }
            temp = mainWindow.teamManager.FindTeam(Team2);
            if (temp != null)
            {
                Team2 = temp;
            }
            else
            {
                if (Team2 != null)
                    Team2.UpdatePlayerConnections(mainWindow);
            }

        }

        public string ExportInfo()
        {
            return String.Format("{0};{1};{2};VS;{3};{4};{5}", Team1Number, Team1Description, Team1Points, Team2Points, Team2Description, Team2Number);
        }

        public bool CompareMatchUsingPlayers(Team team1, Team team2)
        {
            if (Team1.PlayersEqual(team1.Players.ToArray()) && Team2.PlayersEqual(team2.Players.ToArray()))
            {

                return true;
            }

            if (Team1.PlayersEqual(team2.Players.ToArray()) && Team2.PlayersEqual(team1.Players.ToArray()))
            {
                return true;
            }

            return false;
        }

        public int GetEnemieNumberForTeam(Team team)
        {
            if (Team1 != null && Team1.EqualsTeam(team))
                return Team2Number;
            if (Team2 != null && Team2.EqualsTeam(team))
                return Team1Number;


            return -1;
        }
    }
}
