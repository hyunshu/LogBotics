
//Third (secound-to-last) level of the FRC data structure:
//i.e. holds all Columns in a CSV file
public class DataType {
    public string Name { get; set; }
    public List<Column> Columns { get; set; }

    public DataType() { }

    public DataType(string Name, List<Column> Columns) {
        this.Name = Name;
        this.Columns = Columns;
    }

    /**
     * --- getColumnLabels() ---
     * Returns the list of column labels (Time (s), Angular Velocity (rpm), ect) 
     * that correspond to a dataType object. This should
     * be used to generate the buttons for the second selection criteria for
     * getting a data axis for plotting ect.
     * @return List<string>
     */
    public List<string> getColumnLabels() {
        List<string> columnLabels = new List<string>{};

        foreach (Column column in this.Columns)
        {
            columnLabels.Add(column.Label);
        }

        return columnLabels;
    }

    /**
     * --- getColumn() ---
     * Returns the Column object that corresponds to the columnLabel. For example
     * after the second set of axis selection buttons is chosen (choosing a column)
     * this would convert that string into the corresponding Column object that holds 
     * the data and label.
     * @param columnLabel
     * @return Column
     */
    public Column getColumn(string columnLabel) {
        foreach (Column column in this.Columns)
        {
            if (String.Equals(column.Label,columnLabel)) {
                return column;
            }
        }
        return null;
    }

    public DataType Copy() {
        DataType copy = new DataType();
        copy.Name = this.Name;
        copy.Columns = new List<Column>{};
        foreach (Column col in this.Columns) {
            copy.Columns.Add(col.Copy());
        }
        
        return copy;
    }

}