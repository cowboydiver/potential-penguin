using UnityEngine;

public class ForceArrowInfoBoxLogic : InfoBoxLogic
{
    public override void InfoBoxShown()
    {
        InGameGUI gui = FindObjectOfType<InGameGUI>();
        gui.TotalForceText.gameObject.SetActive(true);
        gui.UpdateForceTextPositions();

        PenguinGUI penguinGUI = FindObjectOfType<PenguinGUI>();
        Penguin penguin = FindObjectOfType<Penguin>();
        penguin.UpdateForceArrow(true);
        //penguinGUI.UpdateForceArrow(1f, false);
    }
}