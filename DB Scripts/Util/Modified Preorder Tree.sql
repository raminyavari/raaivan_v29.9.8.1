USE [EKM_App]
GO



IF NOT EXISTS(
	SELECT TOP(1) *
	FROM sys.objects
	WHERE OBJECT_NAME([object_id]) = N'TMPTree'
) BEGIN

	CREATE TABLE [dbo].[TMPTree](
		[ID] [int] NOT NULL,
		[Left] [int] NULL,
		[Right] [int] NULL,
		[Level] [int] NULL
	 CONSTRAINT [PK_TMPTree] PRIMARY KEY CLUSTERED 
	(
		[ID] ASC
	)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
	) ON [PRIMARY]

END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[TT_Add]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TT_Add]
GO

CREATE PROCEDURE [dbo].[TT_Add]
	@ID			int,
    @ParentID	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	Declare @Left int
	Declare @Right int
	Declare @Level int
	declare @RightParent int 
	declare @rightParentOrig int
	declare @leftParentOrig int

	set @RightParent = null

	if @ParentId is null begin
		set @Left = 1
		set @Right = 2
		set @Level = 0
	end
	else begin
		select @rightParentOrig = [right], @leftParentOrig = [left], 
			   @Right = [right] + 1, @Left = [right], @Level = [Level] + 1 
		from [dbo].[TMPTree] 
		where id = @ParentId
		
		set @RightParent =  @Right  + 1
	end

	UPDATE [dbo].[TMPTree]
		set [right] = [right] + 2,
			[left] = [left] + 2
	where [left] >=  @Left

	UPDATE [dbo].[TMPTree]
		SET [right] = [right] + 2
	WHERE [left] < @leftParentOrig and [right] > @rightParentOrig 

	INSERT INTO [dbo].[TMPTree]([ID], [Left], [right], [Level])
	VALUES (@ID, @Left, @Right, @Level)

	if @RightParent is not null begin
		update [dbo].[TMPTree] set [right] = @RightParent where id = @ParentId
	end
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[TT_P_Remove]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TT_P_Remove]
GO

CREATE PROCEDURE [dbo].[TT_P_Remove]
	@ID			int,
	@_Result	int output
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	DECLARE @Left int, @Right int
	
	SELECT @Left = [Left], @Right = [Right]
	FROM [dbo].[TMPTree]
	WHERE ID = @ID
	
	UPDATE [dbo].[TMPTree]
		SET [Left] = [Left] - 2,
			[Right] = [Right] - 2
	WHERE [Left] >= @Left
	
	UPDATE [dbo].[TMPTree]
		SET [Right] = [Right] - 2
	WHERE [Left] < @Left AND [Right] > @Right
	
	DECLARE @MaxRight int = (SELECT MAX([Right]) FROM [dbo].[TMPTree])
	
	UPDATE [dbo].[TMPTree]
		SET [Left] = @MaxRight + 1,
			[Right] = @MaxRight + 2,
			[Level] = 0
	WHERE ID = @ID
	
	SET @_Result = 1
END

GO


IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = object_id(N'[dbo].[TT_Update]') AND
	OBJECTPROPERTY(id, N'IsProcedure') = 1)
DROP PROCEDURE [dbo].[TT_Update]
GO

CREATE PROCEDURE [dbo].[TT_Update]
	@ID			int,
    @ParentID	int
WITH ENCRYPTION
AS
BEGIN
	SET NOCOUNT ON
	
	-- Get source span info (Distance between Left & Right plus one)
	DECLARE @SrcSpan int = (SELECT ([Right] - [Left] + 1) FROM [dbo].[TMPTree] WHERE ID = @ID)
	
	-- Find out where the new parent currently ends
	DECLARE @NewParentRight int, @NewParentLevel int
	
	SELECT @NewParentRight = [Right], @NewParentLevel = [Level]
	FROM [dbo].[TMPTree] 
	WHERE ID = @ParentID
	
	-- Create the gap at the bottom of the hierarchy for the
	-- new parent big enough for the source object and its tree
	UPDATE [dbo].[TMPTree]
		SET [Left] = CASE WHEN [Left] > ISNULL(@NewParentRight, 0) THEN [Left] + @SrcSpan ELSE [Left] END,
			[Right] = CASE WHEN [Right] >= ISNULL(@NewParentRight, 0) THEN [Right] + @SrcSpan ELSE [Right] END
	WHERE [Right] >= ISNULL(@NewParentRight, 0)
	
	-- Move the object tree to the newly created gap
	-- (may seem like a repetitive select, but the above Update
	-- may have moved the source object)
	DECLARE @SrcLeft int, @SrcRight int, @LevelOffset int, @MoveOffset int
	
	SELECT @SrcLeft = [Left], @SrcRight = [Right], 
		   @LevelOffset = [Level] - ISNULL(@NewParentLevel, -1) - 1,
		   @MoveOffset = ([Left] - ISNULL(@NewParentRight, 1))
	FROM [dbo].[TMPTree]
	WHERE ID = @ID
	
	UPDATE [dbo].[TMPTree]
		SET [Left] = [Left] - @MoveOffset,
			[Right] = [Right] - @MoveOffset,
			[Level] = [Level] - @LevelOffset
	WHERE [Left] >= @SrcLeft AND [Right] <= @SrcRight
	
	-- Close the gap where the source object was
	UPDATE [dbo].[TMPTree]
		SET [Left] = CASE WHEN [Left] > @SrcLeft THEN [Left] - @SrcSpan ELSE [Left] END,
			[Right] = CASE WHEN [Right] > @SrcLeft THEN [Right] - @SrcSpan ELSE [Right] END
	WHERE [Right] >= @SrcLeft
END

GO



delete [dbo].[TMPTree]
go

exec [dbo].[TT_Add] 1, null
exec [dbo].[TT_Add] 2, 1
exec [dbo].[TT_Add] 3, null
exec [dbo].[TT_Add] 4, 2
exec [dbo].[TT_Add] 5, 2
exec [dbo].[TT_Add] 6, 1
exec [dbo].[TT_Add] 7, 3
exec [dbo].[TT_Add] 8, 7

exec [dbo].[TT_Update] 7, null

DECLARE @SL int-- = 1
DECLARE @SR int-- = 10

select * 
from [dbo].[TMPTree]
where (@SL IS NULL OR [left] >= @SL) and (@SR IS NULL OR [right] <= @SR)
order by [left], [right]