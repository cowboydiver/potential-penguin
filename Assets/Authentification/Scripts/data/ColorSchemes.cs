using UnityEngine;
using System.Collections.Generic;
public class ColorSchemes {

    //Send in an enum. returns the primary color;
    public static Color GetColor(SAHStyles.ColorMode target)
    {
        Color c = RedPrimary;

        switch(target)
        {
            case SAHStyles.ColorMode.Red :
                c = RedPrimary;
                break;
            case SAHStyles.ColorMode.Blue :
                c = BluePrimary;
                break;
            case SAHStyles.ColorMode.Green :
                c = GreenPrimary;
                break;
            case SAHStyles.ColorMode.Orange :
                c = OrangePrimary;
                break;
            case SAHStyles.ColorMode.Purple :
                c = PurplePrimary;
                break;
            case SAHStyles.ColorMode.Yellow :
                c = YellowPrimary;
                break;
        }

        return c;
    }

    public static List<Color> GetColors(SAHStyles.ColorMode target)
    {
        List<Color> colors = new List<Color>();

        switch (target)
        {
            case SAHStyles.ColorMode.Red:
                colors.Add(RedPrimary);
                colors.Add(RedLight);
                colors.Add(RedDark);
                break;
            case SAHStyles.ColorMode.Blue:
                colors.Add(BluePrimary);
                colors.Add(BlueLight);
                colors.Add(BlueDark);
                break;
            case SAHStyles.ColorMode.Green:
                colors.Add(GreenPrimary);
                colors.Add(GreenLight);
                colors.Add(GreenDark);
                break;
            case SAHStyles.ColorMode.Orange:
                colors.Add(OrangePrimary);
                colors.Add(OrangeLight);
                colors.Add(OrangeDark);
                break;
            case SAHStyles.ColorMode.Purple:
                colors.Add(PurplePrimary);
                colors.Add(PurpleLight);
                colors.Add(PurpleDark);
                break;
            case SAHStyles.ColorMode.Yellow:
                colors.Add(YellowPrimary);
                colors.Add(YellowLight);
                colors.Add(YellowDark);
                break;
        }

        return colors;

    }

    public static List<Color> GetTextButtonColors ()
    {
        List<Color> textcolors = new List<Color>();

        float normcolor = 161.0f / 255.0f;
        float overcolor = 218.0f / 255.0f;
        textcolors.Add(new Color(normcolor,normcolor,normcolor));
        textcolors.Add(new Color(overcolor,overcolor,overcolor));

        return textcolors;

    }

    private static Color _PurplePrimary = new Color(79.0f / 255.0f,54.0f / 255.0f,124.0f / 255.0f);

  public static Color PurplePrimary
  {
    get
    {
      return _PurplePrimary;
    }
  }

    private static Color _PurpleDark = new Color(55.0f / 255.0f, 37.0f / 255.0f, 99.0f / 255.0f);

  public static Color PurpleDark
  {
    get
    {
      return _PurpleDark;
    }
  }

    private static Color _PurpleLight = new Color(140.0f / 255.0f, 124.0f / 255.0f, 183.0f / 255.0f);

    public static Color PurpleLight
  {
    get
    {
      return _PurpleLight;
    }
  }

    private static Color _RedPrimary = new Color(202.0f / 255.0f,33.0f / 255.0f,39.0f / 255.0f);

  public static Color RedPrimary
  {
    get
    {
      return _RedPrimary;
    }
  }

    private static Color _RedDark = new Color(178.0f / 255.0f, 36.0f / 255.0f, 53.0f / 255.0f);

  public static Color RedDark
  {
    get
    {
      return _RedDark;
    }
  }

    private static Color _RedLight = new Color(238.0f / 255.0f,53.0f / 255.0f,50.0f / 255.0f);

    public static Color RedLight
  {
    get
    {
      return _RedLight;
    }
  }

  private static Color _OrangePrimary = new Color(237.0f / 255.0f, 98.0f / 255.0f, 58.0f /255);  

