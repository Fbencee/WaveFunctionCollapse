using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WaveFunctionCollapse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private delegate void EmptyDelegate();

        private int[,] entropy;
        private string[,] images;
        private bool first;
        private int n;
        private string jsonString = "Rules.json";
        private int counter = 0;
        private Random rand = new Random();
        private Size size = new(700.0, 700.0);
        private List<Position> rules;
        private bool flag = false;
        private List<string> imageNames;
        private int index = 0;

        protected void DoEvents()
        {
            Dispatcher.CurrentDispatcher.Invoke(new EmptyDelegate(delegate { }), DispatcherPriority.Background);
        }

        public int[,] Entropy { get => entropy; set => entropy = value; }
        public string[,] Images { get => images; set => images = value; }
        public bool First { get => first; set => first = value; }
        public bool Flag
        {
            get => flag;
            set
            {
                flag = value;
            }
        }

        public MainWindow()
        {
            InitializeComponent();

            imageNames = new();
            imageNames.Add("Image_1");
            imageNames.Add("Image_2");
            imageNames.Add("Image_3");
            imageNames.Add("Image_4");
            imageNames.Add("Image_5");

            List<string> positions = new();
            positions.Add("top");
            positions.Add("left");
            positions.Add("bottom");
            positions.Add("right");

            rules = ReadJson(positions);

            first = true;
            n = Convert.ToInt32(Size_slider.Value);
            Entropy = new int[n, n];
            Images = new string[n, n];
            Size actualSize = new(size.Width / n - 1, size.Height / n - 1);

            for (int i = 0; i < n; i++)
            {
                ColumnDefinition column = new();
                column.Name = $"column{i}";
                column.Width = new GridLength(actualSize.Width);
                imageGrid.ColumnDefinitions.Add(column);
            }

            for (int j = 0; j < n; j++)
            {
                RowDefinition row = new();
                row.Name = $"row{j}";
                row.Height = new GridLength(actualSize.Height);
                imageGrid.RowDefinitions.Add(row);
            }

            imageGrid.Height = n * actualSize.Height;
            imageGrid.Width = n * actualSize.Width;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    Images[i, j] = String.Empty;
                }
            }

            CheckEntropy();
        }

        public List<Position> ReadJson(List<string> positions)
        {
            List<Position> output = new();

            foreach (string imageName in imageNames)
            {
                for (int i = 0; i < positions.Count; i++)
                {

                    if (String.IsNullOrEmpty(imageName))
                    {
                        Position position = new(5, positions[i], imageName);
                        for (int j = 0; j < 5; j++)
                        {

                            position.Images[j] = $"Image_{j + 1}";
                        }
                        output.Add(position);

                    }
                    else
                    {

                        using (JsonDocument document = JsonDocument.Parse(File.ReadAllText(jsonString)))
                        {
                            JsonElement root = document.RootElement;
                            JsonElement imageElement = root.GetProperty(imageName);
                            JsonElement rightPositionedElement = imageElement.GetProperty(positions[i]);
                            Position position = new(rightPositionedElement.GetArrayLength(), positions[i], imageName);

                            //foreach (JsonElement image in rightPositionedElement.EnumerateArray())
                            //{
                            //    positionimage.GetProperty("image").ToString();
                            //}

                            for (int j = 0; j < position.Length; j++)
                            {
                                position.Images[j] = rightPositionedElement[j].GetProperty("image").ToString();
                            }

                            output.Add(position);
                        }
                    }
                }

            }



            return output;
        }

        public void CheckEntropy()
        {
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    List<string> top = imageNames.ToList();
                    List<string> left = top.ToList();
                    List<string> bottom = top.ToList();
                    List<string> right = top.ToList();
                    List<string> common = new();
                    foreach (Position position in rules)
                    {

                        if (j != n - 1 && position.ImageName.Equals(images[i, j + 1]) && position.ActualPosition.Equals("top"))
                        {
                            bottom = position.Images.ToList();
                        }

                        if (i != n - 1 && position.ImageName.Equals(images[i + 1, j]) && position.ActualPosition.Equals("left"))
                        {
                            right = position.Images.ToList();
                        }

                        if (j != 0 && position.ImageName.Equals(images[i, j - 1]) && position.ActualPosition.Equals("bottom"))
                        {
                            top = position.Images.ToList();
                        }

                        if (i != 0 && position.ImageName.Equals(images[i - 1, j]) && position.ActualPosition.Equals("right"))
                        {
                            left = position.Images.ToList();
                        }
                    }

                    if (images[i, j] == String.Empty)
                    {
                        common = right.Intersect(bottom.Intersect(top.Intersect(left))).ToList();
                        entropy[i, j] = common.Count;
                    }
                    else
                    {
                        entropy[i, j] = 0;
                    }

                }
            }
        }

        public List<string> TheListOfImagesCanBeThere(int i, int j)
        {
            List<string> top = new();
            top.Add("Image_1");
            top.Add("Image_2");
            top.Add("Image_3");
            top.Add("Image_4");
            top.Add("Image_5");
            List<string> left = top.ToList();
            List<string> bottom = top.ToList();
            List<string> right = top.ToList();
            List<string> common = new();

            foreach (var item in rules)
            {

                if (j != n - 1 && item.ImageName.Equals(images[i, j + 1]) && item.ActualPosition.Equals("top"))
                {
                    bottom = item.Images.ToList();
                }

                if (i != n - 1 && item.ImageName.Equals(images[i + 1, j]) && item.ActualPosition.Equals("left"))
                {
                    right = item.Images.ToList();
                }

                if (j != 0 && item.ImageName.Equals(images[i, j - 1]) && item.ActualPosition.Equals("bottom"))
                {
                    top = item.Images.ToList();
                }

                if (i != 0 && item.ImageName.Equals(images[i - 1, j]) && item.ActualPosition.Equals("right"))
                {
                    left = item.Images.ToList();
                }
            }

            common = right.Intersect(bottom.Intersect(top.Intersect(left))).ToList();

            return common;
        }

        public int FindTheLowestEntropy(int numberOfRows, int numberOfColumns)
        {
            int lowest = 100;

            for (int i = 0; i < numberOfColumns; i++)
            {
                for (int j = 0; j < numberOfRows; j++)
                {
                    if (entropy[i, j] < lowest && entropy[i, j] != 0)
                    {
                        lowest = entropy[i, j];
                    }
                }
            }

            return lowest;
        }

        private void Next_button_Click(object sender, RoutedEventArgs e)
        {
            counter++;
            Size_slider.IsEnabled = false;

            try
            {

                if (counter <= n * n)
                {
                    CheckEntropy();

                    int lowestEntropy = FindTheLowestEntropy(n, n);
                    List<Tuple<int, int>> lowestEntropyPositions = new();

                    for (int i = 0; i < n; i++)
                    {
                        for (int j = 0; j < n; j++)
                        {
                            if (lowestEntropy == entropy[i, j])
                            {
                                lowestEntropyPositions.Add(new Tuple<int, int>(i, j));
                            }
                        }
                    }

                    int random1 = rand.Next(lowestEntropyPositions.Count);

                    List<string> usableImages = TheListOfImagesCanBeThere(lowestEntropyPositions[random1].Item1, lowestEntropyPositions[random1].Item2);

                    int random2 = rand.Next(usableImages.Count);

                    Image img = new();
                    img.Name = $"image{lowestEntropyPositions[random1].Item1}_{lowestEntropyPositions[random1].Item2}";
                    img.SetValue(Grid.ColumnProperty, lowestEntropyPositions[random1].Item1);
                    img.SetValue(Grid.RowProperty, lowestEntropyPositions[random1].Item2);
                    img.HorizontalAlignment = HorizontalAlignment.Stretch;
                    img.VerticalAlignment = VerticalAlignment.Stretch;
                    img.Stretch = Stretch.Fill;

                    img.Source = new BitmapImage(new Uri(System.IO.Path.GetFullPath($"{usableImages[random2]}.png")));
                    Images[lowestEntropyPositions[random1].Item1, lowestEntropyPositions[random1].Item2] = usableImages[random2];

                    imageGrid.Children.Add(img);

                    DoEvents();
                }
                else
                {
                    Next_button.IsEnabled = false;
                }
            }
            catch (Exception)
            {

                MessageBox.Show("This wave can't collapse.");
                New_button_Click(sender, e);
                return;
            }
        }

        private void Start_button_Click(object sender, RoutedEventArgs e)
        {
            Flag = !Flag;

            Next_button.IsEnabled = false;
            Size_slider.IsEnabled = false;

            if (Start_button.Content.Equals("Start"))
            {
                index = 0;
            }

            for (; index < n * n; index++)
            {
                if (Flag)
                {
                    Start_button.Content = "Stop";
                }
                else
                {
                    Start_button.Content = "Continue";
                    break;
                }
                Next_button_Click(sender, e);
            }

             ;

            if (index == n * n)
            {
                MessageBox.Show("The wave collapsed!");
            }
        }

        private void Size_slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (e.OldValue != 0)
            {
                imageGrid.ColumnDefinitions.Clear();
                imageGrid.RowDefinitions.Clear();

                n = Convert.ToInt32(e.NewValue);
                Entropy = new int[n, n];
                Images = new string[n, n];
                Size actualSize = new(size.Width / n - 1, size.Height / n - 1);

                for (int i = 0; i < n; i++)
                {
                    ColumnDefinition column = new();
                    column.Name = $"column{i}";
                    column.Width = new GridLength(actualSize.Width);
                    imageGrid.ColumnDefinitions.Add(column);
                }

                for (int j = 0; j < n; j++)
                {
                    RowDefinition row = new();
                    row.Name = $"row{j}";
                    row.Height = new GridLength(actualSize.Height);
                    imageGrid.RowDefinitions.Add(row);
                }

                imageGrid.Height = n * actualSize.Height;
                imageGrid.Width = n * actualSize.Width;

                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Images[i, j] = String.Empty;
                    }
                }
            }
        }

        private void New_button_Click(object sender, RoutedEventArgs e)
        {
            imageGrid.Children.Clear();
            counter = 0;
            index = n * n + 1;

            Next_button.IsEnabled = true;
            Size_slider.IsEnabled = true;
            Start_button.IsEnabled = true;
            Start_button.Content = "Start";
            flag = false;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    images[i, j] = String.Empty;
                }
            }
        }
    }
}
