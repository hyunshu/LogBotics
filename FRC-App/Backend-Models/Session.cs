
using FRC_App.Models;

//Secound (secound-highest) level of the FRC data structure:
//i.e. holds all files in the CSV file family of a data session
public class Session {
    public string Name { get; set; }
    public List<DataType> DataTypes { get; set; }

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
