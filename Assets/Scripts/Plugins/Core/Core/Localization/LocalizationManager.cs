using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using UnityEditor;
using UnityEngine;

namespace Core
{
    public class LocalizationManager
    {
        public const string c_DefaultSpreadSheet	= "Data.csv";
        public const string c_Spreadsheet			= "Spreadsheet";
        public const string c_KeyHeader				= "Key";

        public const string	c_DefaultKeyValue		= "";

        public const string	c_DefaultLanguage		= "en-US";

        //////////////////////////////////////////////////////////////////////////
        [Serializable]
        public enum ChangeLocalizationKeyMode
        {
            None,

            Block,
            Override,
            Inherit,
            Switch
        }

        //////////////////////////////////////////////////////////////////////////
        private readonly Dictionary<string, Dictionary<string, string>>	m_Dictionary = new Dictionary<string, Dictionary<string, string>>();
        private string													m_Language = c_DefaultLanguage;
        private string													m_DeafultSpreadsheetFolder;
		private string                                                   m_LocalizationPath = "Localization";

        // get or set language.
        public string Language
        {
            get => m_Language;
            set
            {
                // try set available language
                if (m_Dictionary.ContainsKey(value))
                    m_Language = value;
                else
                {
                    if (m_Dictionary.ContainsKey(c_DefaultLanguage))
                    {
                        Debug.Log($"Localization language: {value} not presented in the dictionary, default language will be used: {c_DefaultLanguage}");
                        m_Language = c_DefaultLanguage;
                    }
                    else
                    {
                        m_Language = GetLanguages().FirstOrDefault();
                        if (m_Language != null)
                            Debug.Log($"Localization language: {value} not presented in the dictionary, first available language will be used: {m_Language}");
                    }
                }
            }
        }

        //////////////////////////////////////////////////////////////////////////
        public void AutoLanguage()
        {
            // set default language.
            Language = CultureInfo.CurrentCulture.Name;
        }

        public void Read(string path = null, bool clearDictionary = false)
        {
            // read localization spreadsheets.

            // clear dictionary if flag set
            if (clearDictionary)
                m_Dictionary.Clear();

            // save read path
            if (string.IsNullOrEmpty(path) == false)
                m_LocalizationPath = path;

            var textAssets = Resources.LoadAll<TextAsset>(m_LocalizationPath);
		
            if (textAssets.Length == 0)
                return;

            // parse spreadsheets
            foreach (var textAsset in textAssets)
            {
                // read text with CsvHelper
                using (var memoryStream = new MemoryStream(textAsset.bytes))
                using (var stream = new StreamReader(memoryStream, Encoding.UTF8))
                using (var csv = new CsvReader(stream))
                {
                    // set options
                    csv.Configuration.Delimiter = ",";
                    csv.Configuration.HasHeaderRecord = true;

                    // read header
                    csv.Read();
                    csv.ReadHeader();

                    var languageList = csv.Context.HeaderRecord.Except(new []{ c_KeyHeader, c_Spreadsheet }).ToList();
					
                    // validate languages from headers
                    foreach (var language in languageList)	
                        if (m_Dictionary.TryGetValue(language, out var localizationData) == false)
                        {
                            localizationData = new Dictionary<string, string>();
                            m_Dictionary.Add(language, localizationData);
                        }
#if UNITY_EDITOR	// register spreadsheet
                    if (m_Dictionary.ContainsKey(c_Spreadsheet) == false)
                        m_Dictionary[c_Spreadsheet] = new Dictionary<string, string>();
#endif

                    // parse spreadsheet
                    while (csv.Read())
                    {
                        var key = csv.GetField(c_KeyHeader);
                        // try to read all presented languages
                        foreach (var language in languageList)
                        {
                            var localizationData = m_Dictionary[language];
                            // read data or set default
                            if (csv.TryGetField<string>(language, out var data))
                                localizationData[key] = data;
                            else
                                localizationData[key] = c_DefaultKeyValue;
                        }
#if UNITY_EDITOR		// add key to spreadsheet
                        m_Dictionary[c_Spreadsheet][key] = AssetDatabase.GetAssetPath(textAsset);//System.IO.Path.Combine(path, textAsset.name);
#endif
                    }
                }
            }

#if UNITY_EDITOR
            // add default text file
            m_DeafultSpreadsheetFolder = Path.Combine(Path.GetDirectoryName(m_Dictionary[c_Spreadsheet].First().Value), c_DefaultSpreadSheet).Replace('\\', '/');
#endif

            AutoLanguage();
        }

