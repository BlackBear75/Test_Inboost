﻿using Test_INBOOST.Base;

namespace Test_INBOOST.Entity.WeatherHistory;

public class WeatherHistory : BaseData
{
    public long UserId { get; set; }
    public string City { get; set; } 
    public string WeatherData { get; set; } 
}