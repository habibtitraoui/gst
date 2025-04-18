using System;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace gst.Forms
{
    public partial class Add_Product : Form
    {
        DB_CONNECT dbCon = new DB_CONNECT();




        public Add_Product()
        {
            InitializeComponent();
            dbCon.Opencon();

        }
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {

                string ADD_PROD = "INSERT INTO ADD_PRODUCT VALUES(N'" + BRCODE.Text + "',N'" + NAME_PROD.Text + "','" + float.Parse(QTY_PROD.Text) + "','" + float.Parse(PRIX_ACHAT.Text) + "','" + Int32.Parse(PRIX_VENTE.Text) + "','" + Int32.Parse(PRIX_KG.Text) + "')";
                SqlCommand cmd = new SqlCommand(ADD_PROD, dbCon.GetCon());
                dbCon.Opencon();
                cmd.ExecuteNonQuery();
                dbCon.Closecon();
                panel1.BackColor = Color.Green;
                label7.Text = "تمت إضافة المنتوج بنجاح";
                label7.ForeColor = Color.White;
                Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
            string barcode = BRCODE.Text;
            try
            {
                Zen.Barcode.Code128BarcodeDraw barcodeDrawi = Zen.Barcode.BarcodeDrawFactory.Code128WithChecksum;
                pictureBox1.Image = barcodeDrawi.Draw(barcode, 40);
            }
            catch (Exception) { }
        }

        private void PRINT_BRCODE_Click(object sender, EventArgs e)
        {
            PrintBarCode();
        }

        public void PrintBarCode()
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

        private void PrintDocument_PrintPage(object sender, PrintPageEventArgs e)
        {
            throw new NotImplementedException();
        }
        private void Clear()
        {
            BRCODE.Clear();
            QTY_PROD.Clear();
            NAME_PROD.Clear();
            PRIX_ACHAT.Clear();
            PRIX_VENTE.Clear();
            pictureBox1.Image = null;


        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }


    }
}
