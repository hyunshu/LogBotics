using Microcharts;
using SkiaSharp;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.SKCharts;


public class AxesDifferentLengthsException : Exception {
    public AxesDifferentLengthsException(string message)
        : base(message) { }
 }

public class Plot {
    public string Title { get; set; }
    public string XLabel { get; set; }
    public string YLabel { get; set; }
    public int numPoints { get; set; }
    public ChartEntry[] chart { get; set; }
    private int width = 900;
    private int height = 600;
    private Column x;
    private Column y;

    public Plot(Column x, Column y) {
        this.Title = y.Label + " vs " + x.Label;
        this.XLabel = x.Label;
        this.YLabel = y.Label;

        int xSize = x.Data.Count;
        int ySize = y.Data.Count;
        if (xSize != ySize)
        {
            throw new AxesDifferentLengthsException("Error! The x-axis must have the same number of elements as the y-axis for plotting.");
        }
        this.numPoints = xSize;

        this.x = x;
        this.y = y;

        chart = new ChartEntry[this.numPoints];
        string plotColor = getRandomHexColor(); 

        for (int i = 0; i < this.numPoints; i++)
        {
            chart[i] = new ChartEntry((float) y.Data[i])
            {
                Label = x.Data[i].ToString(),  // i.e time (x domain)
                ValueLabel = y.Data[i].ToString("#.#####"), // i.e. spin angle (y domain)
                Color = SKColor.Parse(plotColor)
            };
        }
    }

    public SKCartesianChart GetLineChart() {
        var chart = new SKCartesianChart
        {
            Width = width,
            Height = height,
            Series = new[]
            {
                new LineSeries<(double X, double Y)>
                {
                    Values = getValues(),
                }
            },
            Background = SKColor.Parse(getRandomHexColor()),
            XAxes = new[] { new Axis { Labeler = value => this.XLabel } },
            YAxes = new[] { new Axis { Labeler = value => this.YLabel } }
        };

        return chart;
    }

    public SKPolarChart GetPolarChart() {
        var chart = new SKPolarChart
        {
            Width = width,
            Height = height,
            Series = new[]
            {
                new PolarLineSeries<(double X, double Y)>
                {
                    Values = getValues(),
                }
            },
            Background = SKColor.Parse(getRandomHexColor()),
            AngleAxes = new[] { new PolarAxis { Labeler = value => this.XLabel } },
            RadiusAxes = new[] { new PolarAxis { Labeler = value => this.YLabel } }
        };

        return chart;
    }

    public SKCartesianChart GetScatterChart() {
        var chart = new SKCartesianChart
        {
            Width = width,
            Height = height,
            Series = new[]
            {
                new ScatterSeries<(double X, double Y)>
                {
                    Values = getValues(),
                }
            },
            Background = SKColor.Parse(getRandomHexColor()),
            XAxes = new[] { new Axis { Labeler = value => this.XLabel } },
            YAxes = new[] { new Axis { Labeler = value => this.YLabel } }
        };

        return chart;
    }

    private List<(double X, double Y)> getValues() {
        var values = new List<(double X, double Y)>{};
        for (int i = 0; i < this.numPoints; i++) {
            values.Add((x.Data[i],y.Data[i]));
        }
        return values;
    }

    public bool SameAxisCheck() {
        return this.x.Equals(this.y);
    }

    public string getRandomHexColor() {

        const string letters = "0123456789ABCDEF";

        string color = "#";

        for (int i = 0; i < 6; i++) {
            Random rnd = new Random();
            double index = rnd.Next(0,16);
            color += letters[(int)index];
        }

        return color;
    }

}
