using System;

public class FileCollection : HyperMintBaseClass
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public string FullPath { get; set; }

    public FileCollection(string name, string ext, string fullPath)
    {
        this.Name = name;
        this.Extension = ext;
        this.FullPath = fullPath;
    }
}