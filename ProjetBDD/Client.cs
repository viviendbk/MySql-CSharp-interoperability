using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Xml;
using Newtonsoft.Json;


namespace ProjetBDD
{
    internal class Client
    {
        int id;
        string nom;
        string prenom;
        string email;
        string motDePasse;
        string numeroTelephone;
        string adresseFacturation;
        string carteCredit;
        string nom_Fidelite;

        #region Propriétés
        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        public string Nom
        {
            get { return nom; }
            set { nom = value; }
        }

        public string Prenom
        {
            get { return prenom; }
            set { prenom = value; }
        }

        public string Email
        {
            get { return email; }
            set { email = value; }
        }

        public string MotDePasse
        {
            get { return motDePasse; }
            set { motDePasse = value; }
        }

        public string NumeroTelephone
        {
            get { return numeroTelephone; }
            set { numeroTelephone = value; }
        }

        public string AdresseFacturation
        {
            get { return adresseFacturation; }
            set { adresseFacturation = value; }
        }

        public string CarteCredit
        {
            get { return carteCredit; }
            set { carteCredit = value; }
        }

        public string Nom_Fidelite
        {
            get { return nom_Fidelite; }
            set { nom_Fidelite = value; }
        }
        #endregion

        #region Constructeurs
        public Client(string nom, string prenom, string email, string motDePasse, string numeroTelephone, string adresseFacturation, string carteCredit)
        {
            this.id = GenerateRandomId();
            this.nom = nom;
            this.prenom = prenom;
            this.email = email;
            this.motDePasse = motDePasse;
            this.numeroTelephone = numeroTelephone;
            this.adresseFacturation = adresseFacturation;
            this.carteCredit = carteCredit;
            this.nom_Fidelite = null;
            InsertClient();
        }
        public Client(int id)
        {
            this.id = id;
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();


            // Requête SQL pour sélectionner les données du client avec l'adresse e-mail et le mot de passe saisis
            string query = "SELECT * FROM Client WHERE id = "  + id;

            // Exécution de la requête SQL
            MySqlCommand command = new MySqlCommand(query, connection);
            MySqlDataReader reader = command.ExecuteReader();

            // Vérification que les informations sont présentes dans la base de données
            if (reader.HasRows)
            {
                Console.Clear();
                // Récupération des variables nom et prénom
                while (reader.Read())
                {
                    this.nom = reader.GetString("nom");
                    this.prenom = reader.GetString("prenom");
                    this.email = reader.GetString("email");
                    this.carteCredit = reader.GetString("carteCredit");
                    this.motDePasse = reader.GetString("motDePasse");
                    this.adresseFacturation = reader.GetString("adresseFacturation");
                    this.numeroTelephone = reader.GetString("numeroTelephone");
                    if (!reader.IsDBNull(reader.GetOrdinal("nom_Fidelite")))
                    {
                        this.nom_Fidelite = reader.GetString("nom_Fidelite");
                    }
                }
            }

            // Fermeture du lecteur et de la connexion à la base de données
            reader.Close();
            connection.Close();
        }
        #endregion
        void InsertClient()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO Client (id, nom, prenom, email, motDePasse, numeroTelephone, adresseFacturation, carteCredit, nom_Fidelite) VALUES (@id, @nom, @prenom, @email, @motDePasse, @numeroTelephone, @adresseFacturation, @carteCredit, @nom_Fidelite)", connection);

            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@nom", nom);
            cmd.Parameters.AddWithValue("@prenom", prenom);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@motDePasse", motDePasse);
            cmd.Parameters.AddWithValue("@numeroTelephone", numeroTelephone);
            cmd.Parameters.AddWithValue("@adresseFacturation", adresseFacturation);
            cmd.Parameters.AddWithValue("@carteCredit", carteCredit);
            cmd.Parameters.AddWithValue("@nom_Fidelite", nom_Fidelite);

            cmd.ExecuteNonQuery();

            connection.Close();
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
            string query = "SELECT id FROM Client";

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

