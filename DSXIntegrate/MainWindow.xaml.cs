using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using YuanliCore.AffineTransform;
using YuanliCore.CameraLib;
using YuanliCore.Interface;
using YuanliCore.Views.CanvasShapes;

namespace DSXIntegrate
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private string logMessage;
        private IEnumerable<MatchResult> resultPoint;
        private BitmapSource mapImage;
        private WriteableBitmap originImage;
        private BitmapSource heightContourImage;

        private double oriPointX1, oriPointY1, oriPointX2, oriPointY2, oriPointX3, oriPointY3;
        private double mapPointX1, mapPointY1, mapPointX2, mapPointY2, mapPointX3, mapPointY3;
        private ClickOrigin buttonClickOrigin;
        private ClickMap buttonClickMap;
        private ObservableCollection<ROIShape> mapdrawings = new ObservableCollection<ROIShape>();
        private ObservableCollection<ROIShape> drawings = new ObservableCollection<ROIShape>();
        private System.Windows.Point mousePixcel;
        private System.Windows.Point mapmousePixcel;
        private bool isHorizontalMirror, isVerticalMirror;

        private int rValue, gValue, bValue;
        private double heightValueMax = 1240.56, heightValueMin = -562.22;
        private Rect[] transToOriRect;
        private int roiSize = 50;


        public MainWindow()
        {
            InitializeComponent();
        }

        public ObservableCollection<ROIShape> MapDrawings { get => mapdrawings; set => SetValue(ref mapdrawings, value); }
        public ObservableCollection<ROIShape> Drawings { get => drawings; set => SetValue(ref drawings, value); }



        public BitmapSource MapImage { get => mapImage; set => SetValue(ref mapImage, value); }
        public WriteableBitmap OriginImage { get => originImage; set => SetValue(ref originImage, value); }
        public BitmapSource HeightContourImage { get => heightContourImage; set => SetValue(ref heightContourImage, value); }


        public string LogMessage { get => logMessage; set => SetValue(ref logMessage, value); }
        /// <summary>
        /// 滑鼠在影像內 Pixcel 座標
        /// </summary>
        public System.Windows.Point MousePixcel { get => mousePixcel; set => SetValue(ref mousePixcel, value); }
        public System.Windows.Point MapMousePixcel { get => mapmousePixcel; set => SetValue(ref mapmousePixcel, value); }
        public double OriPointX1 { get => oriPointX1; set => SetValue(ref oriPointX1, value); }
        public double OriPointY1 { get => oriPointY1; set => SetValue(ref oriPointY1, value); }
        public double OriPointX2 { get => oriPointX2; set => SetValue(ref oriPointX2, value); }
        public double OriPointY2 { get => oriPointY2; set => SetValue(ref oriPointY2, value); }
        public double OriPointX3 { get => oriPointX3; set => SetValue(ref oriPointX3, value); }
        public double OriPointY3 { get => oriPointY3; set => SetValue(ref oriPointY3, value); }

        public double MapPointX1 { get => mapPointX1; set => SetValue(ref mapPointX1, value); }
        public double MapPointY1 { get => mapPointY1; set => SetValue(ref mapPointY1, value); }
        public double MapPointX2 { get => mapPointX2; set => SetValue(ref mapPointX2, value); }
        public double MapPointY2 { get => mapPointY2; set => SetValue(ref mapPointY2, value); }
        public double MapPointX3 { get => mapPointX3; set => SetValue(ref mapPointX3, value); }
        public double MapPointY3 { get => mapPointY3; set => SetValue(ref mapPointY3, value); }

        public double HeightValueMax { get => heightValueMax; set => SetValue(ref heightValueMax, value); }
        public double HeightValueMin { get => heightValueMin; set => SetValue(ref heightValueMin, value); }

        public int RValue { get => rValue; set => SetValue(ref rValue, value); }
        public int GValue { get => gValue; set => SetValue(ref gValue, value); }
        public int BValue { get => bValue; set => SetValue(ref bValue, value); }

        /// <summary>
        /// 水平鏡像
        /// </summary>
        public bool IsHorizontalMirror { get => isHorizontalMirror; set => SetValue(ref isHorizontalMirror, value); }

        /// <summary>
        /// 垂直鏡像
        /// </summary>
        public bool IsVerticalMirror { get => isVerticalMirror; set => SetValue(ref isVerticalMirror, value); }
        public int ROISize { get => roiSize; set => SetValue(ref roiSize, value); }


        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddShapeAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeAction { get; set; }


        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddShapeMapAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeMapAction { get; set; }


        public ICommand ReadOriginImageCommand => new RelayCommand<string>(async key =>
        {

            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = "Bitmap Files (*.bmp, *.jpg ,*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {// 載入圖片


                    var bms = CreateBmp(dlg.FileName);

                    OriginImage = new WriteableBitmap(bms);

                    //var aaaa=  yuanliVision.ReadImage(dlg.FileName);
                    //  MainImage = new WriteableBitmap(aaaa);
                    //  ImageSouce = aaaa;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });


        public ICommand SelectRecipeCommand => new RelayCommand<string>(async key =>
        {
            //WriteableBitmap writeableBitmap = new WriteableBitmap(HeightContourImage);


            //var value = GetPixelGrayValue(writeableBitmap, (int)HeightValueMin, (int)HeightValueMax);

            //RValue = value.red;
            //GValue = value.green;
            //BValue = value.blue;

        });
        public ICommand OpenRecipeWindowCommand => new RelayCommand(() =>
        {
            try
            {
                MapWindow mapWindow = new MapWindow();
                mapWindow.ShowDialog();
                resultPoint = mapWindow.ResultPoint;

                double leftX = resultPoint.Min(r => r.Center.X);
                double leftY = resultPoint.Min(r => r.Center.Y);
                double rightX = resultPoint.Max(r => r.Center.X);
                double rightY = resultPoint.Max(r => r.Center.Y);

                double imageW = rightX - leftX;
                double imageH = rightY - leftY;

                ClearShapeMapAction.Execute(MapDrawings);

                // 設定圖片大小
                int width = Convert.ToInt32(rightX * 1.25);
                int height = Convert.ToInt32(rightY * 1.25);



                // 創建一張 Bitmap
                System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);

                // 填充顏色，這裡使用白色
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bitmap))
                {
                    g.Clear(System.Drawing.Color.White);
                }

                MapImage = ConvertBitmapToBitmapSource(bitmap);

                bitmap.Dispose();

                int i = 1;
                foreach (var item in resultPoint)
                {
                    var center = new ROIRotatedRect
                    {
                        X = item.Center.X,
                        Y = item.Center.Y,
                        LengthX = 5,
                        LengthY = 5,
                        StrokeThickness = 1,
                        Stroke = System.Windows.Media.Brushes.Green,
                        IsInteractived = false,
                        IsCenterShow = false,
                        ToolTip = $" SN= {i}  X:{item.Center.X.Round(1)} Y:{item.Center.Y.Round(1)} "

                    };
                    AddShapeMapAction.Execute(center);
                    i++;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand OringinImageClickCommand => new RelayCommand<string>(key =>
      {
          try
          {



              switch (key)
              {
                  case "1":
                      buttonClickOrigin = ClickOrigin.Point1;
                      break;

                  case "2":
                      buttonClickOrigin = ClickOrigin.Point2;
                      break;

                  case "3":
                      buttonClickOrigin = ClickOrigin.Point3;
                      break;


                  default:
                      break;
              }


          }
          catch (Exception ex)
          {
              MessageBox.Show(ex.Message);
          }

      });

        public ICommand MapImageClickCommand => new RelayCommand<string>(key =>
      {
          switch (key)
          {
              case "1":
                  buttonClickMap = ClickMap.Point1;
                  break;

              case "2":
                  buttonClickMap = ClickMap.Point2;
                  break;

              case "3":
                  buttonClickMap = ClickMap.Point3;
                  break;


              default:
                  break;
          }
      });
        public ICommand MouseDoubleClickCommand => new RelayCommand(() =>
        {
            try
            {

                switch (buttonClickOrigin)
                {
                    case ClickOrigin.Point1:
                        OriPointX1 = MousePixcel.X.Round(1);
                        OriPointY1 = MousePixcel.Y.Round(1);
                        break;
                    case ClickOrigin.Point2:
                        OriPointX2 = MousePixcel.X.Round(1);
                        OriPointY2 = MousePixcel.Y.Round(1);
                        break;
                    case ClickOrigin.Point3:
                        OriPointX3 = MousePixcel.X.Round(1);
                        OriPointY3 = MousePixcel.Y.Round(1);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand CropROIChangeCommand => new RelayCommand(() =>
        {


        });


        public ICommand MapMouseDoubleClickCommand => new RelayCommand<string>(async key =>
        {
            try
            {

                // resultPoint
                var containsPoint = MapDrawings.Select(d => new Rect(d.LeftTop, d.RightBottom)).Where(r => r.Contains(MapMousePixcel));

                if (containsPoint.Count() != 1) throw new Exception("點選數量不正確");
                var point = containsPoint.First();
                var centerX = point.TopLeft.X + point.Width / 2;
                var centerY = point.TopLeft.Y + point.Height / 2;
                switch (buttonClickMap)
                {
                    case ClickMap.Point1:
                        MapPointX1 = centerX.Round(1);
                        MapPointY1 = centerY.Round(1);
                        break;
                    case ClickMap.Point2:
                        MapPointX2 = centerX.Round(1);
                        MapPointY2 = centerY.Round(1);
                        break;
                    case ClickMap.Point3:
                        MapPointX3 = centerX.Round(1);
                        MapPointY3 = centerY.Round(1);
                        break;
                    default:
                        break;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });


        public ICommand ReadHeighImageCommand => new RelayCommand<string>(async key =>
        {
            try
            {

                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter = "Bitmap Files (*.bmp, *.jpg ,*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {// 載入圖片


                    var bms = CreateBmp(dlg.FileName);

                    HeightContourImage = new WriteableBitmap(bms);

                    //var aaaa=  yuanliVision.ReadImage(dlg.FileName);
                    //  MainImage = new WriteableBitmap(aaaa);
                    //  ImageSouce = aaaa;


                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });

        public ICommand LocateCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                //三點對位
                System.Windows.Point[] oriPoints = new System.Windows.Point[] {
                    new System.Windows.Point(OriPointX1, OriPointY1) ,
                    new System.Windows.Point(OriPointX2, OriPointY2) ,
                    new System.Windows.Point(OriPointX3, OriPointY3) ,
                };
                System.Windows.Point[] mapPoints = new System.Windows.Point[] {
                    new System.Windows.Point(MapPointX1, MapPointY1) ,
                    new System.Windows.Point(MapPointX2, MapPointY2) ,
                    new System.Windows.Point(MapPointX3, MapPointY3) ,
                };

                CogAffineTransform cogAffineTransform = new CogAffineTransform(mapPoints, oriPoints);


                ClearShapeAction.Execute(Drawings);



                var transToOriPoint = resultPoint.Select(r => cogAffineTransform.TransPoint(r.Center)).ToArray(); ;


                int rectW = ROISize * 2;
                int rectH = ROISize * 2;
                int i = 1;
                List<Rect> rects = new List<Rect>();
                foreach (var point in transToOriPoint)
                {

                    ROIRotatedRect rect = CreateDrawingRect(point, ROISize, ROISize, i.ToString());

                    AddShapeAction.Execute(rect);

                    rects.Add(new Rect(point.X - rectW / 2, point.Y - rectH / 2, rectW, rectH));
                    i++;
                }
                transToOriRect = rects.ToArray();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand RunCommand => new RelayCommand<string>(async key =>
        {
            try
            {
                // 獲取當前時間
                DateTime currentTime = DateTime.Now;

                // 將當前時間轉換為資料夾名稱格式（例如：2023-02-14）
                string folderName = currentTime.ToString("MM-dd-HH-mm-ss");
                string folderPath = $"C:\\TEST\\{folderName}";

                // 創建資料夾
                Directory.CreateDirectory(folderPath);

                int i = 1;
                BitmapSource originImg = OriginImage.Clone();
                BitmapSource heightContourImage = HeightContourImage.Clone();
                if (IsHorizontalMirror)
                {
                    originImg = originImg.Flip(FlipTypes.Horizontal);
                    heightContourImage = heightContourImage.Flip(FlipTypes.Horizontal);

                }


                if (IsVerticalMirror)
                {
                    originImg = originImg.Flip(FlipTypes.Vertical);
                    heightContourImage = heightContourImage.Flip(FlipTypes.Horizontal);
                }






                WriteableBitmap heightContourbitmap = new WriteableBitmap(heightContourImage);
                var wbmp = new WriteableBitmap(originImg);
                foreach (var rect in transToOriRect)
                {

                    if (rect.TopLeft.X < 0 || rect.TopLeft.Y < 0 || rect.BottomRight.X > originImg.Width || rect.BottomRight.Y > originImg.Height)
                    {
                        continue;
                    }

                    Point ltPoint = rect.TopLeft;
                    Point rtPoint = rect.TopRight;
                    Point lbPoint = rect.BottomLeft;
                    Point rbPoint = rect.BottomRight;
                    Point centerPoint = new Point(rect.TopLeft.X + rect.Width / 2, rect.TopLeft.Y + rect.Height / 2);

                    //三個色階數值一樣  隨便取一個都可以 ，所以取藍色
                    int ltGray = GetPixelGrayValue(heightContourbitmap, ltPoint).blue;
                    int rtGray = GetPixelGrayValue(heightContourbitmap, rtPoint).blue;
                    int lbGray = GetPixelGrayValue(heightContourbitmap, lbPoint).blue;
                    int rbGray = GetPixelGrayValue(heightContourbitmap, rbPoint).blue;
                    int ctGray = GetPixelGrayValue(heightContourbitmap, centerPoint).blue;

                    //將色階去 乘高度範圍
                    var heightRate = (HeightValueMax - HeightValueMin) / 255;

                    var ltHeigh = (ltGray * heightRate) + HeightValueMin;
                    var rtHeigh = (rtGray * heightRate) + HeightValueMin;
                    var lbHeigh = (lbGray * heightRate) + HeightValueMin;
                    var rbHeigh = (rbGray * heightRate) + HeightValueMin;
                    var ctHeigh = (ctGray * heightRate) + HeightValueMin;

                    var avg = (ltHeigh + rtHeigh + lbHeigh + rbHeigh + ctHeigh) / 5;




                    string imageName = $"SN-{i}_PX{centerPoint.X.Round(1)}_PY{centerPoint.Y.Round(1)}_H{avg.Round(1)}";
                    var bmp = CropImage(wbmp, (int)ltPoint.X, (int)ltPoint.Y, (int)rect.Width, (int)rect.Height);

                    bmp.Save($"{folderPath}\\{imageName}");
                    // Int32Rect cropRect = new Int32Rect((int)rect.LeftTop.X, (int)rect.LeftTop.Y, roiW * 2, roiH * 2);
                    //           var bmp = new CroppedBitmap(OriginImage, cropRect);
                    //            bmp.Save($"D:\\ASD\\TEST\\{i}.bmp");

                    i++;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });

        private ROIRotatedRect CreateDrawingRect(Point point, double heigh, double width, string name = "")
        {

            var center = new ROIRotatedRect
            {
                X = point.X,
                Y = point.Y,
                LengthX = width,
                LengthY = heigh,
                StrokeThickness = 4,
                Stroke = System.Windows.Media.Brushes.Green,
                IsInteractived = false,
                IsCenterShow = false,
                ToolTip = $" SN= {name}  X:{point.X} Y:{point.Y} "

            };

            return center;
        }

        private BitmapSource CropImage(WriteableBitmap writeableBitmap, int leftTopX, int leftTopY, int width, int heigh)
        {
            Int32Rect cropRect = new Int32Rect(leftTopX, leftTopY, width, heigh);
            CroppedBitmap bmp = new CroppedBitmap(writeableBitmap, cropRect);
            return bmp;

        }

        private static BitmapSource ConvertBitmapToBitmapSource(System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return bitmapSource;
        }

        private (byte blue, byte green, byte red) GetPixelGrayValue(WriteableBitmap writeableBitmap, Point pixel)
        {

            int colorLayer = 3;

            // 使用 WriteableBitmap 訪問像素
            Int32Rect rect = new Int32Rect((int)pixel.X, (int)pixel.Y, 1, 1);
            int stride = colorLayer * writeableBitmap.PixelWidth; // 4 bytes per pixel (Bgra32 format)
            byte[] pixels = new byte[colorLayer];

            writeableBitmap.CopyPixels(rect, pixels, stride, 0);


            byte blue = pixels[0];
            byte green = pixels[1];
            byte red = pixels[2];
            return (blue, green, red);
        }

        private BitmapSource CreateBmp(string path)
        {
            BitmapImage bitmapImage = new BitmapImage();

            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(path);
            bitmapImage.EndInit();

            // 將圖片轉換為 BitmapSource
            BitmapSource bitmapSource = bitmapImage;
            //不知道原因  有些圖片資訊會丟失  所以先轉成frame  再轉回 BitmapSource
            var frame = bitmapSource.ToByteFrame();



            var bms = frame.ToBitmapSource();
            if (bms.Format == PixelFormats.Bgr32)
                bms = bms.FormatConvertTo(PixelFormats.Bgr24);
            else if (bms.Format == PixelFormats.Bgra32)
                bms = bms.FormatConvertTo(PixelFormats.Bgr24);
            return bms;
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void SetValue<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value)) return;
            T oldValue = field;
            field = value;
            OnPropertyChanged(propertyName, oldValue, value);
        }
        protected virtual void OnPropertyChanged<T>(string name, T oldValue, T newValue)
        {
            // oldValue 和 newValue 目前沒有用到，代爾後需要再實作。
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }




        public enum ClickOrigin
        {
            Point1,
            Point2,
            Point3,

        }

        public enum ClickMap
        {
            Point1,
            Point2,
            Point3,

        }
    }
}
