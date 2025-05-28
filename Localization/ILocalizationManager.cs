using System;

using HereticalSolutions.Metadata;

namespace HereticalSolutions.Localization
{
    public interface ILocalizationManager
    {
        void SwitchLanguage(
            LanguageSettings settings);
        
        LocalizedString Translate(
            string key);
        
        LocalizedString Translate(
            string key,
            PlaceholderOptions options);

        string GetLocalizedString(
            string key,
            WeaklyTypedMetadata parameters);

        Action<string> OnLanguageChanged { get; set; }
    }
}