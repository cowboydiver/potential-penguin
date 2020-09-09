using UnityEngine;

public class ForceTangentInfoBoxLogic : InfoBoxLogic
{
    public override void InfoBoxShown()
    {
        InGameGUI gui = FindObjectOfType<InGameGUI>();
        gui.ForceTangent0Text.gameObject.SetActive(true);
        gui.ForceTangent1Text.gameObject.SetActive(true);
        gui.UpdateForceTextPositions();

        PenguinGUI penguinGUI = FindObjectOfType<PenguinGUI>();
        penguinGUI.ShowTangent(true);
    }
}