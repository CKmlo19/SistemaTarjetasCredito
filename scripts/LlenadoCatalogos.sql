USE SistemaTarjetasCredito; -- Nombre de la base de datos
DECLARE @XMLDoc XML;

-- Lee el archivo XML y lo almacena en la variable @XMLDoc
SELECT @XMLDoc = CAST(BulkColumn AS XML)
FROM OPENROWSET(BULK 'C:\Catalogos.xml', SINGLE_BLOB) AS x;

-- Se crean dos tablas variables para hacer joins con nuestras tablas en la BD



DECLARE @RN_XML TABLE(
    Nombre VARCHAR(64),
	TTCM VARCHAR(32),
	TipoRN VARCHAR(64),
	Valor INT
)


DECLARE @TM_XML TABLE(
    Nombre VARCHAR(64),
	Accion VARCHAR(16),
	AcumulaOperacionATM VARCHAR(4),
	AcumulaOperacionVentana VARCHAR(4)
)

DECLARE @Usuario_XML TABLE(
    Nombre VARCHAR(64),
	Password NVARCHAR(64),
	idTipoUsuario INT
)



-- Inserta los datos de las tablas temporales a sus respectivas tablas

-- Para RN
INSERT INTO @RN_XML(Nombre, TTCM, TipoRN, Valor)
SELECT
    XMLDATA.value('@Nombre', 'VARCHAR(64)'),
    XMLDATA.value('@TTCM', 'VARCHAR(32)'),
	XMLDATA.value('@TipoRN', 'VARCHAR(64)'),
	XMLDATA.value('@Valor', 'MONEY')
FROM @XMLDoc.nodes('/root/RN/RN') AS XTbl(XMLDATA);

INSERT INTO @TM_XML(Nombre, Accion, AcumulaOperacionATM, AcumulaOperacionVentana)
SELECT
    XMLDATA.value('@Nombre', 'VARCHAR(64)'),
    XMLDATA.value('@Accion', 'VARCHAR(16)'),
	XMLDATA.value('@Acumula_Operacion_ATM', 'VARCHAR(4)'),
	XMLDATA.value('@Acumula_Operacion_Ventana', 'VARCHAR(4)')
FROM @XMLDoc.nodes('/root/TM/TM') AS XTbl(XMLDATA);


-- Inserts a la BD

-- Para TTCM
INSERT INTO dbo.TipoTCM(Nombre)
SELECT
	XMLDATA.value('@Nombre', 'VARCHAR(64)')
FROM @XMLDoc.nodes('/root/TTCM/TTCM') AS XTbl(XMLDATA);

INSERT INTO dbo.TipoUsuario (Tipo)
VALUES ('Administrador'),
       ('TarjetaHabiente');

INSERT INTO Usuario(Nombre, Password, idTipoUsuario)
SELECT
	XMLDATA.value('@Nombre', 'VARCHAR(64)'),
	XMLDATA.value('@Password', 'NVARCHAR(64)'),
	1 AS idTipoUsuario -- 1 es para administrador
FROM @XMLDoc.nodes('/root/UA/Usuario') AS XTbl(XMLDATA);


-- Para TRN
INSERT INTO dbo.TipoReglasNegocio(Nombre, Tipo)
SELECT
    XMLDATA.value('@Nombre', 'VARCHAR(64)'),
    XMLDATA.value('@tipo', 'VARCHAR(32)')
FROM @XMLDoc.nodes('/root/TRN/TRN') AS XTbl(XMLDATA);

INSERT INTO dbo.MotivosInvalidacionTarjeta(Nombre)
SELECT
    XMLDATA.value('@Nombre', 'VARCHAR(64)')
FROM @XMLDoc.nodes('/root/MIT/MIT') AS XTbl(XMLDATA);


INSERT INTO dbo.TMIC(Nombre)
SELECT
    XMLDATA.value('@nombre', 'VARCHAR(64)')
FROM @XMLDoc.nodes('/root/TMIC/TMIC') AS XTbl(XMLDATA);

INSERT INTO dbo.TMIM(Nombre)
SELECT
    XMLDATA.value('@nombre', 'VARCHAR(64)')
FROM @XMLDoc.nodes('/root/TMIM/TMIM') AS XTbl(XMLDATA);




-- Para Reglas de Negocio
INSERT INTO dbo.ReglasNegocio(idTipoTCM, idTipoRN, Nombre, Valor)
SELECT
	TTCM.id,  
	TRN.id,
	RN.Nombre,
	RN.Valor
FROM @RN_XML RN 
INNER JOIN TipoReglasNegocio TRN ON TRN.Nombre = RN.TipoRN
INNER JOIN TipoTCM TTCM ON TTCM.Nombre = RN.TTCM

INSERT INTO dbo.TipoMovimiento(Nombre, Accion, AcumulaOperacionATM, AcumulaOperacionVentana)
SELECT 
	TM.Nombre,
	TM.Accion,
    CASE WHEN AcumulaOperacionATM = 'NO' THEN 0 ELSE 1 END AS AcumulaOperacionATM,
	CASE WHEN AcumulaOperacionVentana = 'NO' THEN 0 ELSE 1 END AS Acumula_Operacion_Ventana
FROM @TM_XML TM


-- Verificar en las tablas
SELECT * FROM dbo.ReglasNegocio;
SELECT * FROM dbo.TipoTCM;
SELECT * FROM dbo.TipoReglasNegocio;
SELECT * FROM dbo.MotivosInvalidacionTarjeta;
SELECT * FROM dbo.TipoMovimiento
SELECT * FROM dbo.TMIC;
SELECT * FROM dbo.TMIM;
SELECT * FROM dbo.Usuario;
SELECT * FROM dbo.TipoUsuario;






-- Esto es magia negra, para borrar por si acaso
DELETE FROM dbo.MotivosInvalidacionTarjeta
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.MotivosInvalidacionTarjeta', RESEED, 0)

DELETE FROM dbo.ReglasNegocio
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.ReglasNegocio', RESEED, 0)

DELETE FROM dbo.TipoTCM
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.TipoTCM', RESEED, 0)

DELETE FROM dbo.TipoReglasNegocio
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.TipoReglasNegocio', RESEED, 0)

DELETE FROM dbo.TipoMovimiento
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.TipoMovimiento', RESEED, 0)

DELETE FROM dbo.Usuario
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.Usuario', RESEED, 0)

DELETE FROM dbo.TipoUsuario
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.TipoUsuario', RESEED, 0)

DELETE FROM dbo.TMIC
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.TMIC', RESEED, 0)


DELETE FROM dbo.TMIM
DBCC CHECKIDENT ('SistemaTarjetasCredito.dbo.TMIM', RESEED, 0)
