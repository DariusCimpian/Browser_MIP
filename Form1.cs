using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProiectMIP
{
    public partial class Form1 : Form
    {
        private string[] lista = new string[]
        {
            "minor", "major", "facebook", "mancare",
            "jocuri", "genai", "codgenerat"
        };
        ComboBox combo = new ComboBox();
        public Form1()
        {
            InitializeComponent();
            SQLiteHandler.GetInstance().ConnectToDb("KeywordsMIP.db");
            InitializareDBculista();
        }
        private void InitializareDBculista()//sa incarcam lista in db
        {
            foreach (var k in lista)
            {
                if (!SQLiteHandler.GetInstance().existakeyword(k))
                {
                    SQLiteHandler.GetInstance().AddKeyword(k);
                }
            }
        }



        private void GoButton_Click(object sender, EventArgs e)
        {
            if (SearchBar.Text == "")
            {
                MessageBox.Show("Search barul nu poate sa fie gol");
                return;
            }
            else
            {
                string url = SearchBar.Text;
                Uri urlbun;
                if (Uri.TryCreate(url, UriKind.Absolute, out urlbun))//din cauza la absolute trebuie sa scriu https:// , altfel nu accepta
                {
                    webBrowser.Navigate(urlbun);
                }
                else
                {
                    MessageBox.Show("URL-ul nu este bun");
                }
            }
        }

        private void HomeButton_Click(object sender, EventArgs e)
        {
            webBrowser.GoHome();
        }
        private void ForwardButton_Click(object sender, EventArgs e)
        {
            webBrowser.GoForward();
        }
        private void BackButton_Click(object sender, EventArgs e)
        {
            webBrowser.GoBack();
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void SearchBar_Click(object sender, EventArgs e)
        {

        }
        private void SearchBar_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (SearchBar.Text == "")
                {
                    MessageBox.Show("Search barul nu poate sa fie gol");
                    return;
                }
                else
                {
                    string url = SearchBar.Text;
                    Uri urlbun;
                    if (Uri.TryCreate(url, UriKind.Absolute, out urlbun))
                    {
                        webBrowser.Navigate(urlbun);
                    }
                    else
                    {
                        MessageBox.Show("URL-ul nu este bun");
                    }
                }
            }
        }


        private async void webBrowser_Navigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            string url = e.Url.ToString();


            List<string> keywords = await SQLiteHandler.GetInstance().GetAllKeywords();

            foreach (var k in keywords)
            {
                if (!string.IsNullOrEmpty(k) && url.Contains(k))
                {
                    MessageBox.Show("Ati accesat un site restrictionat!", "Atentionare");
                    e.Cancel = true;
                    break;
                }
            }
        }

        private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }

        private async void viewDeleteKeywordsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form listaf = new Form();
            listaf.Text = "View/Delete keywords";
            listaf.Size = new Size(400, 300);
            listaf.StartPosition = FormStartPosition.CenterScreen;

            combo = new ComboBox();
            combo.Left = 90;
            combo.Top = 30;
            combo.Width = 150;
            combo.DropDownStyle = ComboBoxStyle.DropDownList;


            combo.Items.Clear();
            var keywords = await SQLiteHandler.GetInstance().GetAllKeywords();
            foreach (var k in keywords)
                combo.Items.Add(k);

            Button butondelete = new Button();
            butondelete.Text = "Delete keyword";
            butondelete.Left = 90;
            butondelete.Top = 70;
            butondelete.Width = 120;
            butondelete.Click += Butondelete_Click;

            listaf.Controls.Add(combo);
            listaf.Controls.Add(butondelete);

            listaf.AcceptButton = butondelete;
            listaf.Show();
        }

        private void Butondelete_Click(object sender, EventArgs e)
        {

            if (combo.SelectedItem == null)
            {
                MessageBox.Show("Selectează un keyword.");
                return;
            }

            string deSters = combo.SelectedItem.ToString();


            SQLiteHandler.GetInstance().DeleteKeyword(deSters);


            combo.Items.Remove(deSters);
        }


        private void addKeywordToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form adauga = new Form();
            adauga.Text = "Add keyword";
            adauga.Width = 300;
            adauga.Height = 150;

            Label etich = new Label();
            etich.Text = "Keyword:";
            etich.Left = 10;
            etich.Top = 20;
            etich.Width = 60;

            TextBox key = new TextBox();
            key.Left = 100;
            key.Top = 30;
            key.Width = 180;

            Button OK = new Button();
            OK.Text = "OK";
            OK.Left = 80;
            OK.Top = 60;
            OK.Width = 60;
            OK.DialogResult = DialogResult.OK;// ca sa vad ce buton a fost apasat 

            Button Cancel = new Button();
            Cancel.Text = "Cancel";
            Cancel.Left = 160;
            Cancel.Top = 60;
            Cancel.Width = 80;
            Cancel.DialogResult = DialogResult.Cancel;

            adauga.Controls.Add(etich);
            adauga.Controls.Add(key);
            adauga.Controls.Add(OK);
            adauga.Controls.Add(Cancel);

            adauga.AcceptButton = OK;
            adauga.CancelButton = Cancel;

            if (adauga.ShowDialog() == DialogResult.OK)//de la showdialog e modala , nu poate fi facut altceva pana nu e inchisa
            {
                string cuvant = key.Text.Trim();

                if (cuvant == "")
                {
                    MessageBox.Show("Cuvantul nu poate fi gol!");
                    return;
                }
                if (cuvant.Length > 64)
                {
                    MessageBox.Show("Cuvantul este prea lung");
                    return;
                }

               
                if (SQLiteHandler.GetInstance().existakeyword(cuvant))
                {
                    MessageBox.Show("Cuvantul deja exista");
                    return;
                }

               
                SQLiteHandler.GetInstance().AddKeyword(cuvant);
                MessageBox.Show("Cuvant adaugat");
            }
        }
    }
}

