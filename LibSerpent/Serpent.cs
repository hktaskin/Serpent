using System;

/*
Author: Halil Kemal TASKIN
Web: http://hkt.me
*/

namespace LibSerpent
{
    public static class Serpent
    {
        // This is a static class which implements SERPENT core operations and some other useful analyze functions

        #region Fields

        private static uint[][] SBox = new uint[][]{
                                       new uint[]{3, 8, 15, 1, 10, 6, 5, 11, 14, 13, 4, 2, 7, 0, 9, 12},
                                       new uint[]{15, 12, 2, 7, 9, 0, 5, 10, 1, 11, 14, 8, 6, 13, 3, 4},
                                       new uint[]{8, 6, 7, 9, 3, 12, 10, 15, 13, 1, 14, 4, 0, 11, 5, 2},
                                       new uint[]{0, 15, 11, 8, 12, 9, 6, 3, 13, 1, 2, 4, 10, 7, 5, 14},
                                       new uint[]{1, 15, 8, 3, 12, 0, 11, 6, 2, 5, 4, 10, 9, 14, 7, 13},
                                       new uint[]{15, 5, 2, 11, 4, 10, 9, 12, 0, 3, 14, 8, 13, 6, 7, 1},
                                       new uint[]{7, 2, 12, 5, 8, 4, 6, 11, 14, 9, 1, 15, 13, 3, 10, 0},
                                       new uint[]{1, 13, 15, 0, 14, 8, 2, 11, 7, 4, 12, 10, 9, 3, 5, 6}};

        #endregion

        #region Private Static Methods

        private static uint[,] XORTable(uint[] SBox, int inputBitLength, int outputBitLength)
        {
            uint inputSize, outputSize;

            unchecked
            {
                inputSize = (uint)(1 << inputBitLength);
                outputSize = (uint)(1 << outputBitLength);
            }

            /*
            if (inputBitLength == 32)
                inputSize = uint.MaxValue;
            if (outputBitLength == 32)
                outputSize = uint.MaxValue;
            */

            if (SBox.Length != inputSize && inputBitLength != 32)
                throw new Exception("S-Box size should be exactly 2^inputBitLength.");
            for (int i = 0; i < inputSize; i++)
            {
                if (SBox[i] >= outputSize && outputBitLength != 32)
                    throw new Exception("S-Box output values can not be greater or equal to 2^outputBitLength.");
            }

            uint[,] xorTable = new uint[inputSize, outputSize];

            for (int i = 0; i < inputSize; i++)
            {
                for (int j = 0; j < inputSize; j++)
                    xorTable[i ^ j, SBox[i] ^ SBox[j]]++;
            }

            return xorTable;
        }

        #endregion

        #region Public Static Methods

        #region Core Operations

        public static Word SerpentSBox(int round, Word input)
        {
            if (input.Contains('?'))
                return Word.Parse("????");

            if (input.IsEqualTo("0000"))
                return Word.Parse("0000");

            int SBoxID = round % 8;

            switch (SBoxID)
            {
                case 0:
                    // Input: [02] 0010 -> Output: 1???
                    if (input.IsEqualTo("0010"))
                        return Word.Parse("1???");
                    // Input: [04] 0100 -> Output: 1???
                    if (input.IsEqualTo("0100"))
                        return Word.Parse("1???");
                    // Input: [06] 0110 -> Output: 0???
                    if (input.IsEqualTo("0110"))
                        return Word.Parse("0???");
                    break;
                case 1:
                    // Input: [04] 0100 -> Output: ?1??
                    if (input.IsEqualTo("0100"))
                        return Word.Parse("?1??");
                    // Input: [08] 1000 -> Output: ?1??
                    if (input.IsEqualTo("1000"))
                        return Word.Parse("?1??");
                    // Input: [0C] 1100 -> Output: ?0??
                    if (input.IsEqualTo("1100"))
                        return Word.Parse("?0??");
                    break;
                case 2:
                    // Input: [02] 0010 -> Output: ???1
                    if (input.IsEqualTo("0010"))
                        return Word.Parse("???1");
                    // Input: [08] 1000 -> Output: ???1
                    if (input.IsEqualTo("1000"))
                        return Word.Parse("???1");
                    // Input: [0A] 1010 -> Output: ???0
                    if (input.IsEqualTo("1010"))
                        return Word.Parse("???0");
                    break;
                case 3:
                    break;
                case 4:
                case 5:
                    // Input: [04] 0100 -> Output: ???1
                    if (input.IsEqualTo("0100"))
                        return Word.Parse("???1");
                    // Input: [0B] 1011 -> Output: ???1
                    if (input.IsEqualTo("1011"))
                        return Word.Parse("???1");
                    // Input: [0F] 1111 -> Output: ???0
                    if (input.IsEqualTo("1111"))
                        return Word.Parse("???0");
                    break;
                case 6:
                    // Input: [02] 0010 -> Output: ??1?
                    if (input.IsEqualTo("0010"))
                        return Word.Parse("??1?");
                    // Input: [04] 0100 -> Output: ??1?
                    if (input.IsEqualTo("0100"))
                        return Word.Parse("??1?");
                    // Input: [06] 0110 -> Output: ??0?
                    if (input.IsEqualTo("0110"))
                        return Word.Parse("??0?");
                    break;
                case 7:
                    break;
            }

            return Word.Parse("????");
        }

