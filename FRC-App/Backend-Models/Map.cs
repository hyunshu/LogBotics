
public class Map {
    private List<double> xPos { get; set; }
    private List<double> yPos { get; set; }

    public Map(Column time, Column xAccel, Column yAccel) {

        List<double> xVel = cumtrapz(time.Data,xAccel.Data);
        List<double> yVel = cumtrapz(time.Data,yAccel.Data);

        this.xPos = cumtrapz(time.Data,xVel);
        this.yPos = cumtrapz(time.Data,yVel);
    }

    private static List<double> diff(List<double> data) {
        List<double> diffData = new List<double>{data.Count()};
        diffData[0] = 0;
        for (int i = 1; i < data.Count(); i++) {
            diffData[i] = data[i] - data[i-1];
        }
        return diffData;
    }

    private static List<double> cumtrapz(List<double> time, List<double> data) {
        List<double> dt = diff(time);
        List<double> integral = new List<double>{data.Count()};
        integral[0] = 0;
        for (int i = 1; i < data.Count(); i++) {
            double avg = (data[i] + data[i-1])/2;
            integral[i] = avg*dt[i];
        }
        return integral;
    }
}