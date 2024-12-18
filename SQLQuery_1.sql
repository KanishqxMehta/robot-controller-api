-- DROP Table robotcommand;

-- CREATE TABLE public.robotcommand (
--         Id int NOT NULL GENERATED ALWAYS AS IDENTITY,
--         "Name" varchar(50) NOT NULL,
--         Description varchar(800) NULL,
--         IsMoveCommand boolean NOT NULL,
--         CreatedDate timestamp NOT NULL,
--         ModifiedDate timestamp NOT NULL
-- );

-- INSERT INTO robotcommand
-- ("Name", Description, IsMoveCommand, CreatedDate, ModifiedDate)
-- VALUES('PLACE', NULL, false, '2024-03-06', '2024-03-06'),
-- ('MOVE', NULL, true, '2024-03-06', '2024-03-06'),
-- ('RIGHT', NULL, true, '2024-03-06', '2024-03-06'),
-- ('LEFT', NULL, true, '2024-03-06', '2024-03-06');

-- SELECT * FROM map;
-- DROP TABLE map;

-- CREATE TABLE public.map(
--     Id int NOT NULL GENERATED ALWAYS AS IDENTITY,
--     Columns INTEGER NOT NULL,
--     Rows INTEGER NOT NULL,
--     "Name" varchar(50) NOT NULL,
--     Description varchar(800) NULL,
--     CreatedDate timestamp NOT NULL,
--     ModifiedDate timestamp NOT NULL
-- );

-- INSERT INTO map(Columns, Rows, "Name", Description, CreatedDate, ModifiedDate) 
-- VALUES(25, 25, 'MAP1', 'TEST MAP', '2024-03-07', '2024-03-07');

-- SELECT * FROM map;

