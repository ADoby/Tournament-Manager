using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BouleTurnier.Source
{
    public class CSVExport
    {

        public const string ExportFolder = "TournamentExport";

        public void CheckAndCreateExportFolder()
        {
            if (!System.IO.Directory.Exists(ExportFolder))
            {
                System.IO.Directory.CreateDirectory(ExportFolder);
            }
        }

        public void ExportPlayers(PlayerManager playerManager, string FileName)
        {
            CheckAndCreateExportFolder();
            if (!FileName.EndsWith(".csv"))
                FileName = String.Format("{0}{1}", FileName, ".csv");

            FileName = String.Format("{0}/{1}", ExportFolder, FileName);
            StreamWriter stream = new StreamWriter(FileName);

            for (int i = 0; i < playerManager.Players.Count; i++)
            {
                stream.WriteLine(playerManager.Players[i].ExportInfo());
            }

            stream.Close();
        }

        public void ExportTeams(TeamManager teamManager, string FileName)
        {
            CheckAndCreateExportFolder();
            if (!FileName.EndsWith(".csv"))
                FileName = String.Format("{0}{1}", FileName, ".csv");

            FileName = String.Format("{0}/{1}", ExportFolder, FileName);
            StreamWriter stream = new StreamWriter(FileName);

            for (int i = 0; i < teamManager.Teams.Count; i++)
            {
                stream.WriteLine(teamManager.Teams[i].ExportInfo());
            }

            stream.Close();
        }

        public void ExportRound(Round round, string FileName)
        {
            CheckAndCreateExportFolder();
            if (!FileName.EndsWith(".csv"))
                FileName = String.Format("{0}{1}", FileName, ".csv");

            FileName = String.Format("{0}/{1}", ExportFolder, FileName);
            StreamWriter stream = new StreamWriter(FileName);

            for (int i = 0; i < round.Matches.Count; i++)
            {
                stream.WriteLine(round.Matches[i].ExportInfo());
            }

            stream.Close();
        }

    }
}
