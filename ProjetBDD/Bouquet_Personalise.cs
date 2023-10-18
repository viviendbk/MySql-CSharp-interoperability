using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ProjetBDD
{
    internal class Bouquet_Personalise
    {
        int id;
        double prix;
        string description;

        public int Id
        {
            get { return this.id; }
        }
        public Bouquet_Personalise(double prix, string description)
        {
            this.id = GenerateRandomId();
            this.prix = prix;
            this.description = description;

            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                connection.Open();

                // Création de la requête SQL pour mettre à jour la quantité du produit
                string insertQuery = "INSERT INTO bouquet_personalise (id, prix, description) VALUES (@id, @prix, @description)";

                // Création de l'objet Command avec la requête SQL et les paramètres
                using (MySqlCommand command = new MySqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.Parameters.AddWithValue("@prix", prix);
                    command.Parameters.AddWithValue("@description", description);

                    // Exécution de la requête SQL
                    int rowsAffected = command.ExecuteNonQuery();
                }
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
            List<int> ids = GetBunchIds();
            while (ids.Contains(id))
            {
                id = random.Next(100000, 999999);
            }

            // Fermeture de la connexion à la base de données
            connection.Close();

            // Retourne l'ID aléatoire
            return id;
        }
        List<int> GetBunchIds()
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            // Requête SQL pour sélectionner toutes les données de la table Client
            string query = "SELECT id FROM bouquet_personalise";

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

        public static void Insert_Constitue_de2(int id_Bouquet, string nom_Produit, int quantity)
        {
            string connectionString = "SERVER=localhost;PORT=3306;DATABASE=fleur;UID=root;PASSWORD=root;";

            MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            MySqlCommand cmd = new MySqlCommand("INSERT INTO constitue_de2 (quantite, id_Bouquet_Personalise, nom_Produit) VALUES (@quantite, @id_Bouquet_Personalise, @nom_Produit)", connection);

            cmd.Parameters.AddWithValue("@quantite", quantity);
            cmd.Parameters.AddWithValue("@id_Bouquet_Personalise", id_Bouquet);
            cmd.Parameters.AddWithValue("@nom_Produit", nom_Produit);

            cmd.ExecuteNonQuery();

            connection.Close();
        }
    }
}
