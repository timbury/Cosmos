﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Cosmos.Debug.Consts;
using System.Windows.Threading;
using System.Threading;

namespace Cosmos.VS.Windows {
  public partial class AssemblyUC : UserControl {
    static public byte[] mData = new byte[0];
    string mCode = "";
    bool mFilter = true;

    public AssemblyUC() {
      InitializeComponent();

      mitmCopy.Click += new RoutedEventHandler(mitmCopy_Click);
      butnPing.Click += new RoutedEventHandler(butnPing_Click);

      Update(mData);
    }

    void butnPing_Click(object sender, RoutedEventArgs e) {
      Global.mPipeUp.SendCommand(Cosmos.Debug.Consts.DwCmd.Ping, null);
    }

    void mitmCopy_Click(object sender, RoutedEventArgs e) {
      Clipboard.SetText(mCode);
    }

    protected void Display(bool aFilter) {
      tblkSource.Inlines.Clear();
      if (mData.Length == 0) {
        return;
      }

      mCode = Encoding.ASCII.GetString(mData);

      // Used for creating a test file for Cosmos.VS.Windows.Test
      //System.IO.File.WriteAllBytes(@"D:\source\Cosmos\source2\VSIP\Cosmos.VS.Windows.Test\SourceTest.bin", mData);

      // Should always be \r\n, but just in case we split by \n and ignore \r
      string[] xLines = mCode.Replace("\r", "").Split('\n');
      foreach (string xLine in xLines) {
        string xDisplayLine = xLine;
        string xTestLine = xLine.Trim().ToUpper();
        var xParts = xTestLine.Split(' ');

        if (aFilter) {
          if (xTestLine == "INT3") {
            continue;
          } else if (xTestLine.Length == 0) {
            continue;
          } else {
            if (xParts.Length > 1) {
              if (xParts[1] == ";ASM") {
                continue;
              }
            }
          }

          xDisplayLine = xDisplayLine.Trim();
          if (xParts[0].EndsWith(":")) {
            // Insert a blank line before labels, but not if its the top line
            if (tblkSource.Inlines.Count > 0) {
              tblkSource.Inlines.Add(new LineBreak());
            }
          } else {
            xDisplayLine = "\t" + xDisplayLine;
          }
        }

        var xRun = new Run(xDisplayLine.Replace("\t", "    "));
        xRun.FontFamily = new FontFamily("Consolas");
        if (xParts[0].EndsWith(":")) {
          xRun.Foreground = Brushes.Black;
        } else if (xTestLine.StartsWith(";")) {
          xRun.Foreground = Brushes.Green;
        } else {
          xRun.Foreground = Brushes.Blue;
        }

        tblkSource.Inlines.Add(xRun);
        tblkSource.Inlines.Add(new LineBreak());
      }
    }

    public void Update(byte[] aData) {
      mData = aData;
      Display(mFilter);
    }

    private void asmFilterButton_Click(object sender, RoutedEventArgs e) {
      mFilter = !mFilter;
      Display(mFilter);
    }

    private void asmFCopyButton_Click(object sender, RoutedEventArgs e) {
      Clipboard.SetText(mCode);
    }

    private void asmStepButton_Click(object sender, RoutedEventArgs e) {
    }

  }
}