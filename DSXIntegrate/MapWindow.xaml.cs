using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using YuanliCore.CameraLib;
using YuanliCore.ImageProcess.Match;
using YuanliCore.Interface;
using YuanliCore.Views.CanvasShapes;

namespace DSXIntegrate
{
    /// <summary>
    /// MapWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MapWindow : Window, INotifyPropertyChanged
    {
        private CogMatcher matcher = new CogMatcher(); //使用Vision pro 實體
        private PatmaxParams matchParam = new PatmaxParams(0);
        private BitmapSource image;
        private BitmapSource sampleImage;
        private ObservableCollection<ROIShape> drawings = new ObservableCollection<ROIShape>();
        private IEnumerable<MatchResult> resultPoint;
        public MapWindow()
        {
            InitializeComponent();
        }

        public BitmapSource Image { get => image; set => SetValue(ref image, value); }
        public BitmapSource SampleImage { get => sampleImage; set => SetValue(ref sampleImage, value); }
        /// <summary>
        /// 滑鼠在影像內 Pixcel 座標
        /// </summary>
        public System.Windows.Point MousePixcel { get; set; }
        /// <summary>
        /// 新增 Shape
        /// </summary>
        public ICommand AddShapeAction { get; set; }
        /// <summary>
        /// 清除 Shape
        /// </summary>
        public ICommand ClearShapeAction { get; set; }

        public bool IsLocate;
        public   IEnumerable<MatchResult> ResultPoint { get => resultPoint;  }

        public ObservableCollection<ROIShape> Drawings { get => drawings; set => SetValue(ref drawings, value); }

        public ICommand ReadMapCommand => new RelayCommand<string>(async key =>
        {
            try
            {


                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

                dlg.Filter  =  "Bitmap Files (*.bmp, *.jpg ,*.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";

                Nullable<bool> result = dlg.ShowDialog();
                if (result == true)
                {// 載入圖片


                    var bms = CreateBmp(dlg.FileName);

                    Image = new WriteableBitmap(bms);

                    //var aaaa=  yuanliVision.ReadImage(dlg.FileName);
                    //  MainImage = new WriteableBitmap(aaaa);
                    //  ImageSouce = aaaa;

                    IsLocate = false;
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand EditPatternCommand => new RelayCommand<string>(async key =>
        {
            try
            {

                matcher.RunParams = matchParam;
                matcher.EditParameter(Image);

                matchParam = (PatmaxParams)matcher.RunParams;
                if (matchParam.PatternImage != null)
                    SampleImage = matchParam.PatternImage.ToBitmapSource();

                //  UpdateRecipe();


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        });
        public ICommand PatternMatchCommand => new RelayCommand<string>(async key =>
        {
            ClearShapeAction.Execute(Drawings);
            resultPoint = matcher.Find(Image.ToByteFrame());
         
            foreach (var item in resultPoint)
            {
                var center = new ROICross
                {
                    X = item.Center.X,
                    Y = item.Center.Y,
                    Size = 5,
                    StrokeThickness = 2,
                    Stroke = Brushes.Red,
                    IsInteractived = false
                };
                AddShapeAction.Execute(center);
           
            }

        });
        public ICommand AIMatchCommand => new RelayCommand<string>(async key =>
        {
            ClearShapeAction.Execute(Drawings);
            

        });
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
                bms =bms.FormatConvertTo(PixelFormats.Bgr24);
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

      
    }
}
