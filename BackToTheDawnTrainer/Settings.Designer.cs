﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace BackToTheDawnTrainer {
    
    
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.14.0.0")]
    internal sealed partial class Settings : global::System.Configuration.ApplicationSettingsBase {
        
        private static Settings defaultInstance = ((Settings)(global::System.Configuration.ApplicationSettingsBase.Synchronized(new Settings())));
        
        public static Settings Default {
            get {
                return defaultInstance;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Windows.Rect MainWindow_RestoreBounds {
            get {
                return ((global::System.Windows.Rect)(this["MainWindow_RestoreBounds"]));
            }
            set {
                this["MainWindow_RestoreBounds"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Windows.WindowState MainWindow_WindowState {
            get {
                return ((global::System.Windows.WindowState)(this["MainWindow_WindowState"]));
            }
            set {
                this["MainWindow_WindowState"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Windows.GridLength MainWindow_Splitter_UpRowHeight {
            get {
                return ((global::System.Windows.GridLength)(this["MainWindow_Splitter_UpRowHeight"]));
            }
            set {
                this["MainWindow_Splitter_UpRowHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.Windows.GridLength MainWindow_Splitter_DownRowHeight {
            get {
                return ((global::System.Windows.GridLength)(this["MainWindow_Splitter_DownRowHeight"]));
            }
            set {
                this["MainWindow_Splitter_DownRowHeight"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::System.DateTime GameFileLastWriteTime {
            get {
                return ((global::System.DateTime)(this["GameFileLastWriteTime"]));
            }
            set {
                this["GameFileLastWriteTime"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::BackToTheDawnTrainer.DiceType Dice1Value {
            get {
                return ((global::BackToTheDawnTrainer.DiceType)(this["Dice1Value"]));
            }
            set {
                this["Dice1Value"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public global::BackToTheDawnTrainer.DiceType Dice2Value {
            get {
                return ((global::BackToTheDawnTrainer.DiceType)(this["Dice2Value"]));
            }
            set {
                this["Dice2Value"] = value;
            }
        }
        
        [global::System.Configuration.UserScopedSettingAttribute()]
        [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public BackToTheDawnTrainer.PokeCardConfig[] GambleValues {
            get {
                return ((BackToTheDawnTrainer.PokeCardConfig[])(this["GambleValues"]));
            }
            set {
                this["GambleValues"] = value;
            }
        }
    }
}
