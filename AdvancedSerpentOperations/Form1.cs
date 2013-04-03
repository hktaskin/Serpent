using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Windows.Forms;
using LibSerpent;

/*
Author: Halil Kemal TASKIN
Web: http://hkt.me
*/

namespace AdvancedSerpentOperations
{
    public partial class Form1 : Form
    {
        #region Constrcutor

        public Form1()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
        }

        #endregion

        #region Fields

        Word[] W = new Word[4];

        Word[] O = new Word[4];

        FormDebug dbg = new FormDebug();

        RichTextBox[] txtList;

        Color RenkOne, RenkZero, RenkQuestion;

        string[] OpTypes = new string[] { "S-Box", "Inverse S-Box", "Permutation", "Inverse Permutation" };

        bool txtUpKontrol = false, txtDownKontrol = false;

        int EskiIdx = -1;

        #endregion

        #region Event Methods

        private void Form1_Load(object sender, EventArgs e)
        {
            AyarYukle();

            this.ShowInTaskbar = true;
            this.Text = "Advanced Serpent Operations - HKT - v" + Application.ProductVersion;

            button1.Tag = txtW0;
            button2.Tag = txtW0;

            button3.Tag = txtW1;
            button4.Tag = txtW1;

            button5.Tag = txtW2;
            button6.Tag = txtW2;

            button7.Tag = txtW3;
            button8.Tag = txtW3;

            button9.Tag = txtO0;
            button10.Tag = txtO0;

            button11.Tag = txtO1;
            button12.Tag = txtO1;

            button13.Tag = txtO2;
            button14.Tag = txtO2;

            button15.Tag = txtO3;
            button16.Tag = txtO3;

            btnCopyO.Tag = "O";
            btnCopyW.Tag = "W";
            btnPasteO.Tag = "O";
            btnPasteW.Tag = "W";

            btnRotLO.Tag = "LO";
            btnRotLW.Tag = "LW";
            btnRotRO.Tag = "RO";
            btnRotRW.Tag = "RW";

            LnkGuncelle();

            txtList = new RichTextBox[] { txtW0, txtW1, txtW2, txtW3, txtO0, txtO1, txtO2, txtO3 };

            cmbAutoOpList.DropDownStyle = ComboBoxStyle.DropDownList;
            for (int i = 0; i < OpTypes.Length; i++)
                cmbAutoOpList.Items.Add(OpTypes[i]);
            cmbAutoOpList.SelectedIndex = 0;

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save current settings
            AyarKaydet();
        }

        private void btnSBox_Click(object sender, EventArgs e)
        {
            ParseWandO();

            Serpent.ApplySerpentSBox(ref W[0], ref W[1], ref W[2], ref W[3], (int)nupSbox.Value);

            FillO(W);
        }

        private void btnPerm_Click(object sender, EventArgs e)
        {
            ParseWandO();

            Serpent.ApplySerpentPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);

