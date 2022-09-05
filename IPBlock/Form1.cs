using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IPBlock
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = "";
                textBox2.Text = "";
                try
                {
                    IEnumerable<string> lines = File.ReadLines(openFileDialog1.FileName);
                    label1.Text = lines.Count<string>().ToString();
                    List<string> asList = lines.ToList<string>();
                    IEnumerable<string> cropped = asList.Select(word =>
                                    word.Substring(31, 24).Split(':')[0]);

                    //textBox1.Text += cropped.FirstOrDefault() + Environment.NewLine;

                    var groups = cropped.GroupBy(n => n)
                         .Select(n => new
                         {
                             MetricName = n.Key,
                             MetricCount = n.Count()
                         })
                         .OrderByDescending(n => n.MetricCount);
                    int sinir = 20;
                    try
                    {
                        sinir = Convert.ToInt32(textBox3.Text);
                    }
                    catch
                    {

                    }
                    foreach (var item in groups)
                    {
                        if (item.MetricCount> sinir && !item.MetricName.Contains("127.0.0.1") && !item.MetricName.Contains("0.0.0.0"))
                        {
                            textBox1.Text += item.MetricName + " Count:" + item.MetricCount + Environment.NewLine;
                            textBox2.Text += @"netsh advfirewall firewall add rule name = ""Block " + item.MetricName +  @""" Dir = In Action = Block RemoteIP =" + item.MetricName + Environment.NewLine; ;
                        }
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                    $"Details:\n\n{ex.StackTrace}");
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {

            textBox1.Text ="";
            textBox2.Text ="";

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            label3.Text = connections.Length.ToString() + "  Connection " + DateTime.Now.ToLongTimeString();

            IEnumerable<string> remoteipler = connections.Select(word =>
                                   word.RemoteEndPoint.ToString().Split(':')[0]);

            var groups = remoteipler.GroupBy(n => n)
                        .Select(n => new
                        {
                            MetricName = n.Key,
                            MetricCount = n.Count()
                        })
                        .OrderByDescending(n => n.MetricCount );

            int sinir = 20;
            try
            {
                sinir = Convert.ToInt32(textBox3.Text);
            }
            catch
            {

            }


            foreach (var item in groups)
            {
                if (item.MetricCount > sinir && !item.MetricName.Contains("127.0.0.1") && !item.MetricName.Contains("0.0.0.0"))
                {
                    textBox1.Text += item.MetricName + " Count:" + item.MetricCount + Environment.NewLine;
                    textBox2.Text += @"netsh advfirewall firewall add rule name = ""Block " + item.MetricName + @""" Dir = In Action = Block RemoteIP =" + item.MetricName + Environment.NewLine; ;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
                button2_Click(null, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            button2_Click(null, null);
        }
    }
}
