
//Fourth (lowest) level of the FRC data structure:
//i.e. holds all the data of a column from a CSV file
public class Column {
    public string Label { get; set; }
    public List<double> Data { get; set; }

    public Column() { }

    public Column(string Label, List<double> Data) {
        this.Label = Label;
        this.Data = Data;
    }

    public bool Equals(Column target)
    {
        if (this.Label != target.Label) {
            return false;
        }
        int size = this.Data.Count;
        if (size != target.Data.Count) {
            return false;
        }
        for (int i = 0; i < size; i++) {
            if (this.Data[i] != target.Data[i]) {
                return false;
            }
        }
        return true;
    }

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