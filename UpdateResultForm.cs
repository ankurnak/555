using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace footballKursova
{
    public partial class UpdateResultForm : Form
    {
        public bool ResultUpdated { get; private set; }
        public string Result { get; private set; }
        private string argument1;
        private string argument2;

        public UpdateResultForm(string arg1, string arg2)
        {
            InitializeComponent();

            argument1 = arg1;
            argument2 = arg2;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string result = textBox.Text;

            if (!string.IsNullOrEmpty(result))
            {
                Result = result;
                ResultUpdated = true;

                this.Close();
            }
            else
            {
                MessageBox.Show("Введите результат матча.", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
