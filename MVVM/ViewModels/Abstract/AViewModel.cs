using System;
using System.Collections.Generic;

using HereticalSolutions.LifetimeManagement;

namespace HereticalSolutions.MVVM
{
    public abstract class AViewModel
        : IViewModel,
          IContainsLifetime,
          ICleanuppable
    {
        protected readonly Dictionary<string, object> Observables =
            new Dictionary<string, object>();
        
        protected readonly Dictionary<string, CommandDelegate> Commands =
            new Dictionary<string, CommandDelegate>();
        
        protected readonly Dictionary<string, CommandWithArgsDelegate> CommandsWithArguments =
            new Dictionary<string, CommandWithArgsDelegate>();
        
        #region IContainsLifetime

        public ILifetimeable Lifetime { get; set; }

        #endregion

        #region ICleanUppable
        
        public virtual void Cleanup()
        {
            UnpublishAllObservables();
            
            UnpublishAllCommands();
        }

        #endregion

        #region IViewModel

        public bool TryGetObservable<T>(
            string key,
            out IObservableProperty<T> observable)
        {
            object output;

            bool result = Observables.TryGetValue(
                key,
                out output);

            observable = result
                ? (IObservableProperty<T>)output
                : null;

            return result;
        }
        
        public CommandDelegate GetCommand(string key)
        {
            CommandDelegate result = null;

            if (!Commands.TryGetValue(key, out result))
            {
                throw new Exception(
                    $"EXPOSED COMMANDS LIST DOES NOT HAVE A KEY \"{key}\"");
            }

            return result;
        }
        
        public CommandWithArgsDelegate GetCommandWithArguments(string key)
        {
            CommandWithArgsDelegate result = null;

            if (!CommandsWithArguments.TryGetValue(key, out result))
            {
                throw new Exception(
                    $"EXPOSED COMMANDS WITH ARGUMENTS LIST DOES NOT HAVE A KEY \"{key}\"");
            }

            return result;
        }

        #endregion
        
        protected void PublishObservable(
            string key,
            object observable)
        {
            if (Observables.ContainsKey(key))
            {
                throw new Exception(
                    $"EXPOSED OBSERVABLES LIST ALREADY HAS A KEY \"{key}\"");
            }

            Observables.Add(key, observable);
        }
        
        protected void TryPollObservable<T>(
            IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
                observableProperty.PollValue();
        }

        protected void TearDownObservable<T>(
            ref IObservableProperty<T> observableProperty)
        {
            if (observableProperty != null)
            {
                observableProperty.Clear();

                observableProperty = null;
            }
        }
        
        protected void UnpublishAllObservables()
        {
            Observables.Clear();
        }
        
        protected void PublishCommand(
            string key,
            CommandDelegate @delegate)
        {
            if (Commands.ContainsKey(key))
            {
                throw new Exception(
                    $"EXPOSED COMMANDS LIST ALREADY HAS A KEY \"{key}\"");
            }

            Commands.Add(
                key,
                @delegate);
        }
        
        protected void PublishCommandWithArguments(
            string key,
            CommandWithArgsDelegate @delegate)
        {
            if (CommandsWithArguments.ContainsKey(key))
            {
                throw new Exception(
                    $"EXPOSED COMMANDS WITH ARGUMENTS LIST ALREADY HAS A KEY \"{key}\"");
            }

            CommandsWithArguments.Add(
                key,
                @delegate);
        }
        
        protected void UnpublishAllCommands()
        {
            Commands.Clear();
            
            CommandsWithArguments.Clear();
        }
    }
}