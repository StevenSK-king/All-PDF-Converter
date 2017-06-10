using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Telerik.WinControls;

namespace PDFtoTxt
{

    public partial class PdfLoginView : Telerik.WinControls.UI.RadForm
    {
        PdfReader pdfReader;
        Boolean NOPASS = true;
        PdfCopy pdfCopy;
        iTextSharp.text.Document pdfDoc;
        FileStream os;
        MainWindow mainwindow = new MainWindow();

        public PdfLoginView()
        {
            InitializeComponent();
        }

        private void radButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        //开始移除密码
        private void radButton1_Click(object sender, EventArgs e)
        {
            procUnlock();
        }

        // 解錠処理
        private void procUnlock()
        {
            // パスワード解除
            string r_password = radTextBox1.Text;
            string w_password = "";

            if (r_password.Length > 0 || w_password.Length > 0)
            {
                ////////////////////////////////////////////////////
                // PDF解錠
                ////////////////////////////////////////////////////

                // 一時ファイル取得
                String tmpFilePath = Path.GetTempFileName();
                Boolean isRP = false;
                Boolean isWP = false;
                Class1.isDecrypted = false;

                // パスワードなしで読み込めるかチェック
                try
                {
                    pdfReader = new PdfReader(Class1.SelectedPDFPath);
                    isRP = false; // ユーザーパスワードなし
                                  // オーナーパスワードが掛っているかチェック
                    isWP = (pdfReader.IsEncrypted()) ? true : false;
                    NOPASS = !(isRP || isWP);
                    pdfReader.Close();
                    pdfReader.Dispose();
                }
                catch
                {
                    isRP = true;
                    NOPASS = false;
                }
                if (NOPASS)
                {
                    // パスワードがかかっていない
                    //label2.Text = PDFProtect.Properties.Resources.pdf2;
                    //dataGridView1.Rows[Rows_index].Cells[6].ToolTipText = "Failed. This PDF is not applied password.";
                    //"This document is not applied password.";
                    pdfReader.Close();
                    pdfReader.Dispose();
                    //return false;
                }
                if (isRP && (r_password.Length == 0))
                {
                    // ユーザーパスワードが掛っているが、入力されていない
                    //label2.Text = PDFProtect.Properties.Resources.pdf3;
                    // "This document has been user password-protected.";
                    //return false;
                }
                if (isWP && (w_password.Length == 0))
                {
                    // オーナーパスワードが掛っているが、入力されていない
                    //label2.Text = PDFProtect.Properties.Resources.pdf4;
                    //"This document has been owner password-protected.";
                    //return false;
                }

                String rp = (r_password.Length == 0) ? null : r_password;
                String wp = (w_password.Length == 0) ? r_password : w_password;

                try
                {
                    pdfReader = new PdfReader(Class1.SelectedPDFPath, (byte[])System.Text.Encoding.ASCII.GetBytes(wp));
                }
                catch
                {
                    //label2.Text = PDFProtect.Properties.Resources.message2;
                    // "Password is incorrect.";
                    MessageBox.Show(this, "Password incorrect!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //return false;
                }


                try
                {
                    pdfDoc = new iTextSharp.text.Document(pdfReader.GetPageSize(1));
                    os = new FileStream(tmpFilePath, FileMode.OpenOrCreate);
                    pdfCopy = new PdfCopy(pdfDoc, os);
                    pdfCopy.Open();

                    pdfDoc.Open();
                    pdfCopy.AddDocument(pdfReader);

                    pdfDoc.Close();
                    pdfCopy.Close();
                    pdfReader.Close();
                    pdfReader.Dispose();
                    // オリジナルファイルと一時ファイルを置き換える  

                    string output = Class1.SelectedPDFPath.Substring(0, Class1.SelectedPDFPath.Length);

                    System.IO.File.Copy(tmpFilePath, output, true);

                    DialogResult dr = MessageBox.Show("Password has been removed", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (dr == DialogResult.OK)
                    {
                        this.Close();
                        Class1.isDecrypted = true;
                    }



                    //dataGridView1.Rows[Rows_index].Cells[4].Value = Properties.Resources.unlocked_pdf;
                    //dataGridView1.Rows[Rows_index].Cells[5].Value = Properties.Resources.unlocked_pdf;
                    //dataGridView1.Rows[Rows_index].Cells[6].Value = Properties.Resources.success_pdf;
                    //dataGridView1.Rows[Rows_index].Cells[6].ToolTipText = "Success！";
                    //System.IO.File.Delete(tmpFilePath);
                }
                catch (Exception eX)
                {
                    //label2.Text = PDFProtect.Properties.Resources.error1 + eX.Message;
                    //"Saving failed." + eX.Message;
                    //return false;
                }
                //label2.ForeColor = Color.Black;
                //label2.BackColor = SystemColors.Control;
                //label2.Text = PDFProtect.Properties.Resources.done;

            }
            else
            {
                //label2.Text = PDFProtect.Properties.Resources.message1;
                // "Please input password.";
                MessageBox.Show("Please input password.", "Warn", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
