USE [SistemaTarjetasCredito]
GO
/****** Object:  StoredProcedure [dbo].[ProcesarNuevosTarjetaHabientes]    Script Date: 11/11/2024 12:14:04 a. m. ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[ProcesarNuevosTarjetaHabientes]
      @FechaActual DATE
	  , @OutResultCode INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        -- Inicialización del código de resultado
        SET @OutResultCode = 0;
		DECLARE @XMLDoc XML;
		DECLARE @idUsuario INT;
		DECLARE @lo INT, @hi INT;

		DECLARE @Nombre VARCHAR(64)
			, @ValorDocIdentidad VARCHAR(32)
			, @FechaNacimiento DATE
			, @NombreUsuario VARCHAR(64)
			, @Password NVARCHAR(64);

		DECLARE @TH_XML TABLE(
			Sec INT IDENTITY(1,1) -- para el bucle
			, Nombre VARCHAR(64)
			, ValorDocumentoIdentidad VARCHAR(32)
			, FechaNacimiento DATE
			, NombreUsuario VARCHAR(64)
			, Password NVARCHAR(64)
		) 

		SELECT @XMLDoc = BulkColumn
		FROM OPENROWSET(
			BULK 'D:\OperacionesFinal.xml', SINGLE_CLOB
		) AS x;

		-- Insertar nuevos Tarjeta Habientes (NTH) cuando la fecha de operación sea la fecha actual
        INSERT INTO @TH_XML(
			
			Nombre
			, ValorDocumentoIdentidad
			, FechaNacimiento
			, NombreUsuario
			, Password
			)
        SELECT 
            NTH.value('@Nombre', 'VARCHAR(64)'),
            NTH.value('@ValorDocIdentidad', 'VARCHAR(16)'),
            NTH.value('@FechaNacimiento', 'DATE'),
            NTH.value('@NombreUsuario', 'VARCHAR(64)'),
            NTH.value('@Password', 'NVARCHAR(64)')
        FROM @XMLDoc.nodes('/root/fechaOperacion[@Fecha = sql:variable("@FechaActual")]
		/NTH/NTH') 
		AS T(NTH);  
				-- Obtener el valor máximo del campo Id en la tabla temporal
		SELECT @hi = MAX(THXML.Sec) FROM @TH_XML THXML;

		SET @lo = 1;

		WHILE @lo <= @hi
		BEGIN
			-- Obtener los datos del registro actual
			SELECT 
				@Nombre = Nombre,
				@ValorDocIdentidad = ValorDocumentoIdentidad,
				@FechaNacimiento = FechaNacimiento,
				@NombreUsuario = NombreUsuario,
				@Password = Password
			FROM @TH_XML THXML
			WHERE  THXML.Sec = @lo;

			-- Paso 1: Insertar en la tabla Usuario y capturar el id generado
			INSERT INTO dbo.Usuario (
				idTipoUsuario
				,Nombre
				, Password)
			VALUES (
				2  -- es el usuario TarjetaHabiente
				@NombreUsuario
				, @Password
				);
    
			-- Capturar el idUsuario que se creo
			SET @idUsuario = SCOPE_IDENTITY();

			-- Paso 2: Insertar en la tabla TH utilizando el idUsuario capturado
			INSERT INTO dbo.TarjetaHabiente(
				idUsuario
				, Nombre
				, ValorDocumentoIdentidad
				, FechaNacimiento
				)
			VALUES (
				@idUsuario
				, @Nombre
				, @ValorDocIdentidad
				, @FechaNacimiento
				);

			-- Incrementar el contador
			SET @lo = @lo + 1;
		END;

        -- Retornamos el código de resultado
        SELECT @OutResultCode AS OutResultCode;

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