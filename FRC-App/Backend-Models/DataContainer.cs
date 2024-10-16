
using FRC_App.Models;

class DataContainer {
    public List<DataType> DataTypes { get; set; }

    public DataContainer(User user) {
        DataImport dataStructure = new DataImport();
        List<List<List<double>>> rawData = dataStructure.RetrieveRawData(user);

        this.DataTypes = new List<DataType>{};

        int fileNum = 0;
        foreach (List<List<double>> type in rawData)
        {
            DataType dataType = new DataType();
            dataType.Name = dataStructure.dataTypes[fileNum];   
            dataType.Columns = new List<Column>{};

            int columnNum = 0;
            foreach (List<double> columnData in type)
            {
                Column column = new Column();
                column.Label = dataStructure.dataUnits[fileNum][columnNum];
                column.Data = columnData;

                dataType.Columns.Add(column);
                columnNum++;
            }

            this.DataTypes.Add(dataType);
            fileNum++;
        }
    }
}
