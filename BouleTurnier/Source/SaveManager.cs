using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Serialization;

namespace BouleTurnier.Source
{
    public class SaveManager
    {
        public const string SaveFolder = "TournamentManager";
        public const string SaveManagerFileName = "SaveManager.dat";

        public List<SaveInfo> saveInfos = new List<SaveInfo>();

        public static string SaveManagerFilePath
        {
            get
            {
                return String.Format("{0}/{1}", SaveFolder, SaveManagerFileName);
            }
        }

        public void SaveTournament(Tournament tournament, int InfoIndex)
        {
            if (InfoIndex < 0 && InfoIndex >= saveInfos.Count)
            {
                return;
            }

            tournament.Save(saveInfos[InfoIndex].FileName);
        }

        public Tournament LoadTournament(int InfoIndex)
        {
            if (InfoIndex < 0 && InfoIndex >= saveInfos.Count)
            {
                return null;
            }

            if (!System.IO.File.Exists(saveInfos[InfoIndex].FileName))
                return null;

            return Tournament.Load(saveInfos[InfoIndex].FileName);
        }

        public void AddSaveInfo()
        {
            SaveInfo newInfo = new SaveInfo();
            newInfo.FileName = String.Format("{0}/SaveInfo{1}.dat", SaveFolder, saveInfos.Count + 1);
            saveInfos.Add(newInfo);
            Save();
        }

        public void RemoveSaveInfo(int InfoIndex)
        {
            if (InfoIndex >= 0 && InfoIndex < saveInfos.Count)
            {
                RemoveSaveInfo(saveInfos[InfoIndex]);
            }
        }

        public void RemoveSaveInfo(SaveInfo info)
        {
            if (MessageBox.Show("You really wanna delete that info ?", "", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                saveInfos.Remove(info);
                Save();
            }
        }

        public SaveManager()
        {

        }

        public static SaveManager LoadOrCreateSaveManager()
        {
            if (!System.IO.Directory.Exists(SaveFolder))
                System.IO.Directory.CreateDirectory(SaveFolder);

            SaveManager saveManager;
            if (System.IO.File.Exists(SaveManagerFilePath))
            {
                saveManager = Load();
            }
            else
            {
                saveManager = new SaveManager();
            }
            return saveManager;
        }

        private void Save()
        {
            string tmpFile = string.Format("_backup_{0}", SaveManagerFilePath);
            if(System.IO.File.Exists(SaveManagerFilePath))
                System.IO.File.Copy(SaveManagerFilePath, tmpFile);

            try
            {
                using (var writer = new System.IO.StreamWriter(SaveManagerFilePath))
                {
                    var serializer = new XmlSerializer(this.GetType());
                    serializer.Serialize(writer, this);
                    writer.Flush();
                }

                if (System.IO.File.Exists(tmpFile))
                    System.IO.File.Delete(tmpFile);
            }
            catch (Exception e)
            {
                if (System.IO.File.Exists(tmpFile))
                    System.IO.File.Move(tmpFile, SaveManagerFilePath);
                throw;
            }
           
        }

        private static SaveManager Load()
        {
            using (var stream = System.IO.File.OpenRead(SaveManagerFilePath))
            {
                var serializer = new XmlSerializer(typeof(SaveManager));
                return serializer.Deserialize(stream) as SaveManager;
            }
        }
    }
}
