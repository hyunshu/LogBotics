
using FRC_App.Models;
using SQLitePCL;

//Secound (secound-highest) level of the FRC data structure:
//i.e. holds all files in the CSV file family of a data session
public class Session {
    public string Name { get; set; }
    public List<DataType> DataTypes { get; set; }

    public DataImport GetImport() {
        List<List<string>> dataUnits = new List<List<string>>{};
        if (this.DataTypes == null || !this.DataTypes.Any()){
            return new DataImport(this.Name, new List<string>{}, new List<List<string>>{});
        }
        foreach (DataType type in this.DataTypes) {
            dataUnits.Add(type.getColumnLabels());
        }
        
        return new DataImport(this.Name, getDataTypeNames(), dataUnits);
    }

    public List<List<List<double>>> getRawData() {
        List<List<List<double>>> rawData = new List<List<List<double>>>{};

        if (this.DataTypes == null || !this.DataTypes.Any()){
            return rawData;
        }
        foreach (DataType type in this.DataTypes) {
            List<List<double>> typeData = new List<List<double>>{};
            foreach (Column column in type.Columns) {
                typeData.Add(column.Data);
            }
            rawData.Add(typeData);
        }

        return rawData;
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


    /**
     * --- Copy() ---
     * Returns the new Session object that is identical to this session object in every way.
     * Can be used to duplicate sessions.
     * @return Session
     */
    public Session Copy() {
        Session copy = new Session();
        copy.Name = this.Name;
        copy.DataTypes = new List<DataType>{};
        foreach (DataType type in this.DataTypes) {
            copy.DataTypes.Add(type.Copy());
        }
        
        return copy;
    }
}
