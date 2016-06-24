using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace BouleTurnier.Source
{
    public class WindowManager : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        public void TriggerPropertyChanges(string Property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(Property));
            }
        }

        private MainWindow mainWindow;
        public WindowManager(MainWindow newOwner)
        {
            mainWindow = newOwner;

            if (mainWindow.TournamentSettingsRow.Height.Value == ShowSettingsHeight)
            {
                TournamentSettings = true;
                TournamentToggleSettings();
            }

            if (mainWindow.RoundTeamColumn.Width.Value == ShowTeamsInRoundWidth)
            {
                RoundTeams = true;
                ToggleRoundTeams();
            }
        }

        private bool playerSelected = false;
        public bool PlayerSelected 
        {
            get
            {
                return playerSelected;
            }
            set 
            {
                playerSelected = value;
                TriggerPropertyChanges("PlayerSelected");
            } 
        }

        private bool teamSelected = false;
        public bool TeamSelected
        {
            get
            {
                return teamSelected;
            }
            set
            {
                teamSelected = value;
                TriggerPropertyChanges("TeamSelected");
            }
        }

        private Player selectedPlayer = null;
        public Player SelectedPlayer
        {
            get
            {
                return selectedPlayer;
            }
            set
            {
                selectedPlayer = value;
                TriggerPropertyChanges("SelectedPlayer");
                PlayerSelected = (selectedPlayer != null);
                mainWindow.UpdateSelectedPlayerBinding();
            }
        }

        private Team selectedTeam = null;
        public Team SelectedTeam
        {
            get
            {
                return selectedTeam;
            }
            set
            {
                selectedTeam = value;
                TriggerPropertyChanges("SelectedTeam");
                TeamSelected = (selectedTeam != null);
                mainWindow.UpdateSelectedTeamBinding();
            }
        }

        public ObservableCollection<Player> SelectedTeamPlayers
        {
            get
            {
                if (SelectedTeam == null)
                    return null;
                return SelectedTeam.Players;
            }
        }

        public const double ShowSettingsHeight = 135;
        public const double HideSettingsHeight = 35;

        public bool TournamentSettings = false;

        public void TournamentToggleSettings()
        {
            double to = ShowSettingsHeight;
            if (TournamentSettings)
            {
                to = HideSettingsHeight;
            }

            GridLengthAnimation anim = new GridLengthAnimation();
            anim.From = mainWindow.TournamentSettingsRow.Height;
            anim.To = new GridLength(to);
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            mainWindow.TournamentSettingsRow.BeginAnimation(RowDefinition.HeightProperty, anim);

            TournamentSettings = !TournamentSettings;
        }

        public const double ShowTeamsInRoundWidth = 200;
        public const double HideTeamsInRoundWidth = 0;

        public bool RoundTeams = false;

        public void ToggleRoundTeams()
        {
            double to = ShowTeamsInRoundWidth;
            if (RoundTeams)
            {
                to = HideTeamsInRoundWidth;
            }

            GridLengthAnimation anim = new GridLengthAnimation();
            anim.From = mainWindow.RoundTeamColumn.Width;
            anim.To = new GridLength(to);
            anim.Duration = new Duration(TimeSpan.FromSeconds(0.2));

            mainWindow.RoundTeamColumn.BeginAnimation(ColumnDefinition.WidthProperty, anim);

            RoundTeams = !RoundTeams;
        }
    }
}
