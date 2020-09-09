using UnityEngine;

public class ForceFinishedInfoBoxLogic : InfoBoxLogic
{
    public override void InfoBoxHidden()
    {
        //InGameGUI gui = FindObjectOfType<InGameGUI>();
        //gui.TotalForceText.gameObject.SetActive(true);
        //gui.UpdateForceTextPositions();

        PenguinGUI penguinGUI = FindObjectOfType<PenguinGUI>();
        penguinGUI.ShowTangent(false);

        Landscape landscape = FindObjectOfType<Landscape>();
        landscape.ShowSurfaceLine(false, 0, 0);

        Penguin penguin = FindObjectOfType<Penguin>();
        penguin.UpdateForceArrow(true);
    }
}