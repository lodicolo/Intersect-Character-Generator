using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using DarkUI.Controls;
using Intersect_Character_Generator;

namespace Intersect.CharacterGenerator
{
    public class Layer
    {
        public LayerSettings Settings { get; }

        public Dictionary<string, string> MaleParts = new Dictionary<string, string>();
        public Dictionary<string, string> FemaleParts = new Dictionary<string, string>();

        private string mDirectory;
        private DarkComboBox mCmbItems;
        private DarkButton mColorBtn;
        private FrmGenerator mFrmGenerator;
        private bool mAleSelected;
        private ColorDialog mColorDialog;
        private TrackBar mIntensityBar;
        private TrackBar mAlphaBar;
        private PictureBox mLockBox;

        private Bitmap mOriginalGraphic;
        private Bitmap mAlteredGraphic;
        public string GraphicPath = "";

        public Layer(string folderName, DarkComboBox itemList, DarkButton colorButton, ColorDialog colorD, TrackBar intBar, TrackBar aBar,PictureBox lockPic, FrmGenerator form)
        {
            this.Settings = new LayerSettings(this);

            if (!Directory.Exists("assets")) Directory.CreateDirectory("assets");
            mFrmGenerator = form;
            mColorDialog = colorD;
            mIntensityBar = intBar;
            mLockBox = lockPic;
            mAlphaBar = aBar;
            mDirectory = folderName;
            mCmbItems = itemList;
            mColorBtn = colorButton;
            LoadItems();
            PopulateList(true);
            mCmbItems.SelectedIndexChanged += cmbItems_SelectedIndexChanged;
            mColorBtn.Click += btnColor_Click;
            intBar.ValueChanged += intBar_ValueChanged;
            mAlphaBar.ValueChanged += alphaBar_ValueChanged;
            mLockBox.Click += LockBox_Click;
        }

        public int Width => mOriginalGraphic?.Width ?? 0;

        public int Height => mOriginalGraphic?.Height ?? 0;

        public string Selected
        {
            get => mCmbItems?.Text;
            set
            {
                if (!string.IsNullOrWhiteSpace(value) && (mCmbItems?.Items.Contains(value) ?? false))
                {
                    mCmbItems.SelectedIndex = mCmbItems.Items.IndexOf(value);
                }
            }
        }

        public int Hue { get => mColorBtn.BackColor.ToArgb(); set => mColorBtn.BackColor = Color.FromArgb(value); }

        public int Saturation { get => mIntensityBar.Value; set => mIntensityBar.Value = value; }

        public int Alpha { get => mAlphaBar.Value; set => mAlphaBar.Value = value; }

        public bool RandomizationLocked
        {
            get => mLockBox.Tag != null;
            set
            {
                if (!value)
                {
                    mLockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                    mLockBox.Tag = null;
                }
                else
                {
                    mLockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                    mLockBox.Tag = 1;
                }
            }
        }

        public void Draw(Graphics g, int frameWidth, int frameHeight)
        {
            if (mOriginalGraphic == null) return;
            var img = (mColorBtn.BackColor.ToArgb() == Color.White.ToArgb() ? mOriginalGraphic : mAlteredGraphic);
            for (var x = 0; x < 4; x++)
            {
                for (var y = 0; y < 4; y++)
                {
                    float[][] matrixItems ={
                                new float[] {1, 0, 0, 0, 0},
                                new float[] {0, 1, 0, 0, 0},
                                new float[] {0, 0, 1, 0, 0},
                                new float[] {0, 0, 0, (100 - mAlphaBar.Value) / 100f, 0},
                                new float[] {0, 0, 0, 0, 1}};
                    var colorMatrix = new ColorMatrix(matrixItems);
                    var imageAttributes = new ImageAttributes();
                    imageAttributes.SetColorMatrix(
                        colorMatrix,
                        ColorMatrixFlag.Default,
                        ColorAdjustType.Bitmap);
                    g.DrawImage(img, new Rectangle(x * frameWidth + (frameWidth - img.Width / 4) / 2, y * frameHeight + (frameHeight - img.Height / 4) / 2, img.Width / 4, img.Height / 4), x * img.Width / 4, y * img.Height / 4, img.Width / 4, img.Height / 4, GraphicsUnit.Pixel,imageAttributes);
                }
            }
        }

        private void cmbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            var redraw = false;
            if (mCmbItems.SelectedIndex == 0)
            {
                if (mOriginalGraphic != null)
                {
                    mOriginalGraphic.Dispose();
                    mAlteredGraphic.Dispose();
                    redraw = true;
                }
                mOriginalGraphic = null;
                mAlteredGraphic = null;
                GraphicPath = "";
            }
            else
            {
                var partList = mAleSelected ? MaleParts : FemaleParts;
                if (partList.ContainsKey(mCmbItems.Text))
                {
                    if (GraphicPath != partList[mCmbItems.Text])
                    {
                        GraphicPath = partList[mCmbItems.Text];
                        mOriginalGraphic = new Bitmap(GraphicPath);
                        mAlteredGraphic = new Bitmap(mOriginalGraphic.Width, mOriginalGraphic.Height);
                        ProcessHue();
                        redraw = true;
                    }
                }
            }
            if (redraw) mFrmGenerator.DrawCharacter();
        }

