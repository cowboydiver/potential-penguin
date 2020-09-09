using UnityEngine;

public class ForceLevelInfoBoxLogic : InfoBoxLogic
{
    void Awake()
    {
        //If replaying level
        if (GameManager.Inst != null && GameManager.Inst.HideInfoBoxes)
        {
            InGameGUI gui = FindObjectOfType<InGameGUI>();
            gui.ForceEqualSignText.transform.parent.gameObject.SetActive(true);
            gui.ForceEqualSignText.gameObject.SetActive(true);
            gui.ForcePotentialText.gameObject.SetActive(true);
            gui.TotalForceText.gameObject.SetActive(true);
            gui.ForceTangent0Text.gameObject.SetActive(true);
            gui.ForceTangent1Text.gameObject.SetActive(true);
            gui.UpdateForceTextPositions();

            Penguin penguin = FindObjectOfType<Penguin>();
            penguin.UpdateForceArrow(true);
        }
    }

    public override void InfoBoxShown()
    {
        InGameGUI gui = FindObjectOfType<InGameGUI>();
        gui.ForceEqualSignText.transform.parent.gameObject.SetActive(true);
        gui.ForceEqualSignText.gameObject.SetActive(true);
        gui.ForcePotentialText.gameObject.SetActive(true);

        gui.TotalForceText.gameObject.SetActive(false);
        gui.ForceTangent0Text.gameObject.SetActive(false);
        gui.ForceTangent1Text.gameObject.SetActive(false);
        gui.UpdateForceTextPositions();

        Landscape landscape = FindObjectOfType<Landscape>();
        landscape.ShowSurfaceLine(true, 0, landscape.SurfacePointCount -1);
    }
}