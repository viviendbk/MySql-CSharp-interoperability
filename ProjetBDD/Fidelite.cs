using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetBDD
{
    internal class Fidelite
    {
        string nom = null;
        int reduction = 0;

        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }
        public int Reduction
        {
            get { return reduction; }
            set { reduction = value; }
        }

        public Fidelite(int NbCommands = 0)
        {
            if (NbCommands < 5)
            {
                this.nom = "Bronze";
                this.reduction = 5;
            }
            else if (NbCommands > 4)
            {
                this.nom = "Or";
                this.reduction = 15;
            }
        }

    }
}
