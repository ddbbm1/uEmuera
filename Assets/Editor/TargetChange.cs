using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;

public class NewBehaviourScript : MonoBehaviour
{
    // Start is called before the first frame update
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
	plistRoot.SetBoolean("UIFileSharingEnabled",true);
	plistRoot.SetBoolean("LSSupportsOpeningDocumentsInPlace",true);
	plistRoot.SetBoolean("UISupportsDocumentBrowser",true);

	
	Debug.Log("[Adjust]: plist UIFileSharingEnabled,LSSupportsOpeningDocumentsInPlace change.");
	File.WriteAllText(plistPath, plist.WriteToString());
	}
#endif
}
