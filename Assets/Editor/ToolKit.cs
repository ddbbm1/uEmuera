using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class ToolKit : EditorWindow 
{
    [MenuItem("Tools/ToolKit", false, 100)]
    public static void Show()
    {
        EditorWindow.GetWindow(typeof(ToolKit));
    }

    void OnGUI()
    {
        some_utils_tool_.OnGUI();
        show_system_info_.OnGUI();
    }

    class SomeUtilsTool
    {
        public void OnGUI()
        {
            fold_out_ = EditorGUILayout.Foldout(fold_out_, "SomeUtilsTool");
            if(!fold_out_)
                return;

            if(GUILayout.Button("Open persistentDataPath"))
            {
                System.Diagnostics.Process.Start(Application.persistentDataPath);
            }
            if(GUILayout.Button("Open dataPath"))
            {
                System.Diagnostics.Process.Start(Application.dataPath);
            }
            var newkey = EditorGUILayout.TextField("Player Prefs Key:", key);
            if(newkey != key)
            {
                key = newkey;
                text = PlayerPrefs.GetString(newkey, "");
            }
            var ptext = EditorGUILayout.TextField(text);
            if(ptext != text)
            {
                PlayerPrefs.SetString(key, ptext);
                text = ptext;
            }
            if(GUILayout.Button("Delete"))
            {
                PlayerPrefs.DeleteKey(key);
            }
        }
        bool fold_out_ = true;

        string key = "";
        string text = "";
    }
    SomeUtilsTool some_utils_tool_ = new SomeUtilsTool();

    class ShowSystemInfo
    {
        public void OnGUI()
        {
            fold_out_ = EditorGUILayout.Foldout(fold_out_, "ShowSystemInfo");
            if(!fold_out_)
                return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("operatingSystem:");
            EditorGUILayout.LabelField(SystemInfo.operatingSystem);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("deviceName:");
            EditorGUILayout.LabelField(SystemInfo.deviceName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("deviceModel:");
            EditorGUILayout.LabelField(SystemInfo.deviceModel);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("systemMemorySize:");
            EditorGUILayout.LabelField(SystemInfo.systemMemorySize.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("deviceType:");
            EditorGUILayout.LabelField(SystemInfo.deviceType.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("graphicsDeviceName:");
            EditorGUILayout.LabelField(SystemInfo.graphicsDeviceName);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("graphicsMemorySize:");
            EditorGUILayout.LabelField(SystemInfo.graphicsMemorySize.ToString());
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("maxTextureSize:");
            EditorGUILayout.LabelField(SystemInfo.maxTextureSize.ToString());
            EditorGUILayout.EndHorizontal();
        }

        bool fold_out_ = true;
    }
    ShowSystemInfo show_system_info_ = new ShowSystemInfo();
}
class targetcahge
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string projectPath)
    {
        RunPostBuildScript(target: target, projectPath: projectPath);
    }
private static void RunPostBuildScript(BuildTarget target, string projectPath = "")
    {
        if (target == BuildTarget.iOS)
        {
#if UNITY_IOS
	            Debug.Log("[Adjust]: Starting to perform post build tasks for iOS platform.");
	
	            string xcodeProjectPath = projectPath + "/Unity-iPhone.xcodeproj/project.pbxproj";
	
	            PBXProject xcodeProject = new PBXProject();
	            xcodeProject.ReadFromFile(xcodeProjectPath);
	
	#if UNITY_2019_3_OR_NEWER
	            string xcodeTarget = xcodeProject.GetUnityMainTargetGuid();
	#else
	            string xcodeTarget = xcodeProject.TargetGuidByName("Unity-iPhone");
	#endif
	            HandlePlistIosChanges(projectPath);
	
	xcodeProject.WriteToFile(xcodeProjectPath);
#endif
	}
}
#if UNITY_IOS
   private static void HandlePlistIosChanges(string projectPath)
{
	        var plistPath = Path.Combine(projectPath, "Info.plist");
        var plist = new PlistDocument();
        plist.ReadFromFile(plistPath);
        var plistRoot = plist.root;
	CreatePlistElementBoolean(plistRoot, "UIFileSharingEnabled");
	CreatePlistElementBoolean(plistRoot, "LSSupportsOpeningDocumentsInPlace");
	
	File.WriteAllText(plistPath, plist.WriteToString());
	}
#endif
   private static  PlistElementBoolean CreatePlistElementBoolean(PlistElementDict root, string key)
    {
        if (!root.values.ContainsKey(key))
        {
            Debug.Log(string.Format("[Adjust]: {0} not found in Info.plist. Creating a new one.", key));
            return root.SetBoolean(key,true);
        }
        var result = root.values[key].AsBoolean(true);
        return result != null ? result : root.SetBoolean(key,true);
    }

}
/*
public static class XcodeOption
{

    [PostProcessBuild(999)]
    public static void OnPostProcessBuild( BuildTarget buildTarget, string path)
    {
        if(buildTarget == BuildTarget.iOS)
        {
            {
                string projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";

                PBXProject pbxProject = new PBXProject();
                pbxProject.ReadFromFile(projectPath);

                string target = pbxProject.TargetGuidByName("Unity-iPhone");            
                pbxProject.SetBuildProperty(target, "ENABLE_BITCODE", "NO");

                pbxProject.WriteToFile (projectPath);
            }

            {
                string infoPlistPath = path + "/Info.plist";

                PlistDocument plistDoc = new PlistDocument();
                plistDoc.ReadFromFile(infoPlistPath);
                if (plistDoc.root != null) {
                	plistDoc.root.SetBoolean("UIFileSharingEnabled", true);
			plistDoc.root.SetBoolean("LSSupportsOpeningDocumentsInPlace", true);
                	plistDoc.WriteToFile(infoPlistPath);
                }
                else {
                    Debug.LogError("ERROR: Can't open " + infoPlistPath);
                }
            }

            //ITSAppUsesNonExemptEncryption
        }
    }
	*/

