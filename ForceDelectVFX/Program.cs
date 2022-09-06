using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForceDelectVFX
{
    class Program
    {
        int _totalNumber = 0;
        static void Main(string[] args)
        {
            Program main = new Program();
            main.RunDelectVFX();

            Console.WriteLine("Delete Vfx And Meat File Number:" + main._totalNumber);
            Console.ReadKey();
        }

        private void RunDelectVFX()
        {
            string currentDir = Environment.CurrentDirectory;
            AssetInfoSetting setting = new AssetInfoSetting();
            setting.AddValidExtension(".vfx");

            AssetInfo root = new AssetInfo(currentDir + "/../", "Root");
            AssetInfo.ReadAssetsInChildren(root, setting);

            DelectFile(root);
        }

        private void DelectFile(AssetInfo root)
        {
            List<AssetInfo> childInfo = root.ChildAssetInfo;
            string fullPath;
            for (int i = 0; i < childInfo.Count; i++)
            {
                if (childInfo[i].AssetFileType == FileType.Folder)
                {
                    DelectFile(childInfo[i]);
                }
                else if (childInfo[i].AssetFileType == FileType.ValidFile)
                {
                    _totalNumber++;
                    fullPath = childInfo[i].AssetFullPath;
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                    fullPath = fullPath + ".meta";
                    if (File.Exists(fullPath))
                    {
                        File.Delete(fullPath);
                    }
                }
            }
        }
    }
}
