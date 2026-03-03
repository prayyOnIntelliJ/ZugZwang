using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "GraphicsSO", menuName = "Scriptable Objects/GraphicsSO", order = 1)]
public class GraphicsSO : ScriptableObject
{
    public RenderPipelineAsset[] mobileAssets;
    public List<string> graphicSteps;
    
    public void SetGraphicSettings(int currentGraphicIndex)
    {
        Prefs.SetKey(Prefs.KEY_TYPES.GRAPHICS, currentGraphicIndex);
        GraphicsSettings.defaultRenderPipeline = mobileAssets[currentGraphicIndex];
        QualitySettings.renderPipeline = mobileAssets[currentGraphicIndex];
    }
}