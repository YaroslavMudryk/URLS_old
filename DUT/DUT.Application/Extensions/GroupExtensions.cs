﻿using DUT.Domain.Models;
using System.Text;

namespace DUT.Application.Extensions
{
    public static class GroupExtensions
    {
        public static void IncreaseCourse(this Group group)
        {
            var value = group.Name[group.IndexNumber];
            var numberForIncrease = Int32.Parse(value.ToString());
            numberForIncrease = numberForIncrease + 1;
            group.Course = numberForIncrease;
            var sb = new StringBuilder();
            for (int i = 0; i < group.Name.Length; i++)
            {
                if (i == group.IndexNumber)
                {
                    sb.Append(numberForIncrease.ToString());
                }
                else
                {
                    sb.Append(group.Name[i]);
                }
            }
            group.Name = sb.ToString();
        }
    }
}
