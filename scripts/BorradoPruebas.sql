
-- Esto es magia negra, para borrar por si acaso
SELECT * FROM dbo.TarjetaHabiente

DELETE FROM dbo.TarjetaHabiente
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.TarjetaHabiente', RESEED, 0)


DELETE FROM dbo.Usuario
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.Usuario', RESEED, 0)