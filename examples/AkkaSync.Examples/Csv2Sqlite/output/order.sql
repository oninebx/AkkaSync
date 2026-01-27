CREATE TABLE IF NOT EXISTS Customer (
    id TEXT PRIMARY KEY,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT,
    phone TEXT,
    dob DATE,
    country TEXT,
    is_active INTEGER NOT NULL CHECK (is_active IN (0,1))
);

CREATE TABLE IF NOT EXISTS Product (
    sku TEXT PRIMARY KEY,
    name TEXT,
    price REAL
);

CREATE TABLE IF NOT EXISTS "Order" (
    id TEXT PRIMARY KEY,
    customer_id TEXT NOT NULL,
    date DATE,
    status TEXT,
    shipping_address TEXT,
    country TEXT,
    is_priority INTEGER NOT NULL CHECK (is_priority IN (0,1)),
    FOREIGN KEY (customer_id) REFERENCES Customer(id)
);

CREATE TABLE IF NOT EXISTS OrderItem (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id TEXT NOT NULL,
    product_sku TEXT NOT NULL,
    quantity INTEGER,
    FOREIGN KEY (order_id) REFERENCES "Order"(id),
    FOREIGN KEY (product_sku) REFERENCES Product(sku)
);

CREATE TABLE IF NOT EXISTS Audit (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    order_id TEXT,
    action TEXT,
    notes TEXT,
    FOREIGN KEY (order_id) REFERENCES "Order"(id)
);
