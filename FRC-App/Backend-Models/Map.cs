using SkiaSharp;

public class Map {
    private List<double> xPos { get; set; }
    private List<double> yPos { get; set; }

    public Map(Column time, Column xAccel, Column yAccel) {
        int timeSize = time.Data.Count;
        int xSize = xAccel.Data.Count;
        int ySize = yAccel.Data.Count;
        if (xSize != timeSize || ySize != timeSize)
        {
            throw new AxesDifferentLengthsException("Error! The time, x-axis acceleration, and y-axis acceleration must have the same number of elements as each other for mapping.");
        }

        List<double> xVel = cumtrapz(time.Data,xAccel.Data);
        List<double> yVel = cumtrapz(time.Data,yAccel.Data);

        this.xPos = cumtrapz(time.Data,xVel);
        this.yPos = cumtrapz(time.Data,yVel);
    }

    public SKBitmap GenerateGrid() {
        return new SKBitmap(this.xPos.Count,this.yPos.Count,false);
    }

    public SKPoint[] GeneratePath() {
        SKPoint[] path = new SKPoint[xPos.Count];
        for (int i = 0; i < path.Count(); i ++) {
            path[i] = new SKPoint((float) xPos[i], (float) yPos[i]);
        }

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