        public static Word SerpentInvSBox(int round, Word output)
        {
            if (output.Contains('?'))
                return Word.Parse("????");

            if (output.IsEqualTo("0000"))
                return Word.Parse("0000");

            int SBoxID = round % 8;

            switch (SBoxID)
            {
                case 0:
                    // Output: [04] 0100 -> Input: ?1??
                    if (output.IsEqualTo("0100"))
                        return Word.Parse("?1??");
                    // Output: [08] 1000 -> Input: ?1??
                    if (output.IsEqualTo("1000"))
                        return Word.Parse("?1??");
                    // Output: [0C] 1100 -> Input: ?0??
                    if (output.IsEqualTo("1100"))
                        return Word.Parse("?0??");
                    break;
                case 1:
                    // Output: [01] 0001 -> Input: 1???
                    if (output.IsEqualTo("0001"))
                        return Word.Parse("1???");
                    // Output: [04] 0100 -> Input: 1???
                    if (output.IsEqualTo("0100"))
                        return Word.Parse("1???");
                    // Output: [05] 0101 -> Input: 0???
                    if (output.IsEqualTo("0101"))
                        return Word.Parse("0???");
                    break;
                case 2:
                    // Output: [01] 0001 -> Input: ???1
                    if (output.IsEqualTo("0001"))
                        return Word.Parse("???1");
                    // Output: [0C] 1100 -> Input: ???1
                    if (output.IsEqualTo("1100"))
                        return Word.Parse("???1");
                    // Output: [0D] 1101 -> Input: ???0
                    if (output.IsEqualTo("1101"))
                        return Word.Parse("???0");
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    break;
                case 6:
                    // Output: [02] 0010 -> Input: ??1?
                    if (output.IsEqualTo("0010"))
                        return Word.Parse("??1?");
                    // Output: [08] 1000 -> Input: ??1?
                    if (output.IsEqualTo("1000"))
                        return Word.Parse("??1?");
                    // Output: [0A] 1010 -> Input: ??0?
                    if (output.IsEqualTo("1010"))
                        return Word.Parse("??0?");
                    break;
                case 7:
                    break;
            }

            return Word.Parse("????"); ;
        }

        public static void ApplySerpentPermutation(ref Word x0, ref Word x1, ref Word x2, ref Word x3)
        {
            x0.RotateLeft(13);
            x2.RotateLeft(3);
            x1 = x1 ^ x0 ^ x2;
            x3 = x3 ^ x2 ^ (x0 << 3);
            x1.RotateLeft(1);
            x3.RotateLeft(7);
            x0 = x0 ^ x1 ^ x3;
            x2 = x2 ^ x3 ^ (x1 << 7);
            x0.RotateLeft(5);
            x2.RotateLeft(22);
        }

        public static void ApplySerpentInvPermutation(ref Word x0, ref Word x1, ref Word x2, ref Word x3)
        {
            x2.RotateRight(22);
            x0.RotateRight(5);
            x2 = x2 ^ x3 ^ (x1 << 7);
            x0 = x0 ^ x1 ^ x3;
            x3.RotateRight(7);
            x1.RotateRight(1);
            x3 = x3 ^ x2 ^ (x0 << 3);
            x1 = x1 ^ x0 ^ x2;
            x2.RotateRight(3);
            x0.RotateRight(13);
        }

