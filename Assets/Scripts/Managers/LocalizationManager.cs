using UnityEngine;
using System.Collections.Generic;
using System.Xml;

public class LocalizationManager : Singleton<LocalizationManager> {
    #region Public Variables
    public delegate void SubscriberList();
    public SubscriberList UpdateTextObjects;
    #endregion
    #region Private Variables

    private IDictionary<string, string> _activeLanguageContent = new Dictionary<string, string>();
    private string _language = "EN";
    private string Language {
        get {
            return _language;
        }
        set {
            if (_language != value) {
                _language = value;
                LoadContent();
            }
        }
    }
    #endregion
    #region Public Functions

    public string GetText(string key) {
        string result = "";
        Content.TryGetValue(key, out result);

        if (string.IsNullOrEmpty(result))
        {
            Debug.LogError(key + "[" + Language + "]" + " No Text defined. Maybe 'TextKey' string is spelled wrong in 'Localization Text' component?");
            return "";
        }

        return result;
    }
    public string GetLanguage() {
        return Language;
    }

    public void SetLanguage(string language) {
        Language = language;
        UpdateLocalization();
    }
    #endregion
    #region Private Functions
    void Awake()
    {
        if (FindObjectsOfType<LocalizationManager>().Length == 1 || inst == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void UpdateLocalization() {
        UpdateTextObjects();
    }

    private IDictionary<string, string> Content {
        get {
            if (_activeLanguageContent == null || _activeLanguageContent.Count == 0)
                LoadContent();
            return _activeLanguageContent;
        }
    }
    private IDictionary<string, string> GetContent() {
        if (_activeLanguageContent == null || _activeLanguageContent.Count == 0) {
            LoadContent();
        }
        return _activeLanguageContent;
    }

    private void AddContent(XmlNode xNode) {
        foreach (XmlNode node in xNode.ChildNodes) {
            if (node.LocalName == "TextKey") {
                string value = node.Attributes.GetNamedItem("name").Value;
                string text = string.Empty;
                foreach (XmlNode langNode in node) {
                    if (langNode.LocalName == _language) {
                        text = langNode.InnerText;
                        if (_activeLanguageContent.ContainsKey(value)) {
                            _activeLanguageContent.Remove(value);
                            _activeLanguageContent.Add(value, value + " has been found multiple times in the XML allowed only once!");
                        }
                        else {
                            _activeLanguageContent.Add(value, (!string.IsNullOrEmpty(text)) ? text : ("No Text for " + value + " found"));
                        }
                        break;
                    }
                }
            }
        }
    }
    private void LoadContent() {
        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(Resources.Load("LocalizationText").ToString());
        if (xmlDocument == null) {
            Debug.Log("Couldn't Load Xml");
            return;
        }
        if (_activeLanguageContent != null) {
            _activeLanguageContent.Clear();
        }
        XmlNode xNode = xmlDocument.ChildNodes.Item(1).ChildNodes.Item(0);
        AddContent(xNode);
    }
}

#endregion
