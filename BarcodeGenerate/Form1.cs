using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ZXing;

namespace BarcodeGenerate
{
    
    public partial class Form1 : Form
    {
        AppData.BarcodeDataTable _barcode;
        private string Filname;
        private string Barcode;
        private string PO;
        private string Vendor;
        private int qty = 0;
        public Form1()
        {
            InitializeComponent();
        }


        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(Barcode))
            {
                MessageBox.Show("Please select the product from Table", "Barcode Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                //BarcodeLib.Barcode barcode = new BarcodeLib.Barcode();
                //Image img = barcode.Encode(BarcodeLib.TYPE.UPCA, Barcode, Color.Black, Color.White, 110,40);
                //pictureBox1.Image = img;

                
                

                BarcodeWriter writer = new BarcodeWriter() { Format = BarcodeFormat.CODE_128 };
                Image img= writer.Write(Barcode);
                pictureBox1.Image =img;

                this.appData1.Clear();
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, ImageFormat.Png);
                    for (int i = 0; i < qty; i++)
                    {
                        this.appData1.Barcode.AddBarcodeRow(PO, Vendor, ms.ToArray());
                    }
                }
                ReportLoad();
                toolStripStatusLabel2.Text = "Barocdes Generated..";
                
                //MessageBox.Show("Barocdes Generated.", "Barcode Generator", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
           

        }

        private void button2_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image.Save("F:/" + "SavedImage.jpg");
            openFileDialog1.Filter = "CSV|*.csv";
            openFileDialog1.ValidateNames = true;
            openFileDialog1.Multiselect = false;
            openFileDialog1.ShowDialog();
            Filname = openFileDialog1.FileName;
            BindData(Filname);
        }


        private void BindData(string filePath)
        {
            try
            {
                DataTable dt = new DataTable();
                string[] lines = System.IO.File.ReadAllLines(filePath);
                if (lines.Length > 0)
                {
                    //first line to create header
                    string firstLine = lines[0];
                    string[] headerLabels = firstLine.Split(',');
                    foreach (string headerWord in headerLabels)
                    {
                        dt.Columns.Add(new DataColumn(headerWord));
                    }
                    //For Data
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string[] dataWords = lines[i].Split(',');
                        DataRow dr = dt.NewRow();
                        int columnIndex = 0;
                        foreach (string headerWord in headerLabels)
                        {
                            dr[headerWord] = dataWords[columnIndex++];
                        }
                        dt.Rows.Add(dr);
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    dataGridView1.DataSource = dt;
                }
            }
            catch { }
            

        }

        private void dataGridView1_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                PO = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
                Vendor = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();
                Barcode = dataGridView1.SelectedRows[0].Cells[2].Value.ToString();
                qty = int.Parse(dataGridView1.SelectedRows[0].Cells[3].Value.ToString());
                toolStripStatusLabel2.Text = "";
            }
            catch
            {

            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            this.reportViewer1.RefreshReport();
        }

        private void ReportLoad()
        {
            this.reportViewer1.RefreshReport();
            _barcode = this.appData1.Barcode;
            ReportDataSource reportDataSource = new ReportDataSource();
            reportDataSource.Name = "DataSet1";
            reportDataSource.Value = _barcode;
            reportViewer1.LocalReport.EnableExternalImages = true;
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);
            this.reportViewer1.RefreshReport();
        }
    }
}
