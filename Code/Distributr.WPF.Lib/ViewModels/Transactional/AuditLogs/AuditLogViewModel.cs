using System;
using Distributr.Core.Repository.Master.UserRepositories;
using Distributr.Core.Repository.Transactional.AuditLogRepositories;
using GalaSoft.MvvmLight;
using Distributr.Core.Domain.Master.UserEntities;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Distributr.Core.Domain.Transactional.AuditLogEntity;
using GalaSoft.MvvmLight.Command;
using System.Linq;

namespace Distributr.WPF.Lib.ViewModels.Transactional.AuditLogs
{
    public class AuditLogViewModel : DistributrViewModelBase
    {
        public ObservableCollection<AuditLogItemsViewModel> AuditLogList { get; set; }
        public RelayCommand LoadAuditLog { get; set; }
        public RelayCommand DoLoadCommand { get; set; }
        public RelayCommand SearchCommand { get; set; }

        public AuditLogViewModel()
        {
            LoadAuditLog = new RelayCommand(RunLoadAuditLog);
            DoLoadCommand = new RelayCommand(DoLoad);
            SearchCommand = new RelayCommand(Search);
            UsersList = new ObservableCollection<User>();
            AuditLogList = new ObservableCollection<AuditLogItemsViewModel>();
        }

        public ObservableCollection<User> UsersList { get; set; }
        /// <summary>
        /// The <see cref="StartDate" /> property's name.
        /// </summary>
        public const string StartDatePropertyName = "StartDate";
        private DateTime _StartDate = DateTime.Now.AddDays(-1);
        /// <summary>
        /// Gets the StartDate property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DateTime StartDate
        {
            get
            {
                return _StartDate;
            }

            set
            {
                if (_StartDate == value)
                {
                    return;
                }

                var oldValue = _StartDate;
                _StartDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(StartDatePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="EndDate" /> property's name.
        /// </summary>
        public const string EndDatePropertyName = "EndDate";
        private DateTime _EndDate = DateTime.Now;
        /// <summary>
        /// Gets the EndDate property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public DateTime EndDate
        {
            get
            {
                return _EndDate;
            }

            set
            {
                if (_EndDate == value)
                {
                    return;
                }

                var oldValue = _EndDate;
                _EndDate = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(EndDatePropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SelectedUser" /> property's name.
        /// </summary>
        public const string SelectedUserPropertyName = "SelectedUser";
        private User _SelectedUser = null;
        /// <summary>
        /// Gets the SelectedUser property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public User SelectedUser
        {
            get
            {
                return _SelectedUser;
            }

            set
            {
                if (_SelectedUser == value)
                {
                    return;
                }

                var oldValue = _SelectedUser;
                _SelectedUser = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SelectedUserPropertyName);
            }
        }

        /// <summary>
        /// The <see cref="SearchParameter" /> property's name.
        /// </summary>
        public const string SearchParameterPropertyName = "SearchParameter";
        private string _SearchParameter = null;
        /// <summary>
        /// Gets the SearchParameter property.
        
        /// Changes to that property's value raise the PropertyChanged event. 
        /// This property's value is broadcasted by the Messenger's default instance when it changes.
        /// </summary>
        public string SearchParameter
        {
            get
            {
                return _SearchParameter;
            }

            set
            {
                if (_SearchParameter == value)
                {
                    return;
                }

                var oldValue = _SearchParameter;
                _SearchParameter = value;

                // Update bindings, no broadcast
                RaisePropertyChanged(SearchParameterPropertyName);
            }
        }


      

        private void RunLoadAuditLog()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<AuditLog> auditLog = Using<IAuditLogRepository>(c).GetByDate(StartDate, EndDate);
                if (SelectedUser.Id != Guid.Empty)
                    auditLog =
                        auditLog.Where(
                            n =>
                            n.ActionUser.Id == SelectedUser.Id &&
                            (n.ActionTimeStamp >= StartDate && n.ActionTimeStamp <= EndDate)).ToList();
                else
                    auditLog =
                        auditLog.Where(n => n.ActionTimeStamp >= StartDate && n.ActionTimeStamp <= EndDate).ToList();
                AuditLogList.Clear();
                auditLog.ForEach(n => AuditLogList.Add(new AuditLogItemsViewModel
                    {
                        Action = n.Action,
                        ActionTimeStamp = n.ActionTimeStamp.ToString("dd-MMM-yyyy hh:mm:ss ttt"),
                        ActionUser = n.ActionUser.Username,
                        Module = n.Module
                    }));
            }
        }

        void DoLoad()
        {
            //bw.RunWorkerAsync();
            LoadUsers();
           // RunLoadAuditLog();
        }

        void Search()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                List<AuditLog> auditLog = Using<IAuditLogRepository>(c).GetAll();
                if (SelectedUser.Id != Guid.Empty)
                    auditLog =
                        auditLog.Where(
                            n =>
                            n.ActionUser.Id == SelectedUser.Id &&
                            (n.ActionTimeStamp >= StartDate && n.ActionTimeStamp <= EndDate)).ToList();
                else
                    auditLog =
                        auditLog.Where(n => n.ActionTimeStamp >= StartDate && n.ActionTimeStamp <= EndDate).ToList();
                auditLog = auditLog.Where(n => n.Module.ToLower().Contains(SearchParameter.ToLower())).ToList();
                AuditLogList.Clear();
                auditLog.ForEach(n => AuditLogList.Add(new AuditLogItemsViewModel
                    {
                        Action = n.Action,
                        ActionTimeStamp = n.ActionTimeStamp.ToString("dd-MMM-yyyy hh:mm:ss ttt"),
                        ActionUser = n.ActionUser.Username,
                        Module = n.Module
                    }));
            }
        }

        void LoadUsers()
        {
            using (StructureMap.IContainer c = NestedContainer)
            {
                UsersList.Clear();
                var user = new User(Guid.Empty) {Username = "--Please Select a Salesman--"};
                UsersList.Add(user);
                SelectedUser = user;
                Using<IUserRepository>(c).GetAll()
                            .OrderBy(n => n.Username).ToList()
                            .ForEach(n => UsersList.Add(n));
            }
        }


        public class AuditLogItemsViewModel : ViewModelBase
        {
            /// <summary>
            /// The <see cref="ActionUser" /> property's name.
            /// </summary>
            public const string ActionUserPropertyName = "ActionUser";
            private string _ActionUser = null;
            /// <summary>
            /// Gets the ActionUser property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string ActionUser
            {
                get
                {
                    return _ActionUser;
                }

                set
                {
                    if (_ActionUser == value)
                    {
                        return;
                    }

                    var oldValue = _ActionUser;
                    _ActionUser = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ActionUserPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Module" /> property's name.
            /// </summary>
            public const string ModulePropertyName = "Module";
            private string _Module = null;
            /// <summary>
            /// Gets the Module property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string Module
            {
                get
                {
                    return _Module;
                }

                set
                {
                    if (_Module == value)
                    {
                        return;
                    }

                    var oldValue = _Module;
                    _Module = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ModulePropertyName);
                }
            }

            /// <summary>
            /// The <see cref="Action" /> property's name.
            /// </summary>
            public const string ActionPropertyName = "Action";
            private string _Action = null;
            /// <summary>
            /// Gets the Action property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string Action
            {
                get
                {
                    return _Action;
                }

                set
                {
                    if (_Action == value)
                    {
                        return;
                    }

                    var oldValue = _Action;
                    _Action = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ActionPropertyName);
                }
            }

            /// <summary>
            /// The <see cref="ActionTimeStamp" /> property's name.
            /// </summary>
            public const string ActionTimeStampPropertyName = "ActionTimeStamp";
            private string _ActionTimeStamp = null;
            /// <summary>
            /// Gets the ActionTimeStamp property.
            
            /// Changes to that property's value raise the PropertyChanged event. 
            /// This property's value is broadcasted by the Messenger's default instance when it changes.
            /// </summary>
            public string ActionTimeStamp
            {
                get
                {
                    return _ActionTimeStamp;
                }

                set
                {
                    if (_ActionTimeStamp == value)
                    {
                        return;
                    }

                    var oldValue = _ActionTimeStamp;
                    _ActionTimeStamp = value;

                    // Update bindings, no broadcast
                    RaisePropertyChanged(ActionTimeStampPropertyName);
                }
            }

        }
    }
}
