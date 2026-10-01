-- ============================================================
--  PIM Seed Data
--  Run AFTER setup_db.sql
--  Get-Content seed_data.sql | docker exec -i database-postgres psql -U postgres -d postgres
-- ============================================================


-- ── Units ─────────────────────────────────────────────────────

INSERT INTO pim_units (symbol) VALUES
                                   ('GB'),
                                   ('TB'),
                                   ('Hz'),
                                   ('MHz'),
                                   ('GHz'),
                                   ('in'),
                                   ('W'),
                                   ('mAh'),
                                   ('kg'),
                                   ('mm'),
                                   ('g'),
                                   ('ms'),
                                   ('dpi'),
                                   ('h'),
                                   ('dB'),
                                   ('MHz'),
                                   ('GB/s'),
                                   ('MB/s'),
                                   ('mm')
    ON CONFLICT DO NOTHING;


-- ── Attributes ───────────────────────────────────────────────

-- No unit
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'CPU',                    'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'GPU',                    'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Motherboard',            'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Cabinet',                'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'CPU Cooler',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Display Type',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Display Resolution',     'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Charger Type',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Ports',                  'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Sensor',                 'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Wireless',               'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Backlighting',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Layout',                 'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Size',                   'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Mechanical',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Switch Technology',      'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Microphone',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Noise Cancelling',       'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Compatibility',          'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Panel Type',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Adaptive Sync',          'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Display Inputs',         'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Brand',                  'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Model',                  'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Socket',                 'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Integrated Graphics',    'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Interface',              'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Power Connector',        'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Outputs',                'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Kit Configuration',      'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'RAM Type',               'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'RGB Lighting',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Storage Type',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Form Factor',            'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Chipset',                'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Memory Slots',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'M.2 Slots',              'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'PCIe Slots',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Connectivity',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'USB Ports',              'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Efficiency Rating',      'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Modularity',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Connectors',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Drive Bays',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Fan Mounts',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Radiator Support',       'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Side Panel',             'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Color',                  'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Button Count',           'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Fan Size',               'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Form Factor Support',    'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Type',                  'string') ON CONFLICT DO NOTHING;
INSERT INTO pim_attributes (unit_id, attribute_name, data_type) VALUES (NULL, 'Length',                  'string') ON CONFLICT DO NOTHING;

-- With units
INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'RAM',                  'string' FROM pim_units WHERE symbol = 'GB'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Storage',              'string' FROM pim_units WHERE symbol = 'GB'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'PSU',                  'string' FROM pim_units WHERE symbol = 'W'    ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Weight',               'string' FROM pim_units WHERE symbol = 'g'    ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Display Size',         'string' FROM pim_units WHERE symbol = 'in'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Display Refresh Rate', 'string' FROM pim_units WHERE symbol = 'Hz'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Battery Life',         'string' FROM pim_units WHERE symbol = 'h'    ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Polling Rate',         'string' FROM pim_units WHERE symbol = 'Hz'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'DPI Range',            'string' FROM pim_units WHERE symbol = 'dpi'  ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Sound Level',          'string' FROM pim_units WHERE symbol = 'dB'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Screen Size',          'string' FROM pim_units WHERE symbol = 'in'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Refresh Rate',         'string' FROM pim_units WHERE symbol = 'Hz'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Response Time',        'string' FROM pim_units WHERE symbol = 'ms'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Base Clock',           'string' FROM pim_units WHERE symbol = 'GHz'  ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Boost Clock',          'string' FROM pim_units WHERE symbol = 'GHz'  ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'TDP',                  'string' FROM pim_units WHERE symbol = 'W'    ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'VRAM',                 'string' FROM pim_units WHERE symbol = 'GB'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'GPU Length',           'string' FROM pim_units WHERE symbol = 'mm'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Capacity',             'string' FROM pim_units WHERE symbol = 'GB'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'RAM Speed',            'string' FROM pim_units WHERE symbol = 'MHz'  ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Read Speed',           'string' FROM pim_units WHERE symbol = 'MB/s' ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Write Speed',          'string' FROM pim_units WHERE symbol = 'MB/s' ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Wattage',              'string' FROM pim_units WHERE symbol = 'W'    ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Max GPU Length',       'string' FROM pim_units WHERE symbol = 'mm'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Max CPU Cooler Height','string' FROM pim_units WHERE symbol = 'mm'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Core / Thread Count',  'string' FROM pim_units WHERE symbol = 'MHz'  ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Latency',              'string' FROM pim_units WHERE symbol = 'ms'   ON CONFLICT DO NOTHING;

INSERT INTO pim_attributes (unit_id, attribute_name, data_type)
SELECT id, 'Dimensions',           'string' FROM pim_units WHERE symbol = 'mm'   ON CONFLICT DO NOTHING;


-- ── Categories ───────────────────────────────────────────────

-- Root categories
INSERT INTO pim_categories (name, parent_id, isProductCategory)
VALUES ('Computers', NULL, false) ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
VALUES ('Monitors', NULL, false) ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
VALUES ('PC Components', NULL, false) ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
VALUES ('Cables', NULL, true) ON CONFLICT DO NOTHING;

-- Under Computers
INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Desktop Computers', id, true FROM pim_categories WHERE name = 'Computers' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Laptop Computers', id, true FROM pim_categories WHERE name = 'Computers' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Computer Accessories', id, false FROM pim_categories WHERE name = 'Computers' ON CONFLICT DO NOTHING;

