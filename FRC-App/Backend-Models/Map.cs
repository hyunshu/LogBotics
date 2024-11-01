using SkiaSharp;

public class SameAxisException : Exception {
    public SameAxisException(string message)
        : base(message) { }
 }

public class Map {
    private List<double> xPos { get; set; }
    private List<double> yPos { get; set; }

    /**
    * --- Map() ---
    * Constructs a map object from the accelorometer data by computing the discrete 
    * integral (via the trapazoidal method) to determine the position over time.
    * @param time
    * @param xAccel
    * @param yAccel
    */
    public Map(Column time, Column xAccel, Column yAccel) {
        int timeSize = time.Data.Count;
        int xSize = xAccel.Data.Count;
        int ySize = yAccel.Data.Count;
        if (xSize != timeSize || ySize != timeSize)
        {
            throw new AxesDifferentLengthsException("Error! The time, x-axis acceleration, and y-axis acceleration must have the same number of elements as each other for mapping.");
        }
        if (time.Equals(xAccel) || time.Equals(yAccel) || xAccel.Equals(yAccel)) {
            throw new SameAxisException("Error! You cannot select 2 or more of time, xAcceleration, or yAcceleration to be the same as each other.");
        }

        List<double> xVel = cumtrapz(time.Data,xAccel.Data);
        List<double> yVel = cumtrapz(time.Data,yAccel.Data);

        this.xPos = cumtrapz(time.Data,xVel);
        this.yPos = cumtrapz(time.Data,yVel);
    }

    /**
    * --- GenerateGrid() ---
    * Generates the SKBitmap for this map object used as a parameter to
    * construct the SKCanvas object to display the map on the front-end.
    * @returns SKBitmap
    */
    public SKBitmap GenerateGrid() {
        return new SKBitmap(this.xPos.Count,this.yPos.Count,false);
    }

    /**
    * --- GeneratePath() ---
    * Generates the SKPath for this map object used as a parameter to
    * for the SKCanvas.DrawPoints() method to display the path on the map
    * in the front-end. This includes all the coordinates (x & y) of the 
    * position to draw the path of the robot on the virtual map.
    * @returns SKPath
    */
    public SKPath GeneratePath() {
        SKPath path = new SKPath();
        path.MoveTo((float) this.xPos[0], (float) this.yPos[0]);
        for (int i = 1; i < this.xPos.Count(); i ++) {
            path.LineTo((float) this.xPos[i], (float) this.yPos[i]);
        }
        path.Close();

        return path;
    }


    private static List<double> diff(List<double> data) {
        List<double> diffData = new List<double>(new double[data.Count()]);
        diffData[0] = 0;
        for (int i = 1; i < data.Count(); i++) {
            diffData[i] = data[i] - data[i-1];
        }
        return diffData;
    }

    private static List<double> cumtrapz(List<double> time, List<double> data) {
        List<double> dt = diff(time);
        List<double> integral = new List<double>(new double[data.Count()]);
        integral[0] = 0;
        for (int i = 1; i < data.Count(); i++) {
            double avg = (data[i] + data[i-1])/2;
            integral[i] = integral[i-1] + avg*dt[i];
        }
        return integral;
    }
}