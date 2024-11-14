DECLARE @XMLDoc XML;
DECLARE @FechaActual DATE;
DECLARE @UltimaFecha DATE;
DECLARE @FechasOperacion TABLE(
    Id INT IDENTITY(1,1),
    FechaOperacion DATE
);

SET NOCOUNT ON;

SELECT @XMLDoc = BulkColumn
FROM OPENROWSET(BULK 'D:\OperacionesFinal.xml', SINGLE_CLOB) AS XMLDoc;

-- Insertar las fechas de operacion
INSERT INTO @FechasOperacion(FechaOperacion)
SELECT XmlDoc.value('@Fecha', 'DATE') AS FechaOperacion
FROM @XMLDoc.nodes('/root/fechaOperacion') AS T(XmlDoc);

-- la primera y ultima fecha
SELECT @FechaActual = MIN(FechaOperacion), @UltimaFecha = MAX(FechaOperacion)
FROM @FechasOperacion;


-- Se ejecuta dia por dia
WHILE @FechaActual <= @UltimaFecha
BEGIN
    -- Para los nuevos TH
    EXEC dbo.ProcesarNuevosTarjetaHabientes @FechaActual, 0;

	-- Para los nuevos TCM
	--EXEC dbo.InsertarTCM @FechaActual, 0;
	-- Para los nuevos TCA
	--EXEC dbo.CrearTCA @FechaActual, 0;

    -- Se agrega un dia para el bucle
    SET @FechaActual = DATEADD(DAY, 1, @FechaActual);
END

SET NOCOUNT OFF;
