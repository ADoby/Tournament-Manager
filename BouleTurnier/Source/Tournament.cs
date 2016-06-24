using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace BouleTurnier.Source
{
    public class Tournament
    {
        private MainWindow mainWindow;

        public PlayerManager playerManager;
        public TeamManager teamManager;
        public TournamentManager tournamentManager;

        public Tournament()
        {

        }

        public void Init()
        {
            playerManager = new PlayerManager();
            teamManager = new TeamManager();
            tournamentManager = new TournamentManager();
        }

        public void RecreatePlayerManager()
        {
            playerManager = new PlayerManager();
        }
        public void RecreateTeamManager()
        {
            teamManager = new TeamManager();
        }
        public void RecreateTournamentManager()
        {
            tournamentManager = new TournamentManager();
        }

        public void SetWindowManager(MainWindow newOwner)
        {
            mainWindow = newOwner;
            playerManager.SetWindowManager(newOwner);
            teamManager.SetWindowManager(newOwner);
            tournamentManager.SetWindowManager(newOwner);
        }

        public void UpdateAfterLoad()
        {
            teamManager.UpdateAfterLoad();
            tournamentManager.UpdateAfterLoad();
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

        public static Tournament Load(string FileName)
        {
            using (var stream = System.IO.File.OpenRead(FileName))
            {
                var serializer = new XmlSerializer(typeof(Tournament));
                return serializer.Deserialize(stream) as Tournament;
            }
        }
    }
}