        private void ProcessHue()
        {
            var btnBrightness = mColorBtn.BackColor.GetBrightness();
            var btnSat = mColorBtn.BackColor.GetSaturation();
            var btnHue = mColorBtn.BackColor.GetHue();
            var intensityVal = mIntensityBar.Value;
            var origBmd = mOriginalGraphic.LockBits(new Rectangle(0, 0, mOriginalGraphic.Width, mOriginalGraphic.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, mOriginalGraphic.PixelFormat);
            var alteredBmd = mAlteredGraphic.LockBits(new Rectangle(0, 0, mAlteredGraphic.Width, mAlteredGraphic.Height), System.Drawing.Imaging.ImageLockMode.ReadWrite, mAlteredGraphic.PixelFormat);
            var pixelSize = 4;

            unsafe
            {
                for (var y = 0; y < origBmd.Height; y++)
                {
                    var origRow = (byte*)origBmd.Scan0 + (y * origBmd.Stride);
                    var alteredRow = (byte*)alteredBmd.Scan0 + (y * alteredBmd.Stride);
                    for (var x = 0; x < origBmd.Width; x++)
                    {
                        var clr = Color.FromArgb(origRow[x * pixelSize + 3], origRow[x * pixelSize + 2], origRow[x * pixelSize + 1], origRow[x * pixelSize]);
                        var alteredColor = ColorHelper.FromAHSB(clr.A, btnHue, btnSat, clr.GetBrightness() * btnBrightness + ((100 - intensityVal) / 100f));
                        alteredRow[x * pixelSize] = alteredColor.B;   //Blue  0-255
                        alteredRow[x * pixelSize + 1] = alteredColor.G; //Green 0-255
                        alteredRow[x * pixelSize + 2] = alteredColor.R;   //Red   0-255
                        alteredRow[x * pixelSize + 3] = alteredColor.A;  //Alpha 0-255
                    }
                }
            }

            mOriginalGraphic.UnlockBits(origBmd);
            mAlteredGraphic.UnlockBits(alteredBmd);
        }

        private void LoadItems()
        {
            MaleParts.Clear();
            FemaleParts.Clear();
            var folder = mDirectory;
            if (!Directory.Exists(Path.Combine("assets", folder))) Directory.CreateDirectory(Path.Combine("assets", folder));
            AddImagesToList(Path.Combine("assets", folder, "male"), MaleParts);
            AddImagesToList(Path.Combine("assets", folder, "female"), FemaleParts);
            AddImagesToList(Path.Combine("assets", folder, "unisex"), MaleParts);
            AddImagesToList(Path.Combine("assets", folder, "unisex"), FemaleParts);
            MaleParts = SortDictionary(MaleParts);
            FemaleParts = SortDictionary(FemaleParts);
        }

        public void PopulateList(bool male)
        {
            mAleSelected = male;
            mCmbItems.Items.Clear();
            mCmbItems.Items.Add("None");
            if (male)
            {
                mCmbItems.Items.AddRange(MaleParts.Keys.ToArray());
            }
            else
            {
                mCmbItems.Items.AddRange(FemaleParts.Keys.ToArray());
            }
            mCmbItems.SelectedIndex = 0;
            if (mCmbItems.Items.IndexOf("base") > 0) mCmbItems.SelectedIndex = mCmbItems.Items.IndexOf("base");
            if (mCmbItems.Items.IndexOf("Base") > 0) mCmbItems.SelectedIndex = mCmbItems.Items.IndexOf("Base");
            cmbItems_SelectedIndexChanged(null, null);
        }

        public void Randomize(Random rand)
        {
            if (mLockBox.Tag == null)
                mCmbItems.SelectedIndex = rand.Next(0, mCmbItems.Items.Count);
        }

        private void AddImagesToList(string folder, Dictionary<string, string> parts)
        {
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            var images = Directory.GetFiles(folder, "*.png");
            foreach (var img in images)
            {
                if (!parts.ContainsKey(Path.GetFileNameWithoutExtension(img)))
                {
                    parts.Add(Path.GetFileNameWithoutExtension(img), img);
                }
            }
        }

        private Dictionary<string, string> SortDictionary(Dictionary<string, string> dict)
        {
            var newDict = new Dictionary<string, string>();
            var strings = dict.Keys.ToArray();
            Array.Sort(strings, new AlphanumComparator());
            foreach (var str in strings)
            {
                newDict.Add(str, dict[str]);
            }
            return newDict;
        }

        private void btnColor_Click(object sender, EventArgs e)
        {
            if (mColorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Button)sender).BackColor = mColorDialog.Color;
                ProcessHue();
                mFrmGenerator.DrawCharacter();
            }
        }

        private void LockBox_Click(object sender, EventArgs e)
        {
            if (mLockBox.Tag != null)
            {
                mLockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                mLockBox.Tag = null;
            }
            else
            {
                mLockBox.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                mLockBox.Tag = 1;
            }
        }

        private void intBar_ValueChanged(object sender, EventArgs e)
        {
            ProcessHue();
            mFrmGenerator.DrawCharacter();
        }

        private void alphaBar_ValueChanged(object sender, EventArgs e)
        {
            mFrmGenerator.DrawCharacter();
        }
    }
}
