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
             */
            private Canvas paintField;
            private Path equationPath;
            private double[] y;
            private double xStart;
            private double xEnd;
            private double xStep;
            private int paintXOffset;
            private int paintYOffset;
            private int paintStep;
            private int startXMainLine;
            private int startYMainLine;
            private bool isCentering;

            public event PropertyChangedEventHandler PropertyChanged;

            public VisualEquation() : this(null, 0, 0, 0, 0, 20, 1)
            {
            }
            public VisualEquation(Canvas paintField) : this(paintField, 0, 0, 0, 0, 20, 1)
            {
            }
            public VisualEquation(Canvas paintField, double xStart, double xEnd, int paintXOffset, int paintYOffset, int paintStep, double xStep)
            {
                this.paintField = paintField;
                XStart = xStart;
                XEnd = xEnd;
                PaintStep = paintStep;
                XStep = xStep;
                PaintXOffset = paintXOffset;
                PaintYOffset = paintYOffset;

                IsCentering = true;

                equationPath = new Path();
                equationPath.Stroke = new SolidColorBrush(Colors.Red);
                equationPath.StrokeThickness = 1;

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

                CalculateEquation();
            }

            public double[] Y { get { return y; } }
            public Canvas Field { get { return paintField; } }
            public Path EquationPath { get { return equationPath; } }

            public double XStart
            {
                get { return xStart; }
                set
                {
                    xStart = value;
                    OnPropertyChanged("XStart");
                    CalculateEquation();
                }
            }
            public double XEnd
            {
                get { return xEnd; }
                set
                {
                    xEnd = value;
                    OnPropertyChanged("XEnd");
                    CalculateEquation();
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
                    CalculateEquation();
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
                    CalculateEquation();
                }
            }
            public int PaintStep
            {
                get { return paintStep; }
                set
                {
                    if (value > 0)
                    {
                        paintStep = value;
                    }
                    else
                    {
                        paintStep = 20;
                    }
                    OnPropertyChanged("PaintStep");
                    PaintField();
                    CalculateEquation();
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
                    }
                    else
                    {
                        xStep = 1;
                    }
                    OnPropertyChanged("XStep");
                    CalculateEquation();
                }
            }
            public bool IsCentering
            {
                get { return isCentering; }
                set
                {
                    isCentering = value;
                    OnPropertyChanged("IsCentering");
                    PaintField();
                    CalculateEquation();
                }
            }

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
                if (IsCentering)
                {
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
                }

                xPath.Stroke = new SolidColorBrush(Colors.LightGray);
                xPath.StrokeThickness = 1;

                //Вертикальные линии
                for (int i = 0; i < paintField.ActualWidth; i += PaintStep)
                {
                    xGeometryGroup.Children.Add(new LineGeometry(new Point(i, 0), new Point(i, paintField.ActualHeight)));
                }

                xPath.Data = xGeometryGroup;
                paintField.Children.Add(xPath);

                yPath.Stroke = new SolidColorBrush(Colors.LightGray);
                yPath.StrokeThickness = 1;

                //Горизонтальные линии
                for (int i = 0; i < paintField.ActualHeight; i += PaintStep)
                {
                    yGeometryGroup.Children.Add(new LineGeometry(new Point(0, i), new Point(paintField.ActualWidth, i)));
                }

                yPath.Data = yGeometryGroup;
                paintField.Children.Add(yPath);

                mainLinePath.Stroke = new SolidColorBrush(Colors.Black);
                mainLinePath.StrokeThickness = 1;

                //Проверка на то, находяться ли линии ох, оу на поле
                if (startXMainLine + PaintYOffset * paintStep >= 0 && startXMainLine + PaintYOffset * paintStep <= paintField.ActualHeight)
                {
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(0, startXMainLine + PaintYOffset * paintStep),
                                                                        new Point(paintField.ActualWidth, startXMainLine + PaintYOffset * paintStep)));
                    //Стрелка
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(paintField.ActualWidth - 15, startXMainLine + PaintYOffset * paintStep - 5),
                                                                        new Point(paintField.ActualWidth, startXMainLine + PaintYOffset * paintStep)));
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(paintField.ActualWidth - 15, startXMainLine + PaintYOffset * paintStep + 5),
                                                                        new Point(paintField.ActualWidth, startXMainLine + PaintYOffset * paintStep)));
                }
                if (startYMainLine + PaintXOffset * paintStep >= 0 && startYMainLine + PaintXOffset * paintStep <= paintField.ActualWidth)
                {
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(startYMainLine + PaintXOffset * paintStep, 0),
                                                                        new Point(startYMainLine + PaintXOffset * paintStep, paintField.ActualHeight)));
                    //Стрелка
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(startYMainLine + PaintXOffset * paintStep + 5, 15),
                                                                        new Point(startYMainLine + PaintXOffset * paintStep, 0)));
                    mainLineGeometryGroup.Children.Add(new LineGeometry(new Point(startYMainLine + PaintXOffset * paintStep - 5, 15),
                                                                        new Point(startYMainLine + PaintXOffset * paintStep, 0)));
                }

                //Проврка на присутсвтвие линий в коллекции
                if (mainLineGeometryGroup.Children.Count > 0)
                {
                    mainLinePath.Data = mainLineGeometryGroup;
                    paintField.Children.Add(mainLinePath);
                }
            }
            public void CalculateEquation()
            {
                if (XStart == 0 && XEnd == 0 ||PaintStep == 0)
                {
                    return;
                }

                y = new double[(int)((Math.Abs(XStart) + Math.Abs(XEnd)) / XStep) + 1];

                int i = 0;
                for (double x = XStart; x <= XEnd && i < y.Length; x += XStep, i++)
                {
                    //-1 нужен для того, чтобы правильно отрисовывать рисунок (инвертировать направление, т.к. чем меньше координата, тем выше
                    y[i] = -1 * (x * x - 2 * Math.Sin(x));
                }

                DrawEquation();
            }

            private void DrawEquation()
            {
                int index = paintField.Children.IndexOf(equationPath);
                if (index >= 0)
                {
                    paintField.Children.RemoveAt(index);
                }

                GeometryGroup geometryGroup = new GeometryGroup();

                int i = 0;
                for (double x = XStart; i < y.Length - 1; i++, x += XStep)
                {
                    //Проверка на границы поля 
                    if (x * PaintStep + PaintXOffset * PaintStep + startYMainLine >= 0 &&
                        x * PaintStep + XStep * PaintStep + PaintXOffset * PaintStep + startYMainLine <= paintField.ActualWidth &&
                        y[i] * PaintStep + PaintYOffset * PaintStep + startXMainLine >= 0 &&
                        y[i + 1] * PaintStep + PaintYOffset * PaintStep + startXMainLine <= paintField.ActualHeight)
                    {
                        geometryGroup.Children.Add(new LineGeometry(new Point(x * PaintStep + PaintXOffset * PaintStep + startYMainLine, y[i] * PaintStep + PaintYOffset * PaintStep + startXMainLine),
                                                                    new Point(x * PaintStep + XStep * PaintStep + PaintXOffset * PaintStep + startYMainLine, y[i + 1] * PaintStep + PaintYOffset * PaintStep + startXMainLine)));
                    }
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
        }

        private void PaintField()
        {
            //DataContext нужен для работы привязки компонентов формы к текущему объекту
            VisualEquation visualEquation;
            if (!(DataContext is VisualEquation) || DataContext == null)
            {
                visualEquation = new VisualEquation(MainPaintField);
                DataContext = visualEquation;
            } else
            {
                visualEquation = (VisualEquation)DataContext;
                visualEquation.PaintField();
                visualEquation.CalculateEquation();
            }
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
