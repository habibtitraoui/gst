//using DGVPrinterHelper;
//using System;
//using System.Data.SqlClient;
//using System.Drawing;
//using System.Windows.Forms;

//namespace gst.Forms
//{
//    public partial class Report : Form
//    {
//        DB_CONNECT dbCon = new DB_CONNECT();
//        public Report()
//        {
//            InitializeComponent();
//        }

//        private void button10_Click(object sender, EventArgs e)
//        {
//            this.Dispose();
//        }
//        public void load_report()
//        {
//            int i = 0;
//            DGV_REPORT.Rows.Clear();
//            DateTime dt1 = DateTime.Parse(dateTimePicker1.Text);
//            DateTime dt2 = DateTime.Parse(dateTimePicker2.Text);
//            string LOAD_STOCK = "SELECT * FROM SELL WHERE DATE_TIME BETWEEN'" + dt1.ToString("MM/dd/yyyy") + "'AND'" + dt2.ToString("MM/dd/yyyy") + "' ORDER BY (ID_SELL) ASC";
//            SqlCommand command = new SqlCommand(LOAD_STOCK, dbCon.GetCon());
//            dbCon.Opencon();
//            SqlDataReader dr = command.ExecuteReader();
//            while (dr.Read())
//            {
//                i++;
//                DGV_REPORT.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), DateTime.Parse(dr[6].ToString()).ToShortDateString());
//            }
//            dr.Close();
//            dbCon.Closecon();
//        }



//        private void Report_Load(object sender, EventArgs e)
//        {
//            int i = 0;
//            DGV_REPORT.Rows.Clear();
//            string LOAD_STOCK = "SELECT * FROM SELL ORDER BY (ID_SELL) ASC";
//            SqlCommand sql = new SqlCommand(LOAD_STOCK, dbCon.GetCon());
//            dbCon.Opencon();
//            SqlDataReader dr = sql.ExecuteReader();
//            while (dr.Read())
//            {
//                i++;
//                DGV_REPORT.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), DateTime.Parse(dr[6].ToString()).ToShortDateString());
//            }
//            dr.Close();
//            dbCon.Closecon();

//        }

//        private void button4_Click(object sender, EventArgs e)
//        {
//            load_report();
//        }

//        private void DGV_REPORT_CellContentClick(object sender, DataGridViewCellEventArgs e)
//        {
//            string colname = DGV_REPORT.Columns[e.ColumnIndex].Name;
//            if (colname == "delete")
//            {
//                if (MessageBox.Show("ARE YOU SURE ?", "DELETE PROD", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
//                {
//                    try
//                    {
//                        string DELETE_PROD = "DELETE FROM SELL WHERE ID_SELL='" + DGV_REPORT[0, e.RowIndex].Value.ToString() + "' AND BARCODE_SELL='" + DGV_REPORT[1, e.RowIndex].Value.ToString() + "'AND NAME_SELL='" + DGV_REPORT[2, e.RowIndex].Value.ToString() + "'";
//                        SqlCommand cmd = new SqlCommand(DELETE_PROD, dbCon.GetCon());
//                        dbCon.Opencon();
//                        cmd.ExecuteNonQuery();
//                        dbCon.Closecon();
//                        int i = 0;
//                        DGV_REPORT.Rows.Clear();
//                        string LOAD_STOCK = "SELECT * FROM SELL";
//                        SqlCommand sql = new SqlCommand(LOAD_STOCK, dbCon.GetCon());
//                        dbCon.Opencon();
//                        SqlDataReader dr = sql.ExecuteReader();
//                        while (dr.Read())
//                        {
//                            i++;
//                            DGV_REPORT.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), DateTime.Parse(dr[6].ToString()).ToShortDateString());
//                        }
//                        dr.Close();
//                        dbCon.Closecon();



//                    }
//                    catch (Exception ex)
//                    {
//                        MessageBox.Show(ex.Message);
//                    }
//                }
//            }
//        }

//        private void button1_Click(object sender, EventArgs e)
//        {
//            DGVPrinter printer = new DGVPrinter();
//            printer.Title = " إجمالي المبيعات "; //give your report name
//            printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
//            printer.PageNumbers = true; // if you need page numbers you can keep this as true other wise false
//            printer.PageNumberInHeader = false;
//            printer.PorportionalColumns = true;
//            printer.HeaderCellAlignment = StringAlignment.Near;
//            printer.Footer = "footer"; //this is the footer
//            printer.FooterSpacing = 15;
//            printer.printDocument.DefaultPageSettings.Landscape = true;
//            printer.PrintDataGridView(DGV_REPORT);
//        }


