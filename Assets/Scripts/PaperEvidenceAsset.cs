using System;
using UnityEngine;

[CreateAssetMenu]
public class PaperEvidenceAsset : EvidenceAsset
{
    [Serializable]
    public struct Side
    {
        public Texture image;
        public Material material;
    }

    public Side front;
    public Side back;
}
