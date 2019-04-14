using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfTilaukset
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DateTime tänään = DateTime.Today;
        Decimal RivinSummaYht = 0;
        public MainWindow()
        {
            InitializeComponent(); //Vakiokomento...
            HaeAsiakkaat(); //Täytetään Asiakas -comboboxin sisältö
            HaeTuotteet(); //Täytetään Tuote -comboboxin sisältö
            dpTilausPvm.SelectedDate = tänään;  //Datepickerin oletuspvm
            dpToimitusPvm.SelectedDate = tänään.AddDays(14);  //Datepickerin oletuspvm
            //Luodaan ikäänkuin ilmaan DataGridTextColumn -tyyppisiä olioita
            DataGridTextColumn colTilausRiviNumero = new DataGridTextColumn();
            DataGridTextColumn colTilausNumero = new DataGridTextColumn();
            DataGridTextColumn colTuoteNumero = new DataGridTextColumn();
            DataGridTextColumn colTuoteNimi = new DataGridTextColumn();
            DataGridTextColumn colMaara = new DataGridTextColumn();
            DataGridTextColumn colAHinta = new DataGridTextColumn();
            DataGridTextColumn colRivinSumma = new DataGridTextColumn();
            //Oliot sidotaan tietyn nimiseen sarakkeeseen < --kohdistuu myöhemmin olion ominaisuuksiin, kunhan olio on ensin viety listalle
            colTilausRiviNumero.Binding = new Binding("TilausRiviNumero");
            colTilausNumero.Binding = new Binding("TilausNumero");
            colTuoteNumero.Binding = new Binding("TuoteNumero");
            colTuoteNimi.Binding = new Binding("TuoteNimi");
            colMaara.Binding = new Binding("Maara");
            colAHinta.Binding = new Binding("AHinta");
            colRivinSumma.Binding = new Binding("Summa");
            //DataGridin otsikot 
            colTilausRiviNumero.Header = "Tilausrivin numero";
            colTilausNumero.Header = "Tilauksen numero";
            colTuoteNumero.Header = "Tuotenumero";
            colTuoteNimi.Header = "Tuotenimi";
            colMaara.Header = "Määrä";
            colAHinta.Header = "A-Hinta";
            colRivinSumma.Header = "Rivin summa";
            //Edellä "ilmaan" luotujen sarakkeiden sijoitus konkreettiseen DataGridiin, joka on luotu formille
            dgTilausRivit.Columns.Add(colTilausRiviNumero);
            dgTilausRivit.Columns.Add(colTilausNumero);
            dgTilausRivit.Columns.Add(colTuoteNumero);
            dgTilausRivit.Columns.Add(colTuoteNimi);
            dgTilausRivit.Columns.Add(colMaara);
            dgTilausRivit.Columns.Add(colAHinta);
            dgTilausRivit.Columns.Add(colRivinSumma);

        }

        private void btnTallenna_Click(object sender, RoutedEventArgs e)
        {
            RivinSummaYht = 0;
            try
            {
                TilausOtsikko Tilaus = new TilausOtsikko();
                Tilaus.AsiakasNumero = int.Parse(txtAsiakasNumero.Text);
                Tilaus.ToimitusOsoite = txtToimitusOsoite.Text;
                Tilaus.Postinumero = txtPostinumero.Text;
                Tilaus.TilausPvm = dpTilausPvm.SelectedDate.Value;
                Tilaus.ToimitusPvm = dpToimitusPvm.SelectedDate.Value;

                txtToimitusAika.Text = Tilaus.LaskeToimitusAika();

                txtTilausNumero.Text = VieTilausKantaan(Tilaus); //Tässä EFW:n kautta tietojen vienti kantaan

                TilausTallennettuInfo(Tilaus);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
 
        }

        private void btnLisaaRivi_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TilausRivi tilausRivi = new TilausRivi();
                tilausRivi.TilausNumero = int.Parse(txtTilausNumero.Text);
                tilausRivi.TuoteNumero = int.Parse(txtTuoteNumero.Text);
                tilausRivi.TuoteNimi = cbTuote.Text;
                tilausRivi.Maara = int.Parse(txtMaara.Text);
                tilausRivi.AHinta = Convert.ToDecimal(txtAHinta.Text);

                tilausRivi.TilausRiviNumero = VieTilausRiviKantaan(tilausRivi);


                RivinSummaYht += tilausRivi.RiviSumma(); //Kuten tämä: RivinSummaYht = RivinSummaYht + TilausR.RiviSumma();
                txtRiviSumma.Text = RivinSummaYht.ToString();
                dgTilausRivit.Items.Add(tilausRivi);

                TilausRiviTallennettuInfo(tilausRivi);

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            txtTuoteNumero.Clear();
            txtMaara.Clear();
            txtAHinta.Clear();
        }

        private void btnTestaaEF_Click(object sender, RoutedEventArgs e)
        {
            TilausDBEntities entities = new TilausDBEntities();

            var tilausOtsikot = from o in entities.Tilaukset
                                where o.Postinumero == "53100"
                                select o;

            foreach (var tilaus in tilausOtsikot)
            {
                MessageBox.Show(tilaus.Toimitusosoite);
            }
        }

        private void btnHaeKannasta_Click(object sender, RoutedEventArgs e)
        {
            TilausDBEntities entities = new TilausDBEntities();

            var tilausRivit = from tr in entities.Tilausrivit
                                select tr;

            foreach (var tilausR in tilausRivit)
            {
                var dbRivi = new TilausRivi();
                dbRivi.TilausNumero = (int)tilausR.TilausID;
                dbRivi.TuoteNumero = (int)tilausR.TuoteID;
                dbRivi.Maara = (int)tilausR.Maara;
                dbRivi.AHinta = (int)tilausR.Ahinta;

                dgTilausRivit.Items.Add(dbRivi);
            }
        }

        private static void TilausTallennettuInfo(TilausOtsikko Tilaus)
        {
            MessageBox.Show("Tilaus tallennettu: " + "\r\n" + "Tilausnumero: " + Tilaus.TilausNumero.ToString() +
                "\r\n" + "Tilauspäivämäärä: " + Tilaus.TilausPvm.ToString() +
                "\r\n" + "Toimituspäivämäärä: " + Tilaus.ToimitusPvm.ToString() +
                "\r\n" + "Asiakasnumero: " + Tilaus.AsiakasNumero.ToString() +
                "\r\n" + "Toimitusosoite: " + Tilaus.ToimitusOsoite
                );
        }

        private static void TilausRiviTallennettuInfo(TilausRivi TilausR)
        {
            MessageBox.Show("Tilausrivi tallennettu: " + "\r\n" + "Tilausnumero: " + TilausR.TilausNumero.ToString() +
                "\r\n" + "Tuotenumero: " + TilausR.TuoteNumero +
                "\r\n" + "Tuotteen nimi: " + TilausR.TuoteNimi +
                "\r\n" + "Määrä: " + TilausR.Maara.ToString() +
                "\r\n" + "A-hinta: " + TilausR.AHinta.ToString() +
                "\r\n" + "Rivisumma: " + TilausR.RiviSumma().ToString()
                );
        }

        private void HaeAsiakkaat()
        {
            List<cbPairAsiakas> cbpairAsiakkaat = new List<cbPairAsiakas>();
            TilausDBEntities entities = new TilausDBEntities();

            var asiakkaat = from a in entities.Asiakkaat
                            select a;

            foreach (var asiakas in asiakkaat)
            {
                cbpairAsiakkaat.Add(new cbPairAsiakas(asiakas.Nimi,asiakas.AsiakasID));
            }
            cbAsiakas.DisplayMemberPath = "asiakasNimi";
            cbAsiakas.SelectedValuePath = "asiakasNro";
            cbAsiakas.ItemsSource = cbpairAsiakkaat;
        }

        private void HaeTuotteet()
        {
            List<cbPairTuote> cbpairTuotteet = new List<cbPairTuote>();
            TilausDBEntities entities = new TilausDBEntities();

            var tuotteet = from a in entities.Tuotteet
                            select a;

            foreach (var tuote in tuotteet)
            {
                cbpairTuotteet.Add(new cbPairTuote(tuote.Nimi, tuote.TuoteID));
            }
            cbTuote.DisplayMemberPath = "tuoteNimi";
            cbTuote.SelectedValuePath = "tuoteNro";
            cbTuote.ItemsSource = cbpairTuotteet;
        }

        private string VieTilausKantaan(TilausOtsikko Tilaus)
        {
            try
            {
                TilausDBEntities entities = new TilausDBEntities();

                Tilaukset dbItem = new Tilaukset()
                {
                    AsiakasID = Tilaus.AsiakasNumero,
                    Toimitusosoite = Tilaus.ToimitusOsoite,
                    Postinumero = Tilaus.Postinumero,
                    Tilauspvm = Tilaus.TilausPvm,
                    Toimituspvm = Tilaus.ToimitusPvm
                };

                entities.Tilaukset.Add(dbItem);
                entities.SaveChanges();

                int id = dbItem.TilausID; //Haetaan juuri lisätyn rivin IDENTITEETTIsarakkeen arvo (eli PK)
                return id.ToString(); //Palautetaan onnistuneen lisäyksen merkiksi uuden tilauksen numero
            }
            catch (Exception)
            {
                return "0";
            }
        }

        private int VieTilausRiviKantaan(TilausRivi TilausR)
        {
            TilausDBEntities entities = new TilausDBEntities();

            Tilausrivit dbItem = new Tilausrivit()
            {
                TilausID = TilausR.TilausNumero,
                TuoteID = TilausR.TuoteNumero,
                Maara = TilausR.Maara,
                Ahinta = TilausR.AHinta
            };

            entities.Tilausrivit.Add(dbItem);
            entities.SaveChanges();

            int id = dbItem.TilausriviID; //Haetaan juuri lisätyn rivin IDENTITEETTIsarakkeen arvo (eli PK)
            return id; //Pa
        }

        public void PianoKosketin_Click(object sender, RoutedEventArgs e)
        {
            Button myButton = (Button)sender;

            if (myButton.Background == Brushes.Red)
            {
                myButton.Background = Brushes.Gray;
            }
            else if (myButton.Background == Brushes.Gray)
            {
                myButton.Background = Brushes.Red;
            }
            else myButton.Background = Brushes.Gray;
        }

        private void CbAsiakas_DropDownClosed(object sender, EventArgs e)
        {
            cbPairAsiakas cbp = (cbPairAsiakas)cbAsiakas.SelectedItem;
            string AsiakasNimi = cbp.asiakasNimi;
            int AsiakasNro = cbp.asiakasNro;
            txtAsiakasNumero.Text = AsiakasNro.ToString();
        }

        private void CbTuote_DropDownClosed(object sender, EventArgs e)
        {
            cbPairTuote cbp = (cbPairTuote)cbTuote.SelectedItem;
            string TuoteNimi = cbp.tuoteNimi;
            int TuoteNro = cbp.tuoteNro;
            txtTuoteNumero.Text = TuoteNro.ToString();
        }
    }
}
