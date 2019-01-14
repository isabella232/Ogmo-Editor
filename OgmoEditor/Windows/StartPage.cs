using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace OgmoEditor.Windows
{
	public partial class StartPage : UserControl
	{
		public StartPage()
		{
			InitializeComponent();
			Dock = DockStyle.Fill;

			//Recent projects
			Ogmo.CheckRecentProjects();
			for (int i = 0; i < Properties.Settings.Default.RecentProjects.Count; i++)
			{
				LinkLabel link = new LinkLabel();
				link.Location = new Point(4, 30 + (i * 20));
				link.LinkColor = Color.Red;
				link.Font = new Font(FontFamily.GenericMonospace, 10);
				link.Size = new Size(172, 16);
				link.Text = Properties.Settings.Default.RecentProjectNames[i];
				link.Name = Properties.Settings.Default.RecentProjects[i];
				link.Click += delegate(object sender, EventArgs e) { Ogmo.LoadProject(link.Name); };
				recentPanel.Controls.Add(link);
			}

			//Browser
			webBrowser.Url = new Uri(Path.Combine(Ogmo.ProgramDirectory, "Content", "changelog.html"));
		}

		private void websiteButton_Click(object sender, EventArgs e)
		{
			Ogmo.WebsiteLink();
		}
	}
}
