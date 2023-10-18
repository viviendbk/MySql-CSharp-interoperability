using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjetBDD
{
    internal class Commande
    {
        int id;
        string type;
        string dateCommande;
        string dateLivraison;
        double prixTotal;
        Client client;
        int id_Magasin;
        string nom_Bouquet_Standart;
        int id_Bouquet_Personalise;
        string code_Etat_Commande;
        string[] composition;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public Client Client
        {
            get { return client; }
            set { client = value; }
        }

        public int Id_Bouquet_Personalise
        {
            get { return id_Bouquet_Personalise; }
            set { id_Bouquet_Personalise = value; }
        }
        public int Id_Magasin
        {
            get { return id_Magasin; }
            set { id_Magasin = value; }
        }
        public string Code_Etat_Commande
        {
            get { return code_Etat_Commande; }
            set { code_Etat_Commande = value;
                using (MySqlConnection connection = new MySqlConnection("SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;"))
                {
                    connection.Open();

                    string sql = "UPDATE Commande SET code_Etat_Commande = @newState WHERE id = @commandeId";

                    using (MySqlCommand command = new MySqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@newState", value);
                        command.Parameters.AddWithValue("@commandeId", this.id);

                        int rowsAffected = command.ExecuteNonQuery();
                    }
                }
            }
        }
        public Commande(int id)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT * FROM Commande WHERE id = @idCommande";

                MySqlCommand command = new MySqlCommand(sql, connection);
                command.Parameters.AddWithValue("@idCommande", id);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        this.id = reader.GetInt32("id");
                        this.type = reader.GetString("type");
                        this.dateCommande = reader.GetDateTime("dateCommande").ToString();
                        this.dateLivraison = reader.GetDateTime("dateLivraison").ToString();
                        this.prixTotal = Convert.ToDouble(reader.GetDecimal("prixTotal"));
                        this.client = new Client(reader.GetInt32("id_Client"));
                        this.id_Magasin = reader.GetInt32("id_Magasin");
                        if (!reader.IsDBNull(reader.GetOrdinal("nom_Bouquet_Standart")))
                        {
                            this.nom_Bouquet_Standart = reader.GetString("nom_Bouquet_Standart");
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("code_Etat_Commande")))
                        {
                            this.code_Etat_Commande = reader.GetString("code_Etat_Commande");
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("id_Bouquet_Personalise")))
                        {
                            this.id_Bouquet_Personalise = reader.GetInt32("id_Bouquet_Personalise");
                        }
                    }
                }
            }
        }

        public Commande(string type, string dateLivraison, double prixTotal, Client client, int id_Magasin, string nom_Bouquet_Standart = null, int id_Bouquet_Personalise = 0, List<string[]> composition = null)
        {
            this.id = GenerateRandomId();
            this.type = type;
            DateTime today = DateTime.Today;
            this.dateCommande = today.ToString("dd/MM/yyyy");
            this.dateLivraison = dateLivraison;
            this.prixTotal = prixTotal;
            this.client = client;
            this.id_Magasin = id_Magasin;
            this.nom_Bouquet_Standart = nom_Bouquet_Standart;
            this.id_Bouquet_Personalise = id_Bouquet_Personalise;
            if (type == "Personalisée" && composition == null && id_Bouquet_Personalise > 0 && nom_Bouquet_Standart == null)
            {
                this.code_Etat_Commande = "CPAV";
                InsertCommand();
                // Commande bouquet personalisé
            }
            else if (type == "Personalisée" && nom_Bouquet_Standart == null && id_Bouquet_Personalise == 0 && composition != null)
            {
                this.code_Etat_Commande = "CPAV";
                InsertCommand();
                for (int i = 0; i < composition.Count; i++)
                {
                    Console.WriteLine("\n" + composition[i][0]);
                    Console.WriteLine(composition[i][1]);

                    InsertConcern(this.id, composition[i][0], Convert.ToInt32(composition[i][1]));
                    UpdateQuantite(id_Magasin, composition[i][0], Convert.ToInt32(composition[i][1]));
                }
                // Commande fleur à l'unité ou accessoire
            }
            else if (type == "Standart" && id_Bouquet_Personalise == 0 && nom_Bouquet_Standart != null)
            {
                this.code_Etat_Commande = "VINV";
                InsertCommand();
                for (int i = 0; i < composition.Count; i++)
                {
                    UpdateQuantite(id_Magasin, composition[i][0], Convert.ToInt32(composition[i][1]));
                }
                // Commande bouquet standart
            }
        }


        int GenerateRandomId()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=loueurd;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            // Génération d'un ID aléatoire de 6 chiffres
            Random random = new Random();
            int id = random.Next(100000, 999999);

            // Vérification que l'ID n'est pas déjà présent dans la base de données
            List<int> ids = GetClientIds();
            while (ids.Contains(id))
            {
                id = random.Next(100000, 999999);
            }

            // Fermeture de la connexion à la base de données
            connection.Close();

            // Retourne l'ID aléatoire
            return id;
        }
        List<int> GetClientIds()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            // Requête SQL pour sélectionner toutes les données de la table Client
            string query = "SELECT id FROM Commande";

            // Exécution de la requête SQL
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            // Création d'une liste pour stocker les ID
            List<int> ids = new List<int>();

            // Ajout des ID à la liste
            while (reader.Read())
            {
                ids.Add(reader.GetInt32(0));
            }

            // Fermeture du lecteur et de la connexion à la base de données
            reader.Close();
            connection.Close();

            // Retourne la liste des ID
            return ids;
        }
        
        public static void InsertConcern(int id, string name, int quantity)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO concerne2 (quantite, id_Commande, nom_Produit) VALUES (@quantite, @id_Commande, @nom_Produit)", connection);
            
            cmd.Parameters.AddWithValue("@quantite", quantity);
            cmd.Parameters.AddWithValue("@id_Commande", id);
            cmd.Parameters.AddWithValue("@nom_Produit", name);

            cmd.ExecuteNonQuery();

            connection.Close();
        }
        void InsertCommand()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO Commande (id, type, dateCommande, dateLivraison, adresseLivraison, prixTotal, id_Client, id_Magasin, nom_Bouquet_Standart, id_Bouquet_Personalise, code_Etat_Commande) VALUES (@id, @type, @dateCommande, @dateLivraison, @adresseLivraison, @prixTotal, @id_Client, @id_Magasin, @nom_Bouquet_Standart, @id_Bouquet_Personalise, @code_Etat_Commande)", connection);

            Console.WriteLine(id);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@dateCommande", dateCommande.Split("/")[2] + dateCommande.Split("/")[1] + dateCommande.Split("/")[0]);
            cmd.Parameters.AddWithValue("@dateLivraison", dateLivraison.Split("/")[2] + dateLivraison.Split("/")[1] + dateLivraison.Split("/")[0]);
            cmd.Parameters.AddWithValue("@adresseLivraison", client.AdresseFacturation);
            cmd.Parameters.AddWithValue("@prixTotal", prixTotal);
            cmd.Parameters.AddWithValue("@id_Client", client.Id);
            cmd.Parameters.AddWithValue("@id_Magasin", id_Magasin);
            cmd.Parameters.AddWithValue("@nom_Bouquet_Standart", nom_Bouquet_Standart);
            cmd.Parameters.AddWithValue("@id_Bouquet_Personalise", id_Bouquet_Personalise);

            cmd.Parameters.AddWithValue("@code_Etat_Commande", code_Etat_Commande);

            cmd.ExecuteNonQuery();

            connection.Close();
        }
        public static void UpdateQuantite(int idMagasin, string nomProduit, int toWithdraw)
        {
            // Connexion à la base de données
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Création de la requête SQL pour mettre à jour la quantité du produit
                string updateQuery = "UPDATE possede SET quantite = quantite - @toWithdraw WHERE id_Magasin = @idMagasin AND nom_Produit = @nomProduit";

                // Création de l'objet Command avec la requête SQL et les paramètres
                using (MySqlCommand command = new MySqlCommand(updateQuery, connection))
                {
                    command.Parameters.AddWithValue("@toWithdraw", toWithdraw);
                    command.Parameters.AddWithValue("@idMagasin", idMagasin);
                    command.Parameters.AddWithValue("@nomProduit", nomProduit);

                    // Exécution de la requête SQL
                    int rowsAffected = command.ExecuteNonQuery();
                }
            }
        }

        public static bool CodeStateExist(string code)
        {
            using (MySqlConnection connection = new MySqlConnection("SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;"))
            {
                connection.Open(); // Ouvrir la connexion à la base de données

                // Définir la requête SQL pour vérifier si le code est présent dans la colonne "code" de la table "Etat_Commande"
                string query = "SELECT COUNT(*) FROM Etat_Commande WHERE code = @code";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Ajouter le paramètre pour la valeur du code à rechercher
                    command.Parameters.AddWithValue("@code", code);

                    // Exécuter la requête SQL et obtenir le nombre de résultats correspondants
                    int count = Convert.ToInt32(command.ExecuteScalar());

                    // Si le nombre de résultats est supérieur à 0, le code est présent dans la table
                    if (count > 0)
                    {
                        return true;
                    }

                    // Sinon, le code n'est pas présent dans la table
                    return false;
                }
            }
        }


    }
}
