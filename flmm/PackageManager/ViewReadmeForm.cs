﻿using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Fomm.Properties;

namespace Fomm.PackageManager
{
  /// <summary>
  ///   The FOMod readme viewer.
  /// </summary>
  public partial class ViewReadmeForm : Form
  {
    #region Contructors

    /// <summary>
    ///   The default constructor.
    /// </summary>
    /// <param name="p_rmeReadme">The <see cref="Readme" /> to be viewed.</param>
    public ViewReadmeForm(Readme p_rmeReadme)
    {
      InitializeComponent();
      Icon = Resources.fomm02;
      Settings.Default.windowPositions.GetWindowPosition("ReadmeViewer", this);

      switch (p_rmeReadme.Format)
      {
        case ReadmeFormat.PlainText:
        case ReadmeFormat.RichText:
          var rtbReadme = new RichTextBox();
          rtbReadme.Multiline = true;
          rtbReadme.ScrollBars = RichTextBoxScrollBars.Vertical;
          rtbReadme.BorderStyle = BorderStyle.None;
          rtbReadme.BackColor = SystemColors.Control;
          rtbReadme.ReadOnly = true;
          rtbReadme.TabStop = false;
          rtbReadme.LinkClicked += rtbReadme_LinkClicked;
          if (p_rmeReadme.Format == ReadmeFormat.PlainText)
          {
            rtbReadme.Font = new Font(FontFamily.GenericMonospace, rtbReadme.Font.Size, rtbReadme.Font.Style);
            rtbReadme.Text = p_rmeReadme.Text;
          }
          else
          {
            rtbReadme.Rtf = p_rmeReadme.Text;
          }
          rtbReadme.Dock = DockStyle.Fill;
          Controls.Add(rtbReadme);
          break;
        case ReadmeFormat.HTML:
          var wbrBrowser = new WebBrowser();
          Controls.Add(wbrBrowser);
          wbrBrowser.Dock = DockStyle.Fill;
          wbrBrowser.DocumentCompleted += delegate
          {
            Text = String.IsNullOrEmpty(wbrBrowser.DocumentTitle) ? "Readme" : wbrBrowser.DocumentTitle;
          };
          wbrBrowser.WebBrowserShortcutsEnabled = false;
          wbrBrowser.AllowWebBrowserDrop = false;
          wbrBrowser.AllowNavigation = false;
          wbrBrowser.DocumentText = p_rmeReadme.Text;
          break;
      }
    }

    /// <summary>
    ///   Handles the <see cref="RichTextBox.LinkClicked" /> event of the readme text box.
    /// </summary>
    /// <remarks>
    ///   Launches clicked links using the default browser.
    /// </remarks>
    /// <param name="sender">The object that trigger the event.</param>
    /// <param name="e">An <see cref="EventArgs" /> describing the event arguments.</param>
    private void rtbReadme_LinkClicked(object sender, LinkClickedEventArgs e)
    {
      Process.Start(e.LinkText);
    }

    #endregion

    /// <summary>
    ///   Raises the <see cref="Form.Closing" /> event.
    /// </summary>
    /// <remarks>
    ///   Saves the window's position.
    /// </remarks>
    /// <param name="e">A <see cref="CancelEventArgs" /> describing the event arguments.</param>
    protected override void OnClosing(CancelEventArgs e)
    {
      Settings.Default.windowPositions.SetWindowPosition("ReadmeViewer", this);
      Settings.Default.Save();
      base.OnClosing(e);
    }
  }
}