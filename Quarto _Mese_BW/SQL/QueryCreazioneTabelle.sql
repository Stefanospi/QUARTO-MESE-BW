CREATE TABLE Categorie (
    CategoriaID INT IDENTITY PRIMARY KEY,
    NomeCategoria NVARCHAR(100) NOT NULL
);


CREATE TABLE Prodotti (
    ProductID INT IDENTITY PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Descrizione NVARCHAR(500) NOT NULL,
    Prezzo DECIMAL(18, 2) NOT NULL,
    ImmagineUrl NVARCHAR(255),
    Stock INT NOT NULL,
    CategoriaID INT FOREIGN KEY REFERENCES Categorie(CategoriaID)
);

CREATE TABLE Anagrafica (
    UserID INT IDENTITY PRIMARY KEY,
    Nome NVARCHAR(100) NOT NULL,
    Cognome NVARCHAR(100) NOT NULL,
    Email NVARCHAR(255) NOT NULL,
    Via NVARCHAR(255) NOT NULL,
    CAP NVARCHAR(10) NOT NULL,
    Città NVARCHAR(100) NOT NULL,
    Provincia NVARCHAR(100) NOT NULL,
    Telefono NVARCHAR(15)
);

CREATE TABLE Ordini (
    OrderID INT IDENTITY PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Anagrafica(UserID),
    DataOrdine DATETIME NOT NULL,
    Stato NVARCHAR(50) NOT NULL,
    Totale DECIMAL(18, 2) NOT NULL
);

CREATE TABLE ProdottiOrdine (
    ProdottiOrdineID INT IDENTITY PRIMARY KEY,
    OrderID INT FOREIGN KEY REFERENCES Ordini(OrderID),
    ProductID INT FOREIGN KEY REFERENCES Prodotti(ProductID),
    Quantità INT NOT NULL,
    PrezzoUnitario DECIMAL(18, 2) NOT NULL
);


CREATE TABLE Carrello (
    CartID INT IDENTITY PRIMARY KEY,
    UserID INT FOREIGN KEY REFERENCES Anagrafica(UserID),
    DataCreazione DATETIME NOT NULL
);


CREATE TABLE ProdottiCarrello (
    ProdottiCarrelloID INT IDENTITY PRIMARY KEY,
    CartID INT FOREIGN KEY REFERENCES Carrello(CartID),
    ProductID INT FOREIGN KEY REFERENCES Prodotti(ProductID),
    Quantità INT NOT NULL
);


INSERT INTO Categorie (NomeCategoria) VALUES ('Elettronica');
INSERT INTO Categorie (NomeCategoria) VALUES ('Libri');
INSERT INTO Categorie (NomeCategoria) VALUES ('Abbigliamento');

INSERT INTO Prodotti (Nome, Descrizione, Prezzo, ImmagineUrl, Stock, CategoriaID) VALUES 
('Smartphone', 'Un fantastico smartphone con tutte le funzionalità più recenti.', 599.99, '/img/smartphone.jpg', 50, 1),
('Libro', 'Un romanzo avvincente che ti terrà incollato alle pagine.', 19.99, '/img/libro.jpg', 100, 2),
('T-shirt', 'Una T-shirt di alta qualità disponibile in varie taglie.', 29.99, '/img/tshirt.jpg', 75, 3);

SELECT * FROM Categorie;

SELECT * FROM PRodotti;

INSERT INTO Categorie (NomeCategoria) VALUES ('Coppe');
INSERT INTO Categorie (NomeCategoria) VALUES ('Coppette');


INSERT INTO Prodotti (Nome, Descrizione, Prezzo, ImmagineUrl, Stock, CategoriaID)
VALUES
('SOUFFLÉ DUE LUNE', 'Soufflé* al cioccolato, gelato al fiordilatte, cioccolato croccante, topping al cioccolato e granella di nocciole', 6.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Extracioccolato')),
('BROWNIE', 'Torta* al cioccolato con noci pecan, accompagnata da gelato al fiordilatte, frutti di bosco* in salsa e topping al cioccolato', 7.50, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Extracioccolato')),
('CHEESECAKE CON NUTELLA', 'Cremosa Cheesecake, guarnita con Nutella (R) e granella di nocciole, con panna montata', 7.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Cheesecake')),
('CHEESECAKE FRUTTI DI BOSCO', 'Cremosa Cheesecake, guarnita con frutti di bosco* in salsa, con panna montata', 7.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Cheesecake')),
('CHEESECAKE PISTACCHIO', 'Cremosa Cheesecake, guarnita con crema al pistacchio, topping al cioccolato, granella di pistacchio, con panna montata', 7.50, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Cheesecake')),
('PANCAKE PISTACCHIO', 'Due soffici pancakes farciti con crema al pistacchio, topping al cioccolato e granella di pistacchio, con panna montata', 6.50, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Pancake')),
('PANCAKE CON NUTELLA®', 'Due soffici pancakes* farciti con Nutella® e gelato al fiordilatte, ricoperti da topping alle arachidi, caramello e arachidi, con panna montata', 6.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Pancake')),
('PANCAKE FRUTTI DI BOSCO', 'Due soffici pancakes* farciti con frutti di bosco* in salsa, sciroppo d acero, con panna montata', 5.50, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Pancake')),
('COPPA ALASKA', 'Gelato al fiordilatte con Nutella®, cioccolato croccante, panna montata, topping al cioccolato e granella di nocciole', 8.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Coppe')),
('COPPA CON KITKAT', 'Gelato al fiordilatte con crema e crumble KitKat®, panna montata e cacao', 9.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Coppe')),
('COPPA AL PISTACCHIO', 'Gelato al fiordilatte con crema al pistacchio, topping al cioccolato, crumble di biscotto Oreo®, panna montata, cacao e granella di pistacchio', 8.50, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Coppe')),
('COPPETTA BAILEYS', 'Gelato al cioccolato, Baileys® Irish Cream , panna montata, topping al cioccolato, granella di nocciole e cacao', 5.50, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Coppe')),
('COPPETTA CARAMELLO', 'Gelato al fiordilatte con topping al caramello, panna montata e arachidi', 6.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Coppette')),
('COPPETTA VENCHI®', 'Gelato al fiordilatte, panna montata, cioccolato croccante, topping al cioccolato e Cuba Rhum Venchi®', 5.00, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Coppette')),
('COPPETTA OREO®', 'Gelato al fiordilatte con crema di nocciole, crumble di biscotti Oreo®, panna montata, granella di nocciole, topping al cioccolato e biscotto Oreo®', 6.50, '', 15, (SELECT CategoriaID FROM Categorie WHERE NomeCategoria = 'Coppette'));

DELETE FROM Prodotti WHERE ProductID = 3;

DELETE FROM ProdottiCarrello;