        public void Save(string spreadsheetName = null)
        {
#if UNITY_EDITOR
            // only in editor mode
            if (EditorApplication.isPlaying)
                return;
#endif
            /*m_SaveTimer = 5.0f;
            if (m_SaveProcess == false)
                _SaveImplementation(spreadsheetName);*/
            
            var languageList = m_Dictionary.Keys.Except(new []{c_Spreadsheet}).ToList();

            var dictionaryList = m_Dictionary.Where(n => n.Key != c_Spreadsheet).ToList();

            var headerList = new List<string>();
            headerList.Add(c_KeyHeader);
            headerList.AddRange(languageList);


            foreach (var spreadsheet in m_Dictionary[c_Spreadsheet].ToLookup(key => key.Value, value => value.Key))
            {
                if (spreadsheetName == null || spreadsheet.Key == spreadsheetName)
                    using (var fileStream = File.Create(spreadsheet.Key))
                    using (var stream = new StreamWriter(fileStream, Encoding.UTF8))
                    using (var csv = new CsvWriter(stream))
                    {
                        csv.Configuration.Delimiter = ",";
                        csv.Configuration.HasHeaderRecord = false;

                        // write header
                        foreach (var n in headerList)
                            csv.WriteField(n);
                        csv.NextRecord();

                        // write keys
                        foreach (var key in spreadsheet)
                        {
                            // write key
                            csv.WriteField(key);
                            // get value from dictionary or write default
                            foreach (var dic in dictionaryList)
                                if (dic.Value.TryGetValue(key, out var value))
                                    csv.WriteField(value);
                                else
                                    csv.WriteField(c_DefaultKeyValue);

                            csv.NextRecord();
                        }
                    }
#if UNITY_EDITOR
                // reimport asset to take effect
                AssetDatabase.ImportAsset(spreadsheetName);
#endif
            }
        }
        /*
        private static bool         m_SaveProcess;
        private static float        m_SaveTimer;

        static async void _SaveImplementation(string spreadsheetName)
        {
            if (m_SaveProcess)
                return;

            m_SaveProcess = true;
            while (m_SaveTimer > 0.0f)
            {
                await Task.Delay(1000);
                m_SaveTimer -= 1.0f;
            }

            m_SaveProcess = false;
            Debug.Log($"Saved");
        }*/

        public Dictionary<string, string> GetDictionary(string language = null)
        {
            if (m_Dictionary.TryGetValue(language ?? Language, out var dic))
                return dic;

            return null;
        }

        public string CreateKey(string key, string value = c_DefaultKeyValue)
        {
            // read dictionary
            if (m_Dictionary.ContainsKey(Language) == false)
                Read();

            var localizationKey = "None";
            if (string.IsNullOrEmpty(key) == false)
                localizationKey = key;

            if (m_Dictionary[Language].ContainsKey(localizationKey) == false)
            {
                // add to dictionary
                m_Dictionary[Language].Add(localizationKey, value);

                // add to spreadsheet
                m_Dictionary[c_Spreadsheet].Add(localizationKey, m_DeafultSpreadsheetFolder);
				
                // save changes
                Save(_GetSpreadsheetOfKey(localizationKey));
                Read();
            }

            return localizationKey;
        }

