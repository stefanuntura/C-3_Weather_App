﻿CREATE TABLE [dbo].[historical_weather]
(
    [timecode] INT NOT NULL, 
    [id] INT NULL, 
    [main] VARCHAR(50) NULL, 
    [description] VARCHAR(50) NOT NULL,
    [icon] VARCHAR(50) NULL,  
    [temp] FLOAT NOT NULL, 
    [feels_like] FLOAT NULL, 
    [temp_min] FLOAT NULL, 
    [temp_max] FLOAT NULL, 
    [pressure] INT NULL, 
    [humidity] INT NULL, 
    [wind_speed] FLOAT NULL, 
    [wind_deg] INT NULL, 
    [gust] FLOAT NULL,
    [sys_type] INT NULL,   
    [timezone] INT NULL, 
    CONSTRAINT [PK_historical_weather] PRIMARY KEY ([timecode]),  
)
