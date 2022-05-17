using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Test_BieuDo
{
    public partial class frmBD : Form
    {
        string sConnectionString = "Provider=SQLOLEDB;Data Source=192.168.31.245;Initial Catalog=TestBD;User id=lam;Password=thienlam94";

        public frmBD()
        {
            InitializeComponent();
        }
        private void DesktopControl()
        {
            lblX.Top = lblY.Top = ClientSize.Height - lblY.Height - 3;
            chart1.Width = ClientSize.Width - 6;
            chart1.Left = 3;
            chart1.Height = ClientSize.Height - button1.Bottom + 3 - (ClientSize.Height - lblX.Top) - 30;
        }
        private void VeBieuDoKhuDat()
        {
            string sSQL = "SELECT khudat FROM sanluongcausu";
            OleDbConnection odcConnect = new OleDbConnection(sConnectionString);
            OleDbCommand odcCommand = new OleDbCommand(sSQL, odcConnect);

            try
            {
                odcConnect.Open();
                OleDbDataReader odrReader = odcCommand.ExecuteReader();
                try
                {
                    while (odrReader.Read())
                    {
                        BieuDoTheoKhuDat(dateTimePicker1.Value.Month, dateTimePicker1.Value.Year, odrReader["khudat"].ToString());
                    }
                }
                catch (Exception exMsg)
                {
                    if (exMsg != null)
                    {
                        if (odrReader != null) odrReader.Dispose();
                    }
                }
            }
            catch (Exception exMsg)
            {
                if (exMsg != null)
                {
                    if (odcCommand != null) odcCommand.Dispose();
                    if (odcConnect != null) odcConnect.Dispose();
                }
            }
        }
        private void BieuDoTheoKhuDat(int thang, int nam, string khudat)
        {
            #region///////////Create chart
            //chart1.Titles.Add(UserName);
            Series series = chart1.Series.Add(khudat);
            //////Kiểu biểu đồ
            series.ChartType = SeriesChartType.Column;
            //////Độ lớn của đường vẽ
            series.BorderWidth = 2;
            //////Đánh đấu các điểm
            series.MarkerStyle = MarkerStyle.Cross;
            series.MarkerSize = 7;
            #endregion

            OleDbConnection odcConnect = new OleDbConnection(sConnectionString);

            string sSQL = @"declare @month int, @year int
                            IF Object_Id('Tempdb..#TABLE_DAY') IS NOT NULL DROP TABLE #TABLE_DAY
                            CREATE TABLE #TABLE_DAY(dDAY DATE)
                            set @month = " + thang + @"
                            set @year = " + nam + @"
                            /*Select cac ngay trong thang*/
                            INSERT INTO #TABLE_DAY
                            SELECT	CONVERT(VARCHAR, CAST(CAST(@year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-01' AS DATETIME) + Number, 101) DDAY
                            FROM	master..spt_values
                            WHERE	type = 'P' AND (CAST(CAST(@year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-01' AS DATETIME) + Number ) < DATEADD(mm,1,CAST(CAST(@year AS VARCHAR) + '-' + CAST(@Month AS VARCHAR) + '-01' AS DATETIME))

                            SELECT	dDAY, sanluong num
                            FROM	sanluongcausu U RIGHT JOIN #TABLE_DAY D ON CONVERT(DATE, ngaylaysolieu) = dDAY AND khudat = '" + khudat + "' ";

            OleDbCommand odcCommand = new OleDbCommand(sSQL, odcConnect);
            try
            {
                odcConnect.Open();
                OleDbDataReader odrReader = odcCommand.ExecuteReader();
                try
                {
                    int Value = 0;
                    while (odrReader.Read())
                    {
                        int.TryParse(odrReader["NUM"].ToString(), out Value);
                        series.Points.AddXY(odrReader["DDAY"].ToString(), Value);
                    }
                }
                catch (Exception exMsg)
                {
                    if (exMsg != null)
                    {
                        if (odrReader != null) odrReader.Dispose();
                    }
                }
            }
            catch (Exception exMsg)
            {
                if (exMsg != null)
                {
                    if (odcCommand != null) odcCommand.Dispose();
                    if (odcConnect != null) odcConnect.Dispose();
                }
            }
        }
        private void frmBD_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            chart1.Series.Clear();
            chart1.Titles.Clear();
            BieuDoTheoKhuDat(dateTimePicker1.Value.Month, dateTimePicker1.Value.Year, "Khu A");
            BieuDoTheoKhuDat(dateTimePicker1.Value.Month, dateTimePicker1.Value.Year, "Khu B");
            BieuDoTheoKhuDat(dateTimePicker1.Value.Month, dateTimePicker1.Value.Year, "Khu C");
        }

        private void chart1_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                var xv = chart1.ChartAreas[0].AxisX.PixelPositionToValue(e.X);
                var yv = chart1.ChartAreas[0].AxisY.PixelPositionToValue(e.Y);
                lblX.Text = "X: " + Math.Round(xv).ToString();
                lblY.Text = "Y: " + Math.Round(yv).ToString();
            }
            catch (Exception) { }
        }
        private void chart1_Click(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(lblY.Text) >= 0)
                    toolTip.Show(lblY.Text + " actions", chart1);
            }
            catch (Exception) { }
        }

        private void frmBD_Resize(object sender, EventArgs e)
        {
            DesktopControl();
        }

        private void toolTip_Popup(object sender, PopupEventArgs e)
        {

        }

        private void chart2_Click(object sender, EventArgs e)
        {

        }
        private void bieudotron(int thang, int nam)
        {
            Series series = chart2.Series.Add("");
            //////Kiểu biểu đồ
            series.ChartType = SeriesChartType.Pie;

            OleDbConnection odcConnect = new OleDbConnection(sConnectionString);

            string sSQL = @"SELECT SUM(sanluong) NUM, khudat
                            FROM sanluongcausu
                            WHERE MONTH(ngaylaysolieu) = '" + thang.ToString() + "' AND YEAR(ngaylaysolieu) = '" + nam + @"'
                            group by khudat";
            OleDbCommand odcCommand = new OleDbCommand(sSQL, odcConnect);

            try
            {
                odcConnect.Open();
                OleDbDataReader odrReader = odcCommand.ExecuteReader();
                try
                {
                    int Value = 0;
                    int i = 0;
                    while (odrReader.Read())
                    {
                        int.TryParse(odrReader["NUM"].ToString(), out Value);
                        DataPoint dataPoint = new DataPoint(0D, Value);
                        series.Points.Add(dataPoint);
                        series.Points[i].LegendText = (odrReader["khudat"].ToString());
                        series.Points[i].Label = (Math.Round((float)Value / GetTotal(thang,nam) * 100, 2)).ToString() + "%";
                        i++;
                    }
                }
                catch (Exception exMsg)
                {
                    if (exMsg != null)
                    {
                        if (odrReader != null) odrReader.Dispose();
                    }
                }
            }
            catch (Exception exMsg)
            {
                if (exMsg != null)
                {
                    if (odcCommand != null) odcCommand.Dispose();
                    if (odcConnect != null) odcConnect.Dispose();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            chart2.Series.Clear();
            chart2.Titles.Clear();
            bieudotron(dateTimePicker1.Value.Month, dateTimePicker1.Value.Year);
        }

        private int GetTotal(int thang, int nam)
        {
            int Total = 0;
            string sSQL = @"SELECT sum(sanluong) NUM
                            FROM sanluongcausu
                            WHERE MONTH(ngaylaysolieu) = '"+thang.ToString()+"' AND YEAR(ngaylaysolieu) = '"+nam.ToString()+@"' 
                            Group by khudat";
            OleDbConnection odcConnect = new OleDbConnection(sConnectionString);
            OleDbCommand odcCommand = new OleDbCommand(sSQL, odcConnect);

            try
            {
                odcConnect.Open();
                OleDbDataReader odrReader = odcCommand.ExecuteReader();
                try
                {
                    while (odrReader.Read())
                    {
                        int.TryParse(odrReader["NUM"].ToString(), out Total);
                    }
                }
                catch (Exception exMsg)
                {
                    if (exMsg != null)
                    {
                        if (odrReader != null) odrReader.Dispose();
                    }
                }
            }
            catch (Exception exMsg)
            {
                if (exMsg != null)
                {
                    if (odcCommand != null) odcCommand.Dispose();
                    if (odcConnect != null) odcConnect.Dispose();
                }
            }
            return Total;
        }

    }
}