            FillO(W);
        }

        private void btnInvSBox_Click(object sender, EventArgs e)
        {
            ParseWandO();

            Serpent.ApplySerpentInvSBox(ref O[0], ref O[1], ref O[2], ref O[3], (int)nupSbox.Value);

            FillW(O);
        }

        private void btnInvPerm_Click(object sender, EventArgs e)
        {
            ParseWandO();

            Serpent.ApplySerpentInvPermutation(ref O[0], ref O[1], ref O[2], ref O[3]);

            FillW(O);
        }

        private void btnImp_Click(object sender, EventArgs e)
        {
            ParseWandO();

            try
            {
                MessageBox.Show(Serpent.Impossible(W, O).ToString());
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }

        }

        private void btnNonZeroColumnNumber_Click(object sender, EventArgs e)
        {
            ParseWandO();

            int nw = Serpent.NonZeroColumnNumber(W);
            int no = Serpent.NonZeroColumnNumber(O);

            MessageBox.Show(string.Format("Non-zero Column Number\nW: {0}\nO: {1}", nw, no));
        }

        private void btnClearO_Click(object sender, EventArgs e)
        {
            txtO0.Clear();
            txtO1.Clear();
            txtO2.Clear();
            txtO3.Clear();
        }

        private void btnClearW_Click(object sender, EventArgs e)
        {
            txtW0.Clear();
            txtW1.Clear();
            txtW2.Clear();
            txtW3.Clear();
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            // Minimum number of non-zero columns

            string[] a1 = { "1101", "1111" };
            string[] a2 = { "0101", "1100" };
            string[] a3 = { "0011", "0110", "1001", "1010", "1011", "1100", "1110", "1111" };

            int min = 32, mini = 0, minj = 0, mink = 0;

            for (int i = 0; i < a1.Length; i++)
            {
                for (int j = 0; j < a2.Length; j++)
                {
                    for (int k = 0; k < a3.Length; k++)
                    {
                        for (int t = 0; t < 4; t++)
                            W[t] = Word.Parse("0000 0000 0000 0000 0000 0000 0000 0000");

                        Serpent.SetBitSlice(ref W, Word.Parse(a1[i]), 30);
                        Serpent.SetBitSlice(ref W, Word.Parse(a2[j]), 27);
                        Serpent.SetBitSlice(ref W, Word.Parse(a3[k]), 17);

                        Serpent.ApplySerpentInvPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);

                        int cnt = Serpent.NonZeroColumnNumber(W);
                        if (cnt < min)
                        {
                            min = cnt;
                            mini = i;
                            minj = j;
                            mink = k;
                        }
                    }
                }
            }

            MessageBox.Show(string.Format("Min: {0}\n i: {1} j: {2} k: {3}", min, a1[mini], a2[minj], a3[mink]));
        }

        private void btnMNZCAP_Click(object sender, EventArgs e)
        {
            int maxi = 0, maxj = 0, max = 33;

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 32; j++)
                {
                    for (int t = 0; t < 4; t++)
                        W[t] = Word.Parse("0000 0000 0000 0000 0000 0000 0000 0000");

                    W[i].SetBit('1', j);

                    Serpent.ApplySerpentPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);

                    int cnt = Serpent.NonZeroColumnNumber(W);

                    if (cnt == 2)
                    {
                        max = cnt;
                        maxi = i;
                        maxj = j;

                        MessageBox.Show(string.Format("Max: {0}\nWord: {1}\nBit: {2}", max, maxi, maxj));
                    }


                }

            }

            //MessageBox.Show(string.Format("Max: {0}\nWord: {1}\nBit: {2}",max,maxi,maxj));
        }

        private void btnCopyPaster(object sender, EventArgs e)
        {
            ParseWandO();

            try
            {
                string name = (sender as Button).Text;
                string tip = ((sender as Button).Tag as string);

                switch (name)
                {
                    case "Copy":
                        switch (tip)
                        {
                            case "O":
                                // Copy O
                                Clipboard.SetText(string.Format("O0: {0}\r\nO1: {1}\r\nO2: {2}\r\nO3: {3}\r\n", O[0], O[1], O[2], O[3]));
                                break;
                            case "W":
                                // Copy W
                                Clipboard.SetText(string.Format("W0: {0}\r\nW1: {1}\r\nW2: {2}\r\nW3: {3}\r\n", W[0], W[1], W[2], W[3]));
                                break;
                            default:
                                break;
                        }
                        break;
                    case "Paste":
                        string txtData = Clipboard.GetText();
                        if (txtData.Length == 0)
                            break;

                        string[] Wlines = txtData.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
                        RichTextBox[] txtBox;
                        int txtIndex = 0;

                        switch (tip)
                        {
                            case "O":
                                // Paste O
                                txtBox = new RichTextBox[] { txtO0, txtO1, txtO2, txtO3 };
                                break;
                            case "W":
                                // Paste W
                                txtBox = new RichTextBox[] { txtW0, txtW1, txtW2, txtW3 };
                                break;
                            default:
                                txtBox = null;
                                break;
                        }

                        for (int i = 0; i < Wlines.Length; i++)
                        {
                            Word w = Word.Parse(Wlines[i]);
                            if (w.Size == 0)
                                continue;

                            txtBox[txtIndex++].Text = w.ToString();
                            if (txtIndex == 4)
                                break;
                        }
                        break;

                    default:
                        break;
                }
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void btnRot(object sender, EventArgs e)
        {
            string item = (sender as Button).Tag as string;
            int rotcnt = 0;

            ParseWandO();

            switch (item)
            {
                case "LW":
                    rotcnt = (int)nupRotW.Value;
                    for (int i = 0; i < 4; i++)
                        W[i].RotateLeft(rotcnt);
                    FillW(W);
                    break;

                case "RW":
                    rotcnt = (int)nupRotW.Value;
                    for (int i = 0; i < 4; i++)
                        W[i].RotateRight(rotcnt);
                    FillW(W);
                    break;

                case "LO":
                    rotcnt = (int)nupRotO.Value;
                    for (int i = 0; i < 4; i++)
                        O[i].RotateLeft(rotcnt);
                    FillO(O);
                    break;

                case "RO":
                    rotcnt = (int)nupRotO.Value;
                    rotcnt = (int)nupRotO.Value;
                    for (int i = 0; i < 4; i++)
                        O[i].RotateRight(rotcnt);
                    FillO(O);
                    break;

                default:
                    break;
            }
        }

        private void btn05to3_Click(object sender, EventArgs e)
        {
            ParseWandO();

            Serpent.ApplySerpentPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);

            Serpent.ApplySerpentSBox(ref W[0], ref W[1], ref W[2], ref W[3], 1);
            Serpent.ApplySerpentPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);

            Serpent.ApplySerpentSBox(ref W[0], ref W[1], ref W[2], ref W[3], 2);
            Serpent.ApplySerpentPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);

            FillO(W);
        }

        private void btn55to3_Click(object sender, EventArgs e)
        {
            ParseWandO();

            Serpent.ApplySerpentInvSBox(ref O[0], ref O[1], ref O[2], ref O[3], 5);

            Serpent.ApplySerpentInvPermutation(ref O[0], ref O[1], ref O[2], ref O[3]);
            Serpent.ApplySerpentInvSBox(ref O[0], ref O[1], ref O[2], ref O[3], 4);

            Serpent.ApplySerpentInvPermutation(ref O[0], ref O[1], ref O[2], ref O[3]);
            Serpent.ApplySerpentInvSBox(ref O[0], ref O[1], ref O[2], ref O[3], 3);

            FillW(O);

        }

        private void btnWtoO_Click(object sender, EventArgs e)
        {
            if (txtW0.TextLength == 0 && txtW1.TextLength == 0 && txtW2.TextLength == 0 && txtW3.TextLength == 0)
                return;

            txtO0.Text = txtW0.Text;
            txtO1.Text = txtW1.Text;
            txtO2.Text = txtW2.Text;
            txtO3.Text = txtW3.Text;

            txtW0.Text = "";
            txtW1.Text = "";
            txtW2.Text = "";
            txtW3.Text = "";
        }

        private void btnOtoW_Click(object sender, EventArgs e)
        {
            if (txtO0.TextLength == 0 && txtO1.TextLength == 0 && txtO2.TextLength == 0 && txtO3.TextLength == 0)
                return;

            txtW0.Text = txtO0.Text;
            txtW1.Text = txtO1.Text;
            txtW2.Text = txtO2.Text;
            txtW3.Text = txtO3.Text;

            txtO0.Text = "";
            txtO1.Text = "";
            txtO2.Text = "";
            txtO3.Text = "";
        }

        private void btnProbWtoO_Click(object sender, EventArgs e)
        {
            try
            {
                ParseWandO();

                dbg.ClearDebug();

                BigInteger toplamPay = 1, toplamPayda = 1;
                int toplamPaydaExp = 0, toplamPayExp = 0;

                dbg.WriteLine("Finding Probability from Input to Output\r\nS-Box: " + nupSbox.Value + "\r\n");

                for (int i = 0; i < W.Length; i++)
                    dbg.WriteLine(string.Format("W{0}: {1}", i, W[i]));

                dbg.WriteLine("");

                for (int i = 0; i < O.Length; i++)
                    dbg.WriteLine(string.Format("O{0}: {1}", i, O[i]));

                dbg.WriteLine("");

                for (int i = 0; i < W[0].Size; i++)
                {
                    Word w = Serpent.GetBitSlice(W, i);
                    Word o = Serpent.GetBitSlice(O, i);

                    int tmpPay, tmpPayda, tmpGCD;

                    Serpent.InputToOutputProbability(w, o, (int)nupSbox.Value, out tmpPay, out tmpPayda);

                    tmpGCD = GCD(tmpPayda, tmpPay);

                    tmpPayda /= tmpGCD;
                    tmpPay /= tmpGCD;

                    int tmpPaydaExp = (int)Math.Log(tmpPayda, 2);
                    int tmpPayExp = (int)Math.Log(tmpPay, 2);

                    toplamPaydaExp += tmpPaydaExp;
                    toplamPayExp += tmpPayExp;

                    dbg.WriteLine(string.Format("BitSlice:{4} Input: {0} Output: {1} Probability: {2}/{3}", w, o, tmpPay, tmpPayda, i));

                    toplamPay *= tmpPay;
                    toplamPayda *= tmpPayda;

                }

                dbg.WriteLine(string.Format("\r\nTotal Probability:\r\n\r\n{0} (~2^{3})\r\n__________________________\r\n{1} (~2^{2})", toplamPay, toplamPayda, toplamPaydaExp, toplamPayExp));

                if (toplamPay == 0)
                    dbg.WriteLine("\r\nTotal Probability ~= 0");
                else
                    dbg.WriteLine(string.Format("\r\nTotal Probability ~= 2^{0}", toplamPayExp - toplamPaydaExp));

                MessageBox.Show("Operation Completed. Check Log screen for results.", "Find Input to Output Probability", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }

        }

        private void btnLogScreen_Click(object sender, EventArgs e)
        {
            dbg.Show();
            dbg.BringToFront();
        }

        private void btnProbOtoW_Click(object sender, EventArgs e)
        {
            try
            {
                ParseWandO();

                dbg.ClearDebug();

                BigInteger toplamPay = 1, toplamPayda = 1;
                int toplamPaydaExp = 0, toplamPayExp = 0;

                dbg.WriteLine("Finding Inverse Probability from Output to Input\r\nS-Box: " + nupSbox.Value + "\r\n");

                for (int i = 0; i < O.Length; i++)
                    dbg.WriteLine(string.Format("O{0}: {1}", i, O[i]));

                dbg.WriteLine("");

                for (int i = 0; i < W.Length; i++)
                    dbg.WriteLine(string.Format("W{0}: {1}", i, W[i]));

                dbg.WriteLine("");

                for (int i = 0; i < W[0].Size; i++)
                {
                    Word w = Serpent.GetBitSlice(W, i);
                    Word o = Serpent.GetBitSlice(O, i);

                    int tmpPay, tmpPayda, tmpGCD;

                    Serpent.OutputToInputProbability(o, w, (int)nupSbox.Value, out tmpPay, out tmpPayda);

                    tmpGCD = GCD(tmpPayda, tmpPay);

                    tmpPayda /= tmpGCD;
                    tmpPay /= tmpGCD;

                    int tmpPaydaExp = (int)Math.Log(tmpPayda, 2);
                    int tmpPayExp = (int)Math.Log(tmpPay, 2);

                    toplamPaydaExp += tmpPaydaExp;
                    toplamPayExp += tmpPayExp;

                    dbg.WriteLine(string.Format("BitSlice:{4} Output: {1} Input: {0} Probability: {2}/{3}", w, o, tmpPay, tmpPayda, i));

                    toplamPay *= tmpPay;
                    toplamPayda *= tmpPayda;

                }

                dbg.WriteLine(string.Format("\r\nTotal Probability:\r\n\r\n{0} (~2^{3})\r\n__________________________\r\n{1} (~2^{2})", toplamPay, toplamPayda, toplamPaydaExp, toplamPayExp));

                if (toplamPay == 0)
                    dbg.WriteLine("\r\nTotal Probability ~= 0");
                else
                    dbg.WriteLine(string.Format("\r\nTotal Probability ~= 2^{0}", toplamPayExp - toplamPaydaExp));

                MessageBox.Show("Operation Completed. Check Log screen for results.", "Find Output to Input Probability", MessageBoxButtons.OK, MessageBoxIcon.Information);

            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            // Permutation Probability

            ParseWandO();

            int w = Word.WordsContainQuestionMark(W);
            int o = Word.WordsContainQuestionMark(O);

            int result = w > o ? w - o : o - w;

            MessageBox.Show("Probability: 2^-" + result);
        }

        private void btnWquestion_Click(object sender, EventArgs e)
        {
            ParseWandO();

            dbg.ClearDebug();

            int[] a = Serpent.NonZeroColumnPositions(W);

            string pos = string.Join("-", a);

            dbg.WriteLine("Details for W: \r\n");

            dbg.WriteLine(string.Format("Number of 0 in W: {0}", Word.WordsContainZero(W)));
            dbg.WriteLine(string.Format("Number of 1 in W: {0}", Word.WordsContainOne(W)));
            dbg.WriteLine(string.Format("Number of ? in W: {0}\r\n", Word.WordsContainQuestionMark(W)));

            dbg.WriteLine(string.Format("Non-Zero Columns (#{0}): {1}", a.Length, pos));

            MessageBox.Show("General Information has been created for W.\r\nFor details please check Log Screen.");
        }

        private void btnOquestion_Click(object sender, EventArgs e)
        {
            ParseWandO();

            dbg.ClearDebug();

            int[] a = Serpent.NonZeroColumnPositions(O);

            string pos = string.Join("-", a);

            dbg.WriteLine("Details for O: \r\n");

            dbg.WriteLine(string.Format("Number of 0 in O: {0}", Word.WordsContainZero(O)));
            dbg.WriteLine(string.Format("Number of 1 in O: {0}", Word.WordsContainOne(O)));
            dbg.WriteLine(string.Format("Number of ? in O: {0}\r\n", Word.WordsContainQuestionMark(O)));

            dbg.WriteLine(string.Format("Non-Zero Columns (#{0}): {1}", a.Length, pos));

            MessageBox.Show("General Information has been created for O.\r\nFor details please check Log Screen.");
        }

        private void btnTest1_Click(object sender, EventArgs e)
        {
            dbg.ClearDebug();
            dbg.WriteLine("In Serpent We have broken!\r\n");

            Word[] tmpW = new Word[4];

            int[] a = new int[] { 2, 4, 5, 9, 10, 14 };
            int[] b = new int[] { 5, 7, 12, 14 };
            int konuma = 25, konumb = 22;

            for (int i = 0; i < a.Length; i++)
            {
                for (int j = 0; j < b.Length; j++)
                {
                    for (int t = 0; t < 4; t++)
                        tmpW[t] = Word.Parse("0000 0000 0000 0000 0000 0000 0000 0000");

                    Word worda = a[i].ToWord(4);
                    Word wordb = b[j].ToWord(4);

                    Serpent.SetBitSlice(ref tmpW, worda, konuma);
                    Serpent.SetBitSlice(ref tmpW, wordb, konumb);

                    Serpent.ApplySerpentInvPermutation(ref tmpW[0], ref tmpW[1], ref tmpW[2], ref tmpW[3]);

                    int[] nzcpos = Serpent.NonZeroColumnPositions(tmpW);

                    string pos = string.Join("-", nzcpos);

                    dbg.WriteLine(string.Format("a: {0} b: {1} Number of Non-Zero Columns: {2}\r\nPositions: {3}\r\n", a[i], b[j], nzcpos.Length, pos));


                }

            }

            MessageBox.Show("Check Log Screen!");
        }

        private void btnAutoDel_Click(object sender, EventArgs e)
        {
            if (lstAutoOpList.SelectedIndex > -1)
                lstAutoOpList.Items.RemoveAt(lstAutoOpList.SelectedIndex);
        }

        private void btnAutoOpStart_Click(object sender, EventArgs e)
        {
            try
            {
                ParseWandO();

                for (int i = 0; i < lstAutoOpList.Items.Count; i++)
                {
                    string s = lstAutoOpList.Items[i].ToString();
                    if (s.Contains(" - "))
                    {
                        // S-Box
                        int rnd = int.Parse(s.Split(new string[] { " - " }, StringSplitOptions.None)[1]);

                        if (s.Contains("Inverse"))
                        {
                            // Inverse
                            Serpent.ApplySerpentInvSBox(ref W[0], ref W[1], ref W[2], ref W[3], rnd);
                        }
                        else
                        {
                            // Normal
                            Serpent.ApplySerpentSBox(ref W[0], ref W[1], ref W[2], ref W[3], rnd);
                        }

                    }
                    else
                    {
                        // Permutation
                        if (s.Contains("Inverse"))
                        {
                            // Inverse
                            Serpent.ApplySerpentInvPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);

                        }
                        else
                        {
                            // Normal
                            Serpent.ApplySerpentPermutation(ref W[0], ref W[1], ref W[2], ref W[3]);
                        }
                    }
                }

                FillO(W);
            }
            catch
            {
                MessageBox.Show("There was an error!");
            }

        }

        private void btnAutoDeleteSelected_Click(object sender, EventArgs e)
        {
            if (lstAutoOpList.SelectedIndex > -1)
                lstAutoOpList.Items.RemoveAt(lstAutoOpList.SelectedIndex);

            //MessageBox.Show(cmbAutoOpList.SelectedIndex.ToString());
        }

        private void btnAutoClearList_Click(object sender, EventArgs e)
        {
            lstAutoOpList.Items.Clear();
        }

        private void btnAutoSaveTofile_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                sfd.Filter = "Text Files|*.txt";
                sfd.FileName = "Automated Execution List.txt";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;

                string[] str = new string[lstAutoOpList.Items.Count];
                for (int i = 0; i < lstAutoOpList.Items.Count; i++)
                    str[i] = lstAutoOpList.Items[i].ToString();

                File.WriteAllLines(sfd.FileName, str);
            }
            catch
            {

            }
        }

        private void btnAutoLoadFromFile_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog ofd = new OpenFileDialog();
                ofd.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                ofd.Filter = "Text Files|*.txt";

                if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                    return;

                string[] str = File.ReadAllLines(ofd.FileName);

                lstAutoOpList.Items.Clear();
                for (int i = 0; i < str.Length; i++)
                {
                    if (str[i] != "")
                        lstAutoOpList.Items.Add(str[i]);
                }
            }
            catch
            {
                lstAutoOpList.Items.Clear();
            }


        }

        private void btnAutoOpAdd_Click(object sender, EventArgs e)
        {
            if (cmbAutoOpList.SelectedIndex == 0)
            {
                lstAutoOpList.Items.Add(cmbAutoOpList.SelectedItem + " - " + nupAutoSBox.Value);
                nupAutoSBox.Value = (nupAutoSBox.Value + 1) % 8;
                cmbAutoOpList.SelectedIndex = 2;
            }
            else if (cmbAutoOpList.SelectedIndex == 1)
            {
                lstAutoOpList.Items.Add(cmbAutoOpList.SelectedItem + " - " + nupAutoSBox.Value);
                nupAutoSBox.Value = (nupAutoSBox.Value - 1) < 0 ? nupAutoSBox.Value + 7 : nupAutoSBox.Value - 1;
                cmbAutoOpList.SelectedIndex = 3;
            }
            else if (cmbAutoOpList.SelectedIndex == 2)
            {
                lstAutoOpList.Items.Add(cmbAutoOpList.SelectedItem);
                cmbAutoOpList.SelectedIndex = 0;
            }
            else
            {
                lstAutoOpList.Items.Add(cmbAutoOpList.SelectedItem);
                cmbAutoOpList.SelectedIndex = 1;
            }
        }

        private void txtW_DragDrop(object sender, DragEventArgs e)
        {
            btnCopyPaster(btnPasteW, null);
        }

        private void txtW_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        private void txtO_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.StringFormat))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;

        }

        private void txtO_DragDrop(object sender, DragEventArgs e)
        {
            btnCopyPaster(btnPasteO, null);
        }

        private void txtChanged(object sender, EventArgs e)
        {
            RichTextBox r = sender as RichTextBox;

            int ilkkonum = r.SelectionStart;

            //r.Text = Word.Parse(r.Text).ToString();

            for (int i = 0; i < r.TextLength; i++)
            {
                r.Select(i, 1);
                if (r.SelectedText == "1")
                    r.SelectionColor = RenkOne;
                else if (r.SelectedText == "?")
                    r.SelectionColor = RenkQuestion;
                else
                    r.SelectionColor = RenkZero;
            }

            r.Select(ilkkonum, 0);
        }

        private void txtKeyDown(object sender, KeyEventArgs e)
        {
            RichTextBox r = sender as RichTextBox;

            if (e.KeyCode == Keys.Down || e.KeyCode == Keys.Up)
            {
                int index = int.MaxValue;
                for (int i = 0; i < txtList.Length; i++)
                    if (r == txtList[i])
                    {
                        index = i;
                        break;
                    }

                int yeniindex = e.KeyCode == Keys.Down ? (index + 1) % txtList.Length : ((index - 1) < 0 ? index + txtList.Length - 1 : index - 1) % txtList.Length;
                int selkonum = r.SelectionStart;
                txtList[yeniindex].Focus();
                txtList[yeniindex].Select(selkonum, 0);
            }

            if (r.SelectionStart == r.TextLength)
                txtUpKontrol = true;
            if (r.SelectionStart == 0)
                txtDownKontrol = true;

        }

        private void txtKeyUp(object sender, KeyEventArgs e)
        {
            if (txtUpKontrol || txtDownKontrol)
            {
                txtUpKontrol = false;
                txtDownKontrol = false;

                RichTextBox r = sender as RichTextBox;

                if (r.SelectionStart == r.TextLength && e.KeyCode == Keys.Right)
                {
                    r.Select(0, 0);
                }
                else if (r.SelectionStart == 0 && e.KeyCode == Keys.Left)
                {
                    r.Select(r.TextLength + 1, 0);
                }
            }
        }

        private void txtKeyPress(object sender, KeyPressEventArgs e)
        {
            if ("01 \b".IndexOf(e.KeyChar) < 0)
                e.KeyChar = '?';
        }

        private void chkAOT_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = chkAOT.Checked;
        }

        private void FillOneOrZero(object sender, EventArgs e)
        {
            ((sender as Button).Tag as RichTextBox).Text = string.Format("{0} {0} {0} {0} {0} {0} {0} {0}", new string(((Button)sender).Text[0], 4));
        }

        private void lnkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //MessageBox.Show((sender as LinkLabel).Text);

            ColorDialog cd = new ColorDialog();

            string Tip = (sender as LinkLabel).Text;

            switch (Tip)
            {
                case "0":
                    cd.Color = RenkZero;
                    break;
                case "1":
                    cd.Color = RenkOne;
                    break;
                case "?":
                    cd.Color = RenkQuestion;
                    break;
            }

            if (cd.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                return;

            switch (Tip)
            {
                case "0":
                    RenkZero = cd.Color;
                    break;
                case "1":
                    RenkOne = cd.Color;
                    break;
                case "?":
                    RenkQuestion = cd.Color;
                    break;
            }

            LnkGuncelle();

            for (int i = 0; i < txtList.Length; i++)
                txtChanged(txtList[i], null);

        }

        private void cmbAutoOpList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAutoOpList.SelectedIndex == 0 || cmbAutoOpList.SelectedIndex == 1)
                nupAutoSBox.Visible = true;
            else
                nupAutoSBox.Visible = false;
        }

        private void lstAutoOpList_MouseDown(object sender, MouseEventArgs e)
        {
            if (lstAutoOpList.SelectedIndex > -1 && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                lstAutoOpList.DoDragDrop(lstAutoOpList.SelectedItem, DragDropEffects.Move);

            }
        }

        private void lstAutoOpList_DragDrop(object sender, DragEventArgs e)
        {
            label1.Text = "Done";
            EskiIdx = -1;
        }

        private void lstAutoOpList_DragEnter(object sender, DragEventArgs e)
        {

            e.Effect = DragDropEffects.Move;
            //lstAutoOpList.Items.RemoveAt(lstAutoOpList.SelectedIndex);
            //label1.Text = "Enter";

        }

        private void lstAutoOpList_DragOver(object sender, DragEventArgs e)
        {
            int idx = lstAutoOpList.IndexFromPoint(lstAutoOpList.PointToClient(new Point(e.X, e.Y)));

            if (EskiIdx < 0) EskiIdx = lstAutoOpList.SelectedIndex;

            //if (idx > -1 && EskiIdx > -1 && EskiIdx < lstAutoOpList.Items.Count)
            if (idx > -1)
            {
                lstAutoOpList.Items.RemoveAt(EskiIdx);
                //label2.Text = "D: " + idx;
                lstAutoOpList.Items.Insert(idx, e.Data.GetData(DataFormats.Text));
                lstAutoOpList.SelectedIndex = idx;
                EskiIdx = idx;
            }

            //lstAutoOpList.Items.Insert(


        }

        #endregion

        #region Methods

        void AyarYukle()
        {
            Ayarlar a = AyarlarTools.DosyadanAyarYukle();

            RenkOne = a.RenkOne;
            RenkZero = a.RenkZero;
            RenkQuestion = a.RenkQuestion;

            txtW0.Text = a.W0;
            txtW1.Text = a.W1;
            txtW2.Text = a.W2;
            txtW3.Text = a.W3;

            txtO0.Text = a.O0;
            txtO1.Text = a.O1;
            txtO2.Text = a.O2;
            txtO3.Text = a.O3;

            nupRotW.Value = a.RotW;
            nupRotO.Value = a.RotO;
            nupSbox.Value = a.SBox;

            chkAOT.Checked = a.AOT;

            lstAutoOpList.Items.Clear();
            for (int i = 0; i < a.AutoOpList.Length; i++)
                lstAutoOpList.Items.Add(a.AutoOpList[i]);


            dbg.ClearDebug();
            dbg.Write(a.Log);
        }

        void AyarKaydet()
        {
            Ayarlar a = new Ayarlar();

            a.RenkOne = RenkOne;
            a.RenkZero = RenkZero;
            a.RenkQuestion = RenkQuestion;

            a.W0 = txtW0.Text;
            a.W1 = txtW1.Text;
            a.W2 = txtW2.Text;
            a.W3 = txtW3.Text;

            a.O0 = txtO0.Text;
            a.O1 = txtO1.Text;
            a.O2 = txtO2.Text;
            a.O3 = txtO3.Text;

            a.RotW = nupRotW.Value;
            a.RotO = nupRotO.Value;
            a.SBox = nupSbox.Value;

            a.AOT = chkAOT.Checked;

            a.Log = dbg.ReadAllDebug();

            string[] strAutoOpList = new string[lstAutoOpList.Items.Count];
            for (int i = 0; i < strAutoOpList.Length; i++)
                strAutoOpList[i] = lstAutoOpList.Items[i].ToString();

            a.AutoOpList = strAutoOpList;

            AyarlarTools.DosyayaAyarKaydet(a);
        }

        void LnkGuncelle()
        {
            lnkOne.LinkColor = RenkOne;
            //lnkOne.BackColor = Color.FromArgb(int.MaxValue - RenkOne.ToArgb());
            lnkZero.LinkColor = RenkZero;
            lnkQuestion.LinkColor = RenkQuestion;
        }

        void FillO(Word[] o)
        {
            txtO0.Text = o[0].ToString();
            txtO1.Text = o[1].ToString();
            txtO2.Text = o[2].ToString();
            txtO3.Text = o[3].ToString();
        }

        void FillW(Word[] w)
        {
            txtW0.Text = w[0].ToString();
            txtW1.Text = w[1].ToString();
            txtW2.Text = w[2].ToString();
            txtW3.Text = w[3].ToString();
        }

        void ParseWandO()
        {
            try
            {
                W[0] = Word.Parse(txtW0.Text);
                W[1] = Word.Parse(txtW1.Text);
                W[2] = Word.Parse(txtW2.Text);
                W[3] = Word.Parse(txtW3.Text);

                O[0] = Word.Parse(txtO0.Text);
                O[1] = Word.Parse(txtO1.Text);
                O[2] = Word.Parse(txtO2.Text);
                O[3] = Word.Parse(txtO3.Text);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
        }

        int GCD(int a, int b)
        {
            // Recursive Greatest Common Divisor Alogrithm
            return b == 0 ? a : GCD(b, a % b);
        }

        #endregion

    }
}
