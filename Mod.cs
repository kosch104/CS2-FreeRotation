using Colossal.Logging;
using Game;
using Game.Modding;
using Game.SceneFlow;
using Colossal.IO.AssetDatabase;
using FreeRotation.Systems;
using Game.Input;

namespace FreeRotation
{
    public class Mod : IMod
    {
        public static ILog log = LogManager.GetLogger($"{nameof(FreeRotation)}.{nameof(Mod)}")
            .SetShowsErrorsInUI(false);

        public static Setting m_Setting;
        public static ProxyAction FreeRotateLeftAction;
        public static ProxyAction FreeRotateRightAction;
        public static ProxyAction DegreeRotateLeftAction;
        public static ProxyAction DegreeRotateRightAction;
        public const string kFreeRotateLeftName = "FreeRotateLeftBinding";
        public const string kFreeRotateRightName = "FreeRotateRightBinding";
        public const string kDegreeRotateLeftName = "DegreeRotateLeftBinding";
        public const string kDegreeRotateRightName = "DegreeRotateRightBinding";

        public void OnLoad(UpdateSystem updateSystem)
        {
            log.Info(nameof(OnLoad));

            if (GameManager.instance.modManager.TryGetExecutableAsset(this, out var asset))
                log.Info($"Current mod asset at {asset.path}");

            m_Setting = new Setting(this);
            m_Setting.RegisterInOptionsUI();
            GameManager.instance.localizationManager.AddSource("en-US", new LocaleEN(m_Setting));


            m_Setting.RegisterKeyBindings();

            FreeRotateLeftAction = m_Setting.GetAction(kFreeRotateLeftName);
            FreeRotateRightAction = m_Setting.GetAction(kFreeRotateRightName);
            DegreeRotateLeftAction = m_Setting.GetAction(kDegreeRotateLeftName);
            DegreeRotateRightAction = m_Setting.GetAction(kDegreeRotateRightName);

            FreeRotateLeftAction.shouldBeEnabled = true;
            FreeRotateRightAction.shouldBeEnabled = true;
            DegreeRotateLeftAction.shouldBeEnabled = true;
            DegreeRotateRightAction.shouldBeEnabled = true;

            FreeRotateLeftAction.onInteraction += (_, phase) =>
                RotationSystem.Instance.OnFreelyRotateLeft(phase);

            FreeRotateRightAction.onInteraction += (_, phase) =>
                RotationSystem.Instance.OnFreelyRotateRight(phase);

            DegreeRotateLeftAction.onInteraction += (_, phase) =>
                RotationSystem.Instance.OnDegreeRotateLeft(phase);

            DegreeRotateRightAction.onInteraction += (_, phase) =>
                RotationSystem.Instance.OnDegreeRotateRight(phase);

            AssetDatabase.global.LoadSettings(nameof(FreeRotation), m_Setting, new Setting(this));

            updateSystem.UpdateBefore<RotationSystem>(SystemUpdatePhase.Modification1);
        }

        public void OnDispose()
        {
            log.Info(nameof(OnDispose));
            if (m_Setting != null)
            {
                m_Setting.UnregisterInOptionsUI();
                m_Setting = null;
            }
        }
    }
}