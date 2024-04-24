using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using RestSharp;


namespace web_service_client
{
    public partial class Form1 : Form
    {
        private readonly HttpClient httpClient = new HttpClient();
        private List<Label> labels = new List<Label>();
        private List<TextBox> textBoxes = new List<TextBox>();
        string baseURL = "http://localhost/Lezione/";
        string json = "";
        public Form1()
        {
            InitializeComponent();

            // Imposta la ListView per consentire la selezione dell'intera riga
            listView1.FullRowSelect = true;

            // Assicurati di associare l'evento SelectedIndexChanged alla ListView
            listView1.SelectedIndexChanged += listView1_SelectedIndexChanged;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Add("componenti");
            comboBox1.Items.Add("computer");
            comboBox1.Items.Add("dipartimento");
            comboBox1.Items.Add("impiegato");
            comboBox1.Items.Add("installazione_software");
            comboBox1.Items.Add("inventario_componenti");
            comboBox1.Items.Add("software");
        }

        private async Task vedi()
        {
            HttpClient client = new HttpClient();
            string endpoint = comboBox1.SelectedItem.ToString() + "/";

            // Costruzione dell'URL completo
            string requestUrl = baseURL + endpoint;

            // Esempio di richiesta GET
            HttpResponseMessage response = await client.GetAsync(requestUrl);
            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();

                MessageBox.Show("Contenuto JSON: " + jsonResponse);
                textBox1.Text = jsonResponse;
                json = jsonResponse;
                stampa();
            }
            else
            {
                MessageBox.Show("Errore nella richiesta: " + response.StatusCode);
            }

        }

        public void stampa()
        {
            // Rimuovi tutti gli elementi dalla ListView
            listView1.Items.Clear();

            // Rimuovi tutte le colonne dalla ListView
            listView1.Columns.Clear();

            // Deserializza il JSON in un elenco di dizionari chiave-valore
            List<Dictionary<string, string>> dataList = JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(json);

            if (dataList.Count > 0)
            {
                // Prendi le chiavi del primo elemento per ottenere i nomi delle colonne
                var columnNames = dataList[0].Keys;

                // Aggiungi le colonne al ListView
                foreach (var columnName in columnNames)
                {
                    listView1.Columns.Add(columnName);
                }

                // Aggiungi gli elementi al ListView
                foreach (var data in dataList)
                {
                    // Creazione dell'array di stringhe contenente i valori della riga
                    string[] rowValues = new string[columnNames.Count];

                    // Popolamento dell'array con i valori della riga
                    int columnIndex = 0;
                    foreach (var columnName in columnNames)
                    {
                        // Assicurati che il dizionario contenga la chiave
                        if (data.ContainsKey(columnName))
                        {
                            rowValues[columnIndex] = data[columnName];
                        }
                        else
                        {
                            // Se la chiave non è presente, aggiungi una stringa vuota
                            rowValues[columnIndex] = "";
                        }
                        columnIndex++;
                    }

                    // Aggiungi la riga al ListView
                    listView1.Items.Add(new ListViewItem(rowValues));
                }
            }
            else
            {
                MessageBox.Show("Il JSON è vuoto o non è stato possibile deserializzarlo correttamente.");
            }
        }

        private void CreateLabelsAndTextBoxes()
        {
            // Assicurati di cancellare i controlli esistenti
            DeleteLabelsAndTextBoxes();

            // Ottieni il numero di colonne dalla ListView
            int columnsCount = listView1.Columns.Count;

            // Calcola la posizione iniziale dei controlli
            int initialX = 10;
            int initialY = 50;
            int labelWidth = 100;
            int textBoxWidth = 150;
            int spacing = 10;

            for (int i = 0; i < columnsCount; i++)
            {
                // Crea un label per ogni colonna
                Label label = new Label();
                label.Text = listView1.Columns[i].Text;
                label.Location = new System.Drawing.Point(initialX, initialY + (i * (label.Height + spacing)));
                label.Width = labelWidth;
                labels.Add(label);
                this.Controls.Add(label);

                // Crea una textbox per ogni colonna
                TextBox textBox = new TextBox();
                textBox.Location = new System.Drawing.Point(initialX + labelWidth + spacing, initialY + (i * (textBox.Height + spacing)));
                textBox.Width = textBoxWidth;

                if (listView1.Columns[i].Text.ToLower() == "id")
                {
                    // Se il nome della colonna è "id", disabilita la TextBox
                    textBox.Enabled = false;
                }

                textBoxes.Add(textBox);
                this.Controls.Add(textBox);
            }
        }

        private void DeleteLabelsAndTextBoxes()
        {
            // Rimuovi tutti i label e le textbox dalla form e dai rispettivi elenchi
            foreach (var label in labels)
            {
                this.Controls.Remove(label);
            }
            labels.Clear();

            foreach (var textBox in textBoxes)
            {
                this.Controls.Remove(textBox);
            }
            textBoxes.Clear();
        }


        private async void button1_Click(object sender, EventArgs e)
        {
            await vedi();

            // Se i label e le textbox sono già stati creati, cancellali
            if (labels.Count > 0 || textBoxes.Count > 0)
            {
                DeleteLabelsAndTextBoxes();
            }
            // Crea i label e le textbox
            CreateLabelsAndTextBoxes();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private async void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            await vedi();

            // Se i label e le textbox sono già stati creati, cancellali
            if (labels.Count > 0 || textBoxes.Count > 0)
            {
                DeleteLabelsAndTextBoxes();
            }
            // Crea i label e le textbox
            CreateLabelsAndTextBoxes();
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Verifica se è stata selezionata una riga
            if (listView1.SelectedItems.Count > 0)
            {
                // Ottieni l'elemento selezionato
                ListViewItem selectedItem = listView1.SelectedItems[0];

                // Popola le TextBox con i valori delle colonne corrispondenti
                for (int i = 0; i < selectedItem.SubItems.Count; i++)
                {
                    if (i < textBoxes.Count)
                    {
                        textBoxes[i].Text = selectedItem.SubItems[i].Text;
                    }
                }
            }
        }

        private async Task SendPostRequest(string jsonData)
        {
            MessageBox.Show(jsonData);

            string apiUrl = baseURL;

            using (var content = new StringContent(jsonData, Encoding.UTF8, "application/json"))
            {
                // Aggiungi l'header Content-Type
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                try
                {
                    var response = await httpClient.PostAsync(baseURL, content);

                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Richiesta POST completata con successo.");
                    }
                    else
                    {
                        MessageBox.Show($"Errore durante la richiesta POST. Codice: {response.StatusCode}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    MessageBox.Show($"Errore durante la richiesta HTTP: {ex.Message}");
                }
            }

        }

        private async void button2_Click(object sender, EventArgs e)
        {
            // versione precedente
            // Creazione di un oggetto anonimo per memorizzare i valori delle label e delle textbox
            var data = new Dictionary<string, string>();

            // Aggiungi il campo "nome_tabella" con il valore selezionato dalla combobox1
            data["nome_tabella"] = comboBox1.SelectedItem.ToString();

            for (int i = 0; i < labels.Count; i++)
            {
                data[labels[i].Text] = textBoxes[i].Text;
            }

            // Serializzazione dell'oggetto in formato JSON
            string json = JsonConvert.SerializeObject(data);

            textBox1.Text= json;

            // Invio del JSON in una richiesta HTTP POST
            await SendPostRequest(json);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
