namespace JSLintNet.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using System.Windows.Input;
    using JSLintNet.Settings;

    internal partial class SettingsViewModel : ViewModelBase
    {
        public SettingsViewModel(JSLintNetSettings model)
        {
            this.Model = model;

            if (this.Model.Options == null)
            {
                this.Model.Options = new JSLintOptions();
            }

            this.SettingsSavedCommand = new RelayCommand(this.OnSettingsSaved);
            this.SettingsCanceledCommand = new RelayCommand(this.OnSettingsCanceled);
            this.IgnoreAddCommand = new RelayCommand<TextBox>(this.OnIgnoreAdd);
            this.IgnoreDeleteCommand = new RelayCommand<int>(this.OnIgnoreDelete);
        }

        public event EventHandler SettingsSaved;

        public event EventHandler SettingsCanceled;

        public JSLintNetSettings Model { get; private set; }

        public ICommand SettingsSavedCommand { get; private set; }

        public ICommand SettingsCanceledCommand { get; private set; }

        public ICommand IgnoreAddCommand { get; private set; }

        public ICommand IgnoreDeleteCommand { get; private set; }

        public bool OutputError
        {
            get
            {
                return !this.Model.Output.HasValue || this.Model.Output == Output.Error;
            }

            set
            {
                if (value)
                {
                    this.Model.Output = Output.Error;
                }

                this.RaisePropertyChanged(() => this.OutputError);
            }
        }

        public bool OutputWarning
        {
            get
            {
                return this.Model.Output == Output.Warning;
            }

            set
            {
                if (value)
                {
                    this.Model.Output = Output.Warning;
                }

                this.RaisePropertyChanged(() => this.OutputWarning);
            }
        }

        public bool OutputMessage
        {
            get
            {
                return this.Model.Output == Output.Message;
            }

            set
            {
                if (value)
                {
                    this.Model.Output = Output.Message;
                }

                this.RaisePropertyChanged(() => this.OutputMessage);
            }
        }

        public IList<StringItem> Ignore
        {
            get
            {
                var i = 0;

                return this.Model.Ignore
                    .Select(x => new StringItem(i++, x))
                    .ToList();
            }
        }

        public string PredefinedGlobals
        {
            get
            {
                return string.Join(" ", this.Model.Options.PredefinedGlobals.Keys);
            }

            set
            {
                PopulateDictionaryFromString(this.Model.Options.PredefinedGlobals, value);

                this.RaisePropertyChanged(() => this.PredefinedGlobals);
            }
        }

        private static void PopulateDictionaryFromString(Dictionary<string, bool> dictionary, string value)
        {
            dictionary.Clear();

            if (!string.IsNullOrEmpty(value))
            {
                var keys = Regex.Split(value, @"[\s,;'""]+");

                foreach (var key in keys)
                {
                    if (!string.IsNullOrWhiteSpace(key))
                    {
                        dictionary.Add(key, false);
                    }
                }
            }
        }

        private static int? ParseIntFromString(string value)
        {
            int intValue;

            if (int.TryParse(value, out intValue) && intValue > 0)
            {
                return intValue;
            }

            return null;
        }

        private void OnSettingsSaved(object param)
        {
            this.HandleAndClose(this.SettingsSaved, true);
        }

        private void OnSettingsCanceled(object param)
        {
            this.HandleAndClose(this.SettingsCanceled, false);
        }

        private void OnIgnoreAdd(TextBox param)
        {
            this.Model.Ignore.Add(param.Text);
            param.Text = null;
            param.Focus();

            this.RaisePropertyChanged(() => this.Ignore);
        }

        private void OnIgnoreDelete(int param)
        {
            this.Model.Ignore.RemoveAt(param);

            this.RaisePropertyChanged(() => this.Ignore);
        }

        public class StringItem
        {
            public StringItem(int index, string value)
            {
                this.Index = index;
                this.Value = value;
            }

            public int Index { get; set; }

            public string Value { get; set; }
        }
    }
}
