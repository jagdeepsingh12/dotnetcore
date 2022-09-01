
CREATE DATABASE IF NOT EXISTS carinventory ;
USE carinventory ;

CREATE TABLE IF NOT EXISTS cars (
    car_id INT AUTO_INCREMENT PRIMARY KEY,
    brand VARCHAR(255) NOT NULL,
    model VARCHAR(255) NOT NULL,
    year int NOT NULL,
    price DECIMAL(8,2) NOT NULL,
    new bool NOT NULL
    /* created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP  */
);