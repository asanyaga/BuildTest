using System;
using GalaSoft.MvvmLight.Command;

namespace Distributr.WPF.Lib.ViewModels.Utils
{
    public class DistributrMessageBoxViewModel : DistributrViewModelBase
    {
        public DistributrMessageBoxViewModel()
        {
            NewButtonClickedCommand = new RelayCommand(NewButtonClicked);
            HomeButtonClickedCommand = new RelayCommand(HomeButtonClicked);
            OKButtonClickedCommand = new RelayCommand(OKButtonClicked);
            CancelButtonClickedCommand = new RelayCommand(CancelButtonClicked);
            Action1ButtonClickedCommand = new RelayCommand(Action1ButtonClicked);
            ExecuteCommand = new RelayCommand(RunExecuteCommand);
        }

        #region Properties
        public RelayCommand NewButtonClickedCommand { get; set; }
        public RelayCommand HomeButtonClickedCommand { get; set; }
        public RelayCommand OKButtonClickedCommand { get; set; }
        public RelayCommand CancelButtonClickedCommand { get; set; }
        public RelayCommand Action1ButtonClickedCommand { get; set; }
        public RelayCommand ExecuteCommand { get; set; }

        public const string NewButtonTextPropertyName = "NewButtonText";
        private string _newButtonText = "New";
        public string NewButtonText
        {
            get
            {
                return _newButtonText;
            }

            set
            {
                if (_newButtonText == value)
                {
                    return;
                }

                var oldValue = _newButtonText;
                _newButtonText = value;
                RaisePropertyChanged(NewButtonTextPropertyName);
            }
        }

        public const string HomeButtonTextPropertyName = "HomeButtonText";
        private string _homeButtonText = "Home";
        public string HomeButtonText
        {
            get
            {
                return _homeButtonText;
            }

            set
            {
                if (_homeButtonText == value)
                {
                    return;
                }

                var oldValue = _homeButtonText;
                _homeButtonText = value;
                RaisePropertyChanged(HomeButtonTextPropertyName);
            }
        }

        public const string OKButtonTextPropertyName = "OKButtonText";
        private string _okButtonText = "OK";
        public string OKButtonText
        {
            get
            {
                return _okButtonText;
            }

            set
            {
                if (_okButtonText == value)
                {
                    return;
                }

                var oldValue = _okButtonText;
                _okButtonText = value;
                RaisePropertyChanged(OKButtonTextPropertyName);
            }
        }

        public const string CancelButtonTextPropertyName = "CancelButtonText";
        private string _cancelButtonText = "Cancel";
        public string CancelButtonText
        {
            get
            {
                return _cancelButtonText;
            }

            set
            {
                if (_cancelButtonText == value)
                {
                    return;
                }

                var oldValue = _cancelButtonText;
                _cancelButtonText = value;
                RaisePropertyChanged(CancelButtonTextPropertyName);
            }
        }

        public const string Action1ButtonTextPropertyName = "Action1ButtonText";
        private string _action1ButtonText = "View List";
        public string Action1ButtonText
        {
            get
            {
                return _action1ButtonText;
            }

            set
            {
                if (_action1ButtonText == value)
                {
                    return;
                }

                var oldValue = _action1ButtonText;
                _action1ButtonText = value;
                RaisePropertyChanged(Action1ButtonTextPropertyName);
            }
        }

        public const string DialogResultPropertyName = "DialogResult";
        private bool _dialogResult = false;
        public bool DialogResult
        {
            get
            {
                return _dialogResult;
            }

            set
            {
                if (_dialogResult == value)
                {
                    return;
                }

                var oldValue = _dialogResult;
                _dialogResult = value;
                RaisePropertyChanged(DialogResultPropertyName);
            }
        }

        public const string MessageBoxTitlePropertyName = "MessageBoxTitle";
        private string _messageBoxTitle = "Distributr";
        public string MessageBoxTitle
        {
            get
            {
                return _messageBoxTitle;
            }

            set
            {
                if (_messageBoxTitle == value)
                {
                    return;
                }

                var oldValue = _messageBoxTitle;
                _messageBoxTitle = value;
                RaisePropertyChanged(MessageBoxTitlePropertyName);
            }
        }

