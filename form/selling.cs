using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace gst.Forms
{
    public partial class selling : Form
    {
        DB_CONNECT dbCon = new DB_CONNECT();
        public selling()
        {
            InitializeComponent();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            double totalPrice = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(text_code.Text))
                    return;

                dbCon.Opencon();

                string query;
                if (radioButton2.Checked)
                {
                    query = "SELECT ID_PROD, NAME_PROD, PRIX_VENT, QTY_PROD FROM ADD_PRODUCT WHERE BARCODE = @barcode";
                }
                else if (radioButton1.Checked)
                {
                    query = "SELECT ID_PROD, NAME_PROD, PRIX_KG, QTY_PROD FROM ADD_PRODUCT WHERE BARCODE = @barcode";
                }
                else
                {
                    return; // Exit if no radio button is selected
                }

                using (SqlCommand cmd = new SqlCommand(query, dbCon.GetCon()))
                {
                    cmd.Parameters.AddWithValue("@barcode", text_code.Text);

                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                    {
                        DataTable dt = new DataTable();
                        dataAdapter.Fill(dt);

                        foreach (DataRow row in dt.Rows)
                        {
                            string barcode = row["ID_PROD"].ToString();
                            string productName = row["NAME_PROD"].ToString();
                            string priceColumn = radioButton2.Checked ? "PRIX_VENT" : "PRIX_KG";

                            if (!double.TryParse(row[priceColumn].ToString(), out double productPrice))
                                continue; // Skip invalid price

                            // Add a new row directly
                            dataGridView1.Rows.Add(barcode, productName, 1, productPrice, productPrice);
                        }
                    }
                }

                // Calculate total price and update DataGridView
                foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
                {
                    if (dgvRow.Cells[2].Value != null && dgvRow.Cells[3].Value != null)
                    {
                        if (double.TryParse(dgvRow.Cells[2].Value.ToString(), out double quantity) &&
                            double.TryParse(dgvRow.Cells[3].Value.ToString(), out double productPrice))
                        {
                            double subTotal = quantity * productPrice;
                            dgvRow.Cells[4].Value = subTotal.ToString("F2");
                            totalPrice += subTotal;
                        }
                    }
                }

                text_totale.Text = totalPrice.ToString("F2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbCon.Closecon(); // Ensure the connection is closed
            }

            // Clear the text box and refocus
            text_code.Clear();
            text_code.Focus();
        }



        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < dataGridView1.Rows.Count - 1; i++)
                {
                    int id;
                    if (!int.TryParse(textBox3.Text, out id))
                    {
                        MessageBox.Show("Invalid ID format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string productId = dataGridView1.Rows[i].Cells[0].Value?.ToString() ?? string.Empty;
                    string productName = dataGridView1.Rows[i].Cells[1].Value?.ToString() ?? string.Empty;

                    if (!float.TryParse(dataGridView1.Rows[i].Cells[2].Value?.ToString(), out float quantity))
                    {
                        MessageBox.Show("Invalid quantity format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(dataGridView1.Rows[i].Cells[3].Value?.ToString(), out decimal unitPrice))
                    {
                        MessageBox.Show("Invalid unit price format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    if (!decimal.TryParse(dataGridView1.Rows[i].Cells[4].Value?.ToString(), out decimal totalPrice))
                    {
                        MessageBox.Show("Invalid total price format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }

                    string sellQuery = "INSERT INTO SELL (ID_SELL,BARCODE_SELL , NAME_SELL, QTY_SELL, PRIX_SELL,SUB_TOT, DATE_TIME) " +
                                       "VALUES (@id, @productId, @productName, @quantity, @unitPrice, @totalPrice, CURRENT_TIMESTAMP)";

                    using (SqlCommand cmd = new SqlCommand(sellQuery, dbCon.GetCon()))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        cmd.Parameters.AddWithValue("@productId", productId);
                        cmd.Parameters.AddWithValue("@productName", productName);
                        cmd.Parameters.AddWithValue("@quantity", quantity);
                        cmd.Parameters.AddWithValue("@unitPrice", unitPrice);
                        cmd.Parameters.AddWithValue("@totalPrice", totalPrice);

                        dbCon.Opencon();
                        cmd.ExecuteNonQuery();
                        dbCon.Closecon();
                    }
                }

                UpdateStockQuantities();

                if (MessageBox.Show("هل تريد طباعة الفاتورة", "طباعة الفاتورة", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    Form3 form3 = new Form3();
                    form3.ShowDialog();
                }

                dataGridView1.Rows.Clear();
                text_code.Clear();
                text_totale.Clear();
                text_code.Focus();
                getNO();
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateStockQuantities()
        {
            try
            {
                for (int i = 0; i <= dataGridView1.Rows.Count - 1; i++)
                {
                    string barcode = dataGridView1.Rows[i].Cells[0].Value.ToString();
                    float qtySold = float.Parse(dataGridView1.Rows[i].Cells[2].Value.ToString());

                    // Check sales type
                    if (dataGridView1.Rows[i].Cells[5].Value!="كغ") // Selling by Pack
                    {
                        string updateQuery = "UPDATE ADD_PRODUCT SET QTY_PROD = QTY_PROD - @qtySold WHERE ID_PROD = @barcode";
                        using (SqlCommand cmd = new SqlCommand(updateQuery, dbCon.GetCon()))
                        {
                            cmd.Parameters.AddWithValue("@qtySold", qtySold);
                            cmd.Parameters.AddWithValue("@barcode", barcode);
                            dbCon.Opencon();
                            cmd.ExecuteNonQuery();
                            dbCon.Closecon();
                        }
                    }
                    else if (dataGridView1.Rows[i].Cells[5].Value == "كغ") // Selling by Kg
                    {
                        // Fetch the product's capacity
                        string selectQuery = "SELECT Capacite FROM ADD_PRODUCT WHERE ID_PROD = @barcode";
                        float capacity = 0;

                        using (SqlCommand cmd = new SqlCommand(selectQuery, dbCon.GetCon()))
                        {
                            cmd.Parameters.AddWithValue("@barcode", barcode);
                            dbCon.Opencon();
                            var result = cmd.ExecuteScalar();
                            capacity = result != null ? Convert.ToInt32(result) : 0;
                            dbCon.Closecon();
                        }

                        if (capacity > 0)
                        {
                            // Convert kg to packs
                            float packsEquivalent = qtySold / capacity;

                            string updateQuery = "UPDATE ADD_PRODUCT SET QTY_PROD = ROUND(QTY_PROD - @packsEquivalent, 2) WHERE ID_PROD = @barcode";
                            using (SqlCommand cmd = new SqlCommand(updateQuery, dbCon.GetCon()))
                            {
                                cmd.Parameters.AddWithValue("@packsEquivalent", packsEquivalent);
                                cmd.Parameters.AddWithValue("@barcode", barcode);
                                dbCon.Opencon();
                                cmd.ExecuteNonQuery();
                                dbCon.Closecon();
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid capacity for product: " + barcode);
                        }
                    }
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Error updating stock: " + ex.Message);
            }
        }

        private void selling_Load(object sender, EventArgs e)
        {
            text_code.Focus();
            string sql = "SELECT * FROM ADD_PRODUCT ORDER BY NAME_PROD ASC";
            SqlCommand cmd = new SqlCommand(sql, dbCon.GetCon());
            dbCon.Opencon();
            SqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                comboBox1.Items.Add(dr["NAME_PROD"]);
            }
            dbCon.Closecon();
            getNO();
        }



        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = -1;
            text_code.Clear();
            text_totale.Clear();
            dataGridView1.Rows.Clear();
            text_code.Focus();
        }




        private void button6_Click(object sender, EventArgs e)
        {
            Stock stock = new Stock();
            stock.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            One_Day one_Day = new One_Day();
            one_Day.ShowDialog();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            string colname = dataGridView1.Columns[e.ColumnIndex].Name;
            if (colname == "delete")
            {
                dataGridView1.Rows.RemoveAt(e.RowIndex);
                double prix = 0;
                double subprix;
                for (int c = 0; c < dataGridView1.Rows.Count; c++)
                {
                    prix = prix + (double.Parse(dataGridView1.Rows[c].Cells[3].Value.ToString()) * double.Parse(dataGridView1.Rows[c].Cells[2].Value.ToString()));
                    subprix = (double.Parse(dataGridView1.Rows[c].Cells[3].Value.ToString()) * double.Parse(dataGridView1.Rows[c].Cells[2].Value.ToString()));
                    dataGridView1.Rows[c].Cells[4].Value = subprix.ToString();
                }


                text_totale.Text = prix.ToString("F2");



            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            label2.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }
        public void getNO()
        {
            string getnum = "select max(ID_SELL)+1 FROM SELL";
            dbCon.Opencon();
            SqlCommand cmd = new SqlCommand(getnum, dbCon.GetCon());
            SqlDataReader reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    textBox3.Text = reader[0].ToString();
                    if (textBox3.Text == "")
                    {
                        textBox3.Text = "000001";
                    }
                }
            }
            else
            {
                textBox3.Text = "000001";
            }
            dbCon.Closecon();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button5_Click(object sender, EventArgs e)
        {
            Add_Product add_Product = new Add_Product();
            add_Product.ShowDialog();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            button1.PerformClick();
        }

        private void selling_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "QTY" || dataGridView1.Columns[e.ColumnIndex].Name == "Column3")
            {
                double prix = 0;
                double subprix;

                for (int c = 0; c < dataGridView1.Rows.Count; c++)
                {
                    // Check for null or empty values
                    var qtyCell = dataGridView1.Rows[c].Cells[2].Value;
                    var priceCell = dataGridView1.Rows[c].Cells[3].Value;

                    if (qtyCell == null || priceCell == null || string.IsNullOrEmpty(qtyCell.ToString()) || string.IsNullOrEmpty(priceCell.ToString()))
                    {
                        continue; // Skip this row if data is invalid
                    }

                    double quantity = double.Parse(qtyCell.ToString());
                    double productPrice = double.Parse(priceCell.ToString());

                    subprix = quantity * productPrice;
                    dataGridView1.Rows[c].Cells[4].Value = subprix.ToString("F2");
                    prix += subprix;
                }

                text_totale.Text = prix.ToString("F2");
            }

            if (dataGridView1.Columns[e.ColumnIndex].Name == "Column5")
            {
                double subprix = 0;

                for (int c = 0; c < dataGridView1.Rows.Count; c++)
                {
                    var subPriceCell = dataGridView1.Rows[c].Cells[4].Value;

                    if (subPriceCell == null || string.IsNullOrEmpty(subPriceCell.ToString()))
                    {
                        continue; // Skip if the cell is invalid
                    }

                    subprix += double.Parse(subPriceCell.ToString());
                }

                text_totale.Text = subprix.ToString("F2");
            }
        }


        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            double totalPrice = 0;

            try
            {
                if (string.IsNullOrWhiteSpace(comboBox1.Text))
                    return;

                dbCon.Opencon();
                if (radioButton2.Checked)
                {
                    string query = "SELECT ID_PROD, NAME_PROD, PRIX_VENT, QTY_PROD FROM ADD_PRODUCT WHERE NAME_PROD = @productName";
                    using (SqlCommand cmd = new SqlCommand(query, dbCon.GetCon()))
                    {
                        cmd.Parameters.AddWithValue("@productName", comboBox1.Text);

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                string barcode = row["ID_PROD"].ToString();
                                string productName = row["NAME_PROD"].ToString();
                                string unit = radioButton2.Text.ToString();
                                if (!double.TryParse(row["PRIX_VENT"].ToString(), out double productPrice))
                                    continue; // Skip invalid price
                                dataGridView1.Rows.Add(barcode, productName, 1, productPrice, productPrice);
                               
                            }
                        }
                    }

                }
                else if (radioButton1.Checked)
                {
                    string query = "SELECT ID_PROD, NAME_PROD, PRIX_KG, QTY_PROD FROM ADD_PRODUCT WHERE NAME_PROD = @productName";
                    using (SqlCommand cmd = new SqlCommand(query, dbCon.GetCon()))
                    {
                        cmd.Parameters.AddWithValue("@productName", comboBox1.Text);

                        using (SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd))
                        {
                            DataTable dt = new DataTable();
                            dataAdapter.Fill(dt);

                            foreach (DataRow row in dt.Rows)
                            {
                                string barcode = row["ID_PROD"].ToString();
                                string productName = row["NAME_PROD"].ToString();
                                string unit = radioButton1.Text.ToString();
                                if (!double.TryParse(row["PRIX_KG"].ToString(), out double productPrice))
                                    continue; // Skip invalid price

                                dataGridView1.Rows.Add(barcode, productName, 1, productPrice, productPrice,unit);
                                
                            }
                        }
                    }

                }

                // Calculate total price and update DataGridView
                foreach (DataGridViewRow dgvRow in dataGridView1.Rows)
                {
                    if (dgvRow.Cells[2].Value != null && dgvRow.Cells[3].Value != null)
                    {
                        if (double.TryParse(dgvRow.Cells[2].Value.ToString(), out double quantity) &&
                            double.TryParse(dgvRow.Cells[3].Value.ToString(), out double productPrice))
                        {
                            double subTotal = quantity * productPrice;
                            dgvRow.Cells[4].Value = subTotal.ToString("F2");
                            totalPrice += subTotal;
                        }
                    }
                }

                text_totale.Text = totalPrice.ToString("F2");
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                dbCon.Closecon(); // Ensure the connection is closed
            }

            // Reset the ComboBox and set focus back to the text box
            comboBox1.SelectedIndex = -1;
            text_code.Focus();
        }

        private void comboBox1_DropDownClosed(object sender, EventArgs e)
        {

        }
    }
}
