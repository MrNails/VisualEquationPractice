using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace VisualEquation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        class VisualEquation : INotifyPropertyChanged
        {
            /*
             *  startXMainLine - определяет начальную точку линии ох
             *  startYMainLine - определяет начальную точку линии оу
             *  equationPath - это само уравнение
             *  paintXOffset - смещение оси ох
             *  paintYOffset - смещение оси оу
             *  drawingCoeff - коеффициент рисования уравнения 
             *  y - функция, которая принимает double и возвращает double? (вместо числа можно поставить null)
             *  
             */
            private Canvas paintField;
            private Path equationPath;
            private double xStart;
            private double xEnd;
            private double xStep;
            private double drawingCoeff;
            private int paintXOffset;
            private int paintYOffset;
            private int paintStep;
            private int startXMainLine;
            private int startYMainLine;

            public event PropertyChangedEventHandler PropertyChanged;

            public VisualEquation() : this(null, 0, 0, 1, default, 0, 0)
            {
            }
            public VisualEquation(Canvas paintField, double xStart, double xEnd, double xStep, Func<double, double?> y) : this(paintField, xStart, xEnd, xStep, y, 0, 0)
            {
            }
            public VisualEquation(Canvas paintField, double xStart, double xEnd, double xStep, Func<double, double?> y, int paintXOffset, int paintYOffset)
            {
                this.paintField = paintField;
                XStart = xStart;
                XEnd = xEnd;
                PaintStep = 30;
                XStep = xStep;
                PaintXOffset = paintXOffset;
                PaintYOffset = paintYOffset;
                Y = y;
                drawingCoeff = 1;

                equationPath = new Path();
                equationPath.Stroke = new SolidColorBrush(Colors.Red);
                equationPath.StrokeThickness = 1;
            }

            public Canvas Field { get { return paintField; } }
            public Path EquationPath { get { return equationPath; } }

            public double XStart
            {
                get { return xStart; }
                set
                {
                    xStart = value;
                    OnPropertyChanged("XStart");
                    DrawEquation();
                }
            }
            public double XEnd
            {
                get { return xEnd; }
                set
                {
                    xEnd = value;
                    OnPropertyChanged("XEnd");
                    DrawEquation();
                }
            }
            public double XStep
            {
                get { return xStep; }
                set
                {
                    if (value > 0)
                    {
                        xStep = value;
                        OnPropertyChanged("XStep");
                        DrawEquation();
                    }
                }
            }
            public double DrawingCoeff
            {
                get { return drawingCoeff; }
                set
                {
                    if (value > 0)
                    {
                        drawingCoeff = value;
                        OnPropertyChanged("DrawingCoeff");
                        DrawEquation();
                    }
                }
            }
            public int PaintXOffset
            {
                get { return paintXOffset; }
                set
                {

                    paintXOffset = value;

                    OnPropertyChanged("PaintXOffset");
                    PaintField();
                    DrawEquation();
                }
            }
            public int PaintYOffset
            {
                get { return paintYOffset; }
                set
                {

                    paintYOffset = value;

                    OnPropertyChanged("PaintYOffset");
                    PaintField();
                    DrawEquation();
                }
            }
            public int PaintStep
            {
                get { return paintStep; }
                set
                {
                    if (value > 0)
                    {
                        if (value < 5)
                        {
                            DrawingCoeff /= 10;
                            value = 60;
                        } 
                        else if (value > 60)
                        {
                            DrawingCoeff *= 10;
                            value = 5;
                        }

                        paintStep = value;
                        OnPropertyChanged("PaintStep");
                        PaintField();
                        DrawEquation();
                    }
                }
            }

            public Func<double, double?> Y { get; set; }

            public void PaintField()
            {
                //Очистка поля перед рисованием
                paintField.Children.Clear();

                Path xPath = new Path();
                Path yPath = new Path();
                Path mainLinePath = new Path();
                GeometryGroup xGeometryGroup = new GeometryGroup();
                GeometryGroup yGeometryGroup = new GeometryGroup();
                GeometryGroup mainLineGeometryGroup = new GeometryGroup();

                //Производит центрирование осей ох и оу
                if ((int)(paintField.ActualHeight / 2) % PaintStep == 0)
                {
                    startXMainLine = (int)(paintField.ActualHeight / 2);
                }
                else
                {
                    startXMainLine = (int)(paintField.ActualHeight / 2) - (int)(paintField.ActualHeight / 2) % PaintStep;
                }

                if ((int)(paintField.ActualWidth / 2) % PaintStep == 0)
                {
                    startYMainLine = (int)(paintField.ActualWidth / 2);
                }
                else
                {
                    startYMainLine = (int)(paintField.ActualWidth / 2) - (int)(paintField.ActualWidth / 2) % PaintStep;
                }

                xPath.Stroke = new SolidColorBrush(Colors.LightGray);
                xPath.StrokeThickness = 1;

                //Вертикальные линии
                for (int i = -PaintStep; i < paintField.ActualWidth; i += PaintStep)
                {
                    xGeometryGroup.Children.Add(new LineGeometry(new Point(i + (PaintXOffset % PaintStep), 0),
                                                                 new Point(i + (PaintXOffset % PaintStep), paintField.ActualHeight)));
                }

                xPath.Data = xGeometryGroup;
                paintField.Children.Add(xPath);

                yPath.Stroke = new SolidColorBrush(Colors.LightGray);
                yPath.StrokeThickness = 1;

                //Горизонтальные линии
                for (int i = -PaintStep; i < paintField.ActualHeight; i += PaintStep)
                {

                    yGeometryGroup.Children.Add(new LineGeometry(new Point(0, i + (PaintYOffset % PaintStep)),
                                                                 new Point(paintField.ActualWidth, i + (PaintYOffset % PaintStep))));
                }

                yPath.Data = yGeometryGroup;
                paintField.Children.Add(yPath);

                mainLinePath.Stroke = new SolidColorBrush(Colors.Black);
                mainLinePath.StrokeThickness = 1;

                //Проверка на то, находяться ли линии ох, оу на поле
                if (startXMainLine + PaintYOffset >= 0 && startXMainLine + PaintYOffset <= paintField.ActualHeight)
                {
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(0, startXMainLine + PaintYOffset + PaintStep),
                                                                        new Point(paintField.ActualWidth, startXMainLine + PaintYOffset + PaintStep)));
                    //Стрелка
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(paintField.ActualWidth - 15, startXMainLine + PaintYOffset - 5 + PaintStep),
                                                                        new Point(paintField.ActualWidth, startXMainLine + PaintYOffset + PaintStep)));
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(paintField.ActualWidth - 15, startXMainLine + PaintYOffset + 5 + PaintStep),
                                                                        new Point(paintField.ActualWidth, startXMainLine + PaintYOffset + PaintStep)));
                }
                if (startYMainLine + PaintXOffset >= 0 && startYMainLine + PaintXOffset <= paintField.ActualWidth)
                {
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(startYMainLine + PaintXOffset, 0),
                                                                        new Point(startYMainLine + PaintXOffset, paintField.ActualHeight)));
                    //Стрелка
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(startYMainLine + PaintXOffset + 5, 15),
                                                                        new Point(startYMainLine + PaintXOffset, 0)));
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(startYMainLine + PaintXOffset - 5, 15),
                                                                        new Point(startYMainLine + PaintXOffset, 0)));
                }

                //Проврка на присутсвтвие линий в коллекции
                if (mainLineGeometryGroup.Children.Count > 0)
                {
                    mainLinePath.Data = mainLineGeometryGroup;
                    paintField.Children.Add(mainLinePath);
                }
            }

            public void DrawEquation()
            {
                if (XStart == 0 && XEnd == 0 || PaintStep == 0 || xStep == 0)
                {
                    return;
                }

                int index = paintField.Children.IndexOf(equationPath);
                if (index >= 0)
                {
                    paintField.Children.RemoveAt(index);
                }

                GeometryGroup geometryGroup = new GeometryGroup();
                double? yStart, yEnd;
                double _xStart, _xEnd;

                for (double x = XStart; x < xEnd; x += XStep)
                {
                    yStart = -1.0 * Y(x) * PaintStep * drawingCoeff + PaintYOffset + startXMainLine + PaintStep;
                    yEnd = -1.0 * Y(x + xStep) * PaintStep * drawingCoeff + PaintYOffset + startXMainLine + PaintStep;
                    _xStart = x * PaintStep * drawingCoeff + PaintXOffset + startYMainLine;
                    _xEnd = (x + xStep) * PaintStep * drawingCoeff + PaintXOffset + startYMainLine;

                    //Если не удалось посчитать уравнение
                    if (yStart == null || yEnd == null)
                    {
                        continue;
                    }

                    //Проверка на границы поля 
                    if (_xStart <= 0)
                    {
                        _xStart = 0;
                    }
                    else if (_xStart >= paintField.ActualWidth)
                    {
                        _xStart = paintField.ActualWidth;
                    }

                    if (_xEnd <= 0)
                    {
                        _xEnd = 0;
                    }
                    else if (_xEnd >= paintField.ActualWidth)
                    {
                        _xEnd = paintField.ActualWidth;
                    }

                    if (yStart <= 0)
                    {
                        yStart = 0;
                    }
                    else if (yStart >= paintField.ActualHeight)
                    {
                        yStart = paintField.ActualHeight;
                    }

                    if (yEnd.Value <= 0)
                    {
                        yEnd = 0;
                    }
                    else if (yEnd.Value >= paintField.ActualHeight)
                    {
                        yEnd = paintField.ActualHeight;
                    }

                    if (_xStart == _xEnd || yStart == yEnd)
                    {
                        continue;
                    }

                    
                    geometryGroup.Children.Add(new LineGeometry(new Point(_xStart, yStart.Value),
                                                                new Point(_xEnd, yEnd.Value)));
                }

                equationPath.Data = geometryGroup;
                paintField.Children.Add(equationPath);
            }

            public void OnPropertyChanged([CallerMemberName]string prop = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
            }
        }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = null;
            this.Loaded += WindowLoaded;
            this.SizeChanged += WindowSizeChanged;
            this.MinHeight = 350;

            this.MouseWheel += (obj, arg) =>
            {
                if (DataContext is VisualEquation && DataContext != null)
                {
                    VisualEquation equation = (VisualEquation)DataContext;
                    if (arg.Delta > 0)
                    {
                        equation.PaintStep++;
                    }
                    else if (arg.Delta < 0)
                    {
                        equation.PaintStep--;
                    }
                }
            };
        }

        private void PaintField()
        {
            //DataContext нужен для работы привязки компонентов формы к текущему объекту
            VisualEquation visualEquation;
            if (!(DataContext is VisualEquation) || DataContext == null)
            {
                visualEquation = new VisualEquation(MainPaintField, 0, 0, 1, Equation);
                DataContext = visualEquation;
            }
            else
            {
                visualEquation = (VisualEquation)DataContext;
                visualEquation.PaintField();
                visualEquation.DrawEquation();
            }
        }

        private double? Equation(double x)
        {
            double? y;
            y = x * x - 2 * Math.Sin(x);
            return y;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            PaintField();
        }
        private void WindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            PaintField();
        }
        private void PaintStepTextChanged(object sender, TextChangedEventArgs e)
        {
            PaintField();
        }
    }
}