//    }
//}
using Microsoft.Reporting.WinForms;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace gst.Forms
{
    public partial class Report : Form
    {
        DB_CONNECT dbCon = new DB_CONNECT();

        public Report()
        {
            InitializeComponent();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        public void load_report()
        {
            int i = 0;
            DGV_REPORT.Rows.Clear();

            DateTime dt1, dt2;
            // Try to parse the date range inputs
            if (DateTime.TryParse(dateTimePicker1.Text, out dt1) && DateTime.TryParse(dateTimePicker2.Text, out dt2))
            {
                // SQL query to load data based on the selected date range
                string LOAD_STOCK = "SELECT * FROM SELL WHERE DATE_TIME BETWEEN @dt1 AND @dt2 ORDER BY ID_SELL ASC";
                SqlCommand command = new SqlCommand(LOAD_STOCK, dbCon.GetCon());
                command.Parameters.AddWithValue("@dt1", dt1);
                command.Parameters.AddWithValue("@dt2", dt2);

                try
                {
                    dbCon.Opencon();
                    SqlDataReader dr = command.ExecuteReader();
                    while (dr.Read())
                    {
                        i++;
                        DGV_REPORT.Rows.Add(
                            dr[0].ToString(),
                            dr[1].ToString(),
                            dr[2].ToString(),
                            dr[3].ToString(),
                            dr[4].ToString(),
                            dr[5].ToString(),
                            DateTime.TryParse(dr[6].ToString(), out DateTime parsedDate) ? parsedDate.ToShortDateString() : "Invalid Date"
                        );
                    }
                    dr.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading report: " + ex.Message);
                }
                finally
                {
                    dbCon.Closecon();
                }
            }
            else
            {
                MessageBox.Show("Invalid date format entered.");
            }
        }

        private void Report_Load(object sender, EventArgs e)
        {
            int i = 0;
            DGV_REPORT.Rows.Clear();

            string LOAD_STOCK = "SELECT * FROM SELL ORDER BY ID_SELL ASC";
            SqlCommand sql = new SqlCommand(LOAD_STOCK, dbCon.GetCon());

            try
            {
                dbCon.Opencon();
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    DGV_REPORT.Rows.Add(
                        dr[0].ToString(),
                        dr[1].ToString(),
                        dr[2].ToString(),
                        dr[3].ToString(),
                        dr[4].ToString(),
                        dr[5].ToString(),
                        DateTime.TryParse(dr[6].ToString(), out DateTime parsedDate) ? parsedDate.ToShortDateString() : "Invalid Date"
                    );
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading report: " + ex.Message);
            }
            finally
            {
                dbCon.Closecon();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            load_report();
        }

        private void DGV_REPORT_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colname = DGV_REPORT.Columns[e.ColumnIndex].Name;
            if (colname == "delete")
            {
                if (MessageBox.Show("ARE YOU SURE ?", "DELETE PROD", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string DELETE_PROD = "DELETE FROM SELL WHERE ID_SELL = @id AND BARCODE_SELL = @barcode AND NAME_SELL = @name";
                        SqlCommand cmd = new SqlCommand(DELETE_PROD, dbCon.GetCon());
                        cmd.Parameters.AddWithValue("@id", DGV_REPORT[0, e.RowIndex].Value.ToString());
                        cmd.Parameters.AddWithValue("@barcode", DGV_REPORT[1, e.RowIndex].Value.ToString());
                        cmd.Parameters.AddWithValue("@name", DGV_REPORT[2, e.RowIndex].Value.ToString());

                        dbCon.Opencon();
                        cmd.ExecuteNonQuery();
                        dbCon.Closecon();

                        // Refresh the DataGridView after deletion
                        RefreshDataGrid();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting record: " + ex.Message);
                    }
                }
            }
        }

        private void RefreshDataGrid()
        {
            int i = 0;
            DGV_REPORT.Rows.Clear();
            string LOAD_STOCK = "SELECT * FROM SELL ORDER BY ID_SELL ASC";
            SqlCommand sql = new SqlCommand(LOAD_STOCK, dbCon.GetCon());

            try
            {
                dbCon.Opencon();
                SqlDataReader dr = sql.ExecuteReader();
                while (dr.Read())
                {
                    i++;
                    DGV_REPORT.Rows.Add(
                        dr[0].ToString(),
                        dr[1].ToString(),
                        dr[2].ToString(),
                        dr[3].ToString(),
                        dr[4].ToString(),
                        dr[5].ToString(),
                        DateTime.TryParse(dr[6].ToString(), out DateTime parsedDate) ? parsedDate.ToShortDateString() : "Invalid Date"
                    );
                }
                dr.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error refreshing DataGrid: " + ex.Message);
            }
            finally
            {
                dbCon.Closecon();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SELL WHERE DATE_TIME BETWEEN @dt1 AND @dt2 ORDER BY ID_SELL ASC", dbCon.GetCon());
                DateTime dt1 = dateTimePicker1.Value;
                DateTime dt2 = dateTimePicker2.Value;

                // Add query parameters
                da.SelectCommand.Parameters.AddWithValue("@dt1", dt1);
                da.SelectCommand.Parameters.AddWithValue("@dt2", dt2);

                DataSet1 ds = new DataSet1();
                da.Fill(ds, "SELL");

                ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(datasource);
                this.reportViewer1.ProcessingMode = ProcessingMode.Local;
                this.reportViewer1.RefreshReport();
                this.reportViewer1.RenderingComplete += (s, args) =>
                {
                    try
                    {
                        reportViewer1.PrintDialog();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error during printing: {ex.Message}");
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
