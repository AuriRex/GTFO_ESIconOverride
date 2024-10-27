using ExSeIcOv.Core;
using System;
using UnityEngine;

namespace ExSeIcOv.Models;

public class SectorOverrideImageData
{
    public SectorIconOverride Override = new();
    public SectorIconOverride RundownTierMarker = new();

    public bool HasData => Override.HasData || RundownTierMarker.HasData;
}
