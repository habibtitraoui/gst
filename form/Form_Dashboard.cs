using System;
using System.Windows.Forms;

namespace gst.Forms
{
    public partial class Form_Dashboard : Form
    {
        public Form_Dashboard()
        {
            InitializeComponent();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            // Close the current form


            // Open the new form (Form1 in this case) modelessly

        }



        private void button6_Click(object sender, EventArgs e)
        {
            using (Stock adb = new Stock())
            {
                adb.ShowDialog();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (selling selling = new selling())
            {
                selling.ShowDialog();


            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (One_Day one_Day = new One_Day())
            {
                one_Day.ShowDialog();

            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (Report report = new Report())
            {
                report.ShowDialog();
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            Add_Product add_Product = new Add_Product();
            add_Product.ShowDialog();
        }

        private void button9_Click(object sender, EventArgs e)
        {

        }

        private void button8_Click(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form3 form3 = new Form3();
            form3.ShowDialog();
        }
    }
}

