﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Windows.Globalization;
using Windows.ApplicationModel.Resources.Core;

using MicaForEveryone.Interfaces;

#nullable enable

namespace MicaForEveryone.Services
{
    internal class LanguageService : ILanguageService
    {
        public LanguageService()
        {
            SupportedLanguages = GetSupportedLanguages().ToArray();
            foreach (var lstLanguage in Windows.System.UserProfile.GlobalizationPreferences.Languages)
            {
                var userLanuage = new Windows.Globalization.Language(lstLanguage);
                var selLanguage = SupportedLanguages.FirstOrDefault(
                language => userLanuage.LanguageTag == language.LanguageTag
                );
                if (selLanguage != null)
                {
                    CurrentLanguage = selLanguage;
                    break;
                }
            }
            if (CurrentLanguage == null)
            {
                CurrentLanguage = SupportedLanguages[0];
            }
        }


        public Language[] SupportedLanguages { get; }

        public Language CurrentLanguage { get; private set; }

        private IEnumerable<Language> GetSupportedLanguages()
        {
            var r = ResourceManager.Current.MainResourceMap["Resources/AppName"];
            foreach (var c in r.Candidates)
            {
                yield return new Language(c.GetQualifierValue("language"));
            }
        }

        public void SetLanguage(Language language)
        {
            CurrentLanguage = language;
            CultureInfo.CurrentCulture = new CultureInfo(language.LanguageTag);

            ResourceContext.SetGlobalQualifierValue("language", language.LanguageTag);
            ResourceContext.SetGlobalQualifierValue("layoutdirection", language.LayoutDirection switch
            {
                LanguageLayoutDirection.Ltr => "LTR",
                LanguageLayoutDirection.Rtl => "RTL",
                LanguageLayoutDirection.TtbLtr => "TTBLTR",
                LanguageLayoutDirection.TtbRtl => "TTBRTL",
                _ => throw new ArgumentOutOfRangeException(),
            });
            ResourceContext.GetForCurrentView().Reset();
            ResourceContext.GetForViewIndependentUse().Reset();
        }
    }
}
