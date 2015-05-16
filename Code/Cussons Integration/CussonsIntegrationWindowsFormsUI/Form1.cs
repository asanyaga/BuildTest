using System;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using PzIntegrations.Lib;
using StructureMap;
using Timer = System.Timers.Timer;

namespace CussonsIntegrationWindowsFormsUI
{
    public partial class Form1 : Form
    {
        string[] _masterDataSource;
        private string selectedSchedule = null;
        private string SelectedMasterData = null;

        private bool _importAllChecked;
        private bool masterdataWorkerIsStarted = false;
        private IPzIntegrationService _integrationService;
        private string LastSuccessMasterDataExecutionHour = "";
        public Form1()
        {
            InitializeComponent();
            _masterDataSource = new[] { "Salesmen","Brands" ,"Products", "Customer", "Shipping Addresses","Inventory"}; 
           this.FormBorderStyle = FormBorderStyle.FixedSingle;
            _integrationService = ObjectFactory.Container.GetNestedContainer().GetInstance<IPzIntegrationService>();

        
        }

       

        #region UI methods

        
        private void Form1_Load(object sender, EventArgs e)
        {
            LoadMasterData();
            LoadTimeOptions();
            if(_integrationService !=null)
            {
              _integrationService.Start();

                AutoMasterTrackerJob();
            }
            
        }
        private void LoadMasterData()
        {
            masterDatalistBox.BeginUpdate();
            masterDatalistBox.Items.Clear();
            masterDatalistBox.Items.AddRange(_masterDataSource);
            masterDatalistBox.EndUpdate();
        }
        private void LoadTimeOptions()
        {
            comboBoxTimeOptions.BeginUpdate();
            comboBoxTimeOptions.Items.Clear();
            var hours = new string[24];
            int counter = 0;
            for (int i = 0; i < 24; i++)
            {
                hours[i] = GetNextHour(counter);
                counter++;

            }
            var selectedSchedule = FileUtility.GetSchedule();
            if(!string.IsNullOrEmpty(selectedSchedule))
            {
                labelScheduleDisplayer.Text = string.Format("Current Schedule :{0}", selectedSchedule);
                comboBoxTimeOptions.Items.AddRange(hours);
                comboBoxTimeOptions.SelectedIndex = 18;
                comboBoxTimeOptions.EndUpdate();
            }
            else
            {
                comboBoxTimeOptions.Items.AddRange(hours);
                comboBoxTimeOptions.SelectedIndex = 18;
                comboBoxTimeOptions.EndUpdate();

                UpdateSelectedSchedule();
            }
         

        }
        
        void UpdateSelectedSchedule()
        {
            var selectedIndex = comboBoxTimeOptions.SelectedIndex;
            if (selectedIndex >= 0)
                selectedSchedule = (string)comboBoxTimeOptions.Items[selectedIndex];

            if(selectedSchedule !=null)
                labelScheduleDisplayer.Text = string.Format("Current Schedule :{0}", selectedSchedule);

            FileUtility.UpdateScheduleSetting(selectedSchedule);
        }

        private string GetNextHour(int currentHour)
        {
           return string.Format("{0}:00 Hrs", currentHour.ToString("00"));

        }
        private void buttonImport_Click(object sender, EventArgs e)
        {
            this.UIThread(() =>
            {
                buttonImport.Enabled = false;
                this.Cursor = Cursors.WaitCursor;
               
            });
           
            UpdateLog("Task Started");

            try
            {
                if (_importAllChecked)
                {
                    UpdateLog("All master data selected");
                    var items = masterDatalistBox.Items.Cast<string>().ToArray();
                    _integrationService.ImportMasterData(items, true);
                }
                else
                {

                    _integrationService.ImportMasterData(new[] { SelectedMasterData });

                }
               
                UpdateLog("Process in progress..Hit View logs for details");
            
            }
            catch (Exception ex)
            {
                UpdateLog("Import completed...with errors" + ex.Message);

            }
            finally
            {
                this.UIThread(() =>
                                  {
                                      buttonImport.Enabled = true;
                                      this.Cursor = Cursors.Default;
                                  });

            }
           
        }
       
        private void SelectedHourChanged(object sender, EventArgs e)
        {
            UpdateSelectedSchedule();
            
        }

        private void SelectedMasterDataChanged(object sender, EventArgs e)
        {
            var selectedIndex = masterDatalistBox.SelectedIndex;
            if(selectedIndex>=0)
            {
                SelectedMasterData = (string) masterDatalistBox.Items[selectedIndex];
            }
        }

       
        private void buttonClearlogs_Click(object sender, EventArgs e)
        {
            if (LogsListBox.Items.Count > 0)
            {
                ClearListBox();
                FileUtility.ClearLogs();
            }
            
        }