        public static void ApplySerpentSBox(ref Word x0, ref Word x1, ref Word x2, ref Word x3, int round)
        {
            for (int i = 0; i < x0.Size; i++)
            {
                Word input = new Word(4);
                input.SetBit(x0.GetBit(i), 0);
                input.SetBit(x1.GetBit(i), 1);
                input.SetBit(x2.GetBit(i), 2);
                input.SetBit(x3.GetBit(i), 3);
                Word output = SerpentSBox(round, input);
                x0.SetBit(output.GetBit(0), i);
                x1.SetBit(output.GetBit(1), i);
                x2.SetBit(output.GetBit(2), i);
                x3.SetBit(output.GetBit(3), i);
            }

            /* ESKİ
             for (int i = 0; i < max; i++)
             {
                 x0.SetNibble(SerpentSBox(round, x0.GetNibble(i)), i);
                 x1.SetNibble(SerpentSBox(round, x1.GetNibble(i)), i);
                 x2.SetNibble(SerpentSBox(round, x2.GetNibble(i)), i);
                 x3.SetNibble(SerpentSBox(round, x3.GetNibble(i)), i);
             }
             */
        }

        public static void ApplySerpentInvSBox(ref Word x0, ref Word x1, ref Word x2, ref Word x3, int round)
        {
            for (int i = 0; i < x0.Size; i++)
            {
                Word output = new Word(4);
                output.SetBit(x0.GetBit(i), 0);
                output.SetBit(x1.GetBit(i), 1);
                output.SetBit(x2.GetBit(i), 2);
                output.SetBit(x3.GetBit(i), 3);
                Word input = SerpentInvSBox(round, output);
                x0.SetBit(input.GetBit(0), i);
                x1.SetBit(input.GetBit(1), i);
                x2.SetBit(input.GetBit(2), i);
                x3.SetBit(input.GetBit(3), i);
            }
        }

        #endregion

        #region Useful Operations

        public static bool Impossible(Word[] c1, Word[] c2)
        {
            if ((c1[0].Size != c2[0].Size) || (c1.Length != c2.Length))
                throw new Exception("Sizes for Words in arrays should be same!");

            for (int i = 0; i < c1.Length; i++)
            {
                for (int j = 0; j < c1[i].Size; j++)
                {
                    if ((c1[i].GetBit(j) == '1' && c2[i].GetBit(j) == '0') ||
                        (c1[i].GetBit(j) == '0' && c2[i].GetBit(j) == '1'))
                        return true;
                }
            }
            return false;
        }

        public static uint[,] SerpentDDT(int sbox)
        {
            return XORTable(SBox[sbox % 8], 4, 4);
        }

        public static void SetBitSlice(ref Word[] w, Word slice, int column)
        {
            if (w.Length != 4 || slice.Size != 4)
                throw new Exception("Word Array Size should be 4 and Slice length should be 4");

            for (int i = 0; i < 4; i++)
                w[i].SetBit(slice.GetBit(i), column);
        }

        public static Word GetBitSlice(Word[] w, int column)
        {
            if (w.Length != 4)
                throw new Exception("Word Array Size should be 4 and Slice length should be 4");

            Word slice = new Word(4);

            for (int i = 0; i < 4; i++)
                slice.SetBit(w[i].GetBit(column), i);

            return slice;
        }

        public static int NonZeroColumnNumber(Word[] w)
        {
            if (w.Length != 4)
                throw new Exception("Word list size should be 4!");
            int sayac = 0;

            for (int i = 0; i < w[0].Size; i++)
            {
                if (w[0].GetBit(i) == '0' && w[1].GetBit(i) == '0' && w[2].GetBit(i) == '0' && w[3].GetBit(i) == '0')
                    sayac++;
            }

            return 32 - sayac;
        }

        public static int[] NonZeroColumnPositions(Word[] w)
        {
            int[] konum = new int[0];

            if (w.Length != 4)
                throw new Exception("Word list size should be 4!");

            for (int i = 0; i < w[0].Size; i++)
            {
                Word t = Serpent.GetBitSlice(w, i);
                if (t.Contains('?') || t.Contains('1'))
                {
                    Array.Resize(ref konum, konum.Length + 1);
                    konum[konum.Length - 1] = i;
                }
            }

            return konum;
        }

