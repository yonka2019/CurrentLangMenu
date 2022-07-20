using System;
using System.Windows.Forms;

namespace CurrentLangMenu
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            LangLabel.Text = KeyboardLayoutWatcher.GetCurrentKeyboardLayoutFormat(InputLanguage.CurrentInputLanguage.Culture.KeyboardLayoutId); // old and new KB layout

            new KeyboardLayoutWatcher().KeyboardLayoutChanged += (layoutCode) => // assign event
            {
                LangLabel.Invoke(new MethodInvoker(delegate
                {
                    LangLabel.Text = KeyboardLayoutWatcher.GetCurrentKeyboardLayoutFormat(layoutCode); // old and new KB layout
                }));
            };
        }
    }
}
