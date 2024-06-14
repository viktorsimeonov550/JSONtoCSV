using System.Security.Cryptography.X509Certificates;

namespace JSONtoCSV
{
    public partial class JsonToCsvForm : Form
    {
        public JsonToCsvForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void RequestButtonClick(object sender, EventArgs e)
        {

            string userInput = textBoxUserInput.Text;

            // Check for invalid user input
            try
            {
                JsonToCsv.CheckUserInput(userInput);
            }
            catch (Exception ex)
            {
                DisplayInvalidInputMessage(ex.Message, "Error");
                return;
            }

            // Get JSON and display it in text box
            try
            {
                textBoxJson.Text = JsonToCsv.GetJson(userInput);
            }
            catch (Exception ex)
            {
                DisplayInvalidInputMessage(ex.Message, "Error");
                return;
            }

        }
        public void DisplayInvalidInputMessage(string message, string title)
        {
            MessageBox.Show(message, title);
        }

        private void labelUrl_Click(object sender, EventArgs e)
        {

        }

        private void JsonTextBoxTextChanged(object sender, EventArgs e)
        {
            if (textBoxJson.Text != string.Empty)
            {
                btnConvert.Visible = true;
                btnConvert.Focus();
                textBoxCsv.Clear();
            }
        }

        private void ConvertButtonClick(object sender, EventArgs e)
        {
            textBoxCsv.Text = JsonToCsv.GetCsv();
        }

        private void CsvTextBoxTextChanged(object sender, EventArgs e)
        {
            if (textBoxCsv.Text != string.Empty)
            {
                textBoxUserInput.Focus();
                btnConvert.Enabled = false;
            }
            else
            {
                btnConvert.Enabled = true;
                btnConvert.Focus();
            }
        }

        private void labelEndPoints_Click(object sender, EventArgs e)
        {

        }
    }
}