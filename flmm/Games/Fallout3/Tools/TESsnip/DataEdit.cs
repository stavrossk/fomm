using System;
using System.Windows.Forms;
using Fomm.Properties;

namespace Fomm.Games.Fallout3.Tools.TESsnip
{
  partial class DataEdit : Form
  {
    public static bool Canceled;
    public static byte[] result;
    public static string resultName;

    public DataEdit(string RecName, byte[] data)
    {
      InitializeComponent();
      Icon = Resources.fomm02;
      tbName.Text = RecName;
      var s = "";
      foreach (var b in data)
      {
        s += (char) b;
      }
      tbEdit.Text = s;
      Canceled = true;
    }

    private void bSave_Click(object sender, EventArgs e)
    {
      Canceled = false;
      result = new byte[tbEdit.Text.Length + 1];
      for (var i = 0; i < tbEdit.Text.Length; i++)
      {
        result[i] = (byte) tbEdit.Text[i];
      }
      result[tbEdit.Text.Length] = 0;
      resultName = tbName.Text;
      Close();
    }

    private void bCancel_Click(object sender, EventArgs e)
    {
      Canceled = true;
      Close();
    }

    private void tbName_KeyPress(object sender, KeyPressEventArgs e)
    {
      if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && !char.IsLetter(e.KeyChar) && e.KeyChar != '_')
      {
        e.Handled = true;
      }
    }

    private void tbName_Leave(object sender, EventArgs e)
    {
      if (tbName.Text.Length < 4)
      {
        tbName.Text = tbName.Text.PadRight(4, '_');
      }
    }
  }
}