        public const string MessageBoxContentPropertyName = "MessageBoxContent";
        private string _messageBoxContent = "";
        public string MessageBoxContent
        {
            get
            {
                return _messageBoxContent;
            }

            set
            {
                if (_messageBoxContent == value)
                {
                    return;
                }

                var oldValue = _messageBoxContent;
                _messageBoxContent = value;
                RaisePropertyChanged(MessageBoxContentPropertyName);
            }
        }

        public const string MyListUriPropertyName = "MyListUri";
        private string _myListUri = "";
        public string MyListUri
        {
            get
            {
                return _myListUri;
            }

            set
            {
                if (_myListUri == value)
                {
                    return;
                }

                var oldValue = _myListUri;
                _myListUri = value;
                RaisePropertyChanged(MyListUriPropertyName);
            }
        }

        public const string NewUriStringPropertyName = "NewUriString";
        private string _newUriString = "";
        public string NewUriString
        {
            get
            {
                return _newUriString;
            }

            set
            {
                if (_newUriString == value)
                {
                    return;
                }

                var oldValue = _newUriString;
                _newUriString = value;
                RaisePropertyChanged(NewUriStringPropertyName);
            }
        }

        public const string ShowHomeButtonPropertyName = "ShowHomeButton";
        private bool _showHomeButton = true;
        public bool ShowHomeButton
        {
            get
            {
                return _showHomeButton;
            }

            set
            {
                if (_showHomeButton == value)
                {
                    return;
                }

                var oldValue = _showHomeButton;
                _showHomeButton = value;
                RaisePropertyChanged(ShowHomeButtonPropertyName);
            }
        }

        public const string ShowOKButtonPropertyName = "ShowOKButton";
        private bool _showOKButton = true;
        public bool ShowOKButton
        {
            get
            {
                return _showOKButton;
            }

            set
            {
                if (_showOKButton == value)
                {
                    return;
                }

                var oldValue = _showOKButton;
                _showOKButton = value;
                RaisePropertyChanged(ShowOKButtonPropertyName);
            }
        }

        public const string ShowCancelButtonPropertyName = "ShowCancelButton";
        private bool _showCancelButton = true;
        public bool ShowCancelButton
        {
            get
            {
                return _showCancelButton;
            }

            set
            {
                if (_showCancelButton == value)
                {
                    return;
                }

                var oldValue = _showCancelButton;
                _showCancelButton = value;
                RaisePropertyChanged(ShowCancelButtonPropertyName);
            }
        }

        public const string ShowNewButtonPropertyName = "ShowNewButton";
        private bool _showNewButton = true;
        public bool ShowNewButton
        {
            get
            {
                return _showNewButton;
            }

            set
            {
                if (_showNewButton == value)
                {
                    return;
                }

                var oldValue = _showNewButton;
                _showNewButton = value;
                RaisePropertyChanged(ShowNewButtonPropertyName);
            }
        }

        public const string ShowAction1ButtonPropertyName = "ShowAction1Button";
        private bool _showAction1Button = true;
        public bool ShowAction1Button
        {
            get
            {
                return _showAction1Button;
            }

            set
            {
                if (_showAction1Button == value)
                {
                    return;
                }

                var oldValue = _showAction1Button;
                _showAction1Button = value;
                RaisePropertyChanged(ShowAction1ButtonPropertyName);
            }
        }

        public const string HomeButtonTooTipPropertyName = "HomeButtonTooTip";
        private string _homeButtonTooTip = "Navigate to Home page";
        public string HomeButtonTooTip
        {
            get
            {
                return _homeButtonTooTip;
            }

            set
            {
                if (_homeButtonTooTip == value)
                {
                    return;
                }

                var oldValue = _homeButtonTooTip;
                _homeButtonTooTip = value;
                RaisePropertyChanged(HomeButtonTooTipPropertyName);
            }
        }

