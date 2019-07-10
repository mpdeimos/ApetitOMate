using System;

namespace ApetitOMate.Core.Api.Apetito
{
    public class StorageLocation : IFormattable
    {
        public int Id { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            switch (format)
            {
                case nameof(Id):
                    return this.Id.ToString();
                case nameof(Name):
                    return this.Name.ToString();
                default:
                    return this.ToString();
            }
        }
    }
}