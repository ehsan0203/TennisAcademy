﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TennisAcademy.Application.DTOs.UserScore
{
    public class AddCreditDto
    {
        public Guid UserId { get; set; }
        public int Amount { get; set; }
    }

}
