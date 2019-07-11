using System.Drawing;

namespace Intersect.CharacterGenerator
{
    public class Project
    {
        private string mProjectPath = "";
        public string ProjectPath
        {
            get => mProjectPath;
            set
            {
                mProjectPath = value;
                FrmGenerator.Current.saveStateToolStripMenuItem.Enabled = !string.IsNullOrEmpty(mProjectPath);
            }
        }
        public int Gender
        {
            get => FrmGenerator.Current.rdoMale?.Checked ?? false ? 0 : 1;
            set
            {
                if (value == 0)
                    FrmGenerator.Current.rdoMale.Checked = true;
                else
                    FrmGenerator.Current.rdoFemale.Checked = true;
            }
        }
        public bool GenderLock
        {
            get => FrmGenerator.Current.picGenderLock?.Tag != null;
            set
            {
                if (value)
                {
                    FrmGenerator.Current.picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_lock_14_0_dcdcdc_none;
                    FrmGenerator.Current.picGenderLock.Tag = 1;
                }
                else
                {
                    FrmGenerator.Current.picGenderLock.BackgroundImage = Properties.Resources.font_awesome_4_7_0_unlock_14_0_dcdcdc_none;
                    FrmGenerator.Current.picGenderLock.Tag = null;
                }
            }
        }
        public int BackgroundColor
        {
            get => FrmGenerator.Current.chkTransparent.Checked ? Color.Transparent.ToArgb() : FrmGenerator.Current.btnBackgroundColor.BackColor.ToArgb();
            set
            {
                if (value == Color.Transparent.ToArgb())
                {
                    FrmGenerator.Current.chkTransparent.Checked = true;
                }
                else
                {
                    FrmGenerator.Current.chkTransparent.Checked = false;
                    FrmGenerator.Current.btnBackgroundColor.BackColor = Color.FromArgb(value);
                }
            }
        }

        // Layers -_-
        public LayerSettings Body => FrmGenerator.Current.FindLayer(NamedLayers.Body)?.Settings;
        public LayerSettings Eyes => FrmGenerator.Current.FindLayer(NamedLayers.Eyes)?.Settings;
        public LayerSettings Hair => FrmGenerator.Current.FindLayer(NamedLayers.Hair)?.Settings;
        public LayerSettings FacialHair => FrmGenerator.Current.FindLayer(NamedLayers.FacialHair)?.Settings;
        public LayerSettings Headwear => FrmGenerator.Current.FindLayer(NamedLayers.Headwear)?.Settings;
        public LayerSettings Shirt => FrmGenerator.Current.FindLayer(NamedLayers.Shirt)?.Settings;
        public LayerSettings Shoulders => FrmGenerator.Current.FindLayer(NamedLayers.Shoulders)?.Settings;
        public LayerSettings Gloves => FrmGenerator.Current.FindLayer(NamedLayers.Gloves)?.Settings;
        public LayerSettings Pants => FrmGenerator.Current.FindLayer(NamedLayers.Pants)?.Settings;
        public LayerSettings Waist => FrmGenerator.Current.FindLayer(NamedLayers.Waist)?.Settings;
        public LayerSettings Boots => FrmGenerator.Current.FindLayer(NamedLayers.Boots)?.Settings;
        public LayerSettings Accessory1 => FrmGenerator.Current.FindLayer(NamedLayers.Accessory1)?.Settings;
        public LayerSettings Accessory2 => FrmGenerator.Current.FindLayer(NamedLayers.Accessory2)?.Settings;
        public LayerSettings Accessory3 => FrmGenerator.Current.FindLayer(NamedLayers.Accessory3)?.Settings;
        public LayerSettings Accessory4 => FrmGenerator.Current.FindLayer(NamedLayers.Accessory4)?.Settings;

        public bool ShirtTuckedIn
        {
            get => FrmGenerator.Current.chkPantsAfterShirt.Checked;
            set => FrmGenerator.Current.chkPantsAfterShirt.Checked = value;
        }
        public bool PantsTuckedIn
        {
            get => FrmGenerator.Current.chkBootsAfterPants.Checked;
            set => FrmGenerator.Current.chkBootsAfterPants.Checked = value;
        }
        public int Zoom
        {
            get => FrmGenerator.Current.trackZoom.Value;
            set => FrmGenerator.Current.trackZoom.Value = value;
        }
        public string SaveFilePath
        {
            get => FrmGenerator.Current.mSaveSpriteDialog.FileName;
            set => FrmGenerator.Current.mSaveSpriteDialog.FileName = value;
        }
    }
}
