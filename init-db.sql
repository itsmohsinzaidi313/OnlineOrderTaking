-- Database Initialization Script

-- Create the products table
CREATE TABLE IF NOT EXISTS products (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    price DECIMAL(10, 2) NOT NULL,
    description TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Insert some sample data
INSERT INTO products (name, price, description) VALUES 
    ('Sample Product 1', 19.99, 'This is a sample product'),
    ('Sample Product 2', 29.99, 'Another sample product'),
    ('Sample Product 3', 39.99, 'Yet another sample product')
ON CONFLICT DO NOTHING;