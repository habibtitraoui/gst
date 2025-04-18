using Microsoft.Reporting.WinForms;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;


namespace gst.Forms
{
    //public partial class One_Day : Form
    //{
    //    DB_CONNECT dbCon = new DB_CONNECT();
    //    public One_Day()
    //    {
    //        InitializeComponent();
    //    }

    //    private void button10_Click(object sender, EventArgs e)
    //    {
    //        this.Dispose();

    //    }

    //    public void load_report()
    //    {


    //        int i = 0;
    //        DGV_REPORT.Rows.Clear();
    //        string LOAD_STOCK = "SELECT * FROM SELL where cast(DATE_TIME as Date) = cast(getdate() as Date)";
    //        SqlCommand command = new SqlCommand(LOAD_STOCK, dbCon.GetCon());
    //        dbCon.Opencon();
    //        SqlDataReader dr = command.ExecuteReader();
    //        while (dr.Read())
    //        {
    //            i++;
    //            DGV_REPORT.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
    //        }
    //        dr.Close();
    //        dbCon.Closecon();
    //    }

    //    private void One_Day_Load(object sender, EventArgs e)
    //    {
    //        load_report();

    //    }



    //    private void button4_Click(object sender, EventArgs e)
    //    {
    //        load_report();
    //    }


    //    private void DGV_REPORT_CellContentClick(object sender, DataGridViewCellEventArgs e)
    //    {
    //        string colname = DGV_REPORT.Columns[e.ColumnIndex].Name;
    //        if (colname == "delete")
    //        {
    //            if (MessageBox.Show("ARE YOU SURE ?", "DELETE PROD", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
    //            {
    //                try
    //                {
    //                    string DELETE_PROD = "DELETE FROM SELL WHERE ID_SELL='" + DGV_REPORT[0, e.RowIndex].Value.ToString() + "' AND BARCODE_SELL='" + DGV_REPORT[1, e.RowIndex].Value.ToString() + "'AND NAME_SELL='" + DGV_REPORT[2, e.RowIndex].Value.ToString() + "'";
    //                    SqlCommand cmd = new SqlCommand(DELETE_PROD, dbCon.GetCon());
    //                    dbCon.Opencon();
    //                    cmd.ExecuteNonQuery();
    //                    dbCon.Closecon();
    //                    load_report();


    //                }
    //                catch (Exception ex)
    //                {
    //                    MessageBox.Show(ex.Message);
    //                }
    //            }
    //        }
    //    }

    //    private void button1_Click(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SELL WHERE CONVERT(DATE, DATE_TIME) = CONVERT(DATE, GETDATE())", dbCon.GetCon());
    //            DataSet1 ds = new DataSet1();
    //            da.Fill(ds, "SELL");

    //            if (ds.Tables[0].Rows.Count == 0)
    //            {
    //                MessageBox.Show("No data available for today.");
    //                return;
    //            }

    //            ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

    //            this.reportViewer2.LocalReport.DataSources.Clear();
    //            this.reportViewer2.LocalReport.DataSources.Add(datasource);
    //            this.reportViewer2.RefreshReport();
    //            reportViewer2.PrintDialog();
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show("Error: " + ex.Message);
    //        }
    //    }



    //    private void textBox1_TextChanged(object sender, EventArgs e)
    //    {



    //    }

    //    private void reportViewer2_Load(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SELL WHERE CONVERT(DATE, DATE_TIME) = CONVERT(DATE, GETDATE())", dbCon.GetCon());
    //            DataSet1 ds = new DataSet1();
    //            da.Fill(ds, "SELL");



    //            ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

    //            this.reportViewer2.LocalReport.DataSources.Clear();
    //            this.reportViewer2.LocalReport.DataSources.Add(datasource);
    //            this.reportViewer2.RefreshReport();
    //            reportViewer2.PrintDialog();
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show("Error: " + ex.Message);
    //        }
    //    }
    //}
    public partial class One_Day : Form
    {
        private readonly DB_CONNECT dbCon = new DB_CONNECT();

