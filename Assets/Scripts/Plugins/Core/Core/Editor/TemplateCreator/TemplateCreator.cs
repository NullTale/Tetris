using System.IO;
using System.Text;
using Core;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;

namespace Core
{
    public class DoCreateCodeFile : EndNameEditAction
    {
        public string		m_WipeString = string.Empty;
		 
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            Object o = CodeTemplates.CreateScript(pathName, resourceFile, m_WipeString);
            ProjectWindowUtil.ShowCreatedAsset(o);
        }
    }

    public class CodeTemplates 
    {
	    // The C# script icon.
	    private static Texture2D scriptIcon = (EditorGUIUtility.IconContent("cs Script Icon").image as Texture2D);
		    
	    internal static UnityEngine.Object CreateScript(string pathName, string templatePath, string wipeString)
	    {
		    string newFilePath				= Path.GetFullPath(pathName);
		    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(pathName);
		    string className				= NormalizeClassName(fileNameWithoutExtension, wipeString);

		    string templateText = string.Empty;

		    if(File.Exists(templatePath))
		    {
			    using(var sr = new StreamReader(templatePath))
			    {
				    templateText = sr.ReadToEnd();
			    }

			    templateText = templateText.Replace("$Name", className);
				    
			    UTF8Encoding encoding = new UTF8Encoding (true, false);
			    using(var sw = new StreamWriter(newFilePath, false, encoding))
			    {
				    sw.Write (templateText);
			    }
				    
			    AssetDatabase.ImportAsset(pathName);
				    
			    return AssetDatabase.LoadAssetAtPath(pathName, typeof(Object));
		    }
		    else
		    {
			    Debug.LogError(string.Format("The template file was not found: {0}", templatePath));

			    return null;
		    }
	    }

	    private static string NormalizeClassName(string fileName, string wipeString)
	    {
		    return fileName.Replace(" ", string.Empty).Replace(wipeString, string.Empty);
	    }
	    
	    public static void CreateFromTemplate(string initialName, string templatePath, string wipeString)
	    {
		    var doCreateFile = ScriptableObject.CreateInstance<DoCreateCodeFile>();
		    doCreateFile.m_WipeString = wipeString;

		    ProjectWindowUtil.StartNameEditingIfProjectWindowExists(
			    0,
			    doCreateFile,
			    initialName,
			    scriptIcon,
			    templatePath);
	    }
    }

    public class TemplateMenuItems
    {
	    //////////////////////////////////////////////////////////////////////////
	    private const string MENU_ITEM_PATH = "Assets/Create/TemplateScript/";

	    private const int MENU_ITEM_PRIORITY = 70;


	    //////////////////////////////////////////////////////////////////////////
	    [MenuItem(MENU_ITEM_PATH + "CharacterClass", false, MENU_ITEM_PRIORITY)]
	    private static void CreateCharacterClass()
	    {
		    CodeTemplates.CreateFromTemplate(
			    "NameCharacterClass.cs", 
			    @"Assets/Scripts/Editor/CodeTemplates/CharacterClass.txt",
			    "CharacterClass");
	    }
	    [MenuItem(MENU_ITEM_PATH + "CharacterCharacteristic", false, MENU_ITEM_PRIORITY)]
	    private static void CreateCharacterCharacteristic()
	    {
		    CodeTemplates.CreateFromTemplate(
			    "NameCharacterCharacteristic.cs", 
			    @"Assets/Scripts/Editor/CodeTemplates/CharacterCharacteristic.txt",
			    "CharacterCharacteristic");
	    }
	    [MenuItem(MENU_ITEM_PATH + "TileNodeLogic", false, MENU_ITEM_PRIORITY)]
	    private static void CreateTileNodeLogic()
	    {
		    CodeTemplates.CreateFromTemplate(
			    "TileNodeLogic.cs", 
			    @"Assets/Scripts/Editor/CodeTemplates/TileNodeLogic.txt",
			    "TileNodeLogic");
	    }
	    
    }
}

