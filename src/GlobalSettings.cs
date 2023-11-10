using InControl;
using Modding;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using Modding.Converters;

namespace FartKnight
{
    // Taken from Satchel, need to set up an incontrol system (set and action)
    // The player actions are then fed into the keybind menu
    // https://github.com/PrashantMohta/Satchel/blob/master/BetterMenus/Example/Extras.cs#L6
    public class KeyBinds : PlayerActionSet
    {
        public PlayerAction Action;

        public KeyBinds()
        {
            Action = CreatePlayerAction("fart");
            Action.RepeatDelay = 0.1f; // Add small delay on repeat inputs to allow code to run
            DefaultBinds();
        }

        private void DefaultBinds()
        {
            Action.AddDefaultBinding(Key.F);
        }
    }

    public class SaveSettings { }

    public class GlobalSettings
    {
        public bool EnableSound = true;

        [JsonConverter(typeof(PlayerActionSetConverter))]
        public KeyBinds KeyBinds = new KeyBinds();

        [JsonIgnore]
        public LogLevel LogLevel = LogLevel.Fine; // https://prashantmohta.github.io/ModdingDocs/logging.html
    }
}