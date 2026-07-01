-- Seed data: productos variados con stock 0 para testear flujo proveedor
-- Marcas existentes: 1=Nike, 2=Adidas
-- Categorias existentes: 1=Indumentaria, 2=Electronica
INSERT INTO Productos (Codigo, NombreProducto, Descripcion, StockActual, StockMinimo, Precio, PorcentajeGanancia, IdMarca, IdCategoria, Estado) VALUES
('P0002', 'Remera Running Dri-Fit',    'Remera deportiva de secado rapido, ideal para entrenamiento',         0, 5,  4500.00, 40.00, 1, 1, 1),
('P0003', 'Campera Rompevientos',       'Campera liviana impermeable para actividades al aire libre',          0, 3, 12000.00, 35.00, 2, 1, 1),
('P0004', 'Short Deportivo Training',   'Short de poliester con bolsillos laterales',                         0, 5,  3800.00, 40.00, 2, 1, 1),
('P0005', 'Medias Deportivas Pack x3',  'Pack de 3 pares de medias con refuerzo en talon',                   0, 10, 1200.00, 50.00, 1, 1, 1),
('P0006', 'Auriculares Bluetooth Sport','Auriculares inalambricos resistentes al agua, bateria 8hs',          0, 2, 18000.00, 30.00, 2, 2, 1),
('P0007', 'Smartwatch Deportivo GPS',   'Reloj inteligente con GPS, monitor cardiaco y 7 dias de bateria',   0, 2, 45000.00, 25.00, 1, 2, 1);
