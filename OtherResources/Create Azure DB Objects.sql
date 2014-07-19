-- =========================================
-- Create table template SQL Azure Database 
-- =========================================

IF OBJECT_ID('TweetInfo', 'U') IS NOT NULL
  DROP TABLE TweetInfo
GO

CREATE TABLE TweetInfo
(
	TweetID bigint primary key not null,
	CreatedAt datetime not null,
	Sentiment int not null, -- 0=negative, 2=neutral, 4=positive
	Topic varchar(50) null
)
GO

IF OBJECT_ID('sp_InsertTweetInfo', 'P') IS NOT NULL
  DROP PROCEDURE sp_InsertTweetInfo
GO

CREATE PROCEDURE sp_InsertTweetInfo
	@TweetID bigint,
	@CreatedAt datetime,
	@Sentiment int,
	@Topic varchar(100)
AS
BEGIN
	INSERT INTO [dbo].[TweetInfo]
			   ([TweetID]
			   ,[CreatedAt]
			   ,[Sentiment]
			   ,[Topic])
	VALUES	(@TweetID, @CreatedAt, @Sentiment, @Topic)
END
GO

--exec sp_InsertTweetInfo 1, '2012-1-1 08:00:00', 0, 'windows 8'
--exec sp_InsertTweetInfo 2, '2012-1-2 08:00:01', 4, 'surface'
--exec sp_InsertTweetInfo 3, '2012-1-3 08:00:02', 2, 'office'