        public One_Day()
        {
            InitializeComponent();
        }

        private void One_Day_Load(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            LoadReport();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            GenerateAndPrintReport();

        }

        private void DGV_REPORT_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (DGV_REPORT.Columns[e.ColumnIndex].Name == "delete")
            {
                if (MessageBox.Show("ARE YOU SURE?", "DELETE PROD", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    DeleteSellEntry(e.RowIndex);
                }
            }
        }

        private void reportViewer2_Load(object sender, EventArgs e)
        {

        }

        // Method to load data into the DataGridView
        private void LoadReport()
        {
            try
            {
                string query = @"
                SELECT ID_SELL, BARCODE_SELL, NAME_SELL, QTY_SELL, PRIX_SELL, SUB_TOT, DATE_TIME 
                FROM SELL 
                WHERE DATE_TIME >= CAST(GETDATE() AS DATE) 
                  AND DATE_TIME < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))";

                using (SqlCommand command = new SqlCommand(query, dbCon.GetCon()))
                {
                    dbCon.Opencon();
                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        DGV_REPORT.Rows.Clear();
                        while (dr.Read())
                        {
                            DGV_REPORT.Rows.Add(
                                dr["ID_SELL"],
                                dr["BARCODE_SELL"],
                                dr["NAME_SELL"],
                                dr["QTY_SELL"],
                                dr["PRIX_SELL"],
                                dr["SUB_TOT"],
                                dr["DATE_TIME"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}");
            }
            finally
            {
                dbCon.Closecon();
            }
        }

        // Method to delete an entry
        private void DeleteSellEntry(int rowIndex)
        {
            try
            {
                string query = @"
                DELETE FROM SELL 
                WHERE ID_SELL = @ID_SELL AND BARCODE_SELL = @BARCODE_SELL AND NAME_SELL = @NAME_SELL";

                using (SqlCommand cmd = new SqlCommand(query, dbCon.GetCon()))
                {
                    cmd.Parameters.AddWithValue("@ID_SELL", DGV_REPORT[0, rowIndex].Value);
                    cmd.Parameters.AddWithValue("@BARCODE_SELL", DGV_REPORT[1, rowIndex].Value);
                    cmd.Parameters.AddWithValue("@NAME_SELL", DGV_REPORT[2, rowIndex].Value);

                    dbCon.Opencon();
                    cmd.ExecuteNonQuery();
                }

                LoadReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting entry: {ex.Message}");
            }
            finally
            {
                dbCon.Closecon();
            }
        }


        private void GenerateAndPrintReport()
        {
            try
            {
                // Query to fetch today's sales data
                string query = @"
            SELECT * FROM SELL 
            WHERE DATE_TIME >= CAST(GETDATE() AS DATE) 
              AND DATE_TIME < DATEADD(DAY, 1, CAST(GETDATE() AS DATE))";

                using (SqlDataAdapter da = new SqlDataAdapter(query, dbCon.GetCon()))
                {
                    // Fill dataset
                    DataSet1 ds = new DataSet1();
                    da.Fill(ds, "SELL");

                    // Check if data exists
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        MessageBox.Show("No data available for today.");
                        return;
                    }

                    // Set data source
                    ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

                    // Configure ReportViewer
                    this.reportViewer2.LocalReport.DataSources.Clear();
                    this.reportViewer2.LocalReport.DataSources.Add(datasource);

                    // Enable local processing mode
                    reportViewer2.ProcessingMode = ProcessingMode.Local;

                    // Refresh the report
                    reportViewer2.RefreshReport();

                    // Delay to ensure the report is fully rendered before invoking the print dialog
                    reportViewer2.RenderingComplete += (sender, args) =>
                    {
                        try
                        {
                            reportViewer2.PrintDialog();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error during printing: {ex.Message}");
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}");
            }
        }

    }

}

