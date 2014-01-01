namespace JSLintNet.UI.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Windows.Controls;
    using System.Windows.Input;

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
            this.IgnoreAddCommand = new RelayCommand<TextBox>(this.OnIgnoreAdd);
            this.IgnoreDeleteCommand = new RelayCommand<int>(this.OnIgnoreDelete);
        }

        public event EventHandler SettingsSaved;

        public JSLintNetSettings Model { get; private set; }

        public ICommand SettingsSavedCommand { get; private set; }

        public ICommand IgnoreAddCommand { get; private set; }

        public ICommand IgnoreDeleteCommand { get; private set; }

        public bool OutputError
        {
            get
            {
                return this.Model.Output == Output.Error;
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

        public bool RunOnSave
        {
            get
            {
                return this.Model.RunOnSave;
            }

            set
            {
                this.Model.RunOnSave = value;

                this.RaisePropertyChanged(() => this.RunOnSave);
            }
        }

        public bool RunOnBuild
        {
            get
            {
                return this.Model.RunOnBuild;
            }

            set
            {
                this.Model.RunOnBuild = value;

                this.RaisePropertyChanged(() => this.RunOnBuild);
            }
        }

        public bool CancelBuild
        {
            get
            {
                return this.Model.CancelBuild;
            }

            set
            {
                this.Model.CancelBuild = value;

                this.RaisePropertyChanged(() => this.CancelBuild);
            }
        }

        public string ErrorLimit
        {
            get
            {
                var nullable = this.Model.ErrorLimit;

                return nullable.HasValue ? nullable.ToString() : null;
            }

            set
            {
                this.Model.ErrorLimit = ParseIntFromString(value);

                this.RaisePropertyChanged(() => this.ErrorLimit);
            }
        }

        public string FileLimit
        {
            get
            {
                var nullable = this.Model.FileLimit;

                return nullable.HasValue ? nullable.ToString() : null;
            }

            set
            {
                this.Model.FileLimit = ParseIntFromString(value);

                this.RaisePropertyChanged(() => this.FileLimit);
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
            var handler = this.SettingsSaved;

            if (handler != null)
            {
                handler(this, new EventArgs());
            }
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
