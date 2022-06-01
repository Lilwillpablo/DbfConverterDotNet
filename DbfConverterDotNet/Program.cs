using System.Text;
using System.Collections;
using DbfConverterDotNet;
namespace convertor;
public class dbfConvertor
{
    const string carriageReturn = "\r"; //Carriage return. After this symbol record of headers is over.
    const int fieldDescriptorSize = 32; //The size of the DBF file header.
    

    #region File stream wich returns array of headers in bytes.
    public static void Main(string[] args)
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
        Header(bytes, allBytes, value);
    }
    #endregion

    #region Returns an ArrayList containing the field descriptors.
    #region Field descriptors consists of fieldName, fieldType, fieldSize.
    #region By FieldSize we will get records for each header.
    #region After as we got field descriptors we can use it to build JSON.
    public static ArrayList Header(Byte[] bytes, Byte[] allBytes, int value)
    {
        ArrayList arlist = new ArrayList();
        var defaul = new Column("DELETED", "", 1);
        arlist.Add(defaul);// add column for empty fields

        string encFieldName1;
        string encFieldName;
        string fieldName;
        string fieldType;
        int fieldSize;

        for (int c = 0; c < bytes.Length - 32; c += 32)
        {

            ArraySegment<byte> fieldRecordData = new ArraySegment<byte>(bytes, c, c + 1);
            encFieldName = Encoding.UTF8.GetString(fieldRecordData).Replace("\0", "");
            if (encFieldName.Replace(" ", "") == carriageReturn)
            {
                break;
            }
            else
            {
                ArraySegment<byte> fieldRecordData1 = new ArraySegment<byte>(bytes, c, c + fieldDescriptorSize);
                encFieldName1 = Encoding.UTF8.GetString(fieldRecordData1).Trim();
                fieldName = encFieldName1.Substring(0, 11).Replace("\0", "");
                fieldType = encFieldName1.Substring(11, 1);
                var subSize = encFieldName1.Substring(16, 1);
                fieldSize = (int)subSize[0];
                var column = new Column(fieldName, fieldType, fieldSize);
                arlist.Add(column);
               
            }
        }
        RowsRecordsAndOutput(arlist, allBytes, value);
        return arlist;
    }
    #endregion
    #endregion
    #endregion
    #endregion

    #region According ArrayList finds records and then buiuld the JSON
    public static string RowsRecordsAndOutput(ArrayList arlist, Byte[] allBytes, int value)
    {
        string json = "";
        int recordNumber = 0;
        while (value < allBytes.Length - 1)
        {
            json = "{";
            bool skip = false;

            for (int i = 0; i < arlist.Count; i++)
            {
                Column ar = (Column)arlist[i];
                byte[] elements = new byte[ar.ColumnSize];
                for (int j = 0; j < ar.ColumnSize; j++)
                {
                    elements[j] = allBytes[value + j];
                }
                value = value + ar.ColumnSize;
                var value1 = Encoding.ASCII.GetString(elements).Trim();
                if (ar.ColumnType == "")
                {
                    if (value1 == "*")
                    {
                        skip = true;
                    }
                }
                if (ar.ColumnName != "DELETED")
                {
                    json = json + ar.ColumnName + ":'" + value1 + "', ";
                }
            }
            if (skip == false)
            {
                json = json + "_recordNumber:'" + recordNumber + "'}";
                recordNumber++;
                Console.WriteLine(json);
            }
        }
        return json;
    }
    #endregion
}












