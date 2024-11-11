USE [SistemaTarjetasCredito]
GO
/****** Object:  StoredProcedure [dbo].[InsertarEmpleado]    Script Date: 6/11/2024 07:25:59 p. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CrearTCA]
      @FechaActual DATE
	  , @OutResultCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Inicialización del código de resultado
        SET @OutResultCode = 0;
		DECLARE @XMLDoc XML;
		DECLARE @TCAXml TABLE (
			CodigoTCM INT
			, CodigoTCA INT
			, DocTH VARCHAR(16)
		);

		SELECT @XMLDoc = BulkColumn
		FROM OPENROWSET(
			BULK 'D:\OperacionesFinal.xml', SINGLE_CLOB
		) AS x;

		-- Insertar nuevos Tarjeta Habientes (NTH) cuando la fecha de operación sea la fecha actual
        INSERT INTO @TCAXml(
			CodigoTCM
			, CodigoTCA
			, DocTH
		)
        SELECT 
            NTCM.value('@CodigoTCM', 'INT'),
            NTCM.value('@CodigoTCA', 'INT'),
            NTCM.value('@TH', 'VARCHAR(16)')
        FROM @XMLDoc.nodes('/root/fechaOperacion[@Fecha = 
		sql:variable("@FechaActual")]/NTCA/NTCA') 
		AS T(NTCM);
        -- Retornamos el código de resultado
        SELECT @OutResultCode AS OutResultCode;

		 -- Insertar los datos en la tabla TCM
		INSERT INTO dbo.TarjetaCreditoAdicional(
			idTH
			, idTCM
			, CodigoTCA
		)
		SELECT 
			TH.id AS idTH
			, TCM.id AS idTCM
			, TCAXML.CodigoTCA
		FROM @TCAXml TCAXML
		INNER JOIN TarjetaHabiente TH ON TH.ValorDocumentoIdentidad = TCAXML.DocTH
		INNER JOIN dbo.TarjetaCreditoMaestra TCM ON TCM.Codigo = TCAXML.CodigoTCM

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