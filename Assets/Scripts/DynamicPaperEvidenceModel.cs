using System;
using UnityEngine;

[ExecuteAlways]
public class DynamicPaperEvidenceModel : MonoBehaviour
{
    [Serializable]
    public struct Side
    {
        public MeshRenderer renderer;
        private MaterialPropertyBlock materialPropertyBlock;
        public void UpdateMaterial(PaperEvidenceAsset.Side side)
        {
            materialPropertyBlock = materialPropertyBlock ?? new MaterialPropertyBlock();
            materialPropertyBlock.Clear();
            if (side.material != null)
            {
                renderer.material = side.material;
            }
            // Get the current value of the material properties in the renderer.
            renderer.GetPropertyBlock(materialPropertyBlock);
            // Assign our new value.
            if (side.image != null)
            {
                var image = side.image;
                materialPropertyBlock.SetTexture("_BaseMap", image);
                var aspectRatio = image.height / (float)image.width;
                renderer.transform.localScale = new Vector3(Mathf.Min(1, 1/aspectRatio), Mathf.Min(1, aspectRatio), 1);
            }
            // Apply the edited values to the renderer.
            renderer.SetPropertyBlock(materialPropertyBlock);
        }
    }

    [SerializeField] private EvidenceReference evidenceReference;

    public Side front;
    public Side back;

    protected void Update()
    {
        if (evidenceReference == null)
        {
            evidenceReference = GetComponent<EvidenceReference>();
        }
        if (evidenceReference != null && evidenceReference.evidence != null)
        {
            if (evidenceReference.evidence is PaperEvidenceAsset paperEvidence)
            {
                front.UpdateMaterial(paperEvidence.front);
                back.UpdateMaterial(paperEvidence.back);
            }
            else
            {
                Debug.LogErrorFormat("Evidence reference on {0} must be a {1}", gameObject, nameof(PaperEvidenceAsset));
            }
        }
        else
        {
            //Debug.LogError(this.GetType().Name + " is not working");
        }
    }
}