        public static void InputToOutputProbability(Word input, Word output, int sbox, out int probNumerator, out int probDenominator)
        {
            if (input.Size != 4 || output.Size != 4)
                throw new Exception("Input and output word sizes should be 4!");

            Word tmpIn = Word.Parse(input.ToString());
            Word tmpOut = Word.Parse(output.ToString());

            int[] inQ = tmpIn.QuestionMarkPositions();
            int[] outQ = tmpOut.QuestionMarkPositions();

            int[] inputValues = new int[0];
            int[] outputValues = new int[0];

            for (int i = 0; i < (1 << inQ.Length); i++)
            {
                Word w1 = i.ToWord(inQ.Length);
                for (int j = 0; j < inQ.Length; j++)
                    tmpIn.SetBit(w1.GetBit(j), inQ[j]);

                int t = tmpIn.ToInt32();
                Array.Resize(ref inputValues, inputValues.Length + 1);
                inputValues[inputValues.Length - 1] = t;
            }

            for (int i = 0; i < (1 << outQ.Length); i++)
            {
                Word w1 = i.ToWord(outQ.Length);
                for (int j = 0; j < outQ.Length; j++)
                    tmpOut.SetBit(w1.GetBit(j), outQ[j]);

                int t = tmpOut.ToInt32();
                Array.Resize(ref outputValues, outputValues.Length + 1);
                outputValues[outputValues.Length - 1] = t;
            }

            uint[,] ddt = XORTable(SBox[sbox], 4, 4);

            probNumerator = 0;
            probDenominator = 0;

            // Pay: Olası her bir input value için sadece olası output değerlerinin toplamı
            for (int i = 0; i < inputValues.Length; i++)
                for (int j = 0; j < outputValues.Length; j++)
                    probNumerator += (int)ddt[inputValues[i], outputValues[j]];

            // Payda: Olası her bit inputValue için tüm output değerlerinin toplamı
            for (int i = 0; i < inputValues.Length; i++)
                for (int j = 0; j < 16; j++)
                    probDenominator += (int)ddt[inputValues[i], j];

            return;
        }

        public static void OutputToInputProbability(Word output, Word input, int sbox, out int probNumerator, out int probDenominator)
        {
            if (input.Size != 4 || output.Size != 4)
                throw new Exception("Input and output word sizes should be 4!");

            Word tmpIn = Word.Parse(input.ToString());
            Word tmpOut = Word.Parse(output.ToString());

            int[] inQ = tmpIn.QuestionMarkPositions();
            int[] outQ = tmpOut.QuestionMarkPositions();

            int[] inputValues = new int[0];
            int[] outputValues = new int[0];

            for (int i = 0; i < (1 << inQ.Length); i++)
            {
                Word w1 = i.ToWord(inQ.Length);
                for (int j = 0; j < inQ.Length; j++)
                    tmpIn.SetBit(w1.GetBit(j), inQ[j]);

                int t = tmpIn.ToInt32();
                Array.Resize(ref inputValues, inputValues.Length + 1);
                inputValues[inputValues.Length - 1] = t;
            }

            for (int i = 0; i < (1 << outQ.Length); i++)
            {
                Word w1 = i.ToWord(outQ.Length);
                for (int j = 0; j < outQ.Length; j++)
                    tmpOut.SetBit(w1.GetBit(j), outQ[j]);

                int t = tmpOut.ToInt32();
                Array.Resize(ref outputValues, outputValues.Length + 1);
                outputValues[outputValues.Length - 1] = t;
            }

            uint[,] ddt = XORTable(SBox[sbox], 4, 4);

            probNumerator = 0;
            probDenominator = 0;

            // Pay: Olası her bir output value için sadece olası input değerlerinin toplamı
            for (int i = 0; i < outputValues.Length; i++)
                for (int j = 0; j < inputValues.Length; j++)
                    probNumerator += (int)ddt[inputValues[j], outputValues[i]];

            // Payda: Olası her bir outputValue için tüm input değerlerinin toplamı
            for (int i = 0; i < outputValues.Length; i++)
                for (int j = 0; j < 16; j++)
                    probDenominator += (int)ddt[j, outputValues[i]];

            return;
        }

        #endregion

        #endregion
    }
}
