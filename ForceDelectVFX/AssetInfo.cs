using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
/// <summary>
/// 资源文件类型
/// </summary>
public enum FileType
{
    /// <summary>
    /// 有效的文件资源
    /// </summary>
    ValidFile,
    /// <summary>
    /// 文件夹
    /// </summary>
    Folder,
    /// <summary>
    /// 无效的文件资源
    /// </summary>
    InValidFile
}
public class AssetInfoSetting
{
    List<string> validExtension = new List<string>();
    List<string> filterDir = new List<string>();

    public void AddValidExtension(string name)
    {
        validExtension.Add(name);
    }
    public void AddFilterDir(string name)
    {
        filterDir.Add(name);
    }

    public bool TryFilterDir(string name)
    {
        return !filterDir.Contains(name);
    }

    public FileType TryValidExtension(string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return FileType.Folder;
        }
        if (validExtension.Contains(name))
            return FileType.ValidFile;
        else
            return FileType.InValidFile;
    }
}
public class AssetInfo
{
    /// <summary>
    /// 资源全路径
    /// </summary>
    public string AssetFullPath
    {
        get;
        private set;
    }
    /// <summary>
    /// 资源名称
    /// </summary>
    public string AssetName
    {
        get;
        private set;
    }
    public string Extension
    {
        get;
        private set;
    }
    /// <summary>
    /// 资源文件类型
    /// </summary>
    public FileType AssetFileType
    {
        get;
        private set;
    }
    /// <summary>
    /// 文件夹的子资源（资源无效）
    /// </summary>
    public List<AssetInfo> ChildAssetInfo
    {
        get;
        set;
    }
    /// <summary>
    /// 文件类型资源
    /// </summary>
    public AssetInfo(string fullPath, string name, string extension = "")
    {
        AssetFullPath = fullPath;
        AssetName = name;
        Extension = extension;
        AssetFileType = GetFileType(extension);
        ChildAssetInfo = new List<AssetInfo>();
    }
    public FileType GetFileType(string extenion)
    {
        if (string.IsNullOrEmpty(extenion))
            return FileType.Folder;
        return FileType.ValidFile;
    }
    //链接至静态工具类【AssetBundleTool.cs】
    /// <summary>
    /// 读取资源文件夹下的所有子资源
    /// </summary>
    public static void ReadAssetsInChildren(AssetInfo asset, AssetInfoSetting setting = null)
    {
        //不是文件夹对象，不存在子对象
        if (asset.AssetFileType != FileType.Folder)
        {
            return;
        }
        asset.ChildAssetInfo.Clear();
        //打开这个文件夹
        DirectoryInfo di = new DirectoryInfo(asset.AssetFullPath);
        //获取其中所有内容，包括文件或子文件夹
        FileSystemInfo[] fileinfo = di.GetFileSystemInfos();
        //遍历这些内容
        foreach (FileSystemInfo fi in fileinfo)
        {
            //如果该内容是文件夹
            if (fi is DirectoryInfo && (setting == null || setting.TryFilterDir(fi.Name)))
            {
                //是合格的文件夹，就创建为文件夹对象，并加入到当前对象的子对象集合
                AssetInfo ai = new AssetInfo(fi.FullName, fi.Name);
                asset.ChildAssetInfo.Add(ai);

                //然后继续深层遍历这个文件夹
                ReadAssetsInChildren(ai, setting);
            }
            //否则该内容是文件
            else if (setting == null || setting.TryValidExtension(fi.Extension) == FileType.ValidFile)
            {
                //是合格的文件，就创建为资源文件对象，并加入到当前对象的子对象集合                    
                asset.ChildAssetInfo.Add(new AssetInfo(fi.FullName, fi.Name, fi.Extension));
            }
        }
    }
}
