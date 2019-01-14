using System;
using System.Windows.Forms;

namespace OgmoEditor.Windows
{
	public partial class AboutWindow : Form
	{
		public AboutWindow()
		{
			InitializeComponent();

			versionLabel.Text = "Community Edition";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			Close();
			Ogmo.MainWindow.Activate();
			Ogmo.MainWindow.EnableEditing();
		}

		private void websiteButton_Click(object sender, EventArgs e)
		{
			Ogmo.WebsiteLink();
		}
	}
}
