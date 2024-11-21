
using SkiaSharp;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.VisualElements;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView.Maui;
using LiveChartsCore.Kernel;
using LiveChartsCore.SkiaSharpView.Painting;


public class AxesDifferentLengthsException : Exception {
    public AxesDifferentLengthsException(string message)
        : base(message) { }
}

public class DataPoint {
    public double X;
    public double Y;

    public DataPoint(double x, double y) {
        X = x;
        Y = y;
    }
}

public class Plot {
    public string Title { get; set; }
    public string XLabel { get; set; }
    public string YLabel { get; set; }
    public int numPoints { get; set; }
    // public ChartEntry[] chart { get; set; }
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

        // chart = new ChartEntry[this.numPoints];
        string plotColor = getRandomHexColor(); 

        // for (int i = 0; i < this.numPoints; i++)
        // {
        //     chart[i] = new ChartEntry((float) y.Data[i])
        //     {
        //         Label = x.Data[i].ToString(),  // i.e time (x domain)
        //         ValueLabel = y.Data[i].ToString("#.#####"), // i.e. spin angle (y domain)
        //         Color = SKColor.Parse(plotColor)
        //     };
        // }
    }

    public CartesianChart GetLineChart(SKColor color) {
        var datapoints = getValues();

        var chart = new CartesianChart
        {
            Series = new[]
            {
                new LineSeries<DataPoint>
                {
                    Values = datapoints,
                    Mapping = (datapoint, index) => new(datapoint.X, datapoint.Y),
                    Stroke = new SolidColorPaint(color), 
                    Fill = null
                }
            },
            XAxes = new[] { new Axis { Labeler = value => this.XLabel } },
            YAxes = new[] { new Axis { Labeler = value => this.YLabel } },

            Title = new LabelVisual
            {
                Text = this.YLabel + " vs " + this.XLabel,
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            }
        };

        return chart;
    }

    // public PolarChart GetPolarChart() {
    //     var datapoints = getValues();

    //     var chart = new PolarChart
    //     {
    //         Series = new[]
    //         {
    //             new PolarLineSeries<DataPoint>
    //             {
    //                 Values = datapoints,
    //                 Mapping = (datapoint, index) => new(datapoint.X, datapoint.Y)
    //             }
    //         },
    //         AngleAxes = new[] { new PolarAxis { Labeler = value => this.XLabel } },
    //         RadiusAxes = new[] { new PolarAxis { Labeler = value => this.YLabel } },

    //         Title = new LabelVisual
    //         {
    //             Text = this.YLabel + " vs " + this.XLabel,
    //             TextSize = 20,
    //             Padding = new LiveChartsCore.Drawing.Padding(15)
    //         }
    //     };

    //     return chart;
    // }

    public CartesianChart GetScatterChart(SKColor color) {
        var datapoints = getValues();

        var chart = new CartesianChart
        {
            Series = new[]
            {
                new ScatterSeries<DataPoint>
                {
                    Values = datapoints,
                    Mapping = (datapoint, index) => new(datapoint.X, datapoint.Y),
                    Stroke = new SolidColorPaint(color),
                    Fill = new SolidColorPaint(color)
                }
            },
            XAxes = new[] { new Axis { Labeler = value => this.XLabel } },
            YAxes = new[] { new Axis { Labeler = value => this.YLabel } },

            Title = new LabelVisual
            {
                Text = this.YLabel + " vs " + this.XLabel,
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            }
        };

        return chart;
    }

     public CartesianChart GetStepLineChart(SKColor color) {
        var datapoints = getValues();

        var chart = new CartesianChart
        {
            Series = new[]
            {
                new StepLineSeries<DataPoint>
                {
                    Values = datapoints,
                    Mapping = (datapoint, index) => new(datapoint.X, datapoint.Y),
                    Stroke = new SolidColorPaint(color),
                    Fill = null
                
                }
            },
            XAxes = new[] { new Axis { Labeler = value => this.XLabel } },
            YAxes = new[] { new Axis { Labeler = value => this.YLabel } },

            Title = new LabelVisual
            {
                Text = this.YLabel + " vs " + this.XLabel,
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            }
        };

        return chart;
    }

     public CartesianChart GetRowChart(SKColor color) {
        var datapoints = getValues();

        var chart = new CartesianChart
        {
            Series = new[]
            {
                new RowSeries<DataPoint>
                {
                    Values = datapoints,
                    Mapping = (datapoint, index) => new(datapoint.X, datapoint.Y),
                    Stroke = new SolidColorPaint(color),
                    Fill = new SolidColorPaint(color)
                
                }
            },
            XAxes = new[] { new Axis { Labeler = value => this.XLabel } },
            YAxes = new[] { new Axis { Labeler = value => this.YLabel } },

            Title = new LabelVisual
            {
                Text = this.YLabel + " vs " + this.XLabel,
                TextSize = 20,
                Padding = new LiveChartsCore.Drawing.Padding(15)
            }
        };

        return chart;
    }

    public bool isLineChart(CartesianChart chart) {
        return chart.Series.First() is LineSeries<DataPoint>;
    }

    public bool isScatterChart (CartesianChart chart) {
        return chart.Series.First() is ScatterSeries<DataPoint>;
    }

    public bool isStepLineChart (CartesianChart chart) {
        return chart.Series.First() is StepLineSeries<DataPoint>;
    }

    public bool isRowChart (CartesianChart chart) {
        return chart.Series.First() is RowSeries<DataPoint>;
    }

    private List<DataPoint> getValues() {
        var datapoints = new List<DataPoint>();
        for (int i = 0; i < this.numPoints; i++) {
            DataPoint datapoint = new DataPoint(x.Data[i],y.Data[i]);
            datapoints.Add(datapoint);
        }

        return datapoints;

        // var values = new List<(double X, double Y)>{};
        // for (int i = 0; i < this.numPoints; i++) {
        //     values.Add((x.Data[i],y.Data[i]));
        // }
        // return values;
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
