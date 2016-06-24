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
        public int Team1Points { get; set; }

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
        public int Team2Points { get; set; }

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

        public void CalculatePoints(int PointsPerWin)
        {

            Team1.AddPoints(Team1Points - Team2Points);
            Team2.AddPoints(Team2Points - Team1Points);

            Team1.SmallPoints += (Team1Points - Team2Points);
            Team2.SmallPoints += (Team2Points - Team1Points);

            if (Team1Points > Team2Points)
            {
                Team1.Wins++;
                Team1.AddPoints(PointsPerWin);
            }
            if (Team2Points > Team1Points)
            {
                Team2.Wins++;
                Team2.AddPoints(PointsPerWin);
            }
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
