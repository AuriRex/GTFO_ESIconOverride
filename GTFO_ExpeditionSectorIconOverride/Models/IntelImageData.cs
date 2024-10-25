using ExSeIcOv.Core;
using UnityEngine;

namespace ExSeIcOv.Models;

internal class IntelImageData
{
    public Sprite Top;
    public Sprite Middle;
    public Sprite Bottom;

    public bool HasData => Top != null || Middle != null || Bottom != null;
}