        private void worker_CompletedWork(object sender, RunWorkerCompletedEventArgs e)
        {
            this.UIThread(() =>
                              {
                                  masterdataWorkerIsStarted = false;
                                  this.buttonImport.Enabled = true;
                                  LastSuccessMasterDataExecutionHour = DateTime.Now.Hour.ToString("00");
                                  buttonViewLogs_Click(null, null);
                              });
            
           
        }
        private void ClearListBox()
        {
            LogsListBox.BeginUpdate();
            LogsListBox.Items.Clear();
            LogsListBox.EndUpdate();
        }

        private void buttonViewLogs_Click(object sender, EventArgs e)
        {
            var path = FileUtility.GetLogFile();
            if (!string.IsNullOrEmpty(path))
            {ClearListBox();
                
                string[] lines = File.ReadAllLines(path);
                foreach (var line in lines)
                {
                    UpdateLog(line);
                }
                if(lines.Length==0)
                    UpdateLog("No logs to display");

            }

        }

        private void UpdateLog(string log)
        {
            this.UIThread(()=>
                              {
                                  LogsListBox.BeginUpdate();
                                  LogsListBox.Items.Add(log);
                                  LogsListBox.EndUpdate();
                              });
            
          
        }
      

        private void SelectAllClicked(object sender, EventArgs e)
        {
            
            _importAllChecked = SelectAllcheckBox.Checked;

        }
        #endregion

        void AutoMasterTrackerJob()
        {
            double interval =TimeSpan.FromHours(1).TotalMilliseconds; // every 1 hour
           
            var checkForTime = new Timer(interval);
            checkForTime.Elapsed += checkForTime_Elapsed;
            checkForTime.Enabled = true;
            checkForTime.AutoReset = true;
        }


        private  void checkForTime_Elapsed(object sender, ElapsedEventArgs e)
        {
            if(masterdataWorkerIsStarted)return;
            var item = FileUtility.GetSchedule();
            if(string.IsNullOrEmpty(item))
            {
                string error =string.Format("masterdate Schedule hour  is not set..{0}",DateTime.Now.ToShortTimeString());
                FileUtility.LogCommandActivity(error);
                UpdateLog(error);
                return;
            }
            var spliter = new string[] {":"};
           var scheduleHour = item.Split(spliter, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();

            DateTime localTime = DateTime.Now;
           string timeString24Hour = localTime.ToString("HH:mm", CultureInfo.CurrentCulture);
            var currentHour = timeString24Hour.Split(spliter, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            if (LastSuccessMasterDataExecutionHour==currentHour)
            {
                UpdateLog(string.Format("Master data upload handed within {0} Hrs already",currentHour));
                return;
                
            }
            if(!string.IsNullOrEmpty(scheduleHour)&& !string.IsNullOrEmpty(currentHour))
            {
                if (scheduleHour.ToLower()==currentHour.ToLower())
                {
                    UpdateLog("Master data upload task started");
                    LastSuccessMasterDataExecutionHour = currentHour;
                    backgroundWorker1.RunWorkerAsync();
                }
                string healthCheck = string.Format("Masterdata auto sync  at {0} health check =>OK !",
                                                   DateTime.Now.ToShortTimeString());
                FileUtility.LogCommandActivity(healthCheck);
                UpdateLog(healthCheck);
            }
        }

       

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // show animated image
            this.UIThread(() =>
            {
                this.progressPictureBox.ImageLocation = (@"Resources/Animation.gif");
                // change button states
                this.buttonImport.Enabled = false;
                masterdataWorkerIsStarted = true;
            });

            // start background operation
            _integrationService.ImportMasterData(masterDatalistBox.Items.Cast<string>().ToArray(), true);
            Thread.Sleep(60000); //sleep for one minute
            var errors = _integrationService.GetImportErrors();
            if(errors.Any())
            {
                foreach (var error in errors)
                {
                    FileUtility.LogCommandActivity(error);
                }
            }


        }

        private void buttonDwnload_Click(object sender, EventArgs e)
        {
            var orderRef = searchOrderTextBox.Text.Trim();
            if(string.IsNullOrEmpty(orderRef))
            {
                MessageBox.Show("No order reference found,task aborted");
                return;
            }
            try
            {
                _integrationService.FindAndExportOrder(orderRef);
            }catch(Exception ex)
            {
                MessageBox.Show("Error occured details\n" + ex.Message);
                FileUtility.LogCommandActivity(ex.Message);
                
            }
            finally
            {
                searchOrderTextBox.Text = string.Empty;
            }

        }

        
       
       
    }
}
