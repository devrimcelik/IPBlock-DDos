using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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

                            string _country = "";
                            if (checkBox2.Checked)
                            {
                                _country = Country(item.MetricName.Trim());
                            }
                            textBox1.Text += item.MetricName + " Count:" + item.MetricCount  +" " + _country + " " + Environment.NewLine;
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


        private static byte[] PostEncoding(string _postData)
        {
            //PostEncoding(postData); eski
            return Encoding.UTF8.GetBytes(_postData);
        }

        public string Country(string ip)
        {
            string sonuc = "";
            var request = (HttpWebRequest)WebRequest.Create("https://api.iplocation.net/?ip="+ip);

            var postData = "un=webapi";
            postData += "&p=webapi";
            var data = PostEncoding(postData);

            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();

            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            try
            {
                JObject json = JObject.Parse(responseString);

               sonuc = json["country_name"].ToString();
            }
            catch (Exception ex)
            {
            }
            return sonuc;
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
                if (item.MetricCount > sinir && !item.MetricName.Contains("127.0.0.1") && !item.MetricName.Contains("0.0.0.0") &&item.MetricName.Split('.').Length==4)
                {
                    string _country = "";
                    if (checkBox2.Checked)
                    {
                        _country = Country(item.MetricName.Trim());
                    }

                    textBox1.Text += item.MetricName + " Count:" + item.MetricCount +" " + _country + " " + Environment.NewLine;
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