  public static Color OrangePrimary
  {
    get
    {
      return _OrangePrimary;
    }
  }

    private static Color _OrangeDark = new Color(226.0f / 255.0f, 70.0f / 255.0f, 39.0f / 255.0f);

  public static Color OrangeDark
  {
    get
    {
      return _OrangeDark;
    }
  }

    private static Color _OrangeLight = new Color(243.0f / 255.0f, 130.0f / 255.0f, 109.0f / 255.0f);

    public static Color OrangeLight
  {
    get
    {
      return _OrangeLight;
    }
  }

    private static Color _YellowPrimary = new Color(253.0f / 255.0f, 203.0f / 255.0f, 69.0f / 255.0f);

  public static Color YellowPrimary
  {
    get
    {
      return _YellowPrimary;
    }
  }

    private static Color _YellowDark = new Color(232.0f / 255.0f, 165.0f / 255.0f, 37.0f / 255.0f);

  public static Color YellowDark
  {
    get
    {
      return _YellowDark;
    }
  }

    private static Color _YellowLight = new Color(252.0f / 255.0f, 218.0f / 255.0f, 144.0f / 255.0f);

    public static Color YellowLight
  {
    get
    {
      return _YellowLight;
    }
  }

    private static Color _GreenPrimary = new Color(85.0f / 255.0f, 185.0f / 255.0f, 72.0f / 255.0f);

  public static Color GreenPrimary
  {
    get
    {
      return _GreenPrimary;
    }
  }

    private static Color _GreenDark = new Color(30.0f / 255.0f,97.0f / 255.0f,60.0f / 255.0f);

  public static Color GreenDark
  {
    get
    {
      return _GreenDark;
    }
  }

    private static Color _GreenLight = new Color(139.0f / 255.0f,202.0f / 255.0f,134.0f / 255.0f);

    public static Color GreenLight
  {
    get
    {
      return _GreenLight;
    }
  }

    private static Color _BluePrimary = new Color(39.0f / 255.0f, 121.0f / 255.0f, 186.0f / 255.0f);

  public static Color BluePrimary
  {
    get
    {
      return _BluePrimary;
    }
  }

    private static Color _BlueDark = new Color(24.0f / 255.0f, 76.0f / 255.0f, 114.0f / 255.0f);

  public static Color BlueDark
  {
    get
    {
      return _BlueDark;
    }
  }

    private static Color _BlueLight = new Color(87.0f / 255.0f, 192.0f / 255.0f, 232.0f / 255.0f);

  public static Color BlueLight
  {
    get
    {
      return _BlueLight;
    }
  }

    private static Color _GrayPrimary = new Color(98.0f / 255.0f, 105.0f / 255.0f, 111.0f / 255.0f);

  public static Color GrayPrimary
  {
    get
    {
      return _GrayPrimary;
    }
  }

    private static Color _GrayDark = new Color(62.0f / 255.0f, 67.0f / 255.0f, 70.0f / 255.0f);

  public static Color GrayDark
  {
    get
    {
      return _GrayDark;
    }
  }

    private static Color _GrayLight = new Color(176.0f / 255.0f,181.0f / 255.0f,186.0f / 255.0f);

  public static Color GrayLight
  {
    get
    {
      return _GrayLight;
    }
  }

    private static Color _FacebookPrimary = new Color(139.0f / 255.0f, 157.0f / 255.0f, 195.0f / 255.0f);

  public static Color FacebookPrimary
  {
    get
    {
      return _FacebookPrimary;
    }
  }

    private static Color _FacebookDark = new Color(59.0f / 255.0f, 89.0f / 255.0f, 152.0f / 255.0f);

  public static Color FacebookDark
  {
    get
    {
      return _FacebookDark;
    }
  }
    private static Color _FacebookLight = new Color(223.0f / 255.0f, 227.0f / 255.0f, 238.0f / 255.0f);

  public static Color FacebookLight
  {
    get
    {
      return _FacebookLight;
    }
  }
}
