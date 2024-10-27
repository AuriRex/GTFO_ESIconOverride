using UnityEngine;

namespace ExSeIcOv.Core.Info;

public class ImageFileInfo : GenericFileInfo
{
    public ImageFileInfo(string filePath) : base(filePath)
    {
    }

    public ImageFileInfo(GenericFileInfo genericFile) : base(genericFile.FilePath)
    {
    }

    public Sprite LoadAsSprite()
    {
        return ImageLoader.LoadSprite(FilePath);
    }

    public Texture2D LoadAsTex2D()
    {
        return ImageLoader.LoadTex2D(FilePath);
    }
}