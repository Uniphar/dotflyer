namespace DotFlyer.Service.AzureDataExplorer.Tables;

public abstract class BaseTable
{
    public required string TableName { get; set; }

    public required string MappingName { get; set; }

    public required List<Column> Schema { get; set; }

    public class Column
    {
        public required string Name { get; set; }

        public required string Type { get; set; }
    }

    public List<ColumnMapping> Mapping
    {
        get
        {
            List<ColumnMapping> columnMapping = [];

            Schema.ForEach(column =>
            {
                columnMapping.Add(new()
                {
                    ColumnName = column.Name,
                    Properties = new()
                    {
                        { MappingConsts.Path, $"$.{column.Name}" }
                    }
                });
            });

            return columnMapping;
        }
    }
}