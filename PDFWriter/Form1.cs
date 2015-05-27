using System;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace PDFWriter
{
    public partial class Form1 : Form
    {
        // Simplifys the code nl stands for newline
       string nl = System.Environment.NewLine;

     //   string nl = "\n";  // (works but triggers do you want to save message when closing the PDF so further investagation is needed)
   
       string basepath;

       // Define the List of byte arrays to hold the indirect objects
       List<byte[]> ListOfByteArrays;

       // This stores the position of each object relative to the begining of the file
       SortedDictionary<int, int> dictionary1;

       int object_counter = 0;
       int file_position = 0;

        // ========================================================================================

        private void Output_To_Screen(string p)
        {
            textBox1.AppendText(p + nl);
        }

        // ========================================================================================

        public Form1()
        {
            InitializeComponent();

            // Initialize the List and dictionary
            ListOfByteArrays = new List<byte[]>();
            dictionary1 = new SortedDictionary<int, int>();
        }

        // ========================================================================================

        private void button1_Click(object sender, EventArgs e)
        {
            string filename = "pdf-sample.pdf";

            Generate_PDF(basepath + filename);

            // Open the PDF with the default reader
            System.Diagnostics.Process.Start(basepath + filename);
        }

        // ========================================================================================

        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "PDF Writer V1.6";

            basepath = "..\\..\\..\\zOutputFiles\\";

            if (!(Directory.Exists(basepath))) { Directory.CreateDirectory(basepath); };
        }

        // ========================================================================================

        private void Generate_PDF(string fileName)
        {
            Remove_Old_File_If_Present(fileName);

            using (BinaryWriter writer = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                // First Build Up The PDF as a generic list of strings

                object_counter = 0;
                file_position = 0;

                file_position = Add_Header_To_List(ref ListOfByteArrays);
                dictionary1.Add(object_counter, file_position);

                // Object 1 - Catalog
                object_counter = object_counter + 1;
                file_position = Add_Catalog_To_List(object_counter, ref ListOfByteArrays);
                dictionary1.Add(object_counter, file_position);

                // Object 2 - Root page
                object_counter = object_counter + 1;
                file_position = Add_Root_Page_To_List(object_counter, ref ListOfByteArrays);
                dictionary1.Add(object_counter,file_position);


                int page_width = 612;
                int page_height = 792;

                // Object 3 - First page
                object_counter = object_counter + 1;
                file_position = Add_First_Page_To_List(object_counter, page_width, page_height, ref ListOfByteArrays);
                dictionary1.Add(object_counter,file_position);

                // Object 4 - Font Definition
                object_counter = object_counter + 1;
                file_position = Add_Font_Definition_To_List(object_counter, ref ListOfByteArrays);
                dictionary1.Add(object_counter,file_position);

                // Object 5 - Font Definition2
                object_counter = object_counter + 1;
                file_position = Add_Font_Definition_To_List2(object_counter, ref ListOfByteArrays);
                dictionary1.Add(object_counter, file_position);

                // Object 6 - First page
                object_counter = object_counter + 1;
                file_position = Add_First_Page_Text_Layout_To_List(object_counter, page_width, page_height, ref ListOfByteArrays);       
                 
                // the last file position doesnt get output to the xref table so we dont add it to the dictionary
                //    dictionary1.Add(object_counter,file_position);

                int starting_position_of_xref = file_position;

                // the number of objects are stored in the cross reference table and the trailer so we still pass that in
                object_counter = object_counter + 1; // (Add to object_counter one more time to account for 0 based object listing)
                file_position = Add_Cross_Reference_Table_To_List(object_counter, ref ListOfByteArrays, dictionary1);

                // at this point we are done incrementing the object_counter 

                file_position = Add_Trailer_To_List(object_counter, ref ListOfByteArrays);

                Add_End_Of_File(ref ListOfByteArrays, starting_position_of_xref);

                Physically_Write_Out_The_PDF(ref ListOfByteArrays, writer);

                Output_To_Screen("Done Creating The PDF File");
            }
        }

        // ========================================================================================

        private int Add_Header_To_List(ref List<byte[]> listofbytes)
        {
            listofbytes.Clear();
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("%PDF-1.1" + nl));
            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================
        // Object 1 - Catalog

        private int Add_Catalog_To_List(int object_counter, ref List<byte[]> listofbytes)
        {
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(object_counter.ToString() 
                + " 0 obj <</Type /Catalog /Pages 2 0 R>>" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("endobj" + nl));
            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================
        // Object 2 - Root page

        private int Add_Root_Page_To_List(int object_counter, ref List<byte[]> listofbytes)
        {
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(object_counter.ToString() 
                + " 0 obj <</Type /Pages /Kids [3 0 R] /Count 1>>" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("endobj" + nl));
            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================
        // Object 3 - First page

        private int Add_First_Page_To_List(int object_counter, int  page_width, int page_height, ref List<byte[]> listofbytes)
        {

            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(object_counter.ToString() + " 0 obj" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("<<" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("/Type /Page" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("/MediaBox [0 0 " + page_width.ToString() + " " + page_height.ToString() + "]" + nl));

            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("/Parent 2 0 R" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("/Resources 4 0 R" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("/Contents [6 0 R]" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(">>" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("endobj" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("" + nl));

            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================
        // Object 4 - Font Definition

        private int Add_Font_Definition_To_List(int object_counter, ref List<byte[]> listofbytes)
        {
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(object_counter.ToString() 
                + " 0 obj<</Font <</F0 5 0 R>>>>" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("endobj" + nl));
            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================
        // Object 5 - Font Definition2

        private int Add_Font_Definition_To_List2(int object_counter, ref List<byte[]> listofbytes)
        {
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(object_counter.ToString() 
                + " 0 obj<</Type /Font /Subtype /Type1 /BaseFont /Helvetica>>" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("endobj" + nl));
            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================
        // Object 6 - First page

        private int Add_First_Page_Text_Layout_To_List(int object_counter, int page_width, int page_height, ref List<byte[]> listofbytes)
        {
            // Option 2 a loop demonstrating writing 79 lines to the file
            int vertical_position = 793;
            int horzontal_position = 10;
            int font_point_size = 9;
            int line_spacing = 10;
            int number_of_lines = 79;
            string stream_text_for_page = "";
            stream_text_for_page = stream_text_for_page + "BT " + nl;
            // font size
            stream_text_for_page = stream_text_for_page + "/F0 " + font_point_size.ToString() + " Tf " + nl; ;
            stream_text_for_page = stream_text_for_page + horzontal_position.ToString() + " " + vertical_position.ToString() + " Td " + nl;
            for (int i = 1; i <= number_of_lines; i++)  // number of lines
            {
                // line seperation                
                stream_text_for_page = stream_text_for_page + "0 -" + line_spacing.ToString() + " Td ";
                // line content
                stream_text_for_page = stream_text_for_page + "(Line Number " + i.ToString().PadLeft(2, ' ') + ") Tj " + nl;
            }
            stream_text_for_page = stream_text_for_page + "ET " + nl;

            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(object_counter.ToString() + " 0 obj" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("<</Length " 
                + (stream_text_for_page.Length + 1).ToString() + ">>" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("stream" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(stream_text_for_page + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("endstream" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("endobj" + nl));
            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================

        private int Add_Cross_Reference_Table_To_List(int object_counter, ref List<byte[]> listofbytes, 
            SortedDictionary<int, int> obj_pos_dictionary)
        {
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("xref" + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("0 " + object_counter.ToString() + nl));
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("0000000000 65535 f" + nl));

            foreach (KeyValuePair<int, int> pair in obj_pos_dictionary)
            {
                string keystr = pair.Key.ToString().PadLeft(5,'0');

                string object_revision;

                if (keystr != "00000") { object_revision = "00000"; } else { object_revision = "00000"; }

                string valstr = pair.Value.ToString().PadLeft(10, '0');

                listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(valstr + " " + object_revision + " n" + nl));
            }

              return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================

        private int Add_Trailer_To_List(int object_counter, ref List<byte[]> listofbytes)
        {
            listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("trailer <</Size " 
                + object_counter.ToString() + "/Root 1 0 R>>" + nl));
            return Get_List_Of_Bytes_Total_Count(listofbytes);
        }

        // ========================================================================================

        private void Add_End_Of_File(ref List<byte[]> listofbytes,int starting_pos)
        {
           listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("startxref" + nl));
           listofbytes.Add(System.Text.Encoding.UTF8.GetBytes(starting_pos.ToString() + nl));
           listofbytes.Add(System.Text.Encoding.UTF8.GetBytes("%%EOF" + nl));
        }

        // ========================================================================================

        private int Get_List_Of_Bytes_Total_Count(List<byte[]> listofbytes)
        {
            int cnt = 0;
            foreach (byte[] ba1 in listofbytes)
            {
                cnt = cnt + ba1.Length;
            }

            return cnt;
        }

        // ========================================================================================

        private void Remove_Old_File_If_Present(string fileName)
        {
            if (System.IO.File.Exists(fileName) == true)
            {
                try
                {
                    System.IO.File.Delete(fileName);
                }
                catch 
                {
                    File.SetAttributes(fileName, FileAttributes.Normal);
                    try
                    {
                        File.Delete(fileName);
                    }
                    catch
                    {
                        Output_To_Screen("Unable To delete old file it is currently locked");
                    }

                }
            }
        }

        // ========================================================================================

        private void Physically_Write_Out_The_PDF(ref List<byte[]> listofbytes, BinaryWriter writer)
        {
            foreach (byte[] ba1 in listofbytes)
            {
                writer.Write(ba1);
            }
        }

        // ========================================================================================

    }
}
