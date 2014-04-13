/******************************************************************************/
/*                                                                            */
/*   Program: MyHowManySecToTime                                              */
/*   Example for compute the seconds up to a certain time at the same         */
/*   or next day and the use of ValidationRule                                */
/*                                                                            */
/*   18.02.2014 0.0.0.0 uhwgmxorg Start                                       */
/*                                                                            */
/******************************************************************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MyHowManySecToTime
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;

        DispatcherTimer _dispatcherTimer = null;

        /// <summary>
        /// Properties
        /// </summary>
        private bool runFlash;
        public bool RunFlash { get { return runFlash; } set { runFlash = value; OnPropertyChanged("RunFlash"); } }
        
        private int hour;
        public int Hour { get { return hour; } set { hour = value; OnPropertyChanged("Hour"); } }
        private int minute;
        public int Minute { get { return minute; } set { minute = value; OnPropertyChanged("Minute"); } }

        private string timeNow;
        public string TimeNow { get { return timeNow; } set { timeNow = value; OnPropertyChanged("TimeNow"); } }
        private string seconds;
        public string Seconds { get { return seconds; } set { seconds = value; OnPropertyChanged("Seconds"); } }
        private string timeTo;
        public string TimeTo { get { return timeTo; } set { timeTo = value; OnPropertyChanged("TimeTo"); } }

        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            _dispatcherTimer = new DispatcherTimer();
            _dispatcherTimer.Tick += new EventHandler(DispatcherTimer_Tick);
            _dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 1, 0);
            _dispatcherTimer.Start();

            DataContext = this;

            Hour = Properties.Settings.Default.Hour;
            Minute = Properties.Settings.Default.Minute;

            RunFlash = false;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MainWindow()
        {
            Properties.Settings.Default.Hour = Hour;
            Properties.Settings.Default.Minute = Minute;

            Properties.Settings.Default.Save();
        }

        /******************************/
        /*       Button Events        */
        /******************************/
        #region Button Events

        /// <summary>
        /// Button_Calc_Click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Calc_Click(object sender, RoutedEventArgs e)
        {
            TimeTo = CalcDateTimeTo(Hour,Minute).ToString();
        }

        #endregion
        /******************************/
        /*      Menu Events          */
        /******************************/
        #region Menu Events

        #endregion
        /******************************/
        /*      Other Events          */
        /******************************/
        #region Other Events

        /// <summary>
        /// DispatcherTimer_Tick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            TimeNow = DateTime.Now.ToString();
            Seconds = GetRestSecondsToShutdown(Hour, Minute).ToString();
            TimeTo = CalcDateTimeTo(Hour, Minute).ToString();
            if (1 >= int.Parse(Seconds))
                RunFlash = true;
            else
                RunFlash = false;
        }

        #endregion
        /******************************/
        /*      Other Functions       */
        /******************************/
        #region Other Functions

        /// <summary>
        /// GetRestSecondsToShutdown
        /// </summary>
        /// <returns></returns>
        private int GetRestSecondsToShutdown(int hour,int minute)
        {
            DateTime TimeNow = DateTime.Now;
            DateTime TimeTo = CalcDateTimeTo(hour, minute);
            TimeSpan Diff = TimeTo - TimeNow;
            return (int)Math.Round(Diff.TotalSeconds);
        }

        /// <summary>
        /// CalcDateTimeTo
        /// </summary>
        /// <param name="hour"></param>
        /// <param name="minute"></param>
        /// <returns></returns>
        private DateTime CalcDateTimeTo(int hour,int minute)
        {
            DateTime ShutdownTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, 0);
            if (ShutdownTime <= DateTime.Now)
                ShutdownTime = ShutdownTime.AddDays(1);
            return ShutdownTime;
        }

        /// <summary>
        /// OnPropertyChanged
        /// </summary>
        /// <param name="p"></param>
        private void OnPropertyChanged(string p)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(p));
        }

        #endregion

    }

    #region Help Classes
    /**********************/
    /*     RangeRule      */
    /**********************/
    public class RangeRule : ValidationRule
    {
        public int Min { get; set; }
        public int Max { get; set; }

        /// <summary>
        /// ValidationResult
        /// </summary>
        /// <param name="value"></param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public override ValidationResult Validate(object value, CultureInfo cultureInfo) 
        {
            int Value;
            Int32.TryParse((string)value, out Value);
            if (Min <= Value && Value <= Max)
                return new ValidationResult(true,"OK!");
            else
                return new ValidationResult(false, String.Format("Only Values {0} - {1}", Min, Max));
        }
    }

    #endregion

}