        public void UpdateFidelity()
        {
            int nombreCommandes = 0;

            // Connexion à la base de données
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Requête SQL pour compter les commandes du client au cours des 30 derniers jours
                string query = "SELECT COUNT(*) FROM commande WHERE dateCommande >= DATE_SUB(NOW(), INTERVAL 30 DAY) AND id_Client = " + this.id;

                // Création de la commande SQL
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Exécution de la commande SQL et récupération du nombre de commandes
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        nombreCommandes = Convert.ToInt32(result);
                    }
                }
            }
            this.nom_Fidelite = new Fidelite(nombreCommandes).Nom;

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                string query = "UPDATE Client SET nom_fidelite = @newFidelityName WHERE id = @id";

                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@newFidelityName", this.nom_Fidelite);
                    command.Parameters.AddWithValue("@id", this.id);

                    int rowsAffected = command.ExecuteNonQuery();

                }
            }
        }

        public int GetFidelityReduc()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Requête SQL pour sélectionner la réduction de la fidélité du client
                string query = "SELECT reduction FROM fidelite WHERE nom = @nomFidelite";

                // Création de l'objet Command avec la requête SQL et les paramètres
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@nomFidelite", this.nom_Fidelite);

                    // Exécution de la requête SQL et récupération de la réduction de la fidélité
                    object result = command.ExecuteScalar();
                    if (result != null)
                    {
                        int reduction;
                        if (int.TryParse(result.ToString(), out reduction))
                        {
                            return reduction;
                        }
                    }
                }
            }

            // Si la réduction n'a pas pu être récupérée, renvoie 0
            return 0;
        }

        public static void DisplayAllClients()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Requête SQL pour récupérer les clients et leurs commandes avec le type de commande et les produits commandés
                string query = @"SELECT c.nom, c.prenom, c.nom_Fidelite, com.id, com.dateCommande, com.type,
                                    CONCAT_WS('', 
                                        IFNULL(bs.nom, ''),
                                        IF(com.id_Bouquet_Personalise IS NOT NULL, CONCAT('Bouquet personnalisé: ', bp.description), ''),
                                        (
                                            SELECT GROUP_CONCAT(CONCAT(p.nom, ' x', con.quantite) SEPARATOR ', ')
                                            FROM Concerne2 con
                                            INNER JOIN Produit p ON con.nom_Produit = p.nom
                                            WHERE con.id_Commande = com.id
                                        )
                                    ) AS produits,
                                    com.prixTotal
                                FROM Client c
                                INNER JOIN Commande com ON c.id = com.id_Client
                                LEFT JOIN Bouquet_Standart bs ON com.nom_Bouquet_Standart = bs.nom
                                LEFT JOIN Bouquet_Personalise bp ON com.id_Bouquet_Personalise = bp.id
                                GROUP BY c.nom, c.prenom, com.id, com.dateCommande, com.type, bs.nom, bp.description, com.prixTotal
                                ORDER BY c.nom, c.prenom, com.dateCommande, com.id;";

                // Création de la commande SQL
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    // Exécution de la commande SQL
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        string nomClient = null;
                        string prenomClient = null;
                        string nomFidelite = null;
                        while (reader.Read())
                        {
                            // Si on change de client, on affiche le nom et le prénom du nouveau client et ses commandes
                            if (nomClient != reader.GetString(0) || prenomClient != reader.GetString(1))
                            {
                                if (nomClient != null)
                                {
                                    Console.WriteLine("}");
                                }

                                nomClient = reader.GetString(0);
                                prenomClient = reader.GetString(1);
                                nomFidelite = reader.GetString(2);
                                Console.WriteLine($"Nom = {nomClient}, Prénom = {prenomClient}, Fidelité = {nomFidelite}, Commandes = {{");
                            }

                            // Affichage des détails de la commande
                            int idCommande = reader.GetInt32(3);
                            DateTime dateCommande = reader.GetDateTime(4);
                            string typeCommande = reader.IsDBNull(5) ? "" : reader.GetString(5);
                            string produitsCommande = reader.IsDBNull(6) ? "" : reader.GetString(6);
                            decimal prixTotalCommande = reader.IsDBNull(7) ? 0 : reader.GetDecimal(7);
                            Console.WriteLine($"    {{id = {idCommande}, date = {dateCommande.ToString("dd-MM-yyyy")}, type = {typeCommande}, Produits Achetés = {{ {produitsCommande} }}, Prix Total = {prixTotalCommande}}},");
                        }
                        Console.WriteLine("}");
                    }
                }
            }
        }
        public static void ExportXML()
        {
            // Chaîne de connexion à la base de données MySQL
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            // Exécution de la requête SQL pour récupérer les clients
            List<string> clients = new List<string>();
            string query = @"
            SELECT DISTINCT c.id, c.nom, c.prenom, c.email, c.nom_Fidelite
            FROM Client c
            JOIN Commande cmd ON c.id = cmd.id_Client
            WHERE cmd.dateCommande >= DATE_SUB(NOW(), INTERVAL 1 MONTH)
            GROUP BY c.id, c.nom, c.prenom, c.email, c.nom_Fidelite
            HAVING COUNT(DISTINCT cmd.id) > 1
        ";

            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    string client = $"<client id=\"{reader.GetInt32(0)}\"><nom>{reader.GetString(1)}</nom><prenom>{reader.GetString(2)}</prenom><email>{reader.GetString(3)}</email><nom_fidelite>{reader.GetString(4)}</nom_fidelite></client>";
                    clients.Add(client);
                }
                reader.Close();
            }

            // Création du document XML
            XmlDocument xmlDocument = new XmlDocument();
            XmlElement rootElement = xmlDocument.CreateElement("clients");
            foreach (string client in clients)
            {
                XmlElement clientElement = xmlDocument.CreateElement("client");
                clientElement.InnerXml = client;
                rootElement.AppendChild(clientElement);
            }
            xmlDocument.AppendChild(rootElement);

            // Export du document XML dans un fichier
            xmlDocument.Save("clients.xml");
        }

        public static void ExportJSON()
        {
            // Chaîne de connexion à la base de données MySQL
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            // Exécution de la requête SQL pour récupérer les clients
            List<object> clients = new List<object>();

            string query = @"
            SELECT DISTINCT c.id, c.nom, c.prenom, c.email, c.nom_Fidelite
            FROM Client c
            LEFT JOIN Commande cmd ON c.id = cmd.id_Client
            WHERE cmd.id IS NULL OR cmd.dateCommande < DATE_SUB(NOW(), INTERVAL 6 MONTH)";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            using (MySqlCommand command = new MySqlCommand(query, connection))
            {
                connection.Open();
                MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    object client = new
                    {
                        id = reader.GetInt32(0),
                        nom = reader.GetString(1),
                        prenom = reader.GetString(2),
                        email = reader.GetString(3),
                        nom_fidelite = reader.IsDBNull(4) ? null : reader.GetString(4)
                    };
                    clients.Add(client);
                }
                reader.Close();
            }

            // Conversion des résultats en format JSON
            string json = JsonConvert.SerializeObject(clients, Newtonsoft.Json.Formatting.Indented);

            // Export du document JSON dans un fichier
            System.IO.File.WriteAllText(@"clients.json", json);
        }

    }
}