-- Under Computer Accessories
INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Mouse', id, true FROM pim_categories WHERE name = 'Computer Accessories' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Keyboards', id, true FROM pim_categories WHERE name = 'Computer Accessories' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Headset', id, true FROM pim_categories WHERE name = 'Computer Accessories' ON CONFLICT DO NOTHING;

-- Under Monitors
INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Computer Monitors', id, true FROM pim_categories WHERE name = 'Monitors' ON CONFLICT DO NOTHING;

-- Under PC Components
INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'CPU', id, true FROM pim_categories WHERE name = 'PC Components' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'GPU', id, true FROM pim_categories WHERE name = 'PC Components' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'RAM', id, true FROM pim_categories WHERE name = 'PC Components' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Storage', id, true FROM pim_categories WHERE name = 'PC Components' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Motherboard', id, true FROM pim_categories WHERE name = 'PC Components' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'PSU', id, true FROM pim_categories WHERE name = 'PC Components' ON CONFLICT DO NOTHING;

INSERT INTO pim_categories (name, parent_id, isProductCategory)
SELECT 'Cabinet', id, true FROM pim_categories WHERE name = 'PC Components' ON CONFLICT DO NOTHING;


-- ── Category Attributes ──────────────────────────────────────

-- Desktop Computers
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'CPU'              ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'GPU'              ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'RAM'              ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'Storage'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'Motherboard'      ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'PSU'              ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'Cabinet'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'CPU Cooler'       ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'Weight'           ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Desktop Computers' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Laptop Computers
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'CPU'               ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'GPU'               ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'RAM'               ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Storage'           ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Display Size'      ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Display Type'      ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Display Refresh Rate' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Display Resolution' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Weight'            ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Battery Life'      ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Charger Type'      ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Ports'             ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Laptop Computers' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Mouse
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'Sensor'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'Polling Rate'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'DPI Range'        ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'Weight'           ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'Wireless'         ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'Button Count'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'Battery Life'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Mouse' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Keyboards
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Keyboards' AND a.attribute_name = 'Backlighting'       ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Keyboards' AND a.attribute_name = 'Layout'            ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Keyboards' AND a.attribute_name = 'Size'              ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Keyboards' AND a.attribute_name = 'Mechanical'        ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Keyboards' AND a.attribute_name = 'Switch Technology' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Keyboards' AND a.attribute_name = 'Wireless'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Keyboards' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Headset
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Wireless'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Microphone'        ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Weight'            ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Noise Cancelling'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Battery Life'      ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Compatibility'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Sound Level'       ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Headset' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Computer Monitors
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Screen Size'    ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Display Resolution' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Refresh Rate'   ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Panel Type'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Response Time'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Adaptive Sync'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Display Inputs' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Computer Monitors' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- CPU
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'Brand'               ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'Model'               ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'Core / Thread Count' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'Base Clock'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'Boost Clock'         ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'TDP'                 ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'Socket'              ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'CPU' AND a.attribute_name = 'Integrated Graphics' ON CONFLICT DO NOTHING;

-- GPU
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'Brand'           ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'VRAM'            ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'Boost Clock'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'Interface'       ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'Power Connector' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'TDP'             ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'Outputs'         ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'GPU' AND a.attribute_name = 'GPU Length'      ON CONFLICT DO NOTHING;

-- RAM
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'RAM' AND a.attribute_name = 'Capacity'         ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'RAM' AND a.attribute_name = 'Kit Configuration' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'RAM' AND a.attribute_name = 'RAM Type'         ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'RAM' AND a.attribute_name = 'RAM Speed'        ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'RAM' AND a.attribute_name = 'Latency'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'RAM' AND a.attribute_name = 'RGB Lighting'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'RAM' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Storage
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Storage' AND a.attribute_name = 'Storage Type' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Storage' AND a.attribute_name = 'Capacity'     ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Storage' AND a.attribute_name = 'Form Factor'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Storage' AND a.attribute_name = 'Interface'    ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Storage' AND a.attribute_name = 'Read Speed'   ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Storage' AND a.attribute_name = 'Write Speed'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Storage' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Motherboard
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'Chipset'      ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'Socket'       ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'Form Factor'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'Memory Slots' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'M.2 Slots'    ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'PCIe Slots'   ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'Connectivity'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'USB Ports'    ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Motherboard' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- PSU
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'PSU' AND a.attribute_name = 'Wattage'           ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'PSU' AND a.attribute_name = 'Efficiency Rating' ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'PSU' AND a.attribute_name = 'Modularity'        ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'PSU' AND a.attribute_name = 'Form Factor'       ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'PSU' AND a.attribute_name = 'Fan Size'          ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'PSU' AND a.attribute_name = 'Connectors'        ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'PSU' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Cabinet
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Form Factor Support'    ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Dimensions'             ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Max GPU Length'         ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Max CPU Cooler Height'  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Drive Bays'             ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Fan Mounts'             ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Radiator Support'       ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Side Panel'             ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Color'                  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cabinet' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;

-- Cables
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cables' AND a.attribute_name = 'Type'             ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cables' AND a.attribute_name = 'Length'                  ON CONFLICT DO NOTHING;
INSERT INTO pim_category_attributes (category_id, attribute_id) SELECT c.id, a.id FROM pim_categories c, pim_attributes a WHERE c.name = 'Cables' AND a.attribute_name = 'Brand'            ON CONFLICT DO NOTHING;