
using FRC_App.Models;

//First (highest) level of the FRC data structure:
//i.e. holds all files in the CSV file family
public class DataContainer {
    private List<DataType> DataTypes { get; set; }

    /**
     * --- DataContainer() ---
     * Constructs a DataContainer object based on the user data from
     * the user parameter. Automatically generates all the DataType 
     * objects and their subsequent Column objects completing the full
     * data structure with names and data for every type and column under
     * DataTypes. Allows below getter methods to work.
     * @param User
     */
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

    /**
     * --- getDataTypeNames() ---
     * Returns the list of dataType names (motor, sensor, ect). This should
     * be used to generate the buttons for the first selection criteria for
     * getting a data axis for plotting ect.
     * @return List<string>
     */
    public List<string> getDataTypeNames() {
        List<string> typeNames = new List<string>{};

        foreach (DataType type in this.DataTypes)
        {
            typeNames.Add(type.Name);
        }

        return typeNames;
    }

    /**
     * --- getDataType() ---
     * Returns the DataType object that corresponds to the datatypeName. For example
     * after the first set of axis selection buttons is chosen (choosing a dataType)
     * this would convert that string into the corresponding DataType object that holds 
     * the columns with data & labels.
     * @param dataTypeName
     * @return DataType
     */
    public DataType getDataType(string dataTypeName) {
        foreach (DataType type in this.DataTypes)
        {
            if (String.Equals(type.Name,dataTypeName)) {
                return type;
            }
        }
        return null;
    }
}
