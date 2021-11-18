using System;

namespace API.Extensions
{
    public static class DateTimeExtentions
    {
        //calculating the age base on the date given
        public static int CalculateAge(this DateTime dob)
        {
            var today = DateTime.Today;
            var age =today.Year - dob.Year;
            //if birthday haven't been done that year minus the age by 1
            if (dob.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}
