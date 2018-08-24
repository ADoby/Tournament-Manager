using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouleTurnier.Source
{
	public class Round : INotifyPropertyChanged
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
		public SObservableCollection<Match> Matches = new SObservableCollection<Match>();

		public SObservableCollection<Team> Teams = new SObservableCollection<Team>();

		public bool GlobalTeams = false;

		public bool AllowSameMatches = false;

        public bool IsFinal = false;

		public Round(MainWindow newOwner)
		{
			mainWindow = newOwner;
		}

		public Round()
		{
		}

		public void SetWindowManager(MainWindow newOwner)
		{
			mainWindow = newOwner;
			for (int i = 0; i < Matches.Count; i++)
			{
				Matches[i].SetWindowManager(newOwner);
			}
		}

		#region MatchMaking

		public bool GenerateRandomMatches(bool useGlobalTeams, bool allowDoubleMatches)
		{
			GlobalTeams = useGlobalTeams;
			AllowSameMatches = allowDoubleMatches;
			bool result = TryRandomMatching();
			return result;
		}

		public bool TryGeneratingTrandomTeams(out Team[] teams)
		{
			if (WantedTeamSize == 0)
			{
				teams = new Team[0];
				return false;
			}

			Player[] allPlayers;
			allPlayers = mainWindow.playerManager.Players.ToArray();

			int failedCount = 0;
			int tryCount = 0;

			bool failed = false;

			List<Player> avaiblePlayers = new List<Player>();
			avaiblePlayers.AddRange(allPlayers);

			Random random = new Random();

			int[] indexes = new int[WantedTeamSize];
			int direction = -1;
			Player[] players = new Player[WantedTeamSize];

			for (int i = 0; i < WantedTeamSize; i++)
			{
				players[i] = null;
			}
			Team team;

			List<Team> newTeams = new List<Team>();

			for (int i = 0; i < allPlayers.Length; i += WantedTeamSize)
			{
				do
				{
					if (avaiblePlayers.Count < WantedTeamSize)
					{
						Team freeTeam = new Team();
						freeTeam.Number = newTeams.Count + 1;
						for (int a = 0; a < avaiblePlayers.Count; a++)
						{
							freeTeam.AddPlayer(avaiblePlayers[a]);
						}
						newTeams.Add(freeTeam);
						break;
					}

					failed = false;
					for (int a = 0; a < indexes.Length; a++)
					{
						indexes[a] = random.Next(0, avaiblePlayers.Count);
					}

					for (int a = 0; a < indexes.Length; a++)
					{
						direction = random.Next(0, 2);
						if (direction == 0)
							direction = -1;
						tryCount = 0;

						do
						{
							indexes[a] += direction;

							if (indexes[a] < 0)
								indexes[a] = avaiblePlayers.Count - 1;
							if (indexes[a] >= avaiblePlayers.Count)
								indexes[a] = 0;

							bool isOk = true;
							for (int b = 0; b < indexes.Length; b++)
							{
								if (a != b && indexes[a] == indexes[b])
								{
									isOk = false;
									break;
								}
							}
							if (isOk)
								break;

							tryCount++;
							if (tryCount > avaiblePlayers.Count)
							{
								//When tried every player, break out
								failed = true;
								break;
							}
						} while (true);

						if (failed)
							break;
					}

					if (!failed && !AllowSameTeams && mainWindow.teamManager.ContainsTeamWithPlayers(players))
					{
						failed = true;
					}

					if (!failed)
					{
						for (int a = 0; a < players.Length; a++)
						{
							players[a] = avaiblePlayers[indexes[a]];
						}

						team = new Team();
						team.Number = newTeams.Count + 1;
						for (int c = 0; c < players.Length; c++)
						{
							team.AddPlayer(players[c]);
							avaiblePlayers.Remove(players[c]);
						}
						team.UpdateDescription();
						newTeams.Add(team);
						break;
					}

					failedCount++;
					if (failedCount >= 50)
					{
						teams = new Team[0];
						return false;
					}
				} while (true);
			}
			teams = newTeams.ToArray();
			return true;
		}

		private bool TryFinalMatching()
		{
			int failedCount = 0;
			int tryCount = 0;

			bool failed = false;
            
            var allTeams = new List<Team>();
            //If there is another final use its teams for our final
            //else use global teams (we are the first final)
            var lastFinal = mainWindow.tournamentManager.Rounds.LastOrDefault(m => m.IsFinal && m != this);

            if (lastFinal != null)
            {
                //Only use teams that won the last game
                allTeams = lastFinal.Teams.Where(team => lastFinal.Matches.Any(match => match.TeamThatWon == team)).ToList();
            }
            else
            {
                if (GlobalTeams)
                {
                    allTeams = mainWindow.tournament.teamManager.Teams.ToList();
                }
                else
                {
                    //TODO Create random Teams
                }
            }

            //Order descending
            allTeams.Sort(Team.PointsComparer());
            allTeams.Reverse();

            var avaibleTeams = allTeams.Take(WantedTeamsInFinal).ToList();
            
			int matchCount = avaibleTeams.Count / 2;

			Random random = new Random();

			int index1, index2;
			int direction = -1;
			Team team1, team2;

			for (int i = 0; i < matchCount; i++)
			{
				failedCount = 0;
				tryCount = 0;
				do
				{
					if (avaibleTeams.Count == 1)
					{
						index1 = 0;

						//Only one left
						Team freeTeam = new Team();
						if (AllowFreeWin)
						{
							freeTeam.Description = "Free Team";
						}
						else
						{
							freeTeam.Description = "No Team";
						}

						team1 = avaibleTeams[index1];
						team2 = freeTeam;

						Match newMatch = AddMatch(team1, team2);
						if (AllowFreeWin)
							newMatch.Team1Points = PointsPerFreeWin;
						avaibleTeams.Remove(team1);
						avaibleTeams.Remove(team2);
						break;
					}

					failed = false;
					index1 = random.Next(0, avaibleTeams.Count);
					index2 = random.Next(0, avaibleTeams.Count);

					direction = random.Next(0, 2);
					if (direction == 0)
						direction = -1;

					do
					{
						index2 += direction;

						if (index2 < 0)
							index2 = avaibleTeams.Count - 1;
						if (index2 >= avaibleTeams.Count)
							index2 = 0;

						if (index1 != index2 && (AllowSameMatches || !mainWindow.tournament.tournamentManager.MatchAlreadyExistsUsingPlayers(avaibleTeams[index1], avaibleTeams[index2])))
						{
							break;
						}

						//Fail Save
						tryCount++;
						if (tryCount > avaibleTeams.Count)
						{
							//When tried every player, break out
							failed = true;
							break;
						}
					} while (true);

					if (!failed)
					{
						team1 = avaibleTeams[index1];
						team2 = avaibleTeams[index2];
						AddMatch(team1, team2);
						avaibleTeams.Remove(team1);
						avaibleTeams.Remove(team2);
						break;
					}

					failedCount++;
					if (failedCount >= 20)
					{
						Debug.WriteLine("To many Same Team");
						return false;
					}
				} while (true);
			}
			SortTeamsByNumber(true);

			return true;
		}

		private bool TryRandomMatching()
		{
			Team[] allTeams = new Team[0];
			if (GlobalTeams)
			{
				allTeams = mainWindow.tournament.teamManager.Teams.ToArray();
			}

			if (RandomTeams)
			{
				//Create Random Teams here
				bool result = TryGeneratingTrandomTeams(out allTeams);
				Debug.WriteLine(String.Format("RandomTeams: {0}, and something {1}", allTeams.Length, result));
				if (!result)
					return false;
			}

			int failedCount = 0;
			int tryCount = 0;

			bool failed = false;

			List<Team> avaibleTeams = new List<Team>();
			avaibleTeams.AddRange(allTeams);

			Random random = new Random();

			int index1, index2;
			int direction = -1;
			Team team1, team2;

			for (int i = 0; i < allTeams.Length; i += 2)
			{
				failedCount = 0;
				tryCount = 0;
				do
				{
					if (avaibleTeams.Count == 1)
					{
						index1 = 0;

						//Only one left
						Team freeTeam = new Team();
						if (AllowFreeWin)
						{
							freeTeam.Description = "Free Team";
						}
						else
						{
							freeTeam.Description = "No Team";
						}

						team1 = avaibleTeams[index1];
						team2 = freeTeam;

						Match newMatch = AddMatch(team1, team2);
						if (AllowFreeWin)
							newMatch.Team1Points = PointsPerFreeWin;
						avaibleTeams.Remove(team1);
						avaibleTeams.Remove(team2);
						break;
					}

					failed = false;
					index1 = random.Next(0, avaibleTeams.Count);
					index2 = random.Next(0, avaibleTeams.Count);

					direction = random.Next(0, 2);
					if (direction == 0)
						direction = -1;

					do
					{
						index2 += direction;

						if (index2 < 0)
							index2 = avaibleTeams.Count - 1;
						if (index2 >= avaibleTeams.Count)
							index2 = 0;

						if (index1 != index2 && (AllowSameMatches || !mainWindow.tournament.tournamentManager.MatchAlreadyExistsUsingPlayers(avaibleTeams[index1], avaibleTeams[index2])))
						{
							break;
						}

						//Fail Save
						tryCount++;
						if (tryCount > avaibleTeams.Count)
						{
							//When tried every player, break out
							failed = true;
							break;
						}
					} while (true);

					if (!failed)
					{
						team1 = avaibleTeams[index1];
						team2 = avaibleTeams[index2];
						AddMatch(team1, team2);
						avaibleTeams.Remove(team1);
						avaibleTeams.Remove(team2);
						break;
					}

					failedCount++;
					if (failedCount >= 20)
					{
						Debug.WriteLine("To many Same Team");
						return false;
					}
				} while (true);
			}
			SortTeamsByNumber(true);
			return true;
		}

		public void ClearMatches()
		{
			Teams.Clear();
			Matches.Clear();
		}

		public Match AddMatch(Team team1, Team team2)
		{
			Match newMatch = new Match(mainWindow);
			newMatch.Team1 = team1;
			newMatch.Team2 = team2;
			Matches.Add(newMatch);

			if (!Teams.Contains(team1))
				Teams.Add(team1);
			if (!Teams.Contains(team2))
				Teams.Add(team2);

			return newMatch;
		}

		public bool ContainsMatch(Team team1, Team team2)
		{
			bool result = false;
			for (int matchIndex = 0; matchIndex < Matches.Count; matchIndex++)
			{
				result = Matches[matchIndex].CompareMatch(team1, team2);
				if (result)
					return true;
			}
			return result;
		}

		#endregion MatchMaking

		public void CalculatePoints()
		{
			if (!GlobalTeams)
			{
				for (int i = 0; i < Teams.Count; i++)
				{
                    Teams[i].ResetPoints();
				}
			}

			for (int i = 0; i < Matches.Count; i++)
			{
				Matches[i].CalculatePoints(PointsPerWin);
			}
		}

		#region LoadingAndSaving

		public void UpdatePlayerConnectionInMatches()
		{
			for (int i = 0; i < Matches.Count; i++)
			{
				Matches[i].UpdatePlayerConnection();
			}
		}

		private void UpdateTeamDescription()
		{
			for (int i = 0; i < Matches.Count; i++)
			{
				Matches[i].Team1.UpdateDescription();
				Matches[i].Team2.UpdateDescription();
			}
		}

		private void LoadAllTeamsFromMatchesIntoTeams()
		{
			Teams.Clear();
			for (int i = 0; i < Matches.Count; i++)
			{
				if (!Teams.Contains(Matches[i].Team1))
				{
					Teams.Add(Matches[i].Team1);
				}
				if (!Teams.Contains(Matches[i].Team2))
				{
					Teams.Add(Matches[i].Team2);
				}
			}
		}

		#region GlobalTeams

		private void UpdateTeamsInMatches()
		{
			for (int i = 0; i < Matches.Count; i++)
			{
				Matches[i].UpdateTeamConnections();
			}
		}

		#endregion GlobalTeams

		public void UpdateAfterLoad()
		{
			if (GlobalTeams)
			{
				UpdateTeamsInMatches();
				LoadAllTeamsFromMatchesIntoTeams();
			}
			else
			{
				UpdatePlayerConnectionInMatches();
				UpdateTeamDescription();
				LoadAllTeamsFromMatchesIntoTeams();
			}
			SortTeamsByNumber(true);
		}

		#endregion LoadingAndSaving

		public void SortTeamsByNumber(bool ascend = true)
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

		public void SortTeamsByPoints(bool ascend = true)
		{
			if (ascend)
			{
				Teams.Sort(Team.PointsComparer());
			}
			else
			{
				Teams.DescendingSort(Team.PointsComparer());
			}
		}

		public bool AllowSameTeams = false;
		public bool AllowFreeWin = false;
		public bool RandomTeams = false;
		public int WantedTeamSize = 0;
		public int WantedTeamsInFinal = 0;
		public int PointsPerWin = 0;
		public int PointsPerFreeWin = 0;

		private string name = "Round";

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

		public bool GenerateRandomMatches(bool globalTeams, bool randomTeams, bool allowSameMatches, bool allowSameTeams, bool allowFreeWin, int wantedTeamSize, int pointsPerWin, int pointsPerFreeWin)
		{
			GlobalTeams = globalTeams;
			RandomTeams = randomTeams;
			AllowSameMatches = allowSameMatches;
			AllowSameTeams = allowSameTeams;
			AllowFreeWin = allowFreeWin;
			WantedTeamSize = wantedTeamSize;
			PointsPerFreeWin = pointsPerFreeWin;
			PointsPerWin = pointsPerWin;
			bool result = TryRandomMatching();
			return result;
		}

		public bool GenerateFinalMatches(bool globalTeams, bool allowFreeWin, int wantedTeamsInFinal, int wantedTeamSize, int pointsPerWin, int pointsPerFreeWin)
		{
			GlobalTeams = globalTeams;
			AllowFreeWin = allowFreeWin;
			WantedTeamSize = wantedTeamSize;
			PointsPerFreeWin = pointsPerFreeWin;
			PointsPerWin = pointsPerWin;
			WantedTeamsInFinal = wantedTeamsInFinal;

			AllowSameMatches = true;
			AllowSameTeams = true;
			RandomTeams = false;
			bool result = TryFinalMatching();
			return result;
		}

		public bool ContainsMatchUsingPlayers(Team team1, Team team2)
		{
			bool result = false;
			for (int matchIndex = 0; matchIndex < Matches.Count; matchIndex++)
			{
				result = Matches[matchIndex].CompareMatchUsingPlayers(team1, team2);
				if (result)
					return true;
			}
			return result;
		}

		public List<int> EveryEnemieForTeamASString(Team team)
		{
			List<int> enemieNumbers = new List<int>();
			for (int i = 0; i < Matches.Count; i++)
			{
				enemieNumbers.Add(Matches[i].GetEnemieNumberForTeam(team));
			}
			return enemieNumbers;
		}
	}
}