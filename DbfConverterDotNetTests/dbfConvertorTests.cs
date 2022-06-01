using Microsoft.VisualStudio.TestTools.UnitTesting;
using convertor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text;
using System.Collections;
using System.IO;
namespace convertor.Tests
{
    public class Stream
    {
        const int fieldDescriptorSize = 32;

        public static byte[] HeaderStream()
        {
            string dbfPath = @"C:\Users\p.gayevsky\Documents\SomeProjects\DbfConverterDotNet\DbfConverterDotNetTests\vertopal.com_JUL13_21 (1).dbf";

            Byte[] allBytes = File.ReadAllBytes(dbfPath);
            int value = BitConverter.ToInt16(allBytes, 8);//Size of the table header in bytes.


            Byte[] bytes = new Byte[10000000];
            using (FileStream reader = new FileStream(dbfPath, FileMode.Open))
            {
                reader.Seek(fieldDescriptorSize, SeekOrigin.Begin);
                reader.Read(bytes, 0, value);
            }
            return bytes;
        }
    }

    [TestClass()]
    public class dbfConvertorTests: Stream 
    {
        [TestMethod()]
        public void HeaderTest()
        {
            Byte[] bytes = HeaderStream();
            Byte[] allBytes = File.ReadAllBytes(@"C:\Users\p.gayevsky\Documents\SomeProjects\DbfConverterDotNet\DbfConverterDotNetTests\vertopal.com_JUL13_21 (1).dbf");
            int value = BitConverter.ToInt16(allBytes, 8);

            var actual = dbfConvertor.Header(bytes, allBytes, value);
            var expected = 5;

            Assert.AreEqual(expected, actual.Count);
        }

        [TestMethod()]
        public void RowsRecordsAndOutputTest1()
        {
            Byte[] bytes = HeaderStream();
            Byte[] allBytes = File.ReadAllBytes(@"C:\Users\p.gayevsky\Documents\SomeProjects\DbfConverterDotNet\DbfConverterDotNetTests\vertopal.com_JUL13_21 (1).dbf");
            int value = BitConverter.ToInt16(allBytes, 8);
            var lsit = dbfConvertor.Header(bytes, allBytes, value);

            var actual = dbfConvertor.RowsRecordsAndOutput(lsit, allBytes, value);
            var expected = "{TRACK_NO:'', PKGID:'1680561', STATUS:'', CARRIER:'DC', _recordNumber:'0'}";

            Assert.AreEqual(expected, actual);
        }
    }
           
}   