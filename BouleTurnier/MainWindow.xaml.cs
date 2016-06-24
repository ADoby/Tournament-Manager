using BouleTurnier.Source;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace BouleTurnier
{
    public class TeamInfo
    {
        public int Number { get; set; }

        public string Description { get; set; }

        public List<int> enemies = new List<int>();

        public string Enemies
        {
            get
            {
                if (enemies.Count == 0)
                    return "";
                string enemiess = "";

                enemiess = enemies[0].ToString();
                for (int i = 1; i < enemies.Count; i++)
                {
                    enemiess = String.Format("{0}; {1}", enemiess, enemies[i]);
                }
                return enemiess;
            }
        }
    }

    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public WindowManager windowManager;

        public SaveManager saveManager;

        public Tournament tournament;

        #region Properties

        public PlayerManager playerManager
        {
            get
            {
                if (tournament == null)
                    return null;
                return tournament.playerManager;
            }
            set
            {
                if (tournament != null)
                {
                    tournament.playerManager = value;
                }
            }
        }

        public TeamManager teamManager
        {
            get
            {
                if (tournament == null)
                    return null;
                return tournament.teamManager;
            }
            set
            {
                if (tournament != null)
                {
                    tournament.teamManager = value;
                }
            }
        }

        public TournamentManager tournamentManager
        {
            get
            {
                if (tournament == null)
                    return null;
                return tournament.tournamentManager;
            }
            set
            {
                if (tournament != null)
                {
                    tournament.tournamentManager = value;
                }
            }
        }

        #endregion Properties

        public MainWindow()
        {
            InitializeComponent();

            saveManager = SaveManager.LoadOrCreateSaveManager();
            if (saveManager.saveInfos.Count == 0)
            {
                AddSaveInfo();

                tournament = new Tournament();
                tournament.Init();
                tournament.SetWindowManager(this);

                saveManager.SaveTournament(tournament, 0);
                cbSaveInfo.SelectedIndex = 0;
            }
            else
            {
                for (int i = 0; i < saveManager.saveInfos.Count; i++)
                {
                    ComboBoxItem newItem = new ComboBoxItem();
                    newItem.Content = String.Format("SaveInfo {0}", i + 1);
                    cbSaveInfo.Items.Add(newItem);
                }

                LoadTournament(0);
                cbSaveInfo.SelectedIndex = 0;
            }

            windowManager = new WindowManager(this);
            gbTeamInfo.DataContext = windowManager;
            gbPlayerInfo.DataContext = windowManager;

            //PlayerInfo
            UpdateSelectedPlayerBinding();

            //TeamInfo
            UpdateSelectedTeamBinding();

            listPlayers.ItemsSource = playerManager.Players;

            listTeams.ItemsSource = teamManager.Teams;
            listAvaiblePlayers.ItemsSource = teamManager.AvaiblePlayers;

            ReloadErrorInfo();
        }

        private void ReloadErrorInfo()
        {
            teaminfos = new SObservableCollection<TeamInfo>();
            TeamInfo teaminfo;
            List<int> numbers;
            for (int teamIndex = 0; teamIndex < teamManager.Teams.Count; teamIndex++)
            {
                teaminfo = new TeamInfo();
                teaminfo.Number = teamManager.Teams[teamIndex].Number;
                teaminfo.Description = teamManager.Teams[teamIndex].Description;
                for (int i = 0; i < tournamentManager.Rounds.Count; i++)
                {
                    numbers = tournamentManager.Rounds[i].EveryEnemieForTeamASString(teamManager.Teams[teamIndex]);
                    for (int numberIndex = 0; numberIndex < numbers.Count; numberIndex++)
                    {
                        if (numbers[numberIndex] != -1)
                        {
                            teaminfo.enemies.Add(numbers[numberIndex]);
                        }
                    }
                }
                teaminfos.Add(teaminfo);
            }

            teaminfos.Sort((x, y) => x.Number.CompareTo(y.Number));
            listTeamsAndEnemies.ItemsSource = teaminfos;
        }

        public SObservableCollection<TeamInfo> teaminfos = new SObservableCollection<TeamInfo>();

        public void UpdateSelectedPlayerBinding()
        {
            if (windowManager != null)
            {
                txtPlayerNumber.DataContext = windowManager.SelectedPlayer;
                txtPlayerName.DataContext = windowManager.SelectedPlayer;
                txtPlayerPoints.DataContext = windowManager.SelectedPlayer;
                txtPlayerLicense.DataContext = windowManager.SelectedPlayer;
            }
        }

        public void UpdateSelectedTeamBinding()
        {
            if (windowManager != null)
            {
                listTeamPlayers.ItemsSource = windowManager.SelectedTeamPlayers;

                txtTeamNumber.DataContext = windowManager.SelectedTeam;
                txtTeamPoints.DataContext = windowManager.SelectedTeam;
                txtTeamDescription.DataContext = windowManager.SelectedTeam;
                txtTeamPositivePoints.DataContext = windowManager.SelectedTeam;
            }
        }

        #region PlayerManager

        private void btnCreatePlayer_Click(object sender, RoutedEventArgs e)
        {
            CreateNewPlayer();
        }

        public void CreateNewPlayer()
        {
            playerManager.AddPlayer();
            listPlayers.SelectedIndex = listPlayers.Items.Count - 1;
        }

        private void listPlayers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listPlayers.SelectedItem != null)
            {
                windowManager.SelectedPlayer = (Player)listPlayers.SelectedItem;

                txtPlayerName.Focus();

                txtPlayerName.SelectAll();
            }
            else
            {
                windowManager.SelectedPlayer = null;
            }
        }

        private void txtPlayerName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (windowManager.SelectedPlayer != null)
                windowManager.SelectedPlayer.Name = txtPlayerName.Text;
        }

        private void txtPlayerPoints_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        private void txtPlayerPoints_TextChanged(object sender, TextChangedEventArgs e)
        {
            int integer = 0;
            bool result = int.TryParse(txtPlayerPoints.Text, out integer);
            if (result)
                windowManager.SelectedPlayer.Points = integer;
        }

        private void txtPlayerLicense_TextChanged(object sender, TextChangedEventArgs e)
        {
            windowManager.SelectedPlayer.License = txtPlayerLicense.Text;
        }

        #region PlayerListSorting

        public enum PlayerListOrder
        {
            PLAYERS_ASCEND_NUMBER,
            PLAYERS_DESCEND_NUMBER,
            PLAYERS_ASCEND_NAME,
            PLAYERS_DESCEND_NAME,
            PLAYERS_ASCEND_POINTS,
            PLAYERS_DESCEND_POINTS
        }

        private void cbPlayerOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (playerManager == null)
                return;

            switch (cbPlayerOrder.SelectedIndex)
            {
                case (int)PlayerListOrder.PLAYERS_ASCEND_NUMBER:
                    playerManager.SortByNumber();
                    break;

                case (int)PlayerListOrder.PLAYERS_DESCEND_NUMBER:
                    playerManager.SortByNumber(false);
                    break;

                case (int)PlayerListOrder.PLAYERS_ASCEND_NAME:
                    playerManager.SortByName();
                    break;

                case (int)PlayerListOrder.PLAYERS_DESCEND_NAME:
                    playerManager.SortByName(false);
                    break;

                case (int)PlayerListOrder.PLAYERS_ASCEND_POINTS:
                    playerManager.SortByPoints();
                    break;

                case (int)PlayerListOrder.PLAYERS_DESCEND_POINTS:
                    playerManager.SortByPoints(false);
                    break;

                default:
                    break;
            }
            //listPlayers.Items.Refresh();
        }

        #endregion PlayerListSorting

        #endregion PlayerManager

        #region TeamManager

        private void btnCreateTeam_Click(object sender, RoutedEventArgs e)
        {
            CreateNewTeam();
        }

        public void CreateNewTeam()
        {
            tournament.teamManager.AddNewTeam();
            listTeams.SelectedIndex = listTeams.Items.Count - 1;
        }

        private void txtTeamDescription_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (windowManager.SelectedTeam != null)
                windowManager.SelectedTeam.Description = txtTeamDescription.Text;
        }

        private void listTeams_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listTeams.SelectedItem != null)
            {
                windowManager.SelectedTeam = (Team)listTeams.SelectedItem;
            }
            else
            {
                windowManager.SelectedTeam = null;
            }
        }

        private void listAvaiblePlayers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listAvaiblePlayers.SelectedItem != null)
            {
                Player selectedPlayer = (Player)listAvaiblePlayers.SelectedItem;

                teamManager.AddPlayerToTeam(windowManager.SelectedTeam, selectedPlayer);

                listAvaiblePlayers.SelectedIndex = -1;
            }
        }

        private void listTeamPlayers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listTeamPlayers.SelectedItem != null)
            {
                Player selectedPlayer = (Player)listTeamPlayers.SelectedItem;

                teamManager.RemovePlayerFromTeam(windowManager.SelectedTeam, selectedPlayer);

                listAvaiblePlayers.SelectedIndex = -1;
            }
        }

        #region TeamListSorting

        public enum TeamListOrder
        {
            TEAMS_ASCEND_NUMBER,
            TEAMS_DESCEND_NUMBER,
            TEAMS_ASCEND_POINTS,
            TEAMS_DESCEND_POINTS
        }

        private void cbTeamOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (teamManager == null)
                return;

            switch (cbTeamOrder.SelectedIndex)
            {
                case (int)TeamListOrder.TEAMS_ASCEND_NUMBER:
                    teamManager.SortByNumber();
                    break;

                case (int)TeamListOrder.TEAMS_DESCEND_NUMBER:
                    teamManager.SortByNumber(false);
                    break;

                case (int)TeamListOrder.TEAMS_ASCEND_POINTS:
                    teamManager.SortByPoints();
                    break;

                case (int)TeamListOrder.TEAMS_DESCEND_POINTS:
                    teamManager.SortByPoints(false);
                    break;

                default:
                    break;
            }
        }

        #endregion TeamListSorting

        #endregion TeamManager

        #region TournamentManager

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            AddRound();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            tournamentManager.CalculatePoints();
        }

        private void cbRounds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tournamentManager != null && cbRounds.SelectedIndex != -1)
            {
                listRound.ItemsSource = tournamentManager.Rounds[cbRounds.SelectedIndex].Matches;
                listTeamsInRound.ItemsSource = tournamentManager.Rounds[cbRounds.SelectedIndex].Teams;
            }
            else
            {
                listRound.ItemsSource = null;
                listTeamsInRound.ItemsSource = null;
            }
        }

        #region RoundCalculation

        public void AddRound()
        {
            if (rbRandomMatchesFromGlobalTeams.IsChecked == true)
            {
                if (teamManager.Teams.Count < 2)
                {
                    MessageBox.Show("Not enough Teams!");
                    return;
                }

                AddAndRandomizeRound();
            }
            if (rbRandomMatchesWithRandomTeams.IsChecked == true)
            {
                int wantedTeamSize = int.Parse(txtWantedTeamSize.Text);
                if (playerManager.Players.Count < wantedTeamSize * 2)
                {
                    MessageBox.Show("Not enough Players!");
                    return;
                }
                AddAndRandomizeRound();
            }
            if (rbFinalRound.IsChecked == true)
            {
                int wantedTeamSize = int.Parse(txtFinalTeams.Text);
                if (playerManager.Players.Count < wantedTeamSize * 2)
                {
                    MessageBox.Show("Not enough Players!");
                    return;
                }
                AddFinalRound(wantedTeamSize);
            }
        }

        public void AddAndRandomizeRound()
        {
            Round currentRound = tournamentManager.AddRound();
            bool globalTeams = rbRandomMatchesFromGlobalTeams.IsChecked == true;
            bool randomTeams = rbRandomMatchesWithRandomTeams.IsChecked == true;
            bool allowSameMatches = cbSameMatches.IsChecked == true;
            bool allowSameTeams = cbSameTeams.IsChecked == true;
            bool allowFreeWin = cbAllowFreeWin.IsChecked == true;

            int wantedTeamSize = int.Parse(txtWantedTeamSize.Text);
            int pointsPerWin = int.Parse(txtPointsForWin.Text);
            int pointsPerFreeWin = int.Parse(txtFreePoints.Text);

            currentRound.Name = txtRoundName.Text;
            bool result = tournamentManager.GenerateRandomMatches(currentRound, globalTeams, randomTeams, allowSameMatches, allowSameTeams, allowFreeWin, wantedTeamSize, pointsPerWin, pointsPerFreeWin);

            if (!result)
            {
                tournamentManager.RemoveRound(currentRound);
                MessageBox.Show("Runde konnte nicht generiert werden, vermutlich können mit der angegebenen Menge an Manschten nicht so viele Runden gelost werden ohne Fehler");
                txtRoundName.Text = String.Format("Round {0}", tournamentManager.Rounds.Count + 1);
                return;
            }

            int roundIndex = tournamentManager.GetRoundIndex(currentRound);

            ComboBoxItem newItem = new ComboBoxItem();
            newItem.Content = currentRound.Name;
            cbRounds.Items.Add(newItem);

            cbRounds.SelectedIndex = roundIndex;
            //Automatically calls "SelectionChanged" on cbRounds

            txtRoundName.Text = String.Format("Round {0}", tournamentManager.Rounds.Count + 1);
        }

        public void AddFinalRound(int finalTeams)
        {
            Round currentRound = tournamentManager.AddRound();
            bool allowFreeWin = cbAllowFreeWin.IsChecked == true;

            int wantedTeamSize = int.Parse(txtWantedTeamSize.Text);
            int pointsPerWin = int.Parse(txtPointsForWin.Text);
            int pointsPerFreeWin = int.Parse(txtFreePoints.Text);

            currentRound.Name = txtRoundName.Text;
            bool result = tournamentManager.GenerateFinalMatches(currentRound, true, allowFreeWin, finalTeams, wantedTeamSize, pointsPerWin, pointsPerFreeWin);

            if (!result)
            {
                tournamentManager.RemoveRound(currentRound);
                MessageBox.Show("Runde konnte nicht generiert werden, vermutlich können mit der angegebenen Menge an Manschten nicht so viele Runden gelost werden ohne Fehler");
                txtRoundName.Text = String.Format("Round {0}", tournamentManager.Rounds.Count + 1);
                return;
            }

            //Select Round

            int roundIndex = tournamentManager.GetRoundIndex(currentRound);

            ComboBoxItem newItem = new ComboBoxItem();
            newItem.Content = currentRound.Name;
            cbRounds.Items.Add(newItem);

            //Automatically calls "SelectionChanged" on cbRounds
            cbRounds.SelectedIndex = roundIndex;

            txtRoundName.Text = String.Format("Round {0}", tournamentManager.Rounds.Count + 1);
        }

        #endregion RoundCalculation

        private void btnExportPlayers_Click(object sender, RoutedEventArgs e)
        {
            CSVExport exporter = new CSVExport();
            exporter.ExportPlayers(playerManager, "Players");
        }

        #endregion TournamentManager

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            saveManager.SaveTournament(tournament, cbSaveInfo.SelectedIndex);
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            CSVExport exporter = new CSVExport();
            exporter.ExportTeams(teamManager, "Teams");
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            if (cbRounds.SelectedIndex == -1)
                return;

            CSVExport exporter = new CSVExport();
            exporter.ExportRound(tournamentManager.Rounds[cbRounds.SelectedIndex], tournamentManager.Rounds[cbRounds.SelectedIndex].Name);
        }

        private void AddMatch()
        {
            Round round = tournamentManager.Rounds[cbRounds.SelectedIndex];
            Team team1 = null;
            Team team2 = null;

            int team1Number = int.Parse(txtTeam1.Text);
            int team2Number = int.Parse(txtTeam2.Text);

            foreach (var team in teamManager.Teams)
            {
                if (team.Number == team1Number)
                    team1 = team;
                else if (team.Number == team2Number)
                    team2 = team;
            }

            if (team1 == null || team2 == null)
                return;
            round.AddMatch(team1, team2);
        }

        private void RemoveMatch()
        {
            Round round = tournamentManager.Rounds[cbRounds.SelectedIndex];
            Team team1 = null;
            Team team2 = null;

            int team1Number = int.Parse(txtTeam1.Text);
            int team2Number = int.Parse(txtTeam2.Text);

            foreach (var team in teamManager.Teams)
            {
                if (team.Number == team1Number)
                    team1 = team;
                else if (team.Number == team2Number)
                    team2 = team;
            }

            if (team1 == null || team2 == null)
                return;
            for (int i = 0; i < round.Matches.Count; i++)
            {
                if (round.Matches[i].CompareMatch(team1, team2))
                {
                    round.Matches.RemoveAt(i);
                    break;
                }
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            AddSaveInfo();
        }

        public void AddSaveInfo()
        {
            saveManager.AddSaveInfo();

            ComboBoxItem newItem = new ComboBoxItem();
            newItem.Content = String.Format("SaveInfo {0}", saveManager.saveInfos.Count);
            cbSaveInfo.Items.Add(newItem);

            cbSaveInfo.SelectedIndex = cbSaveInfo.Items.Count - 1;
        }

        public int lastSelectedSaveInfo = -1;

        private void cbSaveInfo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbSaveInfo.SelectedItem != null)
            {
                if (lastSelectedSaveInfo != -1)
                {
                    saveManager.SaveTournament(tournament, lastSelectedSaveInfo);
                }
                LoadTournament(cbSaveInfo.SelectedIndex);
            }
            lastSelectedSaveInfo = cbSaveInfo.SelectedIndex;
        }

        public void SaveSelectedTournament()
        {
            if (cbSaveInfo.SelectedIndex == -1)
                return;

            saveManager.SaveTournament(tournament, cbSaveInfo.SelectedIndex);
        }

        public void LoadTournament(int infoIndex)
        {
            Tournament newTournament = saveManager.LoadTournament(infoIndex);

            if (newTournament == null)
            {
                newTournament = new Tournament();
                newTournament.Init();
            }

            tournament = newTournament;
            tournament.SetWindowManager(this);
            tournament.UpdateAfterLoad();

            UpdateSelectedPlayerBinding();
            UpdateSelectedTeamBinding();

            listPlayers.ItemsSource = playerManager.Players;
            listPlayers.SelectedIndex = -1;

            listTeams.ItemsSource = teamManager.Teams;
            listAvaiblePlayers.ItemsSource = teamManager.AvaiblePlayers;
            listTeams.SelectedIndex = -1;

            cbRounds.Items.Clear();

            for (int i = 0; i < tournamentManager.Rounds.Count; i++)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                newItem.Content = tournamentManager.Rounds[i].Name;
                cbRounds.Items.Add(newItem);
            }

            txtRoundName.Text = String.Format("Round {0}", tournamentManager.Rounds.Count + 1);

            if (tournamentManager.Rounds.Count > 0)
                cbRounds.SelectedIndex = 0;
            else
                cbRounds.SelectedIndex = -1;
        }

        private void ThisWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Save current tournament ?", "", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
            }
            if (result == MessageBoxResult.Yes)
            {
                SaveSelectedTournament();
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            windowManager.TournamentToggleSettings();
        }

        private void btnToggleRoundTeams_Click(object sender, RoutedEventArgs e)
        {
            windowManager.ToggleRoundTeams();
        }

        private void cbTeamRoundOrder_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (tournamentManager == null || cbRounds.SelectedIndex == -1)
                return;

            switch (cbTeamRoundOrder.SelectedIndex)
            {
                case (int)TeamListOrder.TEAMS_ASCEND_NUMBER:
                    tournamentManager.Rounds[cbRounds.SelectedIndex].SortTeamsByNumber();
                    break;

                case (int)TeamListOrder.TEAMS_DESCEND_NUMBER:
                    tournamentManager.Rounds[cbRounds.SelectedIndex].SortTeamsByNumber(false);
                    break;

                case (int)TeamListOrder.TEAMS_ASCEND_POINTS:
                    tournamentManager.Rounds[cbRounds.SelectedIndex].SortTeamsByPoints();
                    break;

                case (int)TeamListOrder.TEAMS_DESCEND_POINTS:
                    tournamentManager.Rounds[cbRounds.SelectedIndex].SortTeamsByPoints(false);
                    break;

                default:
                    break;
            }
        }

        private void txtWantedTeamSize_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        private void txtPointsForWin_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        private void txtFreePoints_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            char c = Convert.ToChar(e.Text);
            if (Char.IsNumber(c))
                e.Handled = false;
            else
                e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        private void btnDeleteRound_Click(object sender, RoutedEventArgs e)
        {
            if (cbRounds.SelectedItem == null)
                return;

            if (MessageBox.Show("Do you really wan't to delete this round ?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
            {
                return;
            }
            tournamentManager.RemoveRound(cbRounds.SelectedIndex);
            cbRounds.Items.Remove(cbRounds.SelectedItem);

            cbRounds.SelectedIndex = cbRounds.Items.Count - 1;
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            if (cbRounds.SelectedItem == null)
                return;

            tournamentManager.Rounds[cbRounds.SelectedIndex].Name = txtNewRoundName.Text;
            ((ComboBoxItem)cbRounds.SelectedItem).Content = txtNewRoundName.Text;
        }

        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            if (windowManager.SelectedPlayer == null)
                return;

            if (MessageBox.Show("Do you really wan't to delete this player?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            playerManager.RemovePlayer(windowManager.SelectedPlayer);
            listPlayers.SelectedIndex = -1;
        }

        private void Button_Click_9(object sender, RoutedEventArgs e)
        {
            if (windowManager.SelectedTeam == null)
                return;

            if (MessageBox.Show("Do you really wan't to delete this Team?", "", MessageBoxButton.YesNo) == MessageBoxResult.No)
                return;

            teamManager.RemoveTeam(windowManager.SelectedTeam);
            listTeams.SelectedIndex = -1;
        }

        private void Button_Click_10(object sender, RoutedEventArgs e)
        {
            ReloadErrorInfo();
        }

        private void Button_Click_11(object sender, RoutedEventArgs e)
        {
            AddMatch();
        }

        private void Button_Click_12(object sender, RoutedEventArgs e)
        {
            RemoveMatch();
        }

        private void Button_Click_13(object sender, RoutedEventArgs e)
        {
            Round round = tournamentManager.Rounds[cbRounds.SelectedIndex];
            round.Matches.Clear();
        }
    }
}