        public bool ChangeKey(string key, string newKey, ChangeLocalizationKeyMode mode)
        {
            // read dictionary
            if (m_Dictionary.ContainsKey(Language) == false)
                Read();

            // do nothing if mode none
            if (mode == ChangeLocalizationKeyMode.None)
                return true;

            var spreadSheet = _GetSpreadsheetOfKey(key);
            
            var result = true;
            // for all languages
            foreach (var dic in m_Dictionary.Values)
            {
                // is key can be changed
                if (dic.TryGetValue(key, out var localizedData))
                {
                    switch (mode)
                    {
                        case ChangeLocalizationKeyMode.None:
                            return true;
                        case ChangeLocalizationKeyMode.Block:
                        {
                            // do nothing if key exist
                            if (dic.ContainsKey(newKey) == false)
                            {
                                // remove changed key
                                dic.Remove(key);
                                // add new
                                dic.Add(newKey, localizedData);
                            }
                            else
                                result = false;
                        }   break;
                        case ChangeLocalizationKeyMode.Override:
                        {
                            if (dic.ContainsKey(newKey))
                            {
                                // override existed key value
                                dic[newKey] = dic[key];
                                dic.Remove(key);
                            }
                            else
                            {
                                // rename key
                                dic.Remove(key);
                                dic.Add(newKey, localizedData);
                            }
                        }	break;
                        case ChangeLocalizationKeyMode.Inherit:
                        {
                            if (dic.ContainsKey(newKey))
                            {
                                // inherit existed key value
                                dic.Remove(key);
                            }
                            else
                            {
                                // rename key
                                dic.Remove(key);
                                dic.Add(newKey, localizedData);
                            }
                        }	break;
                        case ChangeLocalizationKeyMode.Switch:
                        {
                            if (dic.ContainsKey(newKey))
                            {
                                // don't delete previous key
                            }
                            else
                            {
                                // rename key
                                dic.Remove(key);
                                dic.Add(newKey, localizedData);
                            }
                        }	break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
                    }
                }
            }
			
            // save changes
            Save(spreadSheet);

            return result;
        }

        public void ChangeValue(string key, string value)
        {
            m_Dictionary[Language][key] = value;
			
            //m_Dictionary[c_Spreadsheet].Add(localizationKey, m_DeafultSpreadsheetFolder);
            Save(_GetSpreadsheetOfKey(key));
            //Save();
        }
	
        public void RemoveKey(string localizationKey)
        {
            var spreadsheet = _GetSpreadsheetOfKey(localizationKey);

            // for all languages
            foreach (var dic in m_Dictionary.Values)
            {
                // remove key
                dic.Remove(localizationKey);
            }

            // save changes is the key was removed
            if (spreadsheet != null)
            {
                Save(spreadsheet);
            }
        }

        // get localized value by localization key.
        public string Localize(string localizationKey)
        {
/*#if DEBUG
			if (m_Dictionary.ContainsKey(Language) == false)
				Read();

			if (m_Dictionary[Language].ContainsKey(localizationKey) == false)
				m_Dictionary[Language].Add(localizationKey, "None");

			//if (!m_Dictionary.ContainsKey(Language)) throw new KeyNotFoundException("Language not found: " + Language);
			//if (!m_Dictionary[Language].ContainsKey(localizationKey)) throw new KeyNotFoundException("Translation not found: " + localizationKey);
#endif*/

            if (string.IsNullOrEmpty(localizationKey) == false 
                && m_Dictionary.TryGetValue(Language, out var dic)
                && dic.TryGetValue(localizationKey, out var data))
                return data;

            return c_DefaultKeyValue;
        }

        // get localized value by localization key.
        public string Localize(string localizationKey, params object[] args)
        {
            var pattern = Localize(localizationKey);

            return string.Format(pattern, args);
        }

        public List<string> GetLanguages()
        {
            var result = m_Dictionary.Keys.ToList();
#if UNITY_EDITOR
            result.Remove(c_Spreadsheet);
#endif
            return result;
        }

        public bool ContainsKey(string key)
        {
            if (m_Dictionary.Count == 0)
                Read();
            return m_Dictionary[c_Spreadsheet].ContainsKey(key);
        }

        //////////////////////////////////////////////////////////////////////////
        private string _GetSpreadsheetOfKey(string key)
        {
            if (m_Dictionary[c_Spreadsheet].TryGetValue(key, out string value))
                return value;

            return null;
        }
    }
}