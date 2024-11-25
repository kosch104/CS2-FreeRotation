using Colossal;
using Colossal.IO.AssetDatabase;
using Game.Modding;
using Game.Settings;
using Game.UI;
using Game.UI.Widgets;
using System.Collections.Generic;
using Game.Input;
using Game.UI.Localization;
using UnityEngine.InputSystem;

public enum RotationAxis
{
    X,
    Y,
    Z
}

namespace FreeRotation
{
    [FileLocation($"ModsSettings/{nameof(FreeRotation)}/{nameof(FreeRotation)}")]
    public class Setting : ModSetting
    {
        public Setting(IMod mod) : base(mod)
        {
        }

        [SettingsUIKeyboardBinding(BindingKeyboard.LeftArrow, Mod.kFreeRotateLeftName)]
        public ProxyBinding FreeRotateLeftBinding { get; set; }

        [SettingsUIKeyboardBinding(BindingKeyboard.RightArrow, Mod.kFreeRotateRightName)]
        public ProxyBinding FreeRotateRightBinding { get; set; }

        [SettingsUISlider(min = 0.1f, max = 10, step = 0.1f, unit=Unit.kFloatSingleFraction)]
        public float RotationSpeed { get; set; } = 1;

        public RotationAxis RotationAxis { get; set; } = RotationAxis.Y;

        public override void SetDefaults()
        {
            RotationSpeed = 1;
        }

        public bool ResetBindings
        {
            set
            {
                Mod.log.Info("Reset key bindings");
                ResetKeyBindings();
            }
        }
    }

    public class LocaleEN : IDictionarySource
    {
        private readonly Setting m_Setting;

        public LocaleEN(Setting setting)
        {
            m_Setting = setting;
        }

        public IEnumerable<KeyValuePair<string, string>> ReadEntries(IList<IDictionaryEntryError> errors,
            Dictionary<string, int> indexCounts)
        {
            return new Dictionary<string, string>
            {
                { m_Setting.GetSettingsLocaleID(), "Asset Variation Changer" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FreeRotateLeftBinding)), "Freely rotate to left" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.FreeRotateLeftBinding)),
                    "Freely rotate selection to the left while this key is held down"
                },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.FreeRotateRightBinding)), "Freely rotate to right" },
                {
                    m_Setting.GetOptionDescLocaleID(nameof(Setting.FreeRotateRightBinding)),
                    "Freely rotate selection to the right while this key is held down"
                },


                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.ResetBindings)), "Reset key bindings" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.ResetBindings)), $"" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RotationSpeed)), "Rotation speed" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RotationSpeed)), $"Change modifier of rotation speed" },

                { m_Setting.GetOptionLabelLocaleID(nameof(Setting.RotationAxis)), "Rotation Axis" },
                { m_Setting.GetOptionDescLocaleID(nameof(Setting.RotationAxis)), $"Change Axis of Rotation. Y is default" },

                { m_Setting.GetBindingKeyLocaleID(Mod.kFreeRotateLeftName), "Freely rotate to left" },
                { m_Setting.GetBindingKeyLocaleID(Mod.kFreeRotateRightName), "Freely rotate to right" },
            };
        }

        public void Unload()
        {
        }
    }
}