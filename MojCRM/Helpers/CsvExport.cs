using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MojCRM.Helpers
{
    public class CsvExport<T> where T : class
    {
        public IQueryable<T> Objects;
        public char Delimiter = ',';

        public CsvExport(IQueryable<T> objects)
        {
            Objects = objects;
        }

        public CsvExport(IQueryable<T> objects, char delimiter)
        {
            Objects = objects;
            Delimiter = delimiter;
        }

        public string Export()
        {
            return Export(true);
        }

        public string Export(bool includeHeaderLine)
        {

            StringBuilder sb = new StringBuilder();
            //Get properties using reflection.
            IList<PropertyInfo> propertyInfos = typeof(T).GetProperties();

            if (includeHeaderLine)
            {
                //add header line.
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    sb.Append(propertyInfo.Name).Append(Delimiter);
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            //add value for each property.
            foreach (T obj in Objects)
            {
                foreach (PropertyInfo propertyInfo in propertyInfos)
                {
                    var test = propertyInfo.GetValue(obj, null);
                    sb.Append(MakeValueCsvFriendly(propertyInfo.GetValue(obj, null))).Append(Delimiter);
                }
                sb.Remove(sb.Length - 1, 1).AppendLine();
            }

            return sb.ToString();
        }

        //export to a file.
        public void ExportToFile(string path, bool includeHeader = false)
        {
            File.WriteAllText(path, Export(includeHeader), Encoding.UTF8);
        }

        //export as binary data.
        public byte[] ExportToBytes(bool includeHeader = false)
        {
            return Encoding.UTF8.GetBytes(Export(includeHeader));
        }

        //get the csv value for field.
        private string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is Nullable && ((INullable)value).IsNull) return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();

            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';

            return output;

        }
    }
}