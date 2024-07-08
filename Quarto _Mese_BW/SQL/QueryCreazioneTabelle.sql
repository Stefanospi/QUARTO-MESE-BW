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

