USE [SistemaTarjetasCredito]
GO
/****** Object:  StoredProcedure [dbo].[InsertarEmpleado]    Script Date: 6/11/2024 07:25:59 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[InsertarNuevoTCM]
      @FechaActual DATE
	  , @OutResultCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Inicialización del código de resultado
        SET @OutResultCode = 0;
		DECLARE @XMLDoc XML;
		DECLARE @TCMXML TABLE (
			Codigo INT
			, TipoTCM VARCHAR(16)
			, LimiteCredito MONEY
			, DocTH VARCHAR(16)
		);

		SELECT @XMLDoc = BulkColumn
		FROM OPENROWSET(
			BULK 'D:\OperacionesFinal.xml', SINGLE_CLOB
		) AS x;

		-- Insertar nuevos Tarjeta Habientes (NTH) cuando la fecha de operación sea la fecha actual
        INSERT INTO @TCMXML(
			Codigo
			, TipoTCM
			, LimiteCredito
			, DocTH
		)
        SELECT 
            NTCM.value('@Codigo', 'INT'),
            NTCM.value('TipoTCM', 'VARCHAR(16)'),
            NTCM.value('@LimiteCredito', 'MONEY'),
            NTCM.value('@TH', 'VARCHAR(16)')
        FROM @XMLDoc.nodes('/root/fechaOperacion[@Fecha = 
		sql:variable("@FechaActual")]/NTCM/NTCM') 
		AS T(NTCM);
        -- Retornamos el código de resultado
        SELECT @OutResultCode AS OutResultCode;

		 -- Insertar los datos en la tabla TCM
		INSERT INTO dbo.TarjetaCreditoMaestra(
			idTH,
			idTipoTCM,
			idEstadoCuenta,
			Codigo,
			LimiteCredito,
			SaldoActual,
			FechaCreacion,
			FechaCorte
		)
		SELECT 
			TH.id AS idTH,
			TTCM.id AS idTipoTCM,
			0 AS idEstadoCuenta, -- Corregir
			TCMXML.Codigo,
			TCMXML.LimiteCredito,
			0 AS SaldoActual, -- Asigna 0 o el valor inicial correspondiente
			@FechaActual AS FechaCreacion,
			CASE 
				WHEN DAY(@FechaActual) > DAY(EOMONTH(DATEADD(MONTH, 1, @FechaActual)))  
				THEN EOMONTH(DATEADD(MONTH, 1, @FechaActual))
				ELSE DATEADD(MONTH, 1, @FechaActual)
			END AS FechaCorte -- Calcula la fecha de corte como 
			                 -- el mismo día del mes siguiente
		FROM @TCMXML TCMXML
		INNER JOIN TarjetaHabiente TH ON TH.ValorDocumentoIdentidad = TCMXML.DocTH
		INNER JOIN TipoTCM TTCM ON TTCM.Nombre = TCMXML.TipoTCM;

    END TRY
    BEGIN CATCH

        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION tinsertEmpleado;  -- Deshacemos la transacción si hay error

        -- Insertamos el error en la tabla DBErrors
        INSERT INTO dbo.DBErrors (
            Username,
            ErrorNumber,
            ErrorState,
            ErrorSeverity,
            ErrorLine,
            ErrorProcedure,
            ErrorMessage,
            ErrorDatetime
        )
        VALUES (
            SUSER_SNAME()
            , ERROR_NUMBER()
            , ERROR_STATE()
            , ERROR_SEVERITY()
            , ERROR_LINE()
            , ERROR_PROCEDURE()
            , ERROR_MESSAGE()
            , GETDATE()
        );


        SET @OutResultCode = 50008;  -- Código de error estándar
    END CATCH;

    SET NOCOUNT OFF;
END;