using System;

/*
Author: Halil Kemal TASKIN
Web: http://hkt.me
*/

namespace LibSerpent
{
    public class Word
    {
        #region Constructor

        public Word(int bitsize)
        {
            _bits = new char[bitsize];
            _size = bitsize;
        }

        #endregion

        #region Fields

        private int _size;
        private char[] _bits;

        #endregion

        #region Properties

        public int Size { get { return _size; } }
        public int MaxNibbleSize { get { return _size / 4; } }
        public char[] Bits
        {
            get
            {
                return _bits;
            }
            // set
            // {
            //     _bits = value;
            // }
        }

        #endregion

        #region Public Methods

        public char GetBit(int pos)
        {
            return _bits[pos];
        }

        public void SetBit(char bit, int pos)
        {
            _bits[pos] = bit;
        }

        public void SetBits(string bits)
        {
            string s = CleanInputData(bits); // bits.Replace(" ", "");
            if (s.Length != this._size)
                throw new Exception("Bit String length should be equal to Word size.");
            for (int i = 0; i < s.Length; i++)
                _bits[i] = s[_size - i - 1];
        }

        public void RotateLeft(int count)
        {
            char[] tmp = new char[_size];
            for (int i = 0; i < _size; i++)
                tmp[i] = _bits[(i - count < 0 ? (_size + ((i - count) % _size)) % _size : i - count)];
            _bits = tmp;
        }

        public void RotateRight(int count)
        {
            char[] tmp = new char[_size];
            for (int i = 0; i < _size; i++)
                tmp[i] = _bits[(i + count) % _size];
            _bits = tmp;
        }

        public Word GetNibble(int block)
        {
            Word n = new Word(4);
            for (int i = 0; i < 4; i++)
                n.SetBit(_bits[i + (block * 4)], i);
            return n;
        }

        public void SetNibble(Word nb, int block)
        {
            if (nb.Size != 4)
                throw new Exception("Nibble size should be 4!");


            for (int i = 0; i < 4; i++)
                _bits[i + (block * 4)] = nb.GetBit(i);
        }

        public bool IsEqualTo(string s)
        {
            Word w = Word.Parse(s.Replace(" ", ""));
            if (_size != w.Size)
                return false;

            for (int i = 0; i < _size; i++)
                if (w.GetBit(i) != _bits[i])
                    return false;

            return true;
        }

        public bool Contains(char bit)
        {
            for (int i = 0; i < _size; i++)
                if (_bits[i] == bit)
                    return true;
            return false;
        }

        public int[] QuestionMarkPositions()
        {
            int[] tmp = new int[0];
            for (int i = 0; i < _size; i++)
            {
                if (_bits[i] == '?')
                {
                    Array.Resize(ref tmp, tmp.Length + 1);
                    tmp[tmp.Length - 1] = i;
                }
            }

            return tmp;
        }

        public int ToInt32()
        {
            if (_bits.Length > 32)
                throw new Exception("Word Size should be less or equal to 32!");
            int sonuc = 0;
            for (int i = 0; i < _bits.Length; i++)
                sonuc += (_bits[i] == '1' ? 1 : 0) * (1 << i);
            return sonuc;
        }

        /// <returns>String representation of the bits with LSB on the right</returns>
        public override string ToString()
        {
            //return string.Join("", _bits.Reverse());
            string s = "";
            for (int i = 0; i < _size; i++)
            {
                s += _bits[i];
                if ((i + 1) % 4 == 0)
                    s += ' ';
            }
            char[] c = s.ToCharArray();
            Array.Reverse(c, 0, c.Length);
            return (new string(c)).Trim();
        }

        #endregion

        #region Private Methods

        private static string CleanInputData(string inputdata)
        {
            string data = inputdata;
            if (inputdata.IndexOf(":") > -1)
                data = inputdata.Split(':')[1];

            data = data.Replace('2', '?');

            string s = "";
            for (int i = 0; i < data.Length; i++)
            {
                if (data[i] == '0' || data[i] == '1' || data[i] == '?')
                    s += data[i];
            }

            return s;
        }

        #endregion

        #region Static Methods

        /// <param name="s">LSB value is on the right</param>
        public static Word Parse(string str)
        {
            string s = CleanInputData(str); //str.Replace(" ", "");
            Word w = new Word(s.Length);
            for (int i = 0; i < s.Length; i++)
                w.SetBit(s[s.Length - i - 1], i);
            return w;
        }

        public static Word Parse(char[] c)
        {
            Word w = new Word(c.Length);
            for (int i = 0; i < c.Length; i++)
                w.SetBit(c[i], i);
            return w;
        }

        public static bool WordsContainZeroOrOne(params Word[] values)
        {
            int len = values.Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < values[i].Size; j++)
                {
                    if (values[i].GetBit(j) == '0' || values[i].GetBit(j) == '1')
                        return true;
                }
            }

            return false;
        }

        public static int WordsContainOne(params Word[] values)
        {
            int sayac = 0;
            int len = values.Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < values[i].Size; j++)
                {
                    if (values[i].GetBit(j) == '1')
                        sayac++;
                }
            }

            return sayac;
        }

        public static int WordsContainZero(params Word[] values)
        {
            int sayac = 0;
            int len = values.Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < values[i].Size; j++)
                {
                    if (values[i].GetBit(j) == '0')
                        sayac++;
                }
            }

            return sayac;
        }

        public static int WordsContainQuestionMark(params Word[] values)
        {
            int sayac = 0;
            int len = values.Length;
            for (int i = 0; i < len; i++)
            {
                for (int j = 0; j < values[i].Size; j++)
                {
                    if (values[i].GetBit(j) == '?')
                        sayac++;
                }
            }

            return sayac;
        }

        #endregion

        #region Operators

        public static Word operator ^(Word w1, Word w2)
        {
            if (w1.Size != w2.Size)
                throw new Exception("Word sizes should be same!");

            Word r = new Word(w1.Size);

            for (int i = 0; i < w1.Size; i++)
            {
                if ((w1.GetBit(i) == '0' && w2.GetBit(i) == '0') ||
                    (w1.GetBit(i) == '1' && w2.GetBit(i) == '1'))
                    r.SetBit('0', i);
                else if ((w1.GetBit(i) == '0' && w2.GetBit(i) == '1') ||
                         (w1.GetBit(i) == '1' && w2.GetBit(i) == '0'))
                    r.SetBit('1', i);
                else
                    r.SetBit('?', i);
            }
            return r;
        }

        public static Word operator <<(Word w1, int count)
        {

            char[] tmp = new char[w1.Size];
            for (int i = 0; i < w1.Size; i++)
            {
                if (i - count < 0)
                    tmp[i] = '0';
                else
                    tmp[i] = w1.GetBit(i - count);
            }
            return Word.Parse(tmp);
        }

        public static Word operator >>(Word w1, int count)
        {
            char[] tmp = new char[w1.Size];
            for (int i = 0; i < w1.Size; i++)
            {
                if (i + count > w1.Size - 1)
                    tmp[i] = '0';
                else
                    tmp[i] = w1.GetBit(i + count);
            }
            return Word.Parse(tmp);
        }

        #endregion

    }

    #region Extension Methods Class

    public static class WordStatic
    {
        public static Word ToWord(this int number, int wordLength)
        {
            string s = Convert.ToString(number, 2);
            if (wordLength < s.Length)
                return ToWord(number);
            else
                return Word.Parse(new string('0', wordLength - s.Length) + s);
        }
        public static Word ToWord(this int number)
        {
            return Word.Parse(Convert.ToString(number, 2));
        }
    }

    #endregion

}
