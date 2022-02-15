﻿using System.ComponentModel.DataAnnotations;

namespace DUT.Application.ViewModels.Setting
{
    public class SettingEditModel : SettingCreateModel
    {
        [Required]
        public int Id { get; set; }
    }
}
