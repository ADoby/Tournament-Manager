using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BouleTurnier.Source
{
	public class TournamentManager
	{
		private MainWindow mainWindow;
		public SObservableCollection<Round> Rounds = new SObservableCollection<Round>();

		public TournamentManager(MainWindow newOwner)
		{
			mainWindow = newOwner;
		}

		public TournamentManager()
		{
		}

		public void SetWindowManager(MainWindow newOwner)
		{
			mainWindow = newOwner;
			for (int i = 0; i < Rounds.Count; i++)
			{
				Rounds[i].SetWindowManager(newOwner);
			}
		}

		public Round AddRound()
		{
			Round newRound = new Round(mainWindow);
			Rounds.Add(newRound);
			return newRound;
		}

		public bool GenerateRandomMatches(Round round, bool useGlobalTeams = true, bool allowDoubleMatches = true)
		{
			return round.GenerateRandomMatches(useGlobalTeams, allowDoubleMatches);
		}

		public bool MatchAlreadyExists(Team team1, Team team2)
		{
			bool result = false;
			for (int i = 0; i < Rounds.Count; i++)
			{
				result = Rounds[i].ContainsMatch(team1, team2);
				if (result)
					return true;
			}
			return false;
		}

		public void CalculatePoints()
		{
			for (int i = 0; i < mainWindow.tournament.teamManager.Teams.Count; i++)
			{
				mainWindow.tournament.teamManager.Teams[i].ResetPoints();
			}
			for (int i = 0; i < mainWindow.tournament.playerManager.Players.Count; i++)
			{
				mainWindow.tournament.playerManager.Players[i].ResetPoints();
			}
			for (int i = 0; i < Rounds.Count; i++)
			{
				Rounds[i].CalculatePoints();
			}
		}

		public int GetRoundIndex(Round currentRound)
		{
			for (int i = 0; i < Rounds.Count; i++)
			{
				if (Rounds[i] == currentRound)
				{
					return i;
				}
			}
			return -1;
		}

		public void RemoveRound(Round currentRound)
		{
			Rounds.Remove(currentRound);
		}

		public void RemoveRound(int index)
		{
			if (index < 0 || index >= Rounds.Count)
				return;
			Rounds.RemoveAt(index);
		}

		public void UpdateAfterLoad()
		{
			for (int i = 0; i < Rounds.Count; i++)
			{
				Rounds[i].UpdateAfterLoad();
			}
		}

		public bool GenerateRandomMatches(Round currentRound, bool globalTeams, bool randomTeams, bool allowSameMatches, bool allowSameTeams, bool allowFreeWin, int wantedTeamSize, int pointsPerWin, int pointsPerFreeWin)
		{
			return currentRound.GenerateRandomMatches(globalTeams, randomTeams, allowSameMatches, allowSameTeams, allowFreeWin, wantedTeamSize, pointsPerWin, pointsPerFreeWin);
		}

		public bool GenerateFinalMatches(Round currentRound, bool globalTeams, bool allowFreeWin, int wantedTeamsInFinal, int wantedTeamSize, int pointsPerWin, int pointsPerFreeWin)
		{
			return currentRound.GenerateFinalMatches(globalTeams, allowFreeWin, wantedTeamsInFinal, wantedTeamSize, pointsPerWin, pointsPerFreeWin);
		}

		public bool MatchAlreadyExistsUsingPlayers(Team team1, Team team2)
		{
			bool result = false;
			for (int i = 0; i < Rounds.Count; i++)
			{
				result = Rounds[i].ContainsMatchUsingPlayers(team1, team2);
				if (result)
				{
					return true;
				}
			}
			return false;
		}
	}
}