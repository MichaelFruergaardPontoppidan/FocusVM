﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FocusVM
{
    class XmlReader
    {
        public Encoding fileEncoding;
        private UTF8Encoding utf8Encoding = new UTF8Encoding(true, true); // turn on BOM and error checking
        private string text;
        private char[] bufferText;
        /// <summary>
        /// The contents of the file with header information removed
        /// </summary>
        //public char[] Text
        //{
        //	get
        //	{
        //		return bufferText;
        //	}
        //}
        public char[] TextAsCharArray()
        {
            return bufferText;
        }
        public string Text()
        {
            return text;
        }

        /// <summary>
        /// Constructor of the Xpo reader class
        /// </summary>
        /// <param name="file">Xpo file to read</param>
        public XmlReader(string file)
        {
            //Open file and read contents
            FileStream fsr = null;
            BinaryReader breader = null;

            try
            {
                fsr = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read);

                breader = new BinaryReader(fsr);
                byte[] binaryContent = new byte[fsr.Length];
                breader.Read(binaryContent, 0, (int)fsr.Length);
                // having read the entire contents of the buffer into memory all at once, it's easy to check for character set.
                try
                {
                    if (fsr.Length > 3 &&
                       binaryContent[0] == 0xEF &&
                       binaryContent[1] == 0xBB &&
                       binaryContent[2] == 0xBF)
                    {
                        fileEncoding = utf8Encoding;
                        text = fileEncoding.GetString(binaryContent, 3, binaryContent.Length - 3);
                    }
                    else
                    {
                        fileEncoding = Encoding.GetEncoding(1252); // all existing XPO files should be in 1252. Danish ones will be a problem.
                        text = fileEncoding.GetString(binaryContent, 0, binaryContent.Length);
                    }
                }
                catch (Exception)
                {
                    text = "";
                }
                bufferText = text.ToCharArray();

            }
            finally
            {
                if (breader != null)
                    breader.Close();
            }
        }

    }
}
