-- ============================================================
--  PIM Database Setup — Table Creation Only
--  Run with:
--  Get-Content setup_db.sql | docker exec -i database-postgres psql -U postgres -d postgres
-- ============================================================

CREATE TABLE IF NOT EXISTS pim_units (
    id SERIAL PRIMARY KEY,
    symbol TEXT NOT NULL UNIQUE
);

CREATE TABLE IF NOT EXISTS pim_categories (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    parent_id INTEGER REFERENCES pim_categories(id) ON DELETE RESTRICT,
    isProductCategory BOOLEAN NOT NULL DEFAULT false
    );

CREATE TABLE IF NOT EXISTS pim_attributes (
    id SERIAL PRIMARY KEY,
    unit_id INTEGER REFERENCES pim_units(id) ON DELETE SET NULL,
    attribute_name TEXT NOT NULL UNIQUE,
    data_type TEXT NOT NULL DEFAULT 'string'
    );

CREATE TABLE IF NOT EXISTS pim_category_attributes (
    category_id  INTEGER NOT NULL REFERENCES pim_categories(id) ON DELETE CASCADE,
    attribute_id INTEGER NOT NULL REFERENCES pim_attributes(id) ON DELETE CASCADE,
    PRIMARY KEY (category_id, attribute_id)
    );

CREATE TABLE IF NOT EXISTS pim_products (
    id SERIAL PRIMARY KEY,
    name TEXT NOT NULL,
    category_id INTEGER NOT NULL REFERENCES pim_categories(id) ON DELETE RESTRICT
    );

CREATE TABLE IF NOT EXISTS pim_product_variants (
    id SERIAL PRIMARY KEY,
    product_id INTEGER NOT NULL REFERENCES pim_products(id) ON DELETE CASCADE,
    sku TEXT NOT NULL UNIQUE,
    price NUMERIC(10, 2) NOT NULL,
    base_price NUMERIC(10, 2) NOT NULL,
    cost_price NUMERIC(10, 2) NOT NULL,
    isActive BOOLEAN NOT NULL DEFAULT TRUE
    );

CREATE TABLE IF NOT EXISTS pim_variant_attribute_values (
    variant_id INTEGER NOT NULL REFERENCES pim_product_variants(id) ON DELETE CASCADE,
    attribute_id INTEGER NOT NULL REFERENCES pim_attributes(id) ON DELETE RESTRICT,
    value TEXT NOT NULL,
    PRIMARY KEY (variant_id, attribute_id)
    );