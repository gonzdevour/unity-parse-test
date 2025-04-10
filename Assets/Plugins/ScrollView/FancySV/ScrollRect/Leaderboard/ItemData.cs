﻿/*
 * FancyScrollView (https://github.com/setchi/FancyScrollView)
 * Copyright (c) 2020 setchi
 * Licensed under MIT (https://github.com/setchi/FancyScrollView/blob/master/LICENSE)
 */

using System.Diagnostics;

namespace FancyScrollView.Leaderboard
{
    class ItemData
    {
        // Leaderboard
        public string Rank { get; }
        public string UserName { get; }
        public string Score { get; }
        public string ImageUrl { get; }
        public string Message { get; }

        public ItemData(string rank = "0", string userName = "UNKNOWN", string score = "No-Data", string imageUrl = "", string message = "")
        {
            Rank = rank;
            UserName = userName;
            Score = score;
            ImageUrl = imageUrl;
            Message = message;
        }
    }
}
