using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusVM
{
    class Cleaner
    {
        const string backupAosFolder = @"C:\AOSService\PackagesLocalDirectoryBackup";
        const string aosFolder = @"C:\AOSService\PackagesLocalDirectory";
        string gitFolder = Environment.GetEnvironmentVariable("INETROOT") +@"\Source\Metadata";
        string gitBackupFolder = Environment.GetEnvironmentVariable("INETROOT") + @"\..\ApplicationSuiteBackup\Source\Metadata";

        public HashSet<string> RequiredModels = new HashSet<string>();
        public HashSet<string> GitModels = new HashSet<string>();

        public void CleanUpAOSFolderSafe()
        {
            if (!Directory.Exists(backupAosFolder))
            {
                Directory.CreateDirectory(backupAosFolder);
            }

            if (Directory.Exists(aosFolder))
            {
                string[] folders = System.IO.Directory.GetDirectories(aosFolder);
                foreach (string folder in folders)
                {
                    DirectoryInfo dInfo = new DirectoryInfo(folder);
                    
                    if (!Directory.Exists(aosFolder+@"\"+ dInfo.Name + @"\Descriptor") ||
                        Directory.Exists(gitFolder+@"\"+dInfo.Name + @"\Descriptor") ||
                        RequiredModels.Contains(dInfo.Name.ToLowerInvariant()))
                    {
                        //Debug.WriteLine(string.Format("Keeping {0}", dInfo.Name));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Moving {0}", dInfo.Name));

                        Directory.Move(folder, backupAosFolder+@"\"+dInfo.Name);
                    }
                }
            }
        }

        public void CleanUpGitFolder()
        {
            if (Directory.Exists(gitBackupFolder))
            {
                Directory.Delete(gitBackupFolder, true);
            }
            Directory.CreateDirectory(gitBackupFolder);

            if (Directory.Exists(gitFolder))
            {
                string[] folders = Directory.GetDirectories(gitFolder);
                foreach (string folder in folders)
                {
                    DirectoryInfo dInfo = new DirectoryInfo(folder);

                    if (!Directory.Exists(gitFolder + @"\" + dInfo.Name + @"\Descriptor") ||
                        RequiredModels.Contains(dInfo.Name.ToLowerInvariant()))
                    {
                        //Debug.WriteLine(string.Format("Keeping {0}", dInfo.Name));
                    }
                    else
                    {
                        Debug.WriteLine(string.Format("Moving {0}", dInfo.Name));

                        Directory.Move(folder, gitBackupFolder + @"\" + dInfo.Name);
                        string aosFolderToMove = aosFolder + @"\" + dInfo.Name;
                        if (Directory.Exists(aosFolderToMove))
                        {
                            Directory.Move(aosFolderToMove, backupAosFolder + @"\" + dInfo.Name);
                        }
                    }
                }
            }
        }

    }
}
