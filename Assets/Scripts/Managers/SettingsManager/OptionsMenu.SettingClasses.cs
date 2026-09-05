using UnityEngine;

/// <summary>
/// This file contains the classes for the different types of settings that can be used in the OptionsMenu.
/// </summary>
public partial class OptionsMenu
{
    private abstract class OptionSetting
    {
        public readonly string Key;
        protected OptionSetting(string key)
        {
            Key = key;
        }
        public abstract void InvokeChangeSettingFunction();
        public abstract void ApplySavedToCurrent();
        public abstract void ApplyCurrentToSaved(); // As in: save new values
        public abstract void WriteToPlayerPrefs();
        public abstract void ResetToDefault();
        public abstract bool Changed(); // As in: is value changed?
    }
    private class StringSetting : OptionSetting
    {
        private readonly System.Action<string> changeSettingFunction;
        public readonly string DefaultValue;
        public string CurrentValue;
        public string SavedValue;
        public StringSetting(string key, string defaultValue, System.Action<string> changeSettingFunction) : base(key)
        {
            DefaultValue = defaultValue;
            this.changeSettingFunction = changeSettingFunction;
            if (PlayerPrefs.HasKey(key))
            {
                SavedValue = PlayerPrefs.GetString(key);
                CurrentValue = SavedValue;
            }
            else
            {
                SavedValue = defaultValue;
                CurrentValue = defaultValue;
                PlayerPrefs.SetString(key, defaultValue);
            }
            allSettings.Add(this);
        }
        public override void InvokeChangeSettingFunction()
        {
            changeSettingFunction?.Invoke(CurrentValue);
        }
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
        }
        public override void ApplyCurrentToSaved()
        {
            SavedValue = CurrentValue;
        }
        public override void WriteToPlayerPrefs()
        {
            PlayerPrefs.SetString(Key, SavedValue);
        }
        public override void ResetToDefault()
        {
            CurrentValue = DefaultValue;
        }
        public override bool Changed()
        {
            return !string.Equals(SavedValue, CurrentValue);
        }
    }
    private class FloatSetting : OptionSetting
    {
        private readonly System.Action<float> changeSettingFunction;
        public readonly float DefaultValue;
        public float CurrentValue;
        public float SavedValue;
        public FloatSetting(string key, float defaultValue, System.Action<float> changeSettingFunction) : base(key)
        {
            DefaultValue = defaultValue;
            this.changeSettingFunction = changeSettingFunction;
            if (PlayerPrefs.HasKey(key))
            {
                SavedValue = PlayerPrefs.GetFloat(key);
                CurrentValue = SavedValue;
            }
            else
            {
                SavedValue = defaultValue;
                CurrentValue = defaultValue;
                PlayerPrefs.SetFloat(key, defaultValue);
            }
            allSettings.Add(this);
        }
        public override void InvokeChangeSettingFunction()
        {
            changeSettingFunction?.Invoke(CurrentValue);
        }
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
        }
        public override void ApplyCurrentToSaved()
        {
            SavedValue = CurrentValue;
        }
        public override void WriteToPlayerPrefs()
        {
            PlayerPrefs.SetFloat(Key, SavedValue);
        }
        public override void ResetToDefault()
        {
            CurrentValue = DefaultValue;
        }
        public override bool Changed()
        {
            return !Mathf.Approximately(CurrentValue, SavedValue);
        }
    }
    private class IntSetting : OptionSetting
    {
        private readonly System.Action<int> changeSettingFunction;
        public readonly int DefaultValue;
        public int CurrentValue;
        public int SavedValue;
        public IntSetting(string key, int defaultValue, System.Action<int> changeSettingFunction) : base(key)
        {
            DefaultValue = defaultValue;
            this.changeSettingFunction = changeSettingFunction;
            if (PlayerPrefs.HasKey(key))
            {
                SavedValue = PlayerPrefs.GetInt(key);
                CurrentValue = SavedValue;
            }
            else
            {
                SavedValue = defaultValue;
                CurrentValue = defaultValue;
                PlayerPrefs.SetInt(key, defaultValue);
            }
            allSettings.Add(this);
        }
        public override void InvokeChangeSettingFunction()
        {
            changeSettingFunction?.Invoke(CurrentValue);
        }
        public override void ApplySavedToCurrent()
        {
            CurrentValue = SavedValue;
        }
        public override void ApplyCurrentToSaved()
        {
            SavedValue = CurrentValue;
        }
        public override void WriteToPlayerPrefs()
        {
            PlayerPrefs.SetInt(Key, SavedValue);
        }
        public override void ResetToDefault()
        {
            CurrentValue = DefaultValue;
        }
        public override bool Changed()
        {
            return CurrentValue != SavedValue;
        }
    }
}
