
//Third (lowest) level of the FRC data structure:
//i.e. holds all the data of a column from a CSV file
public class Column {
    public string Label { get; set; }
    public List<double> Data { get; set; }

    public Column Copy() {
        Column copy = new Column();
        copy.Label = this.Label;
        copy.Data = new List<double>{};
        foreach (double x in this.Data) {
            copy.Data.Add(x);
        }
        
        return copy;
    }
}