        public const string OKButtonToolTipPropertyName = "OKButtonToolTip";
        private string _okButtonTooTip = "OK";
        public string OKButtonToolTip
        {
            get
            {
                return _okButtonTooTip;
            }

            set
            {
                if (_okButtonTooTip == value)
                {
                    return;
                }

                var oldValue = _okButtonTooTip;
                _okButtonTooTip = value;
                RaisePropertyChanged(OKButtonToolTipPropertyName);
            }
        }

        public const string CancelButtonTooTipPropertyName = "CancelButtonTooTip";
        private string _cancelButtonTooTip = "Cancel action";
        public string CancelButtonTooTip
        {
            get
            {
                return _cancelButtonTooTip;
            }

            set
            {
                if (_cancelButtonTooTip == value)
                {
                    return;
                }

                var oldValue = _cancelButtonTooTip;
                _cancelButtonTooTip = value;
                RaisePropertyChanged(CancelButtonTooTipPropertyName);
            }
        }

        public const string NewButtonToolTipPropertyName = "NewButtonToolTip";
        private string _newButtonToolTip = "Create new item";
        public string NewButtonToolTip
        {
            get
            {
                return _newButtonToolTip;
            }

            set
            {
                if (_newButtonToolTip == value)
                {
                    return;
                }

                var oldValue = _newButtonToolTip;
                _newButtonToolTip = value;
                RaisePropertyChanged(NewButtonToolTipPropertyName);
            }
        }

        public const string Action1ButtonToolTipPropertyName = "Action1ButtonToolTip";
        private string _action1ButtonToolTip = "Perform this action";
        public string Action1ButtonToolTip
        {
            get
            {
                return _action1ButtonToolTip;
            }

            set
            {
                if (_action1ButtonToolTip == value)
                {
                    return;
                }

                var oldValue = _action1ButtonToolTip;
                _action1ButtonToolTip = value;
                RaisePropertyChanged(Action1ButtonToolTipPropertyName);
            }
        }

        public const string CommandPropertyName = "Command";
        private CommandToExcecute _command = CommandToExcecute.Action1ButtonClickedCommand;
        public CommandToExcecute Command
        {
            get
            {
                return _command;
            }

            set
            {
                if (_command == value)
                {
                    return;
                }

                var oldValue = _command;
                _command = value;
                RaisePropertyChanged(CommandPropertyName);
            }
        }
        #endregion

        #region Methods
        void NewButtonClicked()
        {
            ConfirmNavigatingAway = false;
            SendNavigationRequestMessage(new Uri(NewUriString, UriKind.Relative));
        }

        void HomeButtonClicked()
        {
            ConfirmNavigatingAway = false;
            SendNavigationRequestMessage(new Uri("/Views/HomeViews/Home.xaml", UriKind.Relative));
        }

        void OKButtonClicked()
        {
            
        }

        void CancelButtonClicked()
        {
            
        }

        void Action1ButtonClicked()
        {
            if(Action1ButtonText == "View List")
                SendNavigationRequestMessage(new Uri(MyListUri, UriKind.Relative));
        }

        void RunExecuteCommand()
        {
            switch(Command)
            {
                case CommandToExcecute.Action1ButtonClickedCommand:
                    Action1ButtonClicked();
                    break;
                case CommandToExcecute.CancelButtonClickedCommand:
                    CancelButtonClicked();
                    break;
                case CommandToExcecute.HomeButtonClickedCommand:
                    HomeButtonClicked();
                    break;
                case CommandToExcecute.NewButtonClickedCommand:
                    NewButtonClicked();
                    break;
                case CommandToExcecute.OKButtonClickedCommand:
                    OKButtonClicked();
                    break;
            }
        }

        public void ClearToolTips()
        {
            HomeButtonTooTip = "";
            NewButtonToolTip = "";
            Action1ButtonToolTip = "";
            CancelButtonTooTip = "";
            OKButtonToolTip = "";
        }

        public enum CommandToExcecute
        {
            NewButtonClickedCommand = 1,
            HomeButtonClickedCommand = 2,
            OKButtonClickedCommand = 3,
            CancelButtonClickedCommand = 4,
            Action1ButtonClickedCommand = 5
        }
        #endregion
    }
}