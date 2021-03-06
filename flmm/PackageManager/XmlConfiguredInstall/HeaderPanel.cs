﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace Fomm.PackageManager.XmlConfiguredInstall
{
  /// <summary>
  ///   The posible positions of the title text.
  /// </summary>
  public enum TextPosition
  {
    /// <summary>
    ///   Indicates the text should be on the left side of the header.
    /// </summary>
    Left,

    /// <summary>
    ///   Indicates the text should be on the right side of the header.
    /// </summary>
    Right,

    /// <summary>
    ///   Indicates the text should be on the right side of the image in the header.
    /// </summary>
    RightOfImage
  }

  /// <summary>
  ///   A panel that displays an image and title.
  /// </summary>
  public class HeaderPanel : UserControl
  {
    /// <summary>
    ///   A label that has a transparent backgroun.
    /// </summary>
    private class TransparentLabel : UserControl
    {
      private Label m_lblLabel = new Label();
      private bool m_booPaintOnce;

      #region Properties

      /// <summary>
      ///   Gets or sets the text of the label.
      /// </summary>
      /// <value>The text of the label.</value>
      public override string Text
      {
        get
        {
          return m_lblLabel.Text;
        }
        set
        {
          m_lblLabel.Text = value;
          updateLayout();
        }
      }

      /// <summary>
      ///   Gets or sets the font of the label.
      /// </summary>
      /// <value>The font of the label.</value>
      public override Font Font
      {
        get
        {
          return m_lblLabel.Font;
        }
        set
        {
          m_lblLabel.Font = value;
          updateLayout();
        }
      }

      #endregion

      /// <summary>
      ///   Raises the <see cref="Control.OnPaintBackground" /> event.
      /// </summary>
      /// <remarks>
      ///   We don't want a backgroun, so this method does nothing.
      /// </remarks>
      /// <param name="e">A <see cref="PaintEventArgs" /> describing the event arguments.</param>
      protected override void OnPaintBackground(PaintEventArgs e) {}

      /// <summary>
      ///   Adjusts the size of the label whenever properties affecting size change.
      /// </summary>
      private void updateLayout()
      {
        using (var g = CreateGraphics())
        {
          Size = g.MeasureString(m_lblLabel.Text, m_lblLabel.Font).ToSize();
        }
      }

      /// <summary>
      ///   Raises the <see cref="Control.OnPaint" /> event.
      /// </summary>
      /// <remarks>
      ///   This paints what is behind the label, and then oerlays that with the label's text.
      /// </remarks>
      /// <param name="e">A <see cref="PaintEventArgs" /> describing the event arguments.</param>
      protected override void OnPaint(PaintEventArgs e)
      {
        if (!m_booPaintOnce)
        {
          m_booPaintOnce = true;
          Visible = false;
          Parent.Invalidate(Bounds);
          Parent.Update();
          Visible = true;
        }
        else
        {
          m_booPaintOnce = false;
          using (var g = e.Graphics)
          {
            var brush = new SolidBrush(ForeColor);
            g.DrawString(Text, Font, brush, 1, 1);
          }
        }
      }
    }

    private const Int32 GRADIENT_SIZE_MULT = 5;

    private PictureBox m_pbxImage = new PictureBox();
    private PictureBox m_pbxGradient = new PictureBox();
    private TextPosition m_tpsPosition = TextPosition.RightOfImage;
    private TransparentLabel m_tlbLabel = new TransparentLabel();
    private string m_strImageLocation;
    private Bitmap m_bmpOriginalImage;
    private bool m_booShowFade = true;

    #region Properties

    /// <summary>
    ///   Gets or sets whether to show the fade effect.
    /// </summary>
    /// <value>Whether to show the fade effect.</value>
    [Browsable(true), Category("Appearance"), DefaultValue(true),
     DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public bool ShowFade
    {
      get
      {
        return m_booShowFade;
      }
      set
      {
        if (m_booShowFade != value)
        {
          m_booShowFade = value;
          updateLayout();
        }
      }
    }

    /// <summary>
    ///   Gets or sets where to position the title text.
    /// </summary>
    /// <value>Where to position the title text.</value>
    [Browsable(true), Category("Appearance"), DefaultValue(TextPosition.RightOfImage),
     DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public TextPosition TextPosition
    {
      get
      {
        return m_tpsPosition;
      }
      set
      {
        if (m_tpsPosition != value)
        {
          m_tpsPosition = value;
          updateLayout();
        }
      }
    }

    /// <summary>
    ///   Gets or sets the title text.
    /// </summary>
    /// <value>The title text.</value>
    [Browsable(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override string Text
    {
      get
      {
        return m_tlbLabel.Text;
      }
      set
      {
        if (m_tlbLabel.Text != value)
        {
          m_tlbLabel.Text = value;
          updateLayout();
        }
      }
    }

    /// <summary>
    ///   Gets or sets the source location of the image to display.
    /// </summary>
    /// <value>The source location of the image to display.</value>
    [Browsable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public string ImageLocation
    {
      get
      {
        return m_strImageLocation;
      }
      set
      {
        if (m_strImageLocation != value)
        {
          m_strImageLocation = value;
          m_bmpOriginalImage = String.IsNullOrEmpty(m_strImageLocation) ? null : new Bitmap(m_strImageLocation);
          m_booShowFade = !String.IsNullOrEmpty(m_strImageLocation);
          updateLayout();
        }
      }
    }

    /// <summary>
    ///   Gets or sets the image to display.
    /// </summary>
    /// <value>The image to display.</value>
    [Browsable(true), Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    public Image Image
    {
      get
      {
        return m_bmpOriginalImage;
      }
      set
      {
        Bitmap bmpValue = null;
        if (value != null)
        {
          bmpValue = new Bitmap(value);
        }
        if (m_bmpOriginalImage != bmpValue)
        {
          m_bmpOriginalImage = bmpValue;
          m_strImageLocation = null;
          m_booShowFade = (m_bmpOriginalImage != null);
          updateLayout();
        }
      }
    }

    /// <summary>
    ///   Gets or sets the height of the header panel.
    /// </summary>
    /// <value>The height of the header panel.</value>
    public new Int32 Height
    {
      get
      {
        return base.Height;
      }
      set
      {
        if (base.Height != value)
        {
          base.Height = value;
          updateLayout();
        }
      }
    }

    #endregion

    #region Constructors

    /// <summary>
    ///   The default constructor.
    /// </summary>
    public HeaderPanel()
    {
      SuspendLayout();
      Controls.Add(m_pbxImage);
      Controls.Add(m_pbxGradient);
      m_tlbLabel.BackColor = Color.Transparent;
      Controls.Add(m_tlbLabel);
      m_pbxImage.SizeMode = PictureBoxSizeMode.StretchImage;
      m_pbxGradient.Dock = DockStyle.Fill;
      m_pbxGradient.SizeMode = PictureBoxSizeMode.StretchImage;
      m_booShowFade = !String.IsNullOrEmpty(m_strImageLocation) || (m_bmpOriginalImage != null);
      ResumeLayout();
      updateLayout();
    }

    #endregion

    /// <summary>
    ///   Positions the controls per the properties.
    /// </summary>
    private void updateLayout()
    {
      SuspendLayout();
      m_pbxImage.Dock = (m_tpsPosition == TextPosition.Left) ? DockStyle.Right : DockStyle.Left;
      m_pbxGradient.BringToFront();
      loadImage();
      m_tlbLabel.Top = (ClientSize.Height - m_tlbLabel.Height)/2;
      switch (m_tpsPosition)
      {
        case TextPosition.Left:
          m_tlbLabel.Left = 15;
          m_tlbLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
          break;
        case TextPosition.RightOfImage:
          m_tlbLabel.Left = m_pbxImage.Width + 15;
          m_tlbLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
          break;
        default:
          m_tlbLabel.Left = ClientSize.Width - m_tlbLabel.Width - 15;
          m_tlbLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
          break;
      }
      m_tlbLabel.BringToFront();
      ResumeLayout();

      Fade();
    }

    /// <summary>
    ///   Raises the <see cref="Control.FontChanged" /> event.
    /// </summary>
    /// <remarks>
    ///   This method updates the font of the label.
    /// </remarks>
    /// <param name="e">An <see cref="EventArgs" /> describing the event arguments.</param>
    protected override void OnFontChanged(EventArgs e)
    {
      base.OnFontChanged(e);
      m_tlbLabel.Font = Font;
      updateLayout();
    }

    /// <summary>
    ///   Raises the <see cref="Control.Reszie" /> event.
    /// </summary>
    /// <remarks>
    ///   This forces the label to refresh if it hasn't already done so.
    /// </remarks>
    /// <param name="e">An <see cref="EventArgs" /> describing the event arguments.</param>
    protected override void OnResize(EventArgs e)
    {
      base.OnResize(e);
      if (((m_tlbLabel.Anchor & AnchorStyles.Left) == AnchorStyles.Left) && m_booShowFade)
      {
        m_tlbLabel.Refresh();
      }
    }

    /// <summary>
    ///   Resizes the source image to fit into the header.
    /// </summary>
    /// <remarks>
    ///   The piture box automatically sizes the image to fit; however, resizing ourselves gives us
    ///   a significant performance advantage when creating the fade effect.
    /// </remarks>
    /// <param name="p_imgSource">The image to resize.</param>
    /// <param name="p_szeSize">The size to which to resize the image.</param>
    /// <returns>The resized image.</returns>
    protected Bitmap resize(Image p_imgSource, Size p_szeSize)
    {
      var bmpSmallPicture = new Bitmap(p_szeSize.Width, p_szeSize.Height, PixelFormat.Format32bppArgb);
      var grpPhoto = Graphics.FromImage(bmpSmallPicture);
      grpPhoto.DrawImage(p_imgSource, new Rectangle(0, 0, p_szeSize.Width, p_szeSize.Height),
                         new Rectangle(0, 0, p_imgSource.Width, p_imgSource.Height), GraphicsUnit.Pixel);
      return bmpSmallPicture;
    }

    /// <summary>
    ///   Loads the image into the header, and adjust the picture box to display the image correctly.
    /// </summary>
    private void loadImage()
    {
      var pbxImage = m_pbxImage;

      if (m_bmpOriginalImage == null)
      {
        m_bmpOriginalImage = new Bitmap(120, 90);
        using (var g = Graphics.FromImage(m_bmpOriginalImage))
        {
          g.FillRectangle(new SolidBrush(BackColor), 0, 0, 120, 90);
        }
      }
      var fltScale = (float) pbxImage.ClientSize.Height/m_bmpOriginalImage.Height;
      pbxImage.ClientSize = new Size((Int32) (Math.Round(fltScale*m_bmpOriginalImage.Width)), pbxImage.ClientSize.Height);
      var bmpImage = resize(m_bmpOriginalImage, pbxImage.ClientSize);
      pbxImage.Image = bmpImage;
    }

    /// <summary>
    ///   Generates the fade effect.
    /// </summary>
    private void Fade()
    {
      if (!m_booShowFade)
      {
        m_pbxGradient.Image = null;
        return;
      }

      var pbxImage = m_pbxImage;
      var pbxGradient = m_pbxGradient;
      var bmpImage = (Bitmap) pbxImage.Image;

      //set background of picture box to average colour of the image
      var intR = 0;
      var intG = 0;
      var intB = 0;
      var intCounter = 0;
      for (var i = 0; i < bmpImage.Width; i += 10)
      {
        for (var j = 0; j < bmpImage.Height; j += 10)
        {
          var clrPixel = bmpImage.GetPixel(i, j);
          intR += clrPixel.R;
          intG += clrPixel.G;
          intB += clrPixel.B;
          intCounter++;
        }
      }
      intR /= intCounter;
      intG /= intCounter;
      intB /= intCounter;
      pbxImage.BackColor = Color.FromArgb(255, intR, intG, intB);

      //fade out the edge of the image
      var intRange = bmpImage.Width/4;
      for (var i = 0; i < bmpImage.Height; i++)
      {
        var clrPixel = bmpImage.GetPixel(bmpImage.Width - intRange, i);
        var dblA = (double) clrPixel.A;
        var dblADelta = dblA/intRange;
        for (var j = intRange; j > 0; j--)
        {
          var intX = (m_tpsPosition == TextPosition.Left) ? j - 1 : bmpImage.Width - j;
          clrPixel = bmpImage.GetPixel(intX, i);
          intR = clrPixel.R;
          intG = clrPixel.G;
          intB = clrPixel.B;
          dblA -= dblADelta;
          var clrTmp = Color.FromArgb((Int32) dblA, intR, intG, intB);
          bmpImage.SetPixel(intX, i, clrTmp);
        }
      }

      //create a gradient fading out the average image colour
      var clrBackColour = pbxImage.BackColor;
      intR = clrBackColour.R;
      intG = clrBackColour.G;
      intB = clrBackColour.B;
      var bmpGradient = new Bitmap(256*GRADIENT_SIZE_MULT, bmpImage.Height);
      if (m_tpsPosition == TextPosition.Left)
      {
        for (var i = 0; i < bmpImage.Height; i++)
        {
          for (var j = 0; j < 256*GRADIENT_SIZE_MULT; j += GRADIENT_SIZE_MULT)
          {
            for (var n = 0; n < GRADIENT_SIZE_MULT; n++)
            {
              bmpGradient.SetPixel(j + n, i, Color.FromArgb(j/GRADIENT_SIZE_MULT, intR, intG, intB));
            }
          }
        }
      }
      else
      {
        for (var i = 0; i < bmpImage.Height; i++)
        {
          for (var j = 0; j < 256*GRADIENT_SIZE_MULT; j += GRADIENT_SIZE_MULT)
          {
            for (var n = 0; n < GRADIENT_SIZE_MULT; n++)
            {
              bmpGradient.SetPixel(j + n, i, Color.FromArgb(255 - j/GRADIENT_SIZE_MULT, intR, intG, intB));
            }
          }
        }
      }
      pbxGradient.Image = bmpGradient;
      m_tlbLabel.Refresh();
    }
  }
}