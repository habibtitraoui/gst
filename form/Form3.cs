using Microsoft.Reporting.WinForms;
using System;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace gst.Forms
{
    public partial class Form3 : Form
    {
        DB_CONNECT dbCon = new DB_CONNECT();
        public Form3()
        {
            InitializeComponent();
        }
        private void reportViewer1_Load_1(object sender, EventArgs e)
        {


        }

        private void Form3_Load_1(object sender, EventArgs e)
        {
            LOAD_BILL();

        }


        public void LOAD_BILL()
        {

            DGV_REPORT.Rows.Clear();
            //string LOAD_STOCK = "SELECT ID_SELL, MAX(DATE_TIME) AS latestdatetime FROM SELL group by  ID_SELL ORDER BY MAX(ID_SELL) DESC";
            string LOAD_STOCK = "SELECT * FROM SELL ORDER BY ID_SELL desc ";
            SqlCommand command = new SqlCommand(LOAD_STOCK, dbCon.GetCon());
            dbCon.Opencon();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {

                DGV_REPORT.Rows.Add(dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
            dr.Close();
            dbCon.Closecon();
        }



        private void DGV_REPORT_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colname = DGV_REPORT.Columns[e.ColumnIndex].Name;
            if (colname == "delete")
            {
                if (MessageBox.Show("ARE YOU SURE ?", "DELETE FACT", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        string DELETE_PROD = "DELETE FROM SELL WHERE ID_SELL='" + DGV_REPORT[0, e.RowIndex].Value.ToString() + "'";
                        SqlCommand cmd = new SqlCommand(DELETE_PROD, dbCon.GetCon());
                        dbCon.Opencon();
                        cmd.ExecuteNonQuery();
                        dbCon.Closecon();
                        LOAD_BILL();
                        SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SELL", dbCon.GetCon());
                        DataSet1 ds = new DataSet1();
                        da.Fill(ds, "SELL");

                        ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

                        this.reportViewer1.LocalReport.DataSources.Clear();
                        this.reportViewer1.LocalReport.DataSources.Add(datasource);
                        this.reportViewer1.RefreshReport();


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            reportViewer1.PrintDialog();
        }

        private void DGV_REPORT_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {

                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM SELL WHERE ID_SELL='" + DGV_REPORT[0, e.RowIndex].Value.ToString() + "'", dbCon.GetCon());
                DataSet1 ds = new DataSet1();
                da.Fill(ds, "SELL");

                ReportDataSource datasource = new ReportDataSource("DataSet1", ds.Tables[0]);

                this.reportViewer1.LocalReport.DataSources.Clear();
                this.reportViewer1.LocalReport.DataSources.Add(datasource);
                this.reportViewer1.RefreshReport();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
