#Ma base de donnees : 

DROP TABLE IF EXISTS Commande;
CREATE TABLE Commande(
        id     Int,
        type     Varchar (25),
        dateCommande     Int,
        dateLivraison     Int,
        adresseLivraison     Varchar (100),
        prixTotal     Int,
        id_Client     Int,
        id_Magasin     Int,
        nom_Bouquet_Standart     Varchar (25),
        code_Etat_Commande     Varchar (25),
        id_Bouquet_Personalise     Int,
        PRIMARY KEY (id)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Client;
CREATE TABLE Client(
        id     Int,
        nom     Varchar (40),
        prenom     Varchar (40),
        email     Varchar (40),
        motDePasse     Varchar (40),
        numeroTelephone     Varchar (10),
        adresseFacturation     Varchar (100),
        carteCredit     Varchar (16),
        nom_Fidelite     Varchar (25),
        PRIMARY KEY (id)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Produit;
CREATE TABLE Produit(
        nom     Varchar (40),
        prix     Float,
        type     Varchar (40),
        id_Disponibilite_Restreinte     Int,
        PRIMARY KEY (nom)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Fidelite;
CREATE TABLE Fidelite(
        nom     Varchar (25),
        reduction     Int,
        PRIMARY KEY (nom)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Magasin;
CREATE TABLE Magasin(
        id     Int,
        adresse     Varchar (100),
        PRIMARY KEY (id)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Disponibilite_Restreinte;
CREATE TABLE Disponibilite_Restreinte(
        id     Int,
        dateDebut     Int,
        dateFin     Int,
        PRIMARY KEY (id)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Bouquet_Standart;
CREATE TABLE Bouquet_Standart(
        nom     Varchar (25),
        prix     Int,
        description     Varchar (100),
        PRIMARY KEY (nom)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Etat_Commande;
CREATE TABLE Etat_Commande(
        code     Varchar (25),
        description     Varchar (200),
        PRIMARY KEY (code)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Bouquet_Personalise;
CREATE TABLE Bouquet_Personalise(
        id     Int,
        prix     Int,
        description     Varchar (400),
        PRIMARY KEY (id)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Constitue_de;
CREATE TABLE Constitue_de(
        quantite     Int,
        nom_Produit     Varchar (40),
        nom_Bouquet_Standart     Varchar (25),
        PRIMARY KEY (nom_Produit,nom_Bouquet_Standart)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Possede;
CREATE TABLE Possede(
        quantite     Int,
        id_Magasin     Int,
        nom_Produit     Varchar (40),
        PRIMARY KEY (id_Magasin,nom_Produit)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Concerne2;
CREATE TABLE Concerne2(
        quantite     Int,
        id_Commande     Int,
        nom_Produit     Varchar (40),
        PRIMARY KEY (id_Commande,nom_Produit)
)ENGINE=InnoDB;



DROP TABLE IF EXISTS Constitue_de2;
CREATE TABLE Constitue_de2(
        quantite     Int,
        id_Bouquet_Personalise     Int,
        nom_Produit     Varchar (40),
        PRIMARY KEY (id_Bouquet_Personalise,nom_Produit)
)ENGINE=InnoDB;



ALTER TABLE Commande ADD CONSTRAINT FK_Commande_id_Client FOREIGN KEY (id_Client) REFERENCES Client(id);
ALTER TABLE Commande ADD CONSTRAINT FK_Commande_id_Magasin FOREIGN KEY (id_Magasin) REFERENCES Magasin(id);
ALTER TABLE Commande ADD CONSTRAINT FK_Commande_nom_Bouquet_Standart FOREIGN KEY (nom_Bouquet_Standart) REFERENCES Bouquet_Standart(nom);
ALTER TABLE Commande ADD CONSTRAINT FK_Commande_code_Etat_Commande FOREIGN KEY (code_Etat_Commande) REFERENCES Etat_Commande(code);
ALTER TABLE Commande ADD CONSTRAINT FK_Commande_id_Bouquet_Personalise FOREIGN KEY (id_Bouquet_Personalise) REFERENCES Bouquet_Personalise(id);
ALTER TABLE Client ADD CONSTRAINT FK_Client_nom_Fidelite FOREIGN KEY (nom_Fidelite) REFERENCES Fidelite(nom);
ALTER TABLE Produit ADD CONSTRAINT FK_Produit_id_Disponibilite_Restreinte FOREIGN KEY (id_Disponibilite_Restreinte) REFERENCES Disponibilite_Restreinte(id);
ALTER TABLE Constitue_de ADD CONSTRAINT FK_Constitue_de_nom_Produit FOREIGN KEY (nom_Produit) REFERENCES Produit(nom);
ALTER TABLE Constitue_de ADD CONSTRAINT FK_Constitue_de_nom_Bouquet_Standart FOREIGN KEY (nom_Bouquet_Standart) REFERENCES Bouquet_Standart(nom);
ALTER TABLE Possede ADD CONSTRAINT FK_Possede_id_Magasin FOREIGN KEY (id_Magasin) REFERENCES Magasin(id);
ALTER TABLE Possede ADD CONSTRAINT FK_Possede_nom_Produit FOREIGN KEY (nom_Produit) REFERENCES Produit(nom);
ALTER TABLE Concerne2 ADD CONSTRAINT FK_Concerne2_id_Commande FOREIGN KEY (id_Commande) REFERENCES Commande(id);
ALTER TABLE Concerne2 ADD CONSTRAINT FK_Concerne2_nom_Produit FOREIGN KEY (nom_Produit) REFERENCES Produit(nom);
ALTER TABLE Constitue_de2 ADD CONSTRAINT FK_Constitue_de2_id_Bouquet_Personalise FOREIGN KEY (id_Bouquet_Personalise) REFERENCES Bouquet_Personalise(id);
ALTER TABLE Constitue_de2 ADD CONSTRAINT FK_Constitue_de2_nom_Produit FOREIGN KEY (nom_Produit) REFERENCES Produit(nom);


INSERT INTO `fleur`.`fidelite` (`nom`, `reduction`) VALUES ('Bronze', 5);
INSERT INTO `fleur`.`fidelite` (`nom`, `reduction`) VALUES ('Or', 15);

INSERT INTO `fleur`.`magasin` (`id`, `adresse`) VALUES (1, "10 avenue leonard de vinci 75001 Paris");
INSERT INTO `fleur`.`magasin` (`id`, `adresse`) VALUES (2, "4 rue de la gare 92300 Levallois");
INSERT INTO `fleur`.`magasin` (`id`, `adresse`) VALUES (3, "15 avenue des lilas d\'Espagne 92400 Courbevoie");

INSERT INTO `fleur`.`disponibilite_restreinte` (`id`, `dateDebut`, `dateFin`) VALUES (1, '0000-05-01', '0000-11-30');

INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Gerbera', 5.00, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Genet', 5.00, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Ginger', 4.00, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Lys', 4.00, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Alstromeria', 3.00, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Orchidee', 3.50, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Glaïeul', 1.00, 'fleur', 1);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Marguerite', 2.25, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Rose rouge', 2.50, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Rose blanche', 2.50, 'fleur', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Vase', 10, 'accessoire', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Boite', 5, 'accessoire', null);
INSERT INTO `fleur`.`produit` (`nom`, `prix`, `type`, `id_Disponibilite_Restreinte`) VALUES ('Ruban', 2, 'accessoire', null);



INSERT INTO `fleur`.`bouquet_standart` (`nom`, `prix`, `description`) VALUES ('Gros Merci', 45, 'Arrangement floral avec marguerites et verdure');
INSERT INTO `fleur`.`bouquet_standart` (`nom`, `prix`, `description`) VALUES ('L\'amoureux', 65, 'Arrangement floral avec roses blanches et roses rouges');
INSERT INTO `fleur`.`bouquet_standart` (`nom`, `prix`, `description`) VALUES ('L\'exotique', 40, 'Arrangement floral avec ginger, oiseaux du paradis, roses et genet');
INSERT INTO `fleur`.`bouquet_standart` (`nom`, `prix`, `description`) VALUES ('Maman', 80, 'Arrangement floral avec gerbera, roses blanches, lys et alstromeria');
INSERT INTO `fleur`.`bouquet_standart` (`nom`, `prix`, `description`) VALUES ('Vive la mariee', 120, 'Arrangement floral avec lys et orchidees');



INSERT INTO `fleur`.`etat_commande` (`code`, `description`) VALUES ('VINV', 'Commande standard pour laquelle un employe doit verifier l’inventaire');
INSERT INTO `fleur`.`etat_commande` (`code`, `description`) VALUES ('CC', 'Commande complète. Tous les items de la commande ont ete indiques et tous ces items se trouvent en stock.');
INSERT INTO `fleur`.`etat_commande` (`code`, `description`) VALUES ('CPAV', 'Commande personnalisee à verifier.');
INSERT INTO `fleur`.`etat_commande` (`code`, `description`) VALUES ('CAL', 'Commande à livrer. La commande est prête !');
INSERT INTO `fleur`.`etat_commande` (`code`, `description`) VALUES ('CL', 'Commande livree. La commande a ete livree à l’adresse indiquee par le client.');

INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Gerbera');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Genet');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Ginger');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Glaïeul');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Lys');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Orchidee');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Alstromeria');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Marguerite');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Rose rouge');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Rose blanche');

INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Vase');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Boïte');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 1, 'Ruban');

INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Gerbera');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Genet');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Ginger');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Glaïeul');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Marguerite');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Rose rouge');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Rose blanche');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Lys');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Orchidee');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Alstromeria');

INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Vase');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Boïte');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 2, 'Ruban');

INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Gerbera');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Genet');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Ginger');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Glaïeul');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Marguerite');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Rose rouge');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Rose blanche');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Lys');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Orchidee');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Alstromeria');

INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Vase');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Boïte');
INSERT INTO `fleur`.`possede` (`quantite`, `id_Magasin`, `nom_Produit`) VALUES (100, 3, 'Ruban');

INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (10, 'Marguerite', 'Gros Merci');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (10, 'Vase', 'Gros Merci');

INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Rose blanche', 'L\'amoureux');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Rose rouge', 'L\'amoureux');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Vase', 'L\'amoureux');

INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (3, 'Ginger', 'L\'exotique');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (3, 'Rose rouge', 'L\'exotique');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (3, 'Rose blanche', 'L\'exotique');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (3, 'Genet', 'L\'exotique');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (3, 'Vase', 'L\'exotique');

INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Rose blanche', 'Maman');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Gerbera', 'Maman');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Lys', 'Maman');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Alstromeria', 'Maman');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (5, 'Vase', 'Maman');

INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (15, 'Lys', 'Vive la mariee');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (15, 'Orchidee', 'Vive la mariee');
INSERT INTO `fleur`.`constitue_de` (`quantite`, `nom_Produit`, `nom_Bouquet_Standart`) VALUES (15, 'Vase', 'Vive la mariee');

INSERT INTO `fleur`.`bouquet_personalise` (`id`, `prix`, `description`) VALUES (0, null, null);
select * from constitue_de;
select * from produit;
select * from possede;
select * from bouquet_personalise;

select * from commande;

select * from client;

SELECT bouquet_standart.nom, bouquet_standart.prix, bouquet_standart.description, constitue_de.quantite, constitue_de.nom_produit FROM bouquet_standart NATURAL JOIN constitue_de WHERE constitue_de.nom_Bouquet_Standart = bouquet_standart.nom;