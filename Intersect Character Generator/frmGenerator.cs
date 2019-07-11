using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Intersect_Character_Generator;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Intersect.CharacterGenerator
{
    public partial class FrmGenerator : Form
    {
        private const string FORMAT_IMAGE_PNG = "PNG Image|*.png";
        private const string FORMAT_PROJECT_GENERATOR = "Intersect Character Generator Project File |*.iprj";

        [NotNull] public static FrmGenerator Current { get; private set; }

        static FrmGenerator()
        {
            FrmGenerator.Current = new FrmGenerator();
        }

        #region Temporary Variables

        /* TODO: private */ public SaveFileDialog mSaveSpriteDialog;
        private SaveFileDialog mSaveProjectDialog;
        private OpenFileDialog mOpenProjectDialog;

        [NotNull] private readonly Project mProject = new Project();
        [NotNull] public readonly List<Layer> mLayers = new List<Layer>();
        [NotNull] private readonly Random mRandom = new Random();

        #endregion

        public FrmGenerator()
        {
            InitializeComponent();
        }

        private void FrmGenerator_Load(object sender, EventArgs e)
        {
            mLayers.Add(new Layer("bodies", cmbBody, btnBodyHue, colorDialog, trkBodyHueIntensity, trkBodyAlpha, picBodyLock, this));
            mLayers.Add(new Layer("eyes", cmbEyes, btnEyesHue, colorDialog, trkEyesHueIntensity, trkEyesAlpha, picEyesLock, this));
            mLayers.Add(new Layer("hair", cmbHair, btnHairHue, colorDialog, trkHairHueIntensity, trkHairAlpha, picHairLock, this));
            mLayers.Add(new Layer("facialhair", cmbFacialHair, btnFacialHairHue, colorDialog, trkFacialHairHueIntensity, trkFacialHairAlpha, picFacialHairLock, this));
            mLayers.Add(new Layer("headwear", cmbHeadwear, btnHeadwearHue, colorDialog, trkHeadwearHueIntensity, trkHeadwearAlpha, picHeadwearLock, this));
            mLayers.Add(new Layer("shirt", cmbShirt, btnShirtHue, colorDialog, trkShirtHueIntensity, trkShirtAlpha, picShirtLock, this));
            mLayers.Add(new Layer("shoulders", cmbShoulders, btnShouldersHue, colorDialog, trkShouldersHueIntensity, trkShouldersAlpha, picShouldersLock, this));
            mLayers.Add(new Layer("gloves", cmbGloves, btnGlovesHue, colorDialog, trkGlovesHueIntensity, trkGlovesAlpha, picGlovesLock, this));
            mLayers.Add(new Layer("pants", cmbPants, btnPantsHue, colorDialog, trkPantsHueIntensity, trkPantsAlpha, picPantsLock, this));
            mLayers.Add(new Layer("waist", cmbWaist, btnWaistHue, colorDialog, trkWaistHueIntensity, trkWaistAlpha, picWaistLock, this));
            mLayers.Add(new Layer("boots", cmbBoots, btnBootsHue, colorDialog, trkBootsHueIntensity, trkBootsAlpha, picBootsLock, this));
            mLayers.Add(new Layer("accessories", cmbAccessory1, btnBootsHue, colorDialog, trkAccessory1HueIntensity, trkAccessory1Alpha, picAccessory1Lock, this));
            mLayers.Add(new Layer("accessories", cmbAccessory2, btnBootsHue, colorDialog, trkAccessory2HueIntensity, trkAccessory2Alpha, picAccessory2Lock, this));
            mLayers.Add(new Layer("accessories", cmbAccessory3, btnBootsHue, colorDialog, trkAccessory3HueIntensity, trkAccessory3Alpha, picAccessory3Lock, this));
            mLayers.Add(new Layer("accessories", cmbAccessory4, btnBootsHue, colorDialog, trkAccessory4HueIntensity, trkAccessory4Alpha, picAccessory4Lock, this));

            mSaveSpriteDialog = new SaveFileDialog
            {
                Filter = FORMAT_IMAGE_PNG,
                Title = "Save Sprite",
                RestoreDirectory = true
            };

            mSaveProjectDialog = new SaveFileDialog
            {
                Filter = FORMAT_PROJECT_GENERATOR,
                Title = "Save Intersect Character Generator Project",
                RestoreDirectory = true
            };

            mOpenProjectDialog = new OpenFileDialog
            {
                Filter = FORMAT_PROJECT_GENERATOR,
                Title = "Select your project file",
                RestoreDirectory = true
            };

            DrawCharacter();
        }

        public Layer FindLayer(NamedLayers namedLayer) => mLayers[(int)namedLayer];

        #region "Controls"
        private void btnColor_Click(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() == DialogResult.OK)
            {
                ((Button)sender).BackColor = colorDialog.Color;
                DrawCharacter();
                if (sender == btnBackgroundColor) chkTransparent.Checked = false;
            }
        }

        private void rdoMale_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var layer in mLayers)
            {
                layer.PopulateList(rdoMale.Checked);
            }
        }

        private void genericEvent_DrawCharacter(object sender, EventArgs e)
        {
            DrawCharacter();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            mSaveSpriteDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (mSaveSpriteDialog.FileName != "")
            {
                var sprite = RenderCharacter(!chkTransparent.Checked);
                sprite.Save(mSaveSpriteDialog.FileName, System.Drawing.Imaging.ImageFormat.Png);
                sprite.Dispose();
            }
        }

        private void btnRandomize_Click(object sender, EventArgs e)
        {
            var maleChecked = rdoMale.Checked;
            if (picGenderLock.Tag == null)
            {
                rdoMale.Checked = mRandom.Next(0, 2) == 1;
                rdoFemale.Checked = !rdoMale.Checked;
            }
            if (maleChecked != rdoMale.Checked)
            {
                foreach (var layer in mLayers)
                {
                    layer.PopulateList(rdoMale.Checked);
                }
            }
            foreach (var layer in mLayers)
            {
                layer.Randomize(mRandom);
            }
        }

        private void supportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://www.ascensiongamedev.com/topic/3189-intersect-character-generator/");
        }

        private void gitHubToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/AscensionGameDev/Intersect-Character-Generator");
        }

        private void picGenderLock_Click(object sender, EventArgs e)
        {
            if (picGenderLock.Tag != null)
            {
                picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                picGenderLock.Tag = null;
            }
            else
            {
                picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                picGenderLock.Tag = 1;
            }
        }
        private void saveStateAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mSaveProjectDialog.ShowDialog();

            // If the file name is not an empty string open it for saving.  
            if (mSaveProjectDialog.FileName != "")
            {
                mProject.Path = mSaveProjectDialog.FileName;
                saveStateToolStripMenuItem.Enabled = !string.IsNullOrWhiteSpace(mSaveProjectDialog.FileName);
                var json = JsonConvert.SerializeObject(mProject, Formatting.Indented);
                File.WriteAllText(mSaveProjectDialog.FileName, json);
            }
        }
        private void openStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (mOpenProjectDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var json = File.ReadAllText(mOpenProjectDialog.FileName);
                JsonConvert.PopulateObject(json, mProject, new JsonSerializerSettings() { ObjectCreationHandling = ObjectCreationHandling.Replace });
                mProject.Path = mOpenProjectDialog.FileName;
                saveStateToolStripMenuItem.Enabled = !string.IsNullOrWhiteSpace(mOpenProjectDialog.FileName);
                this.Text = "Intersect Character Generator - " + mProject.Path;
                DrawCharacter();
            }
        }

        private void saveStateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var json = JsonConvert.SerializeObject(mProject, Formatting.Indented);
            File.WriteAllText(mProject.Path, json);
            this.Text = "Intersect Character Generator - " + mProject.Path;
        }
        #endregion

        #region "Rendering"
        public void DrawCharacter()
        {
            if (picSprite.BackgroundImage != null)
            {
                picSprite.BackgroundImage.Dispose();
                picSprite.BackgroundImage = null;
            }
            picSprite.BackgroundImage = RenderCharacter(true);
            picSprite.Size = new Size(picSprite.BackgroundImage.Size.Width * trackZoom.Value, picSprite.BackgroundImage.Size.Height * trackZoom.Value);
            picSprite.BackColor = Color.Transparent;
        }

        private Bitmap RenderCharacter(bool drawBackground)
        {
            var size = new Size(128, 192);

            //figure out size...
            var maxW = 0;
            var maxH = 0;
            foreach (var layer in mLayers)
            {
                if (layer.Width> maxW) maxW = layer.Width;
                if (layer.Height> maxH) maxH = layer.Height;
            }

            if (maxW > 0) size.Width = maxW;
            if (maxH > 0) size.Height = maxH;

            var rect = new Rectangle(0, 0, size.Width, size.Height);
            var bmp = new Bitmap(size.Width,size.Height);
            var g = Graphics.FromImage(bmp);
            if (drawBackground)
            {
                if (chkTransparent.Checked)
                {
                    var transTile = Properties.Resources.transtile;
                    for (int x = 0; x < size.Width / transTile.Width + 1; x++)
                    {
                        for (int y = 0; y < size.Height / transTile.Height + 1; y++)
                        {
                            g.DrawImage(transTile, new Rectangle(x * transTile.Width, y * transTile.Height, transTile.Width, transTile.Height), new Rectangle(0, 0, transTile.Width, transTile.Height), GraphicsUnit.Pixel);
                        }
                    }
                }
                else
                {
                    g.FillRectangle(new SolidBrush(btnBackgroundColor.BackColor),rect);
                }
            }
            RenderLayers(g, size);
            g.Dispose();
            return bmp;
        }
        private void RenderLayers(Graphics g, Size size)
        {
            if (mLayers.Count >= 15)
            {
                mLayers[0].Draw(g, size.Width / 4, size.Height / 4); //Base
                mLayers[1].Draw(g, size.Width / 4, size.Height / 4); //Eyes
                mLayers[2].Draw(g, size.Width / 4, size.Height / 4); //Hair
                mLayers[3].Draw(g, size.Width / 4, size.Height / 4); //Facial Hair
                mLayers[4].Draw(g, size.Width / 4, size.Height / 4); //Headwear
                if (chkPantsAfterShirt.Checked)
                {
                    //Draw shirt before pants
                    mLayers[5].Draw(g, size.Width / 4, size.Height / 4); //Shirt
                    mLayers[6].Draw(g, size.Width / 4, size.Height / 4); //Shoulders
                    mLayers[7].Draw(g, size.Width / 4, size.Height / 4); //Gloves          
                    if (chkBootsAfterPants.Checked)
                    {
                        mLayers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        mLayers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                        mLayers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                    }
                    else
                    {
                        mLayers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                        mLayers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        mLayers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                    }
                }
                else
                {
                    //Draw pants then shirt
                    if (chkBootsAfterPants.Checked)
                    {
                        mLayers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        mLayers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                        mLayers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                    }
                    else
                    {
                        mLayers[10].Draw(g, size.Width / 4, size.Height / 4); //Boots
                        mLayers[8].Draw(g, size.Width / 4, size.Height / 4); //Pants
                        mLayers[9].Draw(g, size.Width / 4, size.Height / 4); //Waist
                    }
                    mLayers[5].Draw(g, size.Width / 4, size.Height / 4); //Shirt
                    mLayers[6].Draw(g, size.Width / 4, size.Height / 4); //Shoulders
                    mLayers[7].Draw(g, size.Width / 4, size.Height / 4); //Gloves
                }
                mLayers[11].Draw(g, size.Width / 4, size.Height / 4); //Accessory 1
                mLayers[12].Draw(g, size.Width / 4, size.Height / 4); //Accessory 2
                mLayers[13].Draw(g, size.Width / 4, size.Height / 4); //Accessory 3
                mLayers[14].Draw(g, size.Width / 4, size.Height / 4); //Accessory 4
            }
        }

        #endregion


        #region "Saving/Loading Classes"
        
        
        #endregion
    }
}
