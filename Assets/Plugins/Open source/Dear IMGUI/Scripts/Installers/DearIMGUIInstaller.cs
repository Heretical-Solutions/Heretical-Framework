using System.Runtime.InteropServices;

using UnityEngine;

using Zenject;

using ImGuiNET;

namespace HereticalSolutions.DearIMGUI
{
    public unsafe class DearIMGUIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log($"[DearIMGUIInstaller] INSTALLING DearIMGUI");
            
            var version = ImGui.GetVersion();
            
            Debug.Log($"[DearIMGUIInstaller] Dear IMGUI version: {version}");
            
            
            //byte* ret = ImGuiNative.igGetVersion();
            
            //string version = Util.StringFromPtr(ret);
            
            //Debug.Log($"[DearIMGUIInstaller] CUSTOM CALL Dear IMGUI version: {version}");
            
            
            Debug.Log($"[DearIMGUIInstaller] INSTALLING COMPLETE");
        }
        
        //[DllImport("cimgui")]
        //private static extern byte* igGetVersion();
    }
}