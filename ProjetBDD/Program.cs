using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using System.ComponentModel;
using System.Media;
using System.Diagnostics;
using ProjetBDD;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using Google.Protobuf.WellKnownTypes;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using MySqlX.XDevAPI;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Tls.Crypto;


namespace projet_info
{
    public class Program
    {
        static void Main(string[] args)
        {
            Menu();
        }
        static dynamic Menu()
        {
            // Options du menu déroulant
            string[] options = { "Se connecter", "Créer un compte", "Mode admin"};
            int selectedOption = 0;
            // Affichage du menu déroulant
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Sélectionnez une option :");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(options[i]);
                    Console.ResetColor();
                }

                // Récupération de la touche appuyée par l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Traitement de la touche appuyée
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedOption = (selectedOption == 0) ? options.Length - 1 : selectedOption - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedOption = (selectedOption == options.Length - 1) ? 0 : selectedOption + 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    // Traitement de l'option sélectionnée
                    Console.Clear();
                    Console.WriteLine("Vous avez sélectionné l'option : " + options[selectedOption]);
                    if (selectedOption == 0)
                    {
                        ProjetBDD.Client C = ConnectUser();
                        ChooseShop(C);
                    }
                    else if (selectedOption == 1)
                    {
                        ProjetBDD.Client newClient = AccountCreation();
                        return Menu();
                    }
                    else if (selectedOption == 2)
                    {
                        return MenuAdmin();
                    }
                    Console.ReadLine();
                }
            }
            return null;
        }

        static int MenuAdmin()
        {
            string[] options = { "Clients", "Produits", "Commandes", "Statistiques" };
            int selectedOption = 0;
            // Affichage du menu déroulant
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Sélectionnez une option :");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(options[i]);
                    Console.ResetColor();
                }

                // Récupération de la touche appuyée par l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Traitement de la touche appuyée
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedOption = (selectedOption == 0) ? options.Length - 1 : selectedOption - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedOption = (selectedOption == options.Length - 1) ? 0 : selectedOption + 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    // Traitement de l'option sélectionnée
                    Console.Clear();
                    Console.WriteLine("Vous avez sélectionné l'option : " + options[selectedOption]);
                    if (selectedOption == 0)
                    {
                        return ModuleClient();
                    }
                    else if (selectedOption == 1)
                    {
                        return ModuleProduit();
                    }
                    else if (selectedOption == 2)
                    {
                        return ModuleCommandes();
                    }
                    else if (selectedOption == 3)
                    {
                        return ModuleStatistiques();
                    }
                    Console.ReadLine();
                }
            }
            return 0;
        }

        static int ModuleClient()
        {
            ProjetBDD.Client.DisplayAllClients();

            Console.WriteLine("Appuyez sur la barre d'espace pour exporter les clients ayants commandés 1 fois le mois dernier en xml, sur enter pour exporter en json les clients n'ayants pas commandés les 6 derniers mois ou sur une autre touche pour revenir en arrière :");

            ConsoleKeyInfo keyInfo = Console.ReadKey();
            if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                ProjetBDD.Client.ExportXML();
                return ModuleClient();
            }
            else if (keyInfo.Key == ConsoleKey.Enter)
            {
                ProjetBDD.Client.ExportJSON();
                return ModuleClient();
            }
            else
            {
                Console.ReadKey();
                return MenuAdmin();
            }

        }
        
        static int ModuleProduit()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            string query = "SELECT p.nom AS produit, p.type, p.prix, m.id AS magasin, COALESCE(SUM(po.quantite), 0) AS stock " +
                           "FROM Produit p " +
                           "CROSS JOIN Magasin m " +
                           "LEFT JOIN Possede po ON m.id = po.id_Magasin AND p.nom = po.nom_Produit " +
                           "GROUP BY p.nom, p.type, p.prix, m.id " +
                           "UNION " +
                           "SELECT bs.nom AS bouquet_standart, 'bouquet standart' AS type, bs.prix, m.id AS magasin, COALESCE(SUM(cd.quantite), 0) AS stock " +
                           "FROM Bouquet_Standart bs " +
                           "CROSS JOIN Magasin m " +
                           "LEFT JOIN Constitue_de cd ON bs.nom = cd.nom_Bouquet_Standart AND cd.nom_Produit IN (SELECT nom FROM Produit WHERE type = 'Fleur') " +
                           "LEFT JOIN Possede po ON m.id = po.id_Magasin AND cd.nom_Produit = po.nom_Produit " +
                           "WHERE po.nom_Produit IS NOT NULL " +
                           "GROUP BY bs.nom, bs.prix, m.id;";

            string query2 = "SELECT * FROM possede WHERE quantite < 10";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                string nomProduit = "";
                string typeProduit = "";
                float prixProduit = 0.0f;
                Dictionary<int, int> stockMagasins = new Dictionary<int, int>();

                while (reader.Read())
                {
                    string nomProduitCourant = reader.GetString("produit");
                    string typeProduitCourant = reader.GetString("type");
                    float prixProduitCourant = reader.GetFloat("prix");
                    int idMagasinCourant = reader.GetInt32("magasin");
                    int stockProduitCourant = reader.GetInt32("stock");

                    if (nomProduit == "" || nomProduit != nomProduitCourant)
                    {
                        // Nouveau produit
                        if (nomProduit != "")
                        {
                            // Afficher les caractéristiques du produit précédent
                            Console.WriteLine("Nom = {0}, Type = {1}, Prix unitaire = {2} euros, Stock = {{ {3} }}", nomProduit, typeProduit, prixProduit, string.Join(", ", stockMagasins.Select(s => $"magasin {s.Key} : {s.Value}")));
                        }

                        // Initialiser les caractéristiques du nouveau produit
                        nomProduit = nomProduitCourant;
                        typeProduit = typeProduitCourant;
                        prixProduit = prixProduitCourant;
                        stockMagasins = new Dictionary<int, int>();
                    }

                    // Ajouter le stock du produit au magasin courant
                    stockMagasins.Add(idMagasinCourant, stockProduitCourant);
                }

                // Afficher les caractéristiques du dernier produit
                Console.WriteLine("Nom = {0}, Type = {1}, Prix unitaire = {2}, Stock = {{ {3} }}", nomProduit, typeProduit, prixProduit, string.Join(", ", stockMagasins.Select(s => $"magasin {s.Key} : {s.Value}")));

                cmd = new MySqlCommand(query2, conn);

            }

            AfficherProduitsStockInf10();

            Console.WriteLine("\nAppuyez sur une touche pour revenir en arrière");
            Console.ReadKey();
            return MenuAdmin();
        }

        static void AfficherProduitsStockInf10()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            string query = "SELECT * FROM Possede WHERE quantite < 10 ORDER BY id_Magasin, nom_Produit";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                int magasinCourant = -1;
                Dictionary<string, int> produitsStockInf10 = new Dictionary<string, int>();
                bool affichageEffectue = false;

                while (reader.Read())
                {
                    int idMagasin = reader.GetInt32("id_Magasin");
                    string nomProduit = reader.GetString("nom_Produit");
                    int quantite = reader.GetInt32("quantite");

                    if (magasinCourant == -1 || magasinCourant != idMagasin)
                    {
                        // Nouveau magasin
                        if (magasinCourant != -1)
                        {
                            // Afficher les produits avec stock inférieur à 10 du magasin précédent
                            if (!affichageEffectue)
                            {
                                Console.WriteLine("\n\nProduits dont le stock est inférieur à 10 :");
                                affichageEffectue = true;
                            }
                            Console.WriteLine("Magasin {0} :", magasinCourant);
                            foreach (var produit in produitsStockInf10)
                            {
                                Console.WriteLine("{{ nom = {0}, stock = {1} }},", produit.Key, produit.Value);
                            }
                            Console.WriteLine();
                        }

                        // Initialiser les caractéristiques du nouveau magasin
                        magasinCourant = idMagasin;
                        produitsStockInf10 = new Dictionary<string, int>();
                    }

                    // Ajouter le produit avec stock inférieur à 10
                    if (quantite < 10)
                    {
                        produitsStockInf10.Add(nomProduit, quantite);
                    }
                }

                // Afficher les produits avec stock inférieur à 10 du dernier magasin
                if (!affichageEffectue && produitsStockInf10.Count > 0)
                {
                    Console.WriteLine("\n\nProduits dont le stock est inférieur à 10 :");
                    affichageEffectue = true;
                }
                if (affichageEffectue)
                {
                    Console.WriteLine("Magasin {0} :", magasinCourant);
                    foreach (var produit in produitsStockInf10)
                    {
                        Console.WriteLine("{{ nom = {0}, stock = {1} }},", produit.Key, produit.Value);
                    }
                    Console.WriteLine();
                }
            }
        }

        static int ModuleCommandes()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            Dictionary<DateTime, List<string>> commandesParDate = new Dictionary<DateTime, List<string>>();
            Dictionary<string, string> commandesParId = new Dictionary<string, string>();

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT CAST(dateCommande AS DATE) AS date_commande, Commande.id, type, code_Etat_Commande, " +
                             "CONCAT_WS('', " +
                             "    IFNULL(Bouquet_Standart.nom, ''), " +
                             "    IF(Commande.id_Bouquet_Personalise IS NOT NULL, CONCAT('Bouquet personnalisé: ', Bouquet_Personalise.description), ''), " +
                             "    ( " +
                             "        SELECT GROUP_CONCAT(CONCAT(Produit.nom, ' x', Concerne2.quantite) SEPARATOR ', ') " +
                             "        FROM Concerne2 " +
                             "        INNER JOIN Produit ON Concerne2.nom_Produit = Produit.nom " +
                             "        WHERE Concerne2.id_Commande = Commande.id " +
                             "    ) " +
                             ") AS produits_achetes, " +
                             "prixTotal " +
                             "FROM Commande " +
                             "LEFT JOIN Bouquet_Standart ON Commande.nom_Bouquet_Standart = Bouquet_Standart.nom " +
                             "LEFT JOIN Bouquet_Personalise ON Commande.id_Bouquet_Personalise = Bouquet_Personalise.id " +
                             "ORDER BY date_commande, Commande.id";

                MySqlCommand command = new MySqlCommand(sql, connection);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DateTime dateCommande = reader.GetDateTime("date_commande");
                        string idCommande = reader.GetString("id");
                        string typeCommande = reader.GetString("type");
                        string etatCommande = reader.GetString("code_Etat_Commande");
                        string produitsAchetes = reader.GetString("produits_achetes");
                        decimal prixTotal = reader.GetDecimal("prixTotal");

                        if (!commandesParDate.ContainsKey(dateCommande))
                        {
                            commandesParDate[dateCommande] = new List<string>();
                        }

                        string commande = $"{{Id : {idCommande}, Type : {typeCommande}, Etat : {etatCommande}, Produits : {produitsAchetes}, Prix Total : {prixTotal}}}";
                        commandesParDate[dateCommande].Add(commande);

                        commandesParId[idCommande] = commande;
                    }
                }
            }

            foreach (KeyValuePair<DateTime, List<string>> dateCommande in commandesParDate)
            {
                Console.WriteLine($"\nDate de commande : {dateCommande.Key.ToShortDateString()}");
                foreach (string commande in dateCommande.Value)
                {
                    Console.WriteLine($"\t{commande}");
                }
            }

            Console.Write("\nQuelle date voulez-vous regarder en particulier? (dd/mm/aaaa) : ");
            string inputDate = Console.ReadLine();
            bool parseOk = DateTime.TryParse(inputDate, out DateTime date);
            if (!parseOk)
            {
                Console.WriteLine("Format de date invalide.");
                return 0;
            }

            if (commandesParDate.ContainsKey(date))
            {
                Console.WriteLine($"\nCommandes pour la date {date.ToShortDateString()} :");
                foreach (string commande in commandesParDate[date])
                {
                    Console.WriteLine(commande);
                }

                Console.Write("\nEntrez l'ID de la commande que vous souhaitez modifier : ");
                string inputId = Console.ReadLine();

                if (commandesParId.ContainsKey(inputId))
                {
                    string commande = commandesParId[inputId];
                    Console.WriteLine($"\nCommande sélectionnée : {commande}");
                    Commande C1 = new Commande(Convert.ToInt32(inputId));
                    if (C1.Id_Bouquet_Personalise > 0 && C1.Code_Etat_Commande == "CPAV")
                    {
                        List<string[]> optionsComponents = GetProducts("fleur", C1.Id_Magasin);
                        optionsComponents.AddRange(GetProducts("accessoire", C1.Id_Magasin));

                        int selectedOption = 0;
                        int nombre = 0;
                        Dictionary<string, double[]> Bouquet = new Dictionary<string, double[]>();

                        // Affichage du menu déroulant
                        while (true)
                        {
                            Console.Clear();
                            Console.WriteLine($"Commande sélectionnée : {commande}");
                            Console.WriteLine("\nCette commande comporte un bouquet personalisé qui n'a pas encore été fait");
                            Console.WriteLine("\nComment voulez-vous constituer ce bouquet?");

                            if (C1.Client.Nom_Fidelite != "")
                            {
                                Console.WriteLine("Le client a le grade fidélité " + C1.Client.Nom_Fidelite + " qui lui donne une réduction (comprise) de " + C1.Client.GetFidelityReduc() + "%");
                            }

                            for (int i = 0; i < optionsComponents.Count; i++)
                            {
                                if (i == selectedOption)
                                {
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.WriteLine(optionsComponents[i][0] + " : " + Convert.ToDouble(optionsComponents[i][1]) * ((100 - Convert.ToDouble(C1.Client.GetFidelityReduc())) / 100) + " euros x " + nombre + "\t\t\tQuantité Dispo : " + Convert.ToString(Convert.ToInt32(optionsComponents[i][2]) - nombre));
                                }
                                else
                                {
                                    Console.WriteLine(optionsComponents[i][0] + " : " + Convert.ToDouble(optionsComponents[i][1]) * ((100 - Convert.ToDouble(C1.Client.GetFidelityReduc())) / 100) + "\t\t\t\t\tQuantité Dispo : " + optionsComponents[i][2]);

                                }
                                Console.ResetColor();
                            }

                            Console.WriteLine("\nBouquet actuel : ");
                            double prixTotal = 0;

                            foreach (KeyValuePair<string, double[]> element in Bouquet)
                            {
                                prixTotal += Convert.ToDouble(element.Value[0]) * ((100 - Convert.ToDouble(C1.Client.GetFidelityReduc())) / 100) * element.Value[1];

                                // Afficher la clé
                                Console.WriteLine("Nom : " + element.Key + ", prix : " + Convert.ToDouble(element.Value[0]) * ((100 - Convert.ToDouble(C1.Client.GetFidelityReduc())) / 100) + " euros, quantité : " + element.Value[1]);
                            }
                            Console.WriteLine("Prix total du bouquet : " + prixTotal + " euros");
                            Console.WriteLine("Appuyez sur espace pour valider le bouquet");

                            // Récupération de la touche appuyée par l'utilisateur
                            ConsoleKeyInfo keyInfo = Console.ReadKey();

                            // Traitement de la touche appuyée
                            if (keyInfo.Key == ConsoleKey.UpArrow)
                            {
                                nombre = 0;
                                selectedOption = (selectedOption == 0) ? optionsComponents.Count - 1 : selectedOption - 1;
                            }
                            else if (keyInfo.Key == ConsoleKey.DownArrow)
                            {
                                nombre = 0;
                                selectedOption = (selectedOption == optionsComponents.Count - 1) ? 0 : selectedOption + 1;
                            }
                            else if (keyInfo.Key == ConsoleKey.RightArrow && Convert.ToInt32(optionsComponents[selectedOption][2]) > nombre)
                            {
                                nombre += 1;
                            }
                            else if (keyInfo.Key == ConsoleKey.LeftArrow && nombre > 0)
                            {
                                nombre -= 1;
                            }
                            else if (keyInfo.Key == ConsoleKey.Enter && nombre > 0)
                            {
                                if (!Bouquet.ContainsKey(optionsComponents[selectedOption][0]))
                                {
                                    double[] toAdd = new double[2];
                                    toAdd[0] = Convert.ToDouble(optionsComponents[selectedOption][1]);
                                    toAdd[1] = Convert.ToDouble(nombre);
                                    Bouquet.Add(optionsComponents[selectedOption][0], toAdd);
                                }
                                else
                                {
                                    Bouquet[optionsComponents[selectedOption][0]][1] += nombre;
                                }
                                int newQuantity = Convert.ToInt32(optionsComponents[selectedOption][2]) - nombre;
                                optionsComponents[selectedOption][2] = newQuantity.ToString();
                            }
                            else if (keyInfo.Key == ConsoleKey.Spacebar && Bouquet.Count > 0)
                            {
                                double price = 0;
                                List<string[]> compo = new List<string[]>();
                                foreach (KeyValuePair<string, double[]> element in Bouquet)
                                {
                                    compo.Add(new string[] { element.Key, element.Value[1].ToString() });
                                    price += element.Value[1] * element.Value[0];
                                }
                                for (int i = 0; i < compo.Count; i++)
                                {
                                    Commande.InsertConcern(C1.Id, compo[i][0], Convert.ToInt32(compo[i][1]));
                                    Bouquet_Personalise.Insert_Constitue_de2(C1.Id_Bouquet_Personalise, compo[i][0], Convert.ToInt32(compo[i][1]));
                                    Commande.UpdateQuantite(C1.Id_Magasin, compo[i][0], Convert.ToInt32(compo[i][1]));
                                }
                                C1.Code_Etat_Commande = "CC";
                                Console.WriteLine("Modification effectuée!, appuyez sur une touche pour revenir en arrière");
                                Console.ReadKey();
                                return Menu();
                            }
                        }


                        // TODO : Ajouter le code pour modifier la commande correspondante
                    }
                    else
                    {
                        Console.WriteLine($"Commande sélectionnée : {commande}");
                        Console.WriteLine("\n");

                        string query = "SELECT code, description FROM etat_commande";
                        Console.WriteLine("\n etat des commande en fonction du code : ");

                        using (MySqlConnection conn = new MySqlConnection(connectionString))
                        {
                            conn.Open();
                            MySqlCommand cmd = new MySqlCommand(query, conn);
                            MySqlDataReader reader = cmd.ExecuteReader();

                            while (reader.Read())
                            {
                                string code1 = reader.GetString("code");
                                string etat = reader.GetString("description");

                                Console.WriteLine("code : {0}, description : {1}", code1, etat);
                            }
                        }
                        Console.Write("\nEntrez le nouveau code de l'etat de la commande : ");
                        string code = Console.ReadLine();
                        if (Commande.CodeStateExist(code))
                        {
                            C1.Code_Etat_Commande = code;
                        }
                        else
                        {
                            Console.WriteLine("Ce code n'existe pas");
                        }

                    }
                }
                else
                {
                    Console.WriteLine($"Aucune commande trouvée pour l'ID {inputId}.");
                }
            }
            else
            {
                Console.WriteLine($"Aucune commande trouvée pour la date {date.ToShortDateString()}.");
            }
            Console.ReadKey();
            return Menu();
        }

        static int ModuleStatistiques()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);

            try
            {
                connection.Open();

                // Requêtes SQL
                string requete1 = "SELECT AVG(prixTotal) AS prix_moyen_bouquet FROM Commande;";
                string requete2 = "SELECT c.nom, c.prenom, SUM(co.prixTotal) AS Total_Depense " +
                                  "FROM Commande co " +
                                  "JOIN Client c ON co.id_Client = c.id " +
                                  "WHERE co.dateCommande >= DATE_SUB(CURDATE(), INTERVAL 1 MONTH) " +
                                  "GROUP BY co.id_Client " +
                                  "ORDER BY Total_Depense DESC " +
                                  "LIMIT 1;";
                string requete3 = "SELECT c.nom, c.prenom, SUM(co.prixTotal) AS Total_Depense " +
                                  "FROM Commande co " +
                                  "JOIN Client c ON co.id_Client = c.id " +
                                  "WHERE co.dateCommande >= DATE_SUB(CURDATE(), INTERVAL 1 YEAR) " +
                                  "GROUP BY co.id_Client " +
                                  "ORDER BY Total_Depense DESC " +
                                  "LIMIT 1;";
                string requete4 = "SELECT b.nom, COUNT(*) AS nbVentes " +
                                  "FROM Bouquet_Standart b " +
                                  "JOIN Commande lc ON lc.nom_bouquet_standart = b.nom " +
                                  "GROUP BY b.nom " +
                                  "ORDER BY nbVentes DESC " +
                                  "LIMIT 1;";
                string requete5 = "SELECT id_Magasin, SUM(prixTotal) AS chiffre_affaires\r\nFROM Commande\r\nGROUP BY id_Magasin\r\nORDER BY chiffre_affaires DESC\r\nLIMIT 1;";
                string requete6 = "SELECT p.nom, SUM(c.quantite) AS total_ventes " +
                                  "FROM Produit p " +
                                  "JOIN Concerne2 c ON c.nom_Produit = p.nom " +
                                  "WHERE p.type = 'fleur' " +
                                  "GROUP BY p.nom " +
                                  "ORDER BY total_ventes ASC " +
                                  "LIMIT 1;";
                string requete7 = @"
                    SELECT c1.nom_Produit, c2.nom_Produit, COUNT(*) AS nb_commandes
                    FROM concerne2 c1
                    JOIN concerne2 c2 ON c1.id_Commande = c2.id_Commande AND c1.nom_Produit <> c2.nom_Produit
                    GROUP BY c1.nom_Produit, c2.nom_Produit
                    HAVING nb_commandes > 1";

                // Exécution des requêtes et affichage des résultats personnalisés
                AfficherResultat(requete1, "Prix moyen d'une commande : {0}", connection);
                AfficherResultat(requete2, "Meilleur client du mois : Nom = {0}, Prénom = {1}, Total des dépenses = {2}", connection);
                AfficherResultat(requete3, "Meilleur client de l'année : Nom = {0}, Prénom = {1}, Total des dépenses = {2}", connection);
                AfficherResultat(requete4, "Bouquet standard le plus vendu : {0}, Nombre de ventes = {1}", connection);
                AfficherResultat(requete5, "Magasin avec le plus de chiffre d'affaires : Nom = {0}, Chiffre d'affaires = {1}", connection);
                AfficherResultat(requete6, "Fleur la moins vendue : {0}, Total des ventes = {1}", connection);

                Console.WriteLine("\nProduits fréquements achetés ensemble : \n");
                AfficherResultat(requete7, "{{{0}, {1}}}, nombre de commande : {2}", connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }

            Console.ReadLine();
            return 0;
        }

        static void AfficherResultat(string requete, string format, MySqlConnection connection)
        {
            MySqlCommand command = new MySqlCommand(requete, connection);

            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    object[] values = new object[reader.FieldCount];
                    reader.GetValues(values);

                    Console.WriteLine(format, values);
                }
            }

            Console.WriteLine();
        }

        static ProjetBDD.Client AccountCreation()
        {
            Regex regexNomPrenom = new Regex("^[a-zA-Z]+$");
            Regex regexNumTel = new Regex("^[0-9]{10}$");
            Regex regexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            Regex regexCarteCredit = new Regex("^[0-9]{16}$");

            Console.Write("Nom : ");
            string nom = Console.ReadLine();

            while (!regexNomPrenom.IsMatch(nom))
            {
                Console.WriteLine("\nNom invalide. Veuillez entrer un nom valide sans caractères spéciaux ou chiffres.");
                Console.Write("Nom : ");
                nom = Console.ReadLine();
            }

            Console.Write("\nPrénom : ");
            string prenom = Console.ReadLine();

            while (!regexNomPrenom.IsMatch(prenom))
            {
                Console.WriteLine("\nPrénom invalide. Veuillez entrer un prénom valide sans caractères spéciaux ou chiffres.");
                Console.Write("Prénom : ");
                prenom = Console.ReadLine();
            }

            string email = GetValidEmail();

            while (!regexEmail.IsMatch(email))
            {
                Console.WriteLine("\nAdresse email invalide. Veuillez entrer une adresse email valide.");
                Console.Write("Adresse Email : ");
                email = Console.ReadLine();
            }

            Console.Write("\nMot de passe : ");
            string motDePasse = "";
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(true);
                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    motDePasse += key.KeyChar;
                    Console.Write("*");
                }
                else if (key.Key == ConsoleKey.Backspace && motDePasse.Length > 0)
                {
                    motDePasse = motDePasse.Remove(motDePasse.Length - 1);
                    Console.Write("\b \b");
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.Write("\n\nNuméro de téléphone : ");
            string numeroTelephone = Console.ReadLine();

            while (!regexNumTel.IsMatch(numeroTelephone))
            {
                Console.WriteLine("\nNuméro de téléphone invalide. Veuillez entrer un numéro de téléphone valide composé uniquement de 10 chiffres.");
                Console.Write("Numéro de téléphone : ");
                numeroTelephone = Console.ReadLine();
            }


            Console.Write("\nAdresse de facturation : ");
            string adresseFacturation = Console.ReadLine();

            Console.Write("\nCarte de crédit : ");
            string carteCredit = Console.ReadLine();
            Console.WriteLine();

            while (!regexCarteCredit.IsMatch(carteCredit))
            {
                Console.WriteLine("Numéro de carte de crédit invalide. Veuillez entrer un numéro de carte de crédit valide composé de 16 chiffres.");
                Console.Write("Carte de crédit : ");
                carteCredit = Console.ReadLine();
                Console.WriteLine();
            }

            Console.WriteLine("\nVotre Compte a bien été enregisté, retour à la page de connexion!");

            return new ProjetBDD.Client(nom, prenom, email, motDePasse, numeroTelephone, adresseFacturation, carteCredit);
        }
        static ProjetBDD.Client ConnectUser()
        {
            int id = 0;
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            // Demande à l'utilisateur de saisir son adresse e-mail et son mot de passe
            Console.Write("Entrez votre adresse e-mail : ");
            string email = Console.ReadLine();
            Console.Write("\nEntrez votre mot de passe : ");
            string password = Console.ReadLine();

            // Requête SQL pour sélectionner les données du client avec l'adresse e-mail et le mot de passe saisis
            string query = "SELECT * FROM Client WHERE email='" + email + "' AND motDePasse ='" + password + "'";

            // Exécution de la requête SQL
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            // Vérification que les informations sont présentes dans la base de données
            if (reader.HasRows)
            {
                Console.Clear();
                // Récupération des variables nom et prénom
                string nom = "";
                string prénom = "";
                while (reader.Read())
                {
                    nom = reader.GetString("nom");
                    prénom = reader.GetString("prenom");
                    id = reader.GetInt32("id");
                }
                Console.WriteLine("Connexion réussie, bienvenu " + prénom + " " + nom);
            }
            else
            {
                Console.WriteLine("Erreur: adresse e-mail ou mot de passe incorrect.");
                ConnectUser();
            }

            // Fermeture du lecteur et de la connexion à la base de données
            reader.Close();
            connection.Close();
            return new ProjetBDD.Client(id);
        }
        static string GetValidEmail()
        {
            Regex regexEmail = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

            Console.Write("\nAdresse email : ");
            string email = Console.ReadLine();


            if (!regexEmail.IsMatch(email))
            {
                Console.WriteLine("\nAdresse email invalide. Veuillez entrer une adresse email valide.");
                return GetValidEmail();
            }

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "SELECT * FROM Client WHERE email='" + email + "'";

            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                Console.WriteLine("\nAdresse email déjà enregistrée. Veuillez en entrer une nouvelle.");
                return GetValidEmail();
            }
            return email;
        }      
        static int ChooseShop(ProjetBDD.Client Client)
        {
            List<string[]> options = GetShops();

            int selectedOption = 0;
            // Affichage du menu déroulant
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Dans quelle boutique voulez-vous commander?");
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    string[] adresse = options[i][1].Split(' ');
                    Console.WriteLine(adresse[adresse.Length - 1]);
                    Console.ResetColor();
                }

                // Récupération de la touche appuyée par l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Traitement de la touche appuyée
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedOption = (selectedOption == 0) ? options.Count - 1 : selectedOption - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedOption = (selectedOption == options.Count - 1) ? 0 : selectedOption + 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    return ProcessShopping(Convert.ToInt32(options[selectedOption][0]), Client);
                }
            }

            Console.WriteLine("\n\nAppuyez sur une touche pour revenir en arrière");
            Console.ReadKey();
            return Menu();
        }
        static int ProcessShopping(int idMagasin, ProjetBDD.Client Client)
        {
            string[] options = { "Bouquet", "Accessoires", "Fleurs" };
            int selectedOption = 0;
            // Affichage du menu déroulant
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Que voulez-vous commander?");
                for (int i = 0; i < options.Length; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                    }
                    Console.WriteLine(options[i]);
                    Console.ResetColor();
                }

                // Récupération de la touche appuyée par l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Traitement de la touche appuyée
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    selectedOption = (selectedOption == 0) ? options.Length - 1 : selectedOption - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    selectedOption = (selectedOption == options.Length - 1) ? 0 : selectedOption + 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    // Traitement de l'option sélectionnée
                    Console.Clear();
                    Console.WriteLine("Vous avez sélectionné l'option : " + options[selectedOption]);
                    switch (selectedOption)
                    {
                        case 0:
                            return BunchShop(idMagasin, Client);
                        case 1:
                            return AccessoryShop(idMagasin, Client);
                        case 2:
                            return FlowerShop(idMagasin, Client);
                    }
                    Console.ReadLine();
                }
            }
            return 0;
        }
        static int BunchShop(int idMagasin, ProjetBDD.Client Client)
        {
            List < List<string[]>> options = GetBunchs(idMagasin);
            List<string[]> a = new List<string[]>();
            a.Add(new string[] { "Personaliser" });
            a.Add(new string[] { "" });
            a.Add(new string[] { "" });
            options.Add(a);

            int selectedOption = 0;
            int nombre = 0;

            while (true)
            {
                Console.Clear();
                if (Client.Nom_Fidelite != "" && Client.Nom_Fidelite != null)
                {
                    Console.WriteLine("Vous avez le grade fidélité " + Client.Nom_Fidelite + " qui vous donne une réduction comprise de " + Client.GetFidelityReduc() + "%");
                }
                Console.WriteLine("Que voulez-vous commander?");
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        if (options[i][0][0] != "Personaliser")
                        {
                            Console.WriteLine(options[i][0][0] + " : " + Convert.ToDouble(options[i][1][0]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + " euros x " + nombre + "\t\t\tQuantité Dispo : " + Convert.ToString(Convert.ToInt32(options[i][options[i].Count - 1][0]) - nombre));
                        }
                        else
                        {
                            Console.WriteLine(options[i][0][0]);
                        }
                    }
                    else
                    {
                        if (options[i][0][0] != "Personaliser")
                        {
                            Console.WriteLine(options[i][0][0] + " : " + Convert.ToDouble(options[i][1][0]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + "\t\t\t\t\tQuantité Dispo : " + options[i][options[i].Count - 1][0]);
                        }
                        else
                        {
                            Console.WriteLine(options[i][0][0]);
                        }
                    }
                    Console.ResetColor();
                }

                // Récupération de la touche appuyée par l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Traitement de la touche appuyée
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    nombre = 0;
                    selectedOption = (selectedOption == 0) ? options.Count - 1 : selectedOption - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    nombre = 0;
                    selectedOption = (selectedOption == options.Count - 1) ? 0 : selectedOption + 1;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow && Convert.ToInt32(options[selectedOption][options[selectedOption].Count - 1][0]) > nombre && nombre < 1)
                {
                    nombre += 1;
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow && nombre > 0)
                {
                    nombre -= 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter && (nombre > 0 || selectedOption == options.Count - 1))
                {
                    if (selectedOption == options.Count - 1)
                    {
                        Console.Write("\n\nVeuilliez décrire ce que vous souhaitez : ");
                        string description = Console.ReadLine();
                        Console.Write("\n\nQuel est votre prix maximum? : ");
                        double price = Convert.ToDouble(Console.ReadLine());
                        string delivery = AskDeliveryDate();

                        Bouquet_Personalise b = new Bouquet_Personalise(price, description);
                        Commande C = new Commande(type: "Personalisée", dateLivraison: delivery, prixTotal:Convert.ToDouble(price) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100), client: Client, id_Magasin: idMagasin, id_Bouquet_Personalise: b.Id);
                        Client.UpdateFidelity();

                        Console.Clear();
                        Console.WriteLine("Commande effectuée!, appuyez sur une touche pour revenir au menu");
                        Console.ReadKey();
                        return Menu();
                    }
                    else
                    {
                        string delivery = AskDeliveryDate();
                        List<string[]> compo = new List<string[]>();
                        for (int k = 3; k < options[selectedOption].Count - 1; k++)
                        {
                            compo.Add(options[selectedOption][k]);
                        }
                        Commande C = new Commande(type: "Standart", dateLivraison: delivery, prixTotal: Convert.ToDouble(options[selectedOption][1][0]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100), client: Client, id_Magasin: idMagasin, nom_Bouquet_Standart: options[selectedOption][0][0], composition: compo);

                        Client.UpdateFidelity();
                        Console.Clear();
                        Console.WriteLine("Commande effectuée!, appuyez sur une touche pour revenir au menu");
                        Console.ReadKey();
                        return Menu();
                    }
                }
            }
        }
        static int AccessoryShop(int idMagasin, ProjetBDD.Client Client)
        {
            List<string[]> options = GetProducts("accessoire", idMagasin);
            Dictionary<string, double[]> panier = new Dictionary<string, double[]>();

            int selectedOption = 0;
            int nombre = 0;
            // Affichage du menu déroulant
            while (true)
            {
                Console.Clear();
                if (Client.Nom_Fidelite != "" && Client.Nom_Fidelite != null)
                {
                    Console.WriteLine("Vous avez le grade fidélité " + Client.Nom_Fidelite + " qui vous donne une réduction totale de " + Client.GetFidelityReduc() + "%");
                }
                Console.WriteLine("Que voulez-vous commander?");
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine(options[i][0] + " : " + Convert.ToDouble(options[i][1]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + " euros x " + nombre + "\t\t\tQuantité Dispo : " + Convert.ToString(Convert.ToInt32(options[i][2]) - nombre));
                    }
                    else
                    {
                        Console.WriteLine(options[i][0] + " : " + Convert.ToDouble(options[i][1]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + "\t\t\t\t\tQuantité Dispo : " + options[i][2]);

                    }
                    Console.ResetColor();
                }

                Console.WriteLine("\nPanier actuel : ");
                double prixTotal = 0;

                foreach (KeyValuePair<string, double[]> element in panier)
                {
                    prixTotal += Convert.ToDouble(element.Value[0]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) * element.Value[1];

                    // Afficher la clé
                    Console.WriteLine("Nom : " + element.Key + ", prix : " + Convert.ToDouble(element.Value[0]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + " euros, quantité : " + element.Value[1]);
                }
                Console.WriteLine("Prix total du panier : " + prixTotal + " euros");
                Console.WriteLine("Appuyez sur espace pour valider le panier");

                // Récupération de la touche appuyée par l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Traitement de la touche appuyée
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    nombre = 0;
                    selectedOption = (selectedOption == 0) ? options.Count - 1 : selectedOption - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    nombre = 0;
                    selectedOption = (selectedOption == options.Count - 1) ? 0 : selectedOption + 1;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow && Convert.ToInt32(options[selectedOption][2]) > nombre)
                {
                    nombre += 1;
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow && nombre > 0)
                {
                    nombre -= 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter && nombre > 0)
                {
                    if (!panier.ContainsKey(options[selectedOption][0]))
                    {
                        double[] toAdd = new double[2];
                        toAdd[0] = Convert.ToDouble(options[selectedOption][1]);
                        toAdd[1] = Convert.ToDouble(nombre);
                        panier.Add(options[selectedOption][0], toAdd);
                    }
                    else
                    {
                        panier[options[selectedOption][0]][1] += nombre;
                    }
                    int newQuantity = Convert.ToInt32(options[selectedOption][2]) - nombre;
                    options[selectedOption][2] = newQuantity.ToString();
                }
                else if (keyInfo.Key == ConsoleKey.Spacebar && panier.Count > 0)
                {
                    string delivery = AskDeliveryDate();
                    double price = 0;
                    List<string[]> compo = new List<string[]>();
                    foreach (KeyValuePair<string, double[]> element in panier)
                    {
                        compo.Add(new string[] { element.Key, element.Value[1].ToString() });
                        price += element.Value[1] * element.Value[0];

                    }
                    Console.WriteLine(Client.Id);
                    Commande C = new Commande(type: "Personalisée", dateLivraison: delivery, prixTotal: Convert.ToDouble(price) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100), client: Client, id_Magasin: idMagasin, composition: compo);
                    Client.UpdateFidelity();

                    Console.Clear();
                    Console.WriteLine("Commande effectuée!, appuyez sur une touche pour revenir au menu");
                    Console.ReadKey();
                    return Menu();
                }
            }
        }
        static int FlowerShop(int idMagasin, ProjetBDD.Client Client)
        {

            List<string[]> options = GetProducts("fleur", idMagasin);
            Dictionary<string, double[]> panier = new Dictionary<string, double[]>();

            int selectedOption = 0;
            int nombre = 0;
            // Affichage du menu déroulant
            while (true)
            {
                Console.Clear();
                if (Client.Nom_Fidelite != "")
                {
                    Console.WriteLine("Vous avez le grade fidélité " + Client.Nom_Fidelite + " qui vous donne une réduction (comprise) de " + Client.GetFidelityReduc() + "%");
                }
                Console.WriteLine("Que voulez-vous commander?");
                for (int i = 0; i < options.Count; i++)
                {
                    if (i == selectedOption)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine(options[i][0] + " : " + Convert.ToDouble(options[i][1]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + " euros x " + nombre + "\t\t\tQuantité Dispo : " + Convert.ToString(Convert.ToInt32(options[i][2]) - nombre));
                    }
                    else
                    {
                        Console.WriteLine(options[i][0] + " : " + Convert.ToDouble(options[i][1]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + "\t\t\t\t\tQuantité Dispo : " + options[i][2]);

                    }
                    Console.ResetColor();
                }

                Console.WriteLine("\nPanier actuel : ");
                double prixTotal = 0;

                foreach (KeyValuePair<string, double[]> element in panier)
                {
                    prixTotal += Convert.ToDouble(element.Value[0]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) * element.Value[1];

                    // Afficher la clé
                    Console.WriteLine("Nom : " + element.Key + ", prix : " + Convert.ToDouble(element.Value[0]) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100) + " euros, quantité : " + element.Value[1]);
                }
                Console.WriteLine("Prix total du panier : " + prixTotal + " euros");
                Console.WriteLine("Appuyez sur espace pour valider le panier");

                // Récupération de la touche appuyée par l'utilisateur
                ConsoleKeyInfo keyInfo = Console.ReadKey();

                // Traitement de la touche appuyée
                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    nombre = 0;
                    selectedOption = (selectedOption == 0) ? options.Count - 1 : selectedOption - 1;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    nombre = 0;
                    selectedOption = (selectedOption == options.Count - 1) ? 0 : selectedOption + 1;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow && Convert.ToInt32(options[selectedOption][2]) > nombre)
                {
                    nombre += 1;
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow && nombre > 0)
                {
                    nombre -= 1;
                }
                else if (keyInfo.Key == ConsoleKey.Enter && nombre > 0)
                {
                    if (!panier.ContainsKey(options[selectedOption][0]))
                    {
                        double[] toAdd = new double[2];
                        toAdd[0] = Convert.ToDouble(options[selectedOption][1]);
                        toAdd[1] = Convert.ToDouble(nombre);
                        panier.Add(options[selectedOption][0], toAdd);
                    }
                    else
                    {
                        panier[options[selectedOption][0]][1] += nombre;
                    }
                    int newQuantity = Convert.ToInt32(options[selectedOption][2]) - nombre;
                    options[selectedOption][2] = newQuantity.ToString();
                }
                else if (keyInfo.Key == ConsoleKey.Spacebar && panier.Count > 0)
                {
                    string delivery = AskDeliveryDate();
                    double price = 0;
                    List<string[]> compo = new List<string[]>();
                    foreach (KeyValuePair<string, double[]> element in panier)
                    {
                        compo.Add(new string[] { element.Key, element.Value[1].ToString() });
                        price += element.Value[1] * element.Value[0];

                    }
                    Console.WriteLine(Client.Id);
                    Commande C = new Commande(type: "Personalisée", dateLivraison: delivery, prixTotal: Convert.ToDouble(price) * ((100 - Convert.ToDouble(Client.GetFidelityReduc())) / 100), client: Client, id_Magasin: idMagasin, composition: compo);
                    Client.UpdateFidelity();

                    Console.Clear();
                    Console.WriteLine("Commande effectuée!, appuyez sur une touche pour revenir au menu");
                    Console.ReadKey();
                    return Menu();

                }
            }
        }
        static List<List<string[]>> GetBunchs(int idMagasin)
        {
         
            List<List<string[]>> toReturn = new List<List<string[]>>();

            List<string[]> products = GetProducts("", idMagasin);

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string query = "SELECT bouquet_standart.nom, bouquet_standart.prix, bouquet_standart.description, constitue_de.quantite, constitue_de.nom_produit FROM bouquet_standart NATURAL JOIN constitue_de WHERE constitue_de.nom_Bouquet_Standart = bouquet_standart.nom;";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                bool isInside = false;
                for (int i = 0; i < toReturn.Count; i++)
                {
                    if (toReturn[i][0][0] == reader.GetString("nom"))
                    {
                        isInside = true;
                        toReturn[i].Add(new string[] { reader.GetString("nom_produit"), reader.GetString("quantite") });
                    }
                }
                if (!isInside)
                {
                    List<string[]> test = new List<string[]>();
                    test.Add(new string[] { reader.GetString("nom") });
                    test.Add(new string[] { reader.GetString("prix") });
                    test.Add(new string[] { reader.GetString("description") });
                    test.Add(new string[] { reader.GetString("nom_produit"), reader.GetString("quantite") });
                    toReturn.Add(test);
                }
            }
            for (int i = 0; i < toReturn.Count; i++)
            {
                Console.WriteLine(i);
                int minimumQuantity = 999999;
                for (int j = 0; j < products.Count; j++)
                {
                    for (int k = 3; k < toReturn[i].Count; k++)
                    {
                        if (toReturn[i][k][0] == products[j][0])
                        {
                            minimumQuantity = Math.Min(minimumQuantity, Convert.ToInt32(products[j][2]) / Convert.ToInt32(toReturn[i][k][1]));
                        }
                    }
                }
                toReturn[i].Add(new string[] { minimumQuantity.ToString() });
            }
            return toReturn;
        }
        static List<string[]> GetProducts(string type, int idMagasin)
        {
            // Connexion à la base de données MySQL
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string query;
            // Requête SELECT pour récupérer les produits dont le type est spécifié
            if (type != "")
            {
                query = "SELECT produit.nom, produit.prix, possede.quantite FROM produit NATURAL JOIN possede WHERE type = '" + type + "' and id_Magasin = " + idMagasin + " and nom_produit = nom;";
            }
            else
            {
                query = "SELECT produit.nom, produit.prix, possede.quantite FROM produit NATURAL JOIN possede WHERE id_Magasin = " + idMagasin + " and nom_produit = nom;";
            }
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            // Récupération des noms et des prix des produits dans une liste de tableaux
            List<string[]> productsList = new List<string[]>();
            while (reader.Read())
            {
                string[] productData = new string[3];
                productData[0] = reader.GetString("nom");
                productData[1] = reader.GetDecimal("prix").ToString();
                productData[2] = reader.GetInt32("quantite").ToString();
                productsList.Add(productData);
            }

            // Fermeture de la connexion et retour de la liste des produits
            reader.Close();
            connection.Close();
            return productsList;
        }
        static List<string[]> GetShops()
        {
            // Connexion à la base de données MySQL
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            // Requête SELECT pour récupérer les produits dont le type est spécifié
            string query = "SELECT * FROM magasin";
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            // Récupération des noms et des prix des produits dans une liste de tableaux
            List<string[]> productsList = new List<string[]>();
            while (reader.Read())
            {
                string[] productData = new string[2];
                productData[0] = reader.GetInt32("id").ToString();
                productData[1] = reader.GetString("adresse");

                productsList.Add(productData);
            }

            // Fermeture de la connexion et retour de la liste des produits
            reader.Close();
            connection.Close();
            return productsList;
        }
        static string AskDeliveryDate()
        {
            DateTime dateMin = DateTime.Today.AddDays(2); // Date minimale de livraison (2 jours à partir de la date actuelle)
            DateTime deliveryDate = dateMin.AddDays(1); // Initialisation à une date par défaut (la date minimale de livraison plus un jour)

            bool isValidDate = false;
            while (!isValidDate)
            {
                Console.Write("Quand voulez-vous recevoir le colis? (Format: jour/mois/année) ");
                string deliveryDateString = Console.ReadLine();

                if (DateTime.TryParse(deliveryDateString, out DateTime date))
                {
                    if (date >= dateMin)
                    {
                        deliveryDate = date;
                        isValidDate = true;
                    }
                    else
                    {
                        Console.WriteLine("La date doit être au minimum 2 jours plus tard que la date actuelle.");
                    }
                }
                else
                {
                    Console.WriteLine("Le format de date n'est pas valide. Veuillez réessayer.");
                }
            }
            return deliveryDate.ToShortDateString();
        }
    }
}
