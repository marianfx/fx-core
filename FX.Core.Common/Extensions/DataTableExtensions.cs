using FX.Core.Common.Localization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace FX.Core.Common.Extensions
{
    public static class DataTableExtensions
    {
        #region Object(s) to DataTable
        /// <summary>
        /// Cretes a datatable, with Column names = dict's keys, column types = dict's values
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
        public static DataTable CreateDatatableSchema(this Dictionary<string, Type> columns)
        {
            var dt = new DataTable();
            foreach (var kvp in columns)
            {
                dt.Columns.Add(kvp.Key, kvp.Value);
            }
            return dt;
        }

        /// <summary>
        /// Transforms a list of objects of type t into a datatable (with type bindings)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="filteringColumns"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data, IEnumerable<string> filteringColumns = null)
        {
            DataTable output = new DataTable();
            var mainObjectProperties = typeof(T).GetProperties();
            if (filteringColumns != null && filteringColumns.Any()) // if columns are specified, use in the specified order
            {
                var properties = typeof(T).GetProperties();
                mainObjectProperties = filteringColumns
                    .Where(x => properties.Any(y => y.Name == x))
                    .Select(x => properties.FirstOrDefault(y => y.Name == x))
                    .ToArray();
            }

            // add old + new columns
            foreach (var property in mainObjectProperties)
            {
                var columnType = property == null ? typeof(string)
                        : Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                output.Columns.Add(property.Name, columnType);
            }

            // add data
            foreach (var item in data)
            {
                var row = output.NewRow();

                // add new and old column values
                foreach (var property in mainObjectProperties)
                {
                    row[property.Name] = property.GetValue(item) ?? DBNull.Value;
                }
                output.Rows.Add(row);
            }

            return output;
        }
        #endregion


        #region Datatable to object(s)
        /// <summary>
        /// Maps a DataTable to a list of objects of type T, matching the property names.
        /// </summary>
        /// <typeparam name="T">The type to map to.</typeparam>
        /// <param name="table">The datatable with data</param>
        /// <returns></returns>
        public static IList<T> ToList<T>(this DataTable table)
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            IList<T> result = new List<T>();

            try
            {
                foreach (var row in table.Rows)
                {
                    var item = (row as DataRow).CreateItemFromRow<T>(properties: properties);
                    result.Add(item);
                }

            }
            catch (Exception)
            {
                throw new Exception(Resources.CannotConvertToTheSpecifiedType + " (" + typeof(T).Name + ").");
            }

            return result;
        }


        /// <summary>
        /// Given a row from a datatable and a list of columns && object properties, tries mapping data from the row on the object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="itemToUpdate"></param>
        /// <param name="properties"></param>
        /// <param name="columnPropertyMappings"></param>
        /// <returns></returns>
        public static T CreateItemFromRow<T>(this DataRow row, T itemToUpdate = default, IEnumerable<PropertyInfo> properties = null, Dictionary<string, string> columnPropertyMappings = null)
        {
            if (properties == null)
                properties = typeof(T).GetProperties();

            T item = itemToUpdate;
            var typeT = typeof(T);
            var isPrimitive = typeT.IsPrimitive || typeT.IsValueType || (typeT == typeof(string));
            if (isPrimitive)
            {
                try
                {
                    item = (T)row[0];
                }
                catch { }
                return item;
            }

            try
            {
                if (item == null)
                    item = Activator.CreateInstance<T>();

                foreach (var property in properties)
                {
                    var propName = property.Name.ToLower();
                    foreach (DataColumn column in row.Table.Columns)
                    {
                        var columnName = column.ColumnName.ToLower();
                        if (columnPropertyMappings != null && columnPropertyMappings.Keys.Contains(column.ColumnName))
                            columnName = columnPropertyMappings[column.ColumnName].ToLower();

                        if (propName != columnName)
                            continue;

                        item = row.SetColumnValueFromRow(item, column.ColumnName, property);
                    }
                }
            }
            catch (Exception)
            {
                // Console.WriteLine(e.Message);
            }

            return item;
        }

        /// <summary>
        /// Maps a DataRow to an Entity (object) by matching against properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T ToEntity<T>(this DataRow row) where T : new()
        {
            IList<PropertyInfo> properties = typeof(T).GetProperties().ToList();
            return CreateItemFromRow<T>(row, properties: properties);
        }


        /// <summary>
        /// Given an object of type T and a property name + property info of that object, tries finding column value in a data row and sets it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="item"></param>
        /// <param name="columnName"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static T SetColumnValueFromRow<T>(this DataRow row, T item, string columnName, PropertyInfo property)
        {
            var result = row.GetColumnValueMapped(columnName, property.PropertyType);
            try
            {
                property.SetValue(item, result);
            }
            catch (Exception)
            {
                // Console.WriteLine("Cannot set value for {0}. Error: {1}", property.Name, ex.Message);
            }

            return item;
        }

        /// <summary>
        /// Given a property type and a column name, tries finding column value in a data row and returns it.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="columnName"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public static object GetColumnValueMapped(this DataRow row, string columnName, Type outputType)
        {
            var isNull = row[columnName] == null || row[columnName] == DBNull.Value;
            if (isNull)
                return null;

            var cellValue = row[columnName]?.ToString();
            var isNumber = int.TryParse(cellValue, out int theNumber) && outputType.IsNumberType();
            var isDouble = double.TryParse(cellValue, out double theDouble) && outputType.IsDoubleNumberType();
            var isDateTime = DateTime.TryParse(cellValue, out DateTime theDate) && outputType.IsDateType();
            var isBool = bool.TryParse(cellValue, out bool theBool) && outputType == typeof(bool);

            try
            {
                if (isDouble)
                    return theDouble;
                else if (isNumber)
                    return theNumber;
                else if (isDateTime)
                    return theDate.Date;
                else if (isBool)
                    return theBool;
            }
            catch
            {
                // Console.WriteLine("Cannot get value for {0} - {1}", columnName, cellValue);
            }

            return cellValue;
        }

        /// <summary>
        /// Given a property type and a column index, tries finding column value in a data row and returns it.
        /// </summary>
        /// <param name="row"></param>
        /// <param name="index"></param>
        /// <param name="outputType"></param>
        /// <returns></returns>
        public static object GetColumnValueMapped(this DataRow row, int index, Type outputType)
        {
            if (index > row.Table.Columns.Count)
                throw new Exception(Resources.InvalidColumnCount);
            var columnName = row.Table.Columns[index].ColumnName;
            return row.GetColumnValueMapped(columnName, outputType);
        }
        #endregion
    }
}
