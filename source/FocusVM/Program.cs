using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusVM
{
    class Program
    {
        static void Main(string[] args)
        {
            var scanner = new Scanner();
            scanner.includeFolder("SCMTests");

            var cleaner = new Cleaner();
            cleaner.RequiredModels = scanner.RequiredModels;
            cleaner.CleanUpAOSFolderSafe();
            cleaner.CleanUpGitFolder();
        }        
    }
}
