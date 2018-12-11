using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfTilaukset
{
    class TilausOtsikko
    {
        private DateTime tilausPvm;
        private DateTime toimitusPvm;
        public int Lkm; 
        public int TilausNumero { get; set; }
        public string AsiakkaanNimi { get; set; }
        public int AsiakasNumero { get; set; }
        public string ToimitusOsoite { get; set; }
        public DateTime TilausPvm {
            get
            {
                return tilausPvm;
            }
            set
            {
                if (value <= DateTime.Today)
                {
                    tilausPvm = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("Tilauspvm ei voi olla tulevaisuudessa!");
                }
            }
        }

        public DateTime ToimitusPvm
        {
            get
            {
                return toimitusPvm;
            }
            set
            {
                if (value > DateTime.Today)
                {
                    toimitusPvm = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("ToimitusPvm ei voi olla menneisyydessä!");
                }
            }
        }

        internal string LaskeToimitusAika()
        {
            TimeSpan erotus;
            erotus = ToimitusPvm.Subtract(tilausPvm);
            return erotus.ToString();
        }
    }
}
