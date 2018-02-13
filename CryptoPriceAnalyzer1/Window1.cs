using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CryptoPriceAnalyzer1
{
    public partial class Window1 : Form
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void Window1_Load(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }

        public void update1MinPrices(string price)
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.Text += price;
            });
            

        }

        public void clear1MinPrices()
        {
            Invoke((MethodInvoker)delegate
            {
                richTextBox1.Text = "";
            });
           
        }
    }
}
