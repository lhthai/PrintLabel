using PrintLabel.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrintLabel
{
    public partial class FrmMain : Form
    {
        string connectionString = "Data Source=10.50.4.4;Initial Catalog=VACP4DB;User ID=sa; Password=ACVN1234~!;";
        LoadBatchcodeFromWO loadBatchcodeFromWO = new LoadBatchcodeFromWO();
        public FrmMain()
        {
            InitializeComponent();
        }

        private void LoadProduct()
        {
            string queryString = $"SELECT DISTINCT(PRODUCT) FROM BATCHCODE WHERE CONVERT(DATE, CREATEDATE, 103) = '{dpDate.Value.ToString("MM/dd/yyyy")}'";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                try
                {
                    SqlDataAdapter adapter = new SqlDataAdapter(queryString, connectionString);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        cbProduct.DataSource = dt;
                        cbProduct.DisplayMember = "Product";
                        cbProduct.ValueMember = "Product";
                    }
                    else
                    {
                        MessageBox.Show("No release WO this day!");                       
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private void cbProductGroup_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DataRowView drv = (DataRowView)cbProduct.SelectedItem;
            String product = drv["Product"].ToString().Trim();
            DataTable dt = loadBatchcodeFromWO.LoadBatchcode(dpDate.Value.ToString("MM/dd/yyyy"), product);
            dvData.DataSource = dt;
            lblTotal.Text = $"Total: {dt.Rows.Count} labels";
        }

        private void dpDate_ValueChanged(object sender, EventArgs e)
        {
            LoadProduct();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            DataRowView drv = (DataRowView)cbProduct.SelectedItem;
            String product = drv["Product"].ToString().Trim();
            DataTable dt = loadBatchcodeFromWO.LoadBatchcode(dpDate.Value.ToString("MM/dd/yyyy"), product);
            Printer printer = new Printer();
            printer.PrintAll(product, dt);
        }
    }
}
