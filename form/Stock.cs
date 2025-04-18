
using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace gst.Forms
{
    public partial class Stock : Form
    {
        DB_CONNECT dbCon = new DB_CONNECT();
        public Stock()
        {
            InitializeComponent();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
        public void LOAD_STOCK()
        {
            int i = 0;
            DGV_STOCK.Rows.Clear();
            string LOAD_STOCK = "SELECT * FROM ADD_PRODUCT";
            SqlCommand command = new SqlCommand(LOAD_STOCK, dbCon.GetCon());
            dbCon.Opencon();
            SqlDataReader dr = command.ExecuteReader();
            while (dr.Read())
            {
                i++;
                DGV_STOCK.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
            }
            dr.Close();
            dbCon.Closecon();
        }


        private void Stock_Load(object sender, EventArgs e)
        {
            LOAD_STOCK();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Add_Product add_ = new Add_Product();
            add_.ShowDialog();
        }



        private void button7_Click(object sender, EventArgs e)
        {
            LOAD_STOCK();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string ID_PROD = DGV_STOCK.CurrentRow.Cells[1].Value.ToString();

            if (MessageBox.Show("ARE YOU SURE ?", "DELETE PROD", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string DELETE_PROD = "DELETE FROM ADD_PRODUCT WHERE ID_PROD='" + ID_PROD + "'";
                    SqlCommand cmd = new SqlCommand(DELETE_PROD, dbCon.GetCon());
                    dbCon.Opencon();
                    cmd.ExecuteNonQuery();
                    dbCon.Closecon();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            clear();
            LOAD_STOCK();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (DGV_STOCK.CurrentRow == null)
            {
                MessageBox.Show("Please select a product to update.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            int ID_PROD;
            if (!int.TryParse(DGV_STOCK.CurrentRow.Cells[1].Value.ToString(), out ID_PROD))
            {
                MessageBox.Show("Invalid product ID.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            
            float qty, capacite;
            int prixVent, prixKg;

            if (!float.TryParse(QTY.Text, out qty) || !float.TryParse(PRIX_ACHAT.Text, out capacite) ||
                !int.TryParse(PRIX_VENT.Text, out prixVent) || !int.TryParse(PRIX_KG.Text, out prixKg))
            {
                MessageBox.Show("Please enter valid numeric values for Quantity, Capacity, Price, and Price per KG.",
                                "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (MessageBox.Show("ARE YOU SURE ?", "UPDATE PROD", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    string query = "UPDATE ADD_PRODUCT SET BARCODE=@BARCODE, NAME_PROD=@NAME_PROD, QTY_PROD=@QTY_PROD, " +
                                   "Capacite=@Capacite, PRIX_VENT=@PRIX_VENT, PRIX_KG=@PRIX_KG WHERE ID_PROD=@ID_PROD";

                    using (SqlCommand cmd = new SqlCommand(query, dbCon.GetCon()))
                    {
                        cmd.Parameters.AddWithValue("@BARCODE", BRCODE.Text.Trim());
                        cmd.Parameters.AddWithValue("@NAME_PROD", NAME.Text.Trim());
                        cmd.Parameters.AddWithValue("@QTY_PROD", qty);
                        cmd.Parameters.AddWithValue("@Capacite", capacite);
                        cmd.Parameters.AddWithValue("@PRIX_VENT", prixVent);
                        cmd.Parameters.AddWithValue("@PRIX_KG", prixKg);
                        cmd.Parameters.AddWithValue("@ID_PROD", ID_PROD);

                        dbCon.Opencon();
                        cmd.ExecuteNonQuery();
                        MessageBox.Show("PRODUCT updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        dbCon.Closecon();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            LOAD_STOCK();
            clear();
        }


        private void DGV_STOCK_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                BRCODE.Text = DGV_STOCK.Rows[e.RowIndex].Cells[2].Value.ToString();
                NAME.Text = DGV_STOCK.Rows[e.RowIndex].Cells[3].Value.ToString();
                QTY.Text = DGV_STOCK.Rows[e.RowIndex].Cells[4].Value.ToString();
                PRIX_ACHAT.Text = DGV_STOCK.Rows[e.RowIndex].Cells[5].Value.ToString();
                PRIX_VENT.Text = DGV_STOCK.Rows[e.RowIndex].Cells[6].Value.ToString();
                PRIX_KG.Text = DGV_STOCK.Rows[e.RowIndex].Cells[7].Value.ToString();
            }
            catch (Exception ex)
            {
                // Display the exception message in a MessageBox
                MessageBox.Show($"Error in : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string barcode = BRCODE.Text;
            try
            {
                Zen.Barcode.Code128BarcodeDraw barcodeDrawi = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                pictureBox1.Image = barcodeDrawi.Draw(barcode, 40);
            }
            catch (Exception) { }
            PrintBarCode();
        }

        private void PrintBarCode()
        {
            PrintDialog printDialog = new PrintDialog();
            PrintDocument printDocument = new PrintDocument();
            printDocument.PrintPage += Doc_PrintPage;
            printDialog.Document = printDocument;
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                printDocument.Print();
            }
        }
        private void Doc_PrintPage(object sender, PrintPageEventArgs e)
        {
            Bitmap bm = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            pictureBox1.DrawToBitmap(bm, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));
            e.Graphics.DrawImage(bm, 0, 0);
            bm.Dispose();

        }


        private void clear()
        {
            BRCODE.Clear();
            NAME.Clear();
            QTY.Clear();
            PRIX_ACHAT.Clear();
            PRIX_VENT.Clear();
            PRIX_KG.Clear();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LOAD_STOCK();
        }



        private void DGV_STOCK_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            //BRCODE.Text = DGV_STOCK.Rows[e.RowIndex].Cells[2].Value.ToString();
            //NAME.Text = DGV_STOCK.Rows[e.RowIndex].Cells[3].Value.ToString();
            //QTY.Text = DGV_STOCK.Rows[e.RowIndex].Cells[4].Value.ToString();
            //PRIX_ACHAT.Text = DGV_STOCK.Rows[e.RowIndex].Cells[5].Value.ToString();
            //PRIX_VENT.Text = DGV_STOCK.Rows[e.RowIndex].Cells[6].Value.ToString();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Add_Product add_Product = new Add_Product();
            add_Product.ShowDialog();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            LOAD_STOCK();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int i = 0;
                DGV_STOCK.Rows.Clear();

                string SEARCH = "SELECT * FROM ADD_PRODUCT WHERE BARCODE LIKE @Barcode OR NAME_PROD LIKE @ProductName";

                using (SqlCommand command = new SqlCommand(SEARCH, dbCon.GetCon()))
                {
                    command.Parameters.AddWithValue("@Barcode", "%" + textBox1.Text + "%");
                    command.Parameters.AddWithValue("@ProductName", "%" + textBox1.Text + "%");

                    dbCon.Opencon();

                    using (SqlDataReader dr = command.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            DGV_STOCK.Rows.Add(i, dr[0].ToString(), dr[1].ToString(), dr[2].ToString(), dr[3].ToString(), dr[4].ToString(), dr[5].ToString(), dr[6].ToString());
                            i++;
                        }
                    }

                    dbCon.Closecon();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error in : {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
