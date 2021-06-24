using System;
using System.Collections.Generic;

namespace PactNet.Native
{
    /// <summary>
    /// Defines the message scenario builder
    /// </summary>
    public class MessageScenarioBuilder : IMessageScenarioBuilder
    {
        private static IMessageScenarioBuilder _instance;

        /// <summary>
        /// The message scenario builder singleton instance
        /// </summary>
        public static IMessageScenarioBuilder Instance => _instance ??= new MessageScenarioBuilder();

        /// <summary>
        /// The available scenarios
        /// </summary>
        public readonly Dictionary<string, Func<dynamic>> Scenarios;

        /// <summary>
        /// Default value when scenario not set
        /// </summary>
        private static string ValueNotSet => string.Empty;

        /// <summary>
        /// temporary description
        /// </summary>
        private string _descriptionAdded;

        /// <summary>
        /// tag to know if we are creating a scenario
        /// </summary>
        private bool _settingScenario;

        private MessageScenarioBuilder()
        {
            Scenarios = new Dictionary<string, Func<dynamic>>();
        }

        /// <inheritdoc />
        public IMessageScenarioBuilder WhenReceiving(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (Scenarios.TryGetValue(description, out _))
            {
                throw new InvalidOperationException($"Scenario called \"{description}\" has already been added");
            }

            if (_settingScenario)
            {
                FinishSettingScenario();
                throw new InvalidOperationException($"You need to set the scenario action before adding another scenario");
            }

            _settingScenario = true;
            _descriptionAdded = description;

            return this;
        }

        /// <inheritdoc />
        public void WillPublishMessage(Func<dynamic> action)
        {
            if (!_settingScenario)
            {
                throw new InvalidOperationException($"You need to set the scenario description before the action");
            }

            Scenarios[_descriptionAdded] = action ?? throw new ArgumentNullException(nameof(action));

            FinishSettingScenario();
        }

        /// <inheritdoc />
        public dynamic InvokeScenario(string description)
        {
            return Scenarios.TryGetValue(description, out _) ? Scenarios[description].Invoke() : null;
        }

        /// <summary>
        /// Clearing state after setting scenario
        /// </summary>
        private void FinishSettingScenario()
        {
            _settingScenario = false;
            _descriptionAdded = ValueNotSet;
        }
    }
}
