using TMPro;
using UnityEngine;

[RequireComponent(typeof(ResolutionTextReferences))]
public class ResolutionText : TextMeshProUGUI
{
    private int currentGraphicsIndex;
    private ResolutionTextReferences textReferences;
    private SettingsOverwriter settingsOverwriter;
    private GraphicsSO graphicsSO;

    private new void Awake()
    {
        base.Awake();
        textReferences = GetComponent<ResolutionTextReferences>();
        
        graphicsSO = textReferences.graphicsSO;
        settingsOverwriter = textReferences.settingsOverwriter;
    }

    private new void OnEnable()
    {
        base.OnEnable();
        currentGraphicsIndex = Prefs.GetKey(Prefs.KEY_TYPES.GRAPHICS, currentGraphicsIndex);
        UpdateGraphicsText();
    }

    public void OnPlusButtonClicked()
    {
        ChangeGraphicIndex(+1);
    }

    public void OnMinusButtonClicked()
    {
        ChangeGraphicIndex(-1);
    }
    
    private void ChangeGraphicIndex(int direction)
    {
        currentGraphicsIndex = Mathf.Clamp(Prefs.GetKey(Prefs.KEY_TYPES.GRAPHICS, currentGraphicsIndex) + direction, 0, graphicsSO.graphicSteps.Count - 1);

        settingsOverwriter?.SetGraphicSettings(currentGraphicsIndex);
        // also sets the graphicIndex in the PlayerPrefs
        
        UpdateGraphicsText();
    }

    private void UpdateGraphicsText()
    {
        text = graphicsSO.graphicSteps[currentGraphicsIndex];
    }
}
