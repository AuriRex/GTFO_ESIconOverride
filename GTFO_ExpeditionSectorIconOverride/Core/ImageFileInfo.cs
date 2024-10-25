using System.IO;
using UnityEngine;

namespace ExSeIcOv.Core
{
    public class ImageFileInfo
    {
        public string FileName { get; init; }
        public string FileNameLower => FileName.ToLower();
        public string FilePath { get; init; }

        public ImageFileInfo(string filePath)
        {
            FileName = Path.GetFileNameWithoutExtension(filePath);
            FilePath = filePath